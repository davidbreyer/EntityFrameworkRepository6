//-----------------------------------------------------------------------
// <copyright file="CodeFirstDbContext.cs" company="Russell Wilkins">
//     Copyright (c) Russell Wilkins. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EntityFramework.Auditing.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Entity;

    /// <summary>
    /// TODO: Describe the class
    /// </summary>
    public class Context : AuditDbContext
    {
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<CustomerAudit> CustomerAudits { get; set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CodeFirstDbContext class.
        /// </summary>
        public Context(bool proxies, string conn)
            : base(conn)
        {
            this.Configuration.ProxyCreationEnabled = proxies;
        }
        #endregion
    }
}
