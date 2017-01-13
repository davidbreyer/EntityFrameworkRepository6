//-----------------------------------------------------------------------
// <copyright file="IAuditableEntity.cs" company="Russell Wilkins">
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
    /// Any entity class that is to be audited by AuditDbContext 
    /// must implement this inteface.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the DateTime the entity was last updated.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        DateTimeOffset? Updated { get; set; }

        /// <summary>
        /// Gets or sets the User who last updated the entity on the database.
        /// Will be automatically set by AuditDbContext on SaveChanges.
        /// </summary>
        string UpdateUser { get; set; }
    }
}
