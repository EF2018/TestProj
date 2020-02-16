using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TestProj.Models.Entities;

namespace TestProj.Models.ViewModels
{
    

    [NotMapped]
    public class Report1ViewModel //:IReportViewModel
    {
        public int Id { get; set; }
        public string NameReport { get; set; }
        [Required]
        [Display(Name = "Период")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Period { get; set; }
        public string User { get; set; }

        [Display(Name = "Откуда")]
        public string From { get; set; }
        [Display(Name = "Куда")]
        public string To { get; set; }
        [Display(Name = "План")]
        public int Plan { get; set; }
        public List<FactRecord> DayFactRecords { get; set; }

        //public List<IReportViewModel> Get(ApplicationDbContext context)
        //{
        //    List<IReportViewModel> reports = new List<IReportViewModel>();
        //    var list = context.PlanRecords.Include("From")
        //                 .Include("To")
        //                 .Include("DayFactRecords").ToList();
        //    if (from != 0 && from != null)
        //    {
        //        list = list.Where(x => x.From.Id == from).ToList();
        //    }
        //    if (to != 0 && to != null)
        //    {
        //        list = list.Where(x => x.To.Id == to).ToList();
        //    }

        //    foreach (var item in list)
        //    {
        //        Report1ViewModel report1 = new Report1ViewModel()
        //        {
        //            Id = item.Id,
        //            From = item.From.Name,
        //            To = item.To.Name,
        //            Period = item.Month,
        //            Plan = item.Number,
        //            DayFactRecords = context.FactRecords.Where(x => x.PlanRecord.Id == item.Id).ToList()
        //        };
        //        reports.Add(report1);
        //    }
        //    return reports;
        //}
    }

   
}