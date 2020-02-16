using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TestProj.Models.Entities;

namespace TestProj.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        //public ICollection<Report> AcceptedReports { get; set; }
        public bool Report1 { get; set; }
        public bool Report2 { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    //[Table("Reports")]
    //public class Report
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string NameReport { get; set; }
    //    public virtual ICollection<ApplicationUser> Users { get; set; }

    //    public Report()
    //    {
    //        this.Users = new List<ApplicationUser>();
    //    }
    //}


    [Table("Cities")]
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }


    [Table("PlanRecords")]
    public class PlanRecord
    {
        public int Id { get; set; }
        public DateTime Month { get; set; }
        public City From { get; set; } 
        public City To { get; set; }
        public int Number { get; set; }
        public virtual ICollection<FactRecord> DayFactRecords { get; set; }
        public PlanRecord()
        {
            this.DayFactRecords = new List<FactRecord>();
        }
    }

    [Table("FactRecords")]
    public class FactRecord
    {
        public int Id { get; set; }
        public PlanRecord PlanRecord { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int Number { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<FactRecord> FactRecords { get; set; }
        public DbSet<PlanRecord> PlanRecords { get; set; }
        public DbSet<City> Cities { get; set; }
        //public DbSet<Report> Reports { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        
    }
}