//-----------------------------------------------------------------------
// <copyright file="AuditConfigurationSection.cs" company="Russell Wilkins">
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

    internal class AuditConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("entities", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EntityElementCollection),
            AddItemName = "add")]
        internal EntityElementCollection Entities
        {
            get
            {
                return (EntityElementCollection)base["entities"];
            }
        }

        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        internal bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }

            set
            {
                this["enabled"] = value;
            }
        }
    }
}
