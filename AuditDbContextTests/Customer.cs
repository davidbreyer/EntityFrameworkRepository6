//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Russell Wilkins">
//     Copyright (c) Russell Wilkins. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EntityFramework.Auditing.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TODO: Describe the class
    /// </summary>
    public class Customer : AuditableEntity
    {
        //public virtual int CustomerId { get; protected set; }

        public virtual int Id { get; set; }
        public virtual string CustomerName { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }
    }
}
