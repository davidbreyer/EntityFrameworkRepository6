//-----------------------------------------------------------------------
// <copyright file="AuditDbContext.cs" company="Russell Wilkins">
//     Copyright (c) Russell Wilkins 2012. All Rights Reserved.
// </copyright>
// <author>Russell Wilkins</author>
// <license href="license.txt">
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </license>
//-----------------------------------------------------------------------

using System.Data.Entity;

namespace EntityFramework.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity.Core;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;

    /// <summary>
    /// The AuditDbContext adds auditing capabilities tot eh DbContext.
    /// </summary>
    public class AuditDbContext : DbContext
    {
        private const string AuditUpdatedColumnName = "Audited";
        private const string AuditUserColumnName = "AuditUser";
        private const string AuditTypeColumnName = "AuditType";
        private const string AuditSourceIdColumnName = "AuditSourceId";

        private const string EntityUpdatedColumnName = "Updated";
        private const string EntityUpdateUSerColumnName = "UpdateUser";

        private static Dictionary<Type, AuditTypeInfo> auditTypes = new Dictionary<Type, AuditTypeInfo>();

        #region Constructors
        /// <summary>
        /// Initializes static members of the AuditDbContext class.
        /// </summary>
        static AuditDbContext()
        {
            AuditConfigurationSection config = ConfigurationManager.GetSection("entityFramework.Audit") as AuditConfigurationSection;

            if (config == null)
            {
                config = new AuditConfigurationSection();
            }

            AuditEnabled = config.Enabled;

            foreach (EntityElement item in config.Entities)
            {
                var entity = Type.GetType(item.Name);
                var auditEntity = Type.GetType(item.Audit);

                if (entity != null)
                {
                    RegisterAuditType(entity, auditEntity);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the AuditDbContext class
        /// using the given string as the name or connection string
        /// for the database to which a connection will be made. 
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or the connection string.</param>
        public AuditDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuditDbContext class 
        /// using a given DBConnection
        /// for the database which a connection will be made.
        /// </summary>
        /// <param name="connection">DBConnection object that the context will connect to.</param>
        public AuditDbContext(DbConnection connection)
            : base(connection, true)
        {
            
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether auditing is enabled on this context.
        /// </summary>
        public static bool AuditEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this context is using proxies.
        /// </summary>
        public bool Proxies
        {
            get
            {
                if (auditTypes.Count > 0)
                {
                    var f = auditTypes.First();
                    var e = this.Set(f.Value.EntityType).Create();
                    return e.GetType().Namespace != f.Value.EntityType.Namespace;
                }

                return false;
            }
        }

        /// <summary>
        /// Registers and type for auditing.
        /// </summary>
        /// <param name="auditableEntityType">Type to audit, must implement IAuditableEntity.</param>
        /// <param name="auditEntityType">Type of audit entity, must implement IAuditEntity.</param>
        public static void RegisterAuditType(Type auditableEntityType, Type auditEntityType)
        {
            // Basic parameter validation.
            if (auditableEntityType == null)
            {
                throw new ArgumentNullException("auditableEntityType");
            }

            if (auditTypes.ContainsKey(auditableEntityType))
            {
                throw new ArgumentException("Type already registered for auditing.", "auditableEntityType");
            }

            // Validate the entity.
            var iface = auditableEntityType.GetInterface("IAuditableEntity");
            if (iface == null)
            {
                throw new ArgumentException("Entity does implement IAuditableEntity", "auditableEntityType");
            }

            AuditTypeInfo info = new AuditTypeInfo { EntityType = auditableEntityType, AuditEntityType = auditEntityType };

            // Validate the auditEntity
            if (auditEntityType != null)
            {
                iface = auditEntityType.GetInterface("IAuditEntity");
                if (iface == null)
                {
                    throw new ArgumentException("Entity does implement IAuditEntity", "auditEntityType");
                }

                // Extract the list of propeties to audit.
                var properties = auditEntityType.GetProperties();
                var entityProperties = auditableEntityType.GetProperties().ToDictionary(x => x.Name);
                foreach (var property in properties)
                {
                    if (entityProperties.ContainsKey(property.Name))
                    {
                        if (property.PropertyType == entityProperties[property.Name].PropertyType)
                        {
                            info.AuditProperties.Add(property.Name);
                        }
                    }
                }
            }

            // Valid so register.
            auditTypes.Add(auditableEntityType, info);
        }

        /// <summary>
        /// Reloads the entity from the database overwriting any property values with values from the database. 
        /// The entity will be in the Unchanged state after calling this method.
        /// </summary>
        /// <param name="entity">The entity object to reload.</param>
        public void Reload(object entity)
        {
            this.Entry(entity).Reload();
        }

        /// <summary>
        /// Detaches the entity from the context.
        /// </summary>
        /// <param name="entity"></param>
        public void Detach(object entity)
        {
            this.Detach(entity);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// using the current windows user for auditing.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            return this.SaveChanges(WindowsIdentity.GetCurrent().Name);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// using the current windows user for auditing.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(WindowsIdentity.GetCurrent().Name);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// using the user paramater passed for auditing.
        /// </summary>
        /// <param name="user">User name for auditing.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public int SaveChanges(string user)
        {
            if (AuditEnabled)
            {
                // Track all audit entities created in this transaction, will be removed from context on exception.
                List<IAuditEntity> audits = new List<IAuditEntity>();

                // Use the same datetime for all updates in this transaction, retrieved from server when first used.
                DateTimeOffset? updateDateTime = null;

                // Process any auditable objects.
                foreach (DbEntityEntry<IAuditableEntity> auditable in this.ChangeTracker.Entries<IAuditableEntity>())
                {
                    if (auditable.State == EntityState.Added
                        || auditable.State == EntityState.Modified
                        || auditable.State == EntityState.Deleted)
                    {
                        // Need datetime for the audits.
                        if (updateDateTime.HasValue == false)
                        {
                            try
                            {
                                updateDateTime = this.Database.SqlQuery<DateTimeOffset>("select SYSDATETIMEOFFSET()", new object[] { }).First();
                            }
                            catch (Exception)
                            {
                                updateDateTime = DateTimeOffset.Now;
                            }
                        }

                        // Create an audit entity.
                        if (auditable.State == EntityState.Modified
                            || auditable.State == EntityState.Deleted)
                        {
                            // TODO: Find a better way of doing this proxy check.
                            Type entityType = auditable.Entity.GetType();
                            if (entityType.Namespace == "System.Data.Entity.DynamicProxies")
                            {
                                entityType = entityType.BaseType;
                            }

                            if (auditTypes.ContainsKey(entityType) && auditTypes[entityType].AuditEntityType != null)
                            {
                                audits.Add(this.AuditEntity(auditable, auditTypes[entityType], updateDateTime.Value, user));
                            }
                        }

                        // Update the auditable entity.
                        if (auditable.State != EntityState.Deleted)
                        {
                            auditable.Entity.Updated = updateDateTime.Value;
                            auditable.Entity.UpdateUser = user;
                        }
                    }
                }

                // Perform the updates.
                try
                {
                    return base.SaveChanges();
                }
                catch (Exception)
                {
                    // Updated failed so remove the audit entities.
                    foreach (var item in audits)
                    {
                        this.Set(item.GetType()).Remove(item);
                    }


                    throw;
                }
            }
            else
            {
                return base.SaveChanges();
            }
        }

        public async Task<int> SaveChangesAsync(string user)
        {
            if (AuditEnabled)
            {
                // Track all audit entities created in this transaction, will be removed from context on exception.
                List<IAuditEntity> audits = new List<IAuditEntity>();

                // Use the same datetime for all updates in this transaction, retrieved from server when first used.
                DateTimeOffset? updateDateTime = null;

                // Process any auditable objects.
                foreach (DbEntityEntry<IAuditableEntity> auditable in this.ChangeTracker.Entries<IAuditableEntity>())
                {
                    if (auditable.State == EntityState.Added
                        || auditable.State == EntityState.Modified
                        || auditable.State == EntityState.Deleted)
                    {
                        // Need datetime for the audits.
                        if (updateDateTime.HasValue == false)
                        {
                            try { 
                                updateDateTime = this.Database.SqlQuery<DateTimeOffset>("select SYSDATETIMEOFFSET()", new object[] { }).First();
                            } catch (Exception)
                            {
                                updateDateTime = DateTimeOffset.Now;
                            }
                        }

                        // Create an audit entity.
                        if (auditable.State == EntityState.Modified
                            || auditable.State == EntityState.Deleted)
                        {
                            // TODO: Find a better way of doing this proxy check.
                            Type entityType = auditable.Entity.GetType();
                            if (entityType.Namespace == "System.Data.Entity.DynamicProxies")
                            {
                                entityType = entityType.BaseType;
                            }

                            if (auditTypes.ContainsKey(entityType) && auditTypes[entityType].AuditEntityType != null)
                            {
                                audits.Add(this.AuditEntity(auditable, auditTypes[entityType], updateDateTime.Value, user));
                            }
                        }

                        // Update the auditable entity.
                        if (auditable.State != EntityState.Deleted)
                        {
                            auditable.Entity.Updated = updateDateTime.Value;
                            auditable.Entity.UpdateUser = user;
                        }
                    }
                }

                // Perform the updates.
                try
                {
                    return await base.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // Updated failed so remove the audit entities.
                    foreach (var item in audits)
                    {
                        this.Set(item.GetType()).Remove(item);
                    }


                    throw;
                }
            }
            else
            {
                return await base.SaveChangesAsync();
            }
        }

        private IAuditEntity AuditEntity(DbEntityEntry entityEntry, AuditTypeInfo auditTypeInfo, DateTimeOffset auditDateTime, string user)
        {
            // Create audit entity.
            DbSet set = this.Set(auditTypeInfo.AuditEntityType);
            IAuditEntity auditEntity = set.Create() as IAuditEntity;

            // Check audit entity for complex types.
            var complexTypes = GetComplexTypes(auditTypeInfo, entityEntry);

            //Instantiate complex types.
            foreach (var field in complexTypes)
            {
                var property = auditEntity.GetType().GetProperty(field);
                var entityType = property.PropertyType;
                property.SetValue(auditEntity, Activator.CreateInstance(entityType));
            }

            //Attach audit Entity to the context.
            set.Add(auditEntity);

            // Copy the properties.
            DbEntityEntry auditEntityEntry = this.Entry(auditEntity);
            foreach (string propertyName in auditTypeInfo.AuditProperties)
            {
                auditEntityEntry.Property(propertyName).CurrentValue = entityEntry.Property(propertyName).OriginalValue;
            }

            // Set the audit columns.
            auditEntityEntry.Property(AuditUpdatedColumnName).CurrentValue = auditDateTime;
            auditEntityEntry.Property(AuditUserColumnName).CurrentValue = user;
            auditEntityEntry.Property(AuditTypeColumnName).CurrentValue = entityEntry.State == EntityState.Modified ? "update" : "delete";
            auditEntityEntry.Property(AuditSourceIdColumnName).CurrentValue = entityEntry.OriginalValues.GetValue<int>("Id");

            return auditEntity;
        }

        /// <summary>
        /// Checks the destination object for complex types. Returns a list of fields that are complex types.
        /// </summary>
        /// <param name="auditTypeInfo"></param>
        /// <param name="entityEntry"></param>
        /// <returns></returns>
        private ICollection<string> GetComplexTypes(AuditTypeInfo auditTypeInfo, DbEntityEntry entityEntry)
        {
            var returnValue = new List<string>();

            foreach (string propertyName in auditTypeInfo.AuditProperties)
            {
                if (entityEntry.Property(propertyName).GetType().Name.Contains("DbComplexPropertyEntry"))
                {
                    returnValue.Add(propertyName);
                }
            }

            return returnValue;
        }
    }
}
