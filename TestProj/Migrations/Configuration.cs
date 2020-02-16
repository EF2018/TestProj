namespace TestProj.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TestProj.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TestProj.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TestProj.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role_admin = new IdentityRole { Name = "admin" };
            var role_user1 = new IdentityRole { Name = "user1" };
            var role_user2 = new IdentityRole { Name = "user2" };

            roleManager.Create(role_admin);
            roleManager.Create(role_user1);
            roleManager.Create(role_user2);

            // create Admin
            var admin = new ApplicationUser
            {
                Email = "test_admin@ukr.net",
                UserName = "test_admin@ukr.net"
            };
            string password = "12345_aB";

            var result = userManager.Create(admin, password);

            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role_admin.Name);
            }

            //Create User1
            ApplicationUser user1 = new ApplicationUser
            {
                Email = "test_user1@ukr.net",
                UserName = "test_user1@ukr.net",
                Report1 = true
            };

            result = userManager.Create(user1, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(user1.Id, role_user1.Name);
            }

            //Create User2
            ApplicationUser user2 = new ApplicationUser
            {
                Email = "test_user2@ukr.net",
                UserName = "test_user2@ukr.net",
                Report2 = true
            };

            result = userManager.Create(user2, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(user2.Id, role_user2.Name);
            }

            //
            user2.Report2 = true;
            user1.Report1 = true;

            userManager.Update(admin);
            userManager.Update(user1);
            userManager.Update(user2);

            //Create some cities
            City dnepr = new City() { Name = "Dnepr" };
            City cherson = new City() { Name = "Cherson" };
            City cherkasi = new City() { Name = "Cherkasi" };
            City charkov = new City() { Name = "Charkov" };

            context.Cities.AddOrUpdate(dnepr);
            context.Cities.AddOrUpdate(cherson);
            context.Cities.AddOrUpdate(cherkasi);
            context.Cities.AddOrUpdate(charkov);

            //Create records for reports
            PlanRecord record = new PlanRecord()
            {
                From = dnepr,
                To = cherson,
                Month = new DateTime(2014, 1, 1),
                Number = 30
            };

            record.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 1), Number = 4 });
            record.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 2), Number = 2});
            record.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 3), Number = 3});
            record.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 4), Number = 1});
            record.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 5), Number = 5});

            PlanRecord record1 = new PlanRecord()
            {
                From = dnepr,
                To = cherkasi,
                Month = new DateTime(2014, 2, 1),
                Number = 55
            };

            record1.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 1), Number = 4 });
            record1.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 2), Number = 2});
            record1.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 3), Number = 3});
            record1.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 4), Number = 1});
            record1.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 5), Number = 5});

            PlanRecord record2 = new PlanRecord()
            {
                From = dnepr,
                To = charkov,
                Month = new DateTime(2014, 1, 1),
                Number = 62
            };

            record2.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 1), Number = 5 });
            record2.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 2), Number = 5});
            record2.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 3), Number = 5});
            record2.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 4), Number = 5});
            record2.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 5), Number = 5});

            PlanRecord record3 = new PlanRecord()
            {
                From = charkov,
                To = dnepr,
                Month = new DateTime(2014, 1, 1),
                Number = 13,
            };

            record3.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 1), Number = 6});
            record3.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 2), Number = 6});
            record3.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 3), Number = 6});

            PlanRecord record4 = new PlanRecord()
            {
                From = charkov,
                To = cherkasi,
                Month = new DateTime(2014, 1, 1),
                Number = 12
            };

            record4.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 1), Number = 2});
            record4.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 1, 2), Number = 2});

            PlanRecord record5 = new PlanRecord()
            {
                From = cherkasi,
                To = charkov,
                Month = new DateTime(2014, 2, 1),
                Number = 13
            };

            record5.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 1), Number = 3});
            record5.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 2), Number = 3});
            record5.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 1), Number = 3});
            record5.DayFactRecords.Add(new FactRecord() { Date = new DateTime(2014, 2, 2), Number = 3});

            context.PlanRecords.AddOrUpdate(record);
            context.PlanRecords.AddOrUpdate(record1);
            context.PlanRecords.AddOrUpdate(record2);
            context.PlanRecords.AddOrUpdate(record3);
            context.PlanRecords.AddOrUpdate(record4);
            context.PlanRecords.AddOrUpdate(record5);

            base.Seed(context);
        }
    }
}
