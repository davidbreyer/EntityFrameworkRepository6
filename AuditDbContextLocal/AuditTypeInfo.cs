//-----------------------------------------------------------------------
// <copyright file="AuditTypeInfo.cs" company="Russell Wilkins">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class AuditTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the AuditTypeInfo class.
        /// </summary>
        internal AuditTypeInfo()
        {
            this.AuditProperties = new Collection<string>();
        }

        internal Type EntityType { get; set; }

        internal Type AuditEntityType { get; set; }

        internal Collection<string> AuditProperties { get; set; }
    }
}
