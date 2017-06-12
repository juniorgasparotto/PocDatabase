using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace PocDatabase.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestAll()
        {
            #region Add Permission
            var permiId = Guid.NewGuid();
            var pocFile = new PocFile<Schema>();
            var repoPermission = new PocRepository<Schema, Permission>(pocFile);
            var permission = new Permission { Id = permiId, Name = "Perm1" };
            repoPermission.Insert(permission);

            var permission2 = new Permission { Name = "Perm2" };

            try
            { 
                repoPermission.Insert(permission);
                Assert.Fail("Expect exception");
            }
            catch { }

            repoPermission.Insert(permission2);


            pocFile.Save();
            #endregion

            #region Add Customer
            pocFile = new PocFile<Schema>();
            var repo = pocFile.GetRepository<Customer>();
            var order = new Order { Name = "My first order" };
            var user = new User() { Name = "User1" };
            var permissions = new List<Permission>();
            permissions.Add(permission);
            user.Permissions = permissions;

            var customer = new Customer
            {
                Name = "John Doe",
                Order = order,
                User = user
            };

            repo.Insert(customer);
            pocFile.Save();
            #endregion

            #region Test
            pocFile = new PocFile<Schema>();
            var getPermission = pocFile.GetRepository<Permission>().GetById(permiId);
            var getCustomer = pocFile.GetRepository<Customer>().GetById(customer.Id);
            var allCustomer = pocFile.GetRepository<Customer>().GetAll();
            var allPermission = pocFile.GetRepository<Permission>().GetAll();

            Assert.IsTrue(permission.Id == getPermission.Id);
            Assert.IsTrue(allCustomer.Count() == 1);
            Assert.IsTrue(allPermission.Count() == 2);
            Assert.IsTrue(customer.Id == getCustomer.Id);
            #endregion

            #region Edit and delete

            pocFile = new PocFile<Schema>();
            var repoPerm = pocFile.GetRepository<Permission>();
            var repoCust = pocFile.GetRepository<Customer>();
            getCustomer.Name = "Edited";
            getCustomer.User.Name = "User edited";
            repoCust.Update(getCustomer);
            repoPerm.Delete(permission2);
            pocFile.Save();
            #endregion

            #region Test
            pocFile = new PocFile<Schema>();
            getCustomer = pocFile.GetRepository<Customer>().Get(f=>f.Id == customer.Id).First();
            allPermission = pocFile.GetRepository<Permission>().GetAll();

            Assert.IsTrue(allCustomer.Count() == 1);
            Assert.IsTrue(allPermission.Count() == 1);
            Assert.IsTrue(getCustomer.Id == customer.Id);
            Assert.IsTrue(getCustomer.Name == "Edited");
            Assert.IsTrue(getCustomer.User.Name == "User edited");
            #endregion

            pocFile = new PocFile<Schema>();
            pocFile.Truncate();
        }

        public class Customer
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public Order Order { get; set; }
            public User User { get; set; }
        }

        public class Order
        {
            public string Name { get; set; }
        }

        public class User
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public List<Permission> Permissions { get; set; }
        }

        public class Permission
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Schema
        {
            public List<Customer> Customers { get; set; }
            public List<User> Users { get; set; }
            public List<Permission> Permissions { get; set; }
        }
    }
}
