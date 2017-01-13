//-----------------------------------------------------------------------
// <copyright file="IAuditEntity.cs" company="Russell Wilkins">
//     Copyright (c) Russell Wilkins 2012. All Rights Reserved.
// </copyright>
// <author>Russell Wilkins</author>
// <license href="license.txt">
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </license>
//-----------------------------------------------------------------------
namespace EntityFramework.Auditing
{
    using System;

    /// <summary>
    /// Any class used for auditing an entity must implement this inteface.
    /// </summary>
    public interface IAuditEntity
    {
        /// <summary>
        /// Gets or sets the DateTime the entity was last updated.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        DateTimeOffset Updated { get; set; }

        /// <summary>
        /// Gets or sets the User who last updated the entity on the database.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        string UpdateUser { get; set; }

        /// <summary>
        /// Gets or sets the DateTime this audit entity was created.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        DateTimeOffset Audited { get; set; }

        /// <summary>
        /// Gets or sets the user who updated the entity
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        string AuditUser { get; set; }

        /// <summary>
        /// Gets or sets the type of audit. update or delete.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        string AuditType { get; set; }

        int AuditSourceId { get; set; }
    }
}
