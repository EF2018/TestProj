using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TestProj.Models.Entities;

namespace TestProj.Models.ViewModels
{

    public interface IReportViewModel
    {
        string NameReport { get; }
        List<IReportViewModel> Get(ApplicationDbContext context);
    }

    [NotMapped]
    public class Report2ViewModel //: IReportViewModel 
    {
        public string NameReport { get; set; }
        public int Id { get; set; }
        //[Required]
        //[Display(Name = "Период")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime Period { get; set; }
        public string User { get; set; }

        [Display(Name = "Откуда")]
        public string From { get; set; }

        [Display(Name = "Куда")]
        public List<ConsolidatePlan> To { get; set; } = new List<ConsolidatePlan>();

        [Display(Name = "План")]
        public int Plan { get; set; }
        [Display(Name = "Факт")]
        public int Fact { get; set; }

        //public List<IReportViewModel> Get(ApplicationDbContext context)
        //{
        //    List<IReportViewModel> list = new List<IReportViewModel>();
        //    var grouped = context.PlanRecords.GroupBy(d => new { d.From, d.To })
        //       .Select(group => new ConsolidatePlan() { FromTo = new Delivery() { From = group.Key.From, To = group.Key.To }, FactNumber = group.Sum(fact => fact.DayFactRecords.Sum(x => x.Number)), PlanNumber = group.Sum(plan => plan.Number) }).ToList();
        //    var newgrouped = grouped.GroupBy(d => new { d.FromTo.From }).ToList()
        //        .Select(group => new Report2ViewModel() { From = group.Key.From.Name, To = group.ToList() }).ToList();
        //    list.AddRange(newgrouped);
        //    return list;
        //}
    }

   
}