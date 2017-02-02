## Summary
 
AuditDbContext lets you easily implement change auditing on your application entities.  Instead of using Entity Frameworks DbContext you derive your context from AuditDbContext, which will automatically write change audit records to the database for the registered audit types whenever SaveChanges is called.
 
An audit record is written whenever an update of delete operation is applied to an entity and stores the time of the change and the user making the change along with the original entity values. 
 
You end up with a complete audit trail changes to your entity.
  
## Implementing

Just a few simple step to implement:
1. Implement IAuditableEntity on your entity, or use the AuditableEntity base class.
2. Define an audit history entity implementing IAuditEntity, or use the AuditEntry base class.
3. Derive you context from AuditDbContext. 
4. Register the audit types either through app.config, or through code using the RegisterAuditType method.


Here’s a quick guide to getting started with AuditDbContext.
 Add a reference to AuditDbContext in you project. 

Derive you entities from AuditableEntity, or implement the IAuditable interface.     
```csharp
public class Customer : AuditableEntity
    {
        public virtual int CustomerId { get; protected set; }
        public virtual string CustomerName { get; set; }
    }
```
 
Derive you audit entities from AuditEntity, or implement the IAudit interface.     
```csharp
public class CustomerAudit : AuditEntity
    {
        public virtual int CustomerAuditId { get; protected set; }
        public virtual int CustomerId { get; private set; }
        public virtual string CustomerName { get; set; }
    }
 ```
Derive your data context from AuditDbContext instead of DbContext.     
```csharp
public class Context : AuditDbContext
    {
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<CustomerAudit> CustomerAudits { get; set; }
        public Context(string conn)
            : base(conn)
        {
        }
    } 
```
Either: Add each of you auditable classes with their audit entity to the config file.   
```xml
<configSections>
    <section name="entityFramework.Audit" type="EntityFramework.Auditing.AuditConfigurationSection, EntityFramework.Auditing" />
  </configSections>
  <entityFramework.Audit>
      <entities>
        <add name="EntityFramework.Auditing.Test.Customer, EntityFramework.Auditing.Test" audit="EntityFramework.Auditing.Test.CustomerAudit, EntityFramework.Auditing.Test" />
      </entities>
  </entityFramework.Audit> 
```
Or register the audit entities in code using the AuditDbContext.RegisterAuditType method. 
```csharp
AuditDbContext.RegisterAuditType(typeof(Customer), typeof(CustomerAudit));
```


