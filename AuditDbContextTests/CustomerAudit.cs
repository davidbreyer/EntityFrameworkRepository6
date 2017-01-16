//-----------------------------------------------------------------------
// <copyright file="CustomerAudit.cs" company="Russell Wilkins">
//     Copyright (c) Russell Wilkins. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EntityFramework.Auditing.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Describe the class
    /// </summary>
    public class CustomerAudit : AuditEntity
    {

        public virtual int Id { get; set; }
        //public virtual int CustomerAuditId { get; protected set; }

        //public virtual int CustomerId { get; private set; }
        public virtual string CustomerName { get; set; }
    }
}
