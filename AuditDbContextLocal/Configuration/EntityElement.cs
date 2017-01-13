//-----------------------------------------------------------------------
// <copyright file="EntityElement.cs" company="Russell Wilkins">
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
    using System.Configuration;

    internal class EntityElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        internal string Name
        {
            get
            {
                return (string)this["name"];
            }

            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("audit", IsRequired = false)]
        internal string Audit
        {
            get
            {
                return (string)this["audit"];
            }

            set
            {
                this["audit"] = value;
            }
        }
    }
}
