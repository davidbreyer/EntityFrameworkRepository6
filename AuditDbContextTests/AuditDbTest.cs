using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlServerCe;
using System.IO;
using System.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Auditing.Test
{
    /// <summary>
    /// Summary description for CodeFirstTest
    /// </summary>
    [TestClass]
    public class AuditDbTest
    {
        public AuditDbTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            //File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\AuditDbContextUnitTest.sdf");
            var initializer = new DropCreateDatabaseAlways<Context>();
            Database.SetInitializer<Context>(initializer);
            //Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\AuditDbContextUnitTest.sdf");
        }

        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            this.ClearDatabase();
        }

        #endregion

        private Context CreateContext(bool withProxies)
        {
            this.ClearDatabase();
            Context context = new Context(withProxies, contextConnectionString);
            context.SaveChanges();
            Assert.AreEqual(withProxies, context.Proxies);
            return context;
        }

        //string contextConnectionString = "AuditDbContextUnitTest";
        //string sqlConnectionString = @"Data Source=|DataDirectory|\AuditDbContextUnitTest.sdf;Persist Security Info=False;";
        string user = "UnitTest";

        private string contextConnectionString = ConfigurationManager.ConnectionStrings["AuditDbTest"].ToString();

        private void ClearDatabase()
        {
            try
            {
                using (DbConnection sqlConn = this.GetConnection())
                {
                    if (sqlConn.State == ConnectionState.Closed)
                    {
                        sqlConn.Open();
                    }
                    using (DbCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "truncate table customers";
                        cmd.ExecuteNonQuery();
                    }
                    using (DbCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "truncate table customeraudits";
                        cmd.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
            catch { }
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void CreateTestNoProxy()
        {
            using (Context context = this.CreateContext(false))
            {
                this.CreateTest(context);
            }
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void CreateTestProxy()
        {
            using (Context context = this.CreateContext(true))
            {
                this.CreateTest(context);
            }
        }

        private void CreateTest(Context context)
        {
            // Add the customer
            Customer customer = context.Customers.Create();
            customer.CustomerName = "Customer 1";
            context.Customers.Add(customer);
            context.SaveChanges(user);

            using (DbConnection conn = this.GetConnection())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from customers";
                    using (var r = cmd.ExecuteReader())
                    {
                        int records = 0;
                        while (r.Read())
                        {
                            records++;
                        }
                        r.Close();
                        Assert.AreEqual(1, records);
                    }
                }

                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from customeraudits";
                    using (var r = cmd.ExecuteReader())
                    {
                        int records = 0;
                        while (r.Read())
                        {
                            records++;
                        }
                        r.Close();
                        Assert.AreEqual(0, records);
                    }
                }
                conn.Close();
            }
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void ProxiesFalseTest()
        {
            Context context = new Context(false, contextConnectionString);
            Assert.IsFalse(context.Proxies);
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void ProxiesTrueTest()
        {
            Context context = new Context(true, contextConnectionString);
            Assert.IsTrue(context.Proxies);
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void ConcurrencyTest()
        {
            Context context1 = new Context(true, contextConnectionString);
            Context context2 = new Context(true, contextConnectionString);

            Customer customer1 = context1.Customers.Create();
            customer1.CustomerName = "Customer 1";
            Customer customer3 = context1.Customers.Create();
            customer3.CustomerName = "Customer 3";
            context1.Customers.Add(customer3);
            context1.Customers.Add(customer1);
            context1.SaveChanges(user);

            Customer customer2 = context2.Customers.Find(customer1.Id);
            customer2.CustomerName = "Changed 2";
            context2.SaveChanges(user);

            customer3.CustomerName = "arsd";
            customer1.CustomerName = "Concurrency";

            try
            {
                context1.SaveChanges(user);
            }
            catch
            { }

            context1.Reload(customer1);
            customer1.CustomerName = "Changed after concurrency failure";
            context1.SaveChanges(user);
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void UpdateTestNoProxy()
        {
            Context context = this.CreateContext(false);
            this.UpdateTest(context);
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void UpdateTestProxy()
        {
            Context context = this.CreateContext(true);
            this.UpdateTest(context);
        }

        private void UpdateTest(Context context)
        {
            string customerName = "Updated";
            DateTimeOffset updated = DateTimeOffset.Now;

            DbConnection conn = this.GetConnection();

            var newCustomer = new Customer { CustomerName = "Unit Test", Updated = updated, UpdateUser = "UnitTest" };
            context.Customers.Add(newCustomer);
            context.SaveChanges(user);

            Customer customer = context.Customers.Find(1);
            customer.CustomerName = customerName;
            context.SaveChanges(user);

            // Chech the audit records has been created.
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from customeraudits";
                using (var r = cmd.ExecuteReader())
                {
                    int records = 0;
                    while (r.Read())
                    {
                        records++;
                    }
                    Assert.AreEqual(1, records);
                }
            }

            // Check the audit fields.
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from customeraudits";
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    Assert.AreEqual(1, r["Id"]);
                    Assert.AreEqual(1, r["AuditSourceId"]);
                    Assert.AreEqual(user, r["UpdateUser"]);
                    //Assert.AreEqual(updated, r["Updated"]);
                    Assert.AreEqual("Unit Test", r["CustomerName"]);
                    Assert.AreEqual(user, r["AuditUser"]);
                    Assert.AreEqual("update", r["AuditType"]);
                }
            }
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void DeleteTestNoProxy()
        {
            Context context = this.CreateContext(false);
            this.DeleteTest(context);
        }

        [TestCategory("AuditDbContext")]
        [TestMethod]
        public void DeleteTestProxy()
        {
            Context context = this.CreateContext(true);
            this.DeleteTest(context);
        }

        private DbConnection GetConnection()
        {
            //SqlCeConnection conn = new SqlCeConnection(sqlConnectionString);
            SqlConnection conn = new SqlConnection(contextConnectionString);
            conn.Open();

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"set identity_insert customers on";
                cmd.ExecuteNonQuery();
            }

            return conn;
        }

        private void DeleteTest(Context context)
        {
            DbConnection conn = this.GetConnection();

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Customers(Id, CustomerName,Updated,UpdateUser) VALUES (1, 'Unit Test', '2012-01-01 12:00:00', 'Test')";
                int r = cmd.ExecuteNonQuery();
                Assert.AreEqual(1, r);
            }

            Customer customer = context.Customers.Find(1);
            context.Customers.Remove(customer);
            context.SaveChanges(user);

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from customers";
                using (var r = cmd.ExecuteReader())
                {
                    int records = 0;
                    while (r.Read())
                    {
                        records++;
                    }
                    Assert.AreEqual(0, records);
                }
            }

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from customeraudits";
                using (var r = cmd.ExecuteReader())
                {
                    int records = 0;
                    while (r.Read())
                    {
                        records++;
                    }
                    Assert.AreEqual(1, records);

                }
            }

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from customeraudits";
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    Assert.AreEqual(user, r["AuditUser"]);
                    Assert.AreEqual("delete", r["AuditType"]);
                }
            }
        }

        [TestCategory("AuditDbContext")]
        [TestMethod()]
        public void RegisterAuditTypeTest()
        {
            AuditDbContext context = new AuditDbContext("test");
            AuditDbContext.RegisterAuditType(typeof(TestEntityWithComplexType), typeof(TestEntityWithComplexTypeAudit));
        }

        [TestCategory("AuditDbContext")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Entity")]
        public void RegisterAuditTypeTwiceExceptionTest()
        {
            AuditDbContext context = new AuditDbContext("test");
            AuditDbContext.RegisterAuditType(typeof(TestEntity), typeof(TestEntityAudit));
            AuditDbContext.RegisterAuditType(typeof(TestEntity), typeof(TestEntityAudit));
        }

        [TestCategory("AuditDbContext")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Entity")]
        public void RegisterAuditTypeEntityExceptionTest1()
        {
            AuditDbContext context = new AuditDbContext("test");
            AuditDbContext.RegisterAuditType(typeof(TestInvalidEntity), typeof(TestEntityAudit));
        }

        [TestCategory("AuditDbContext")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Entity")]
        public void RegisterAuditTypeEntityExceptionTest2()
        {
            AuditDbContext context = new AuditDbContext("test");
            AuditDbContext.RegisterAuditType(typeof(TestEntity), typeof(TestInvalidAuditEntity));
        }

        #region Unit test classes
        class TestInvalidEntity
        {

        }

        class TestInvalidAuditEntity
        {

        }

        class TestEntity : IAuditableEntity
        {
            public DateTimeOffset? Updated { get; set; }
            public string UpdateUser { get; set; }
        }

        class TestEntityWithComplexType : IAuditableEntity
        {
            public TestComplexType Address { get; set; }
            public DateTimeOffset? Updated { get; set; }
            public string UpdateUser { get; set; }
        }

        [ComplexType]
        class TestComplexType
        {
            public string AddressLine1 { get; set; }
        }

        class TestEntityAudit : IAuditEntity
        {
            public DateTimeOffset Updated { get; set; }
            public string UpdateUser { get; set; }
            public DateTimeOffset Audited { get; set; }
            public string AuditUser { get; set; }
            public string AuditType { get; set; }
            public int AuditSourceId { get; set; }
        }

        class TestEntityWithComplexTypeAudit : IAuditEntity
        {
            public DateTimeOffset Updated { get; set; }
            public string UpdateUser { get; set; }
            public DateTimeOffset Audited { get; set; }
            public string AuditUser { get; set; }
            public string AuditType { get; set; }
            public int AuditSourceId { get; set; }
        }
        #endregion



    }
}
