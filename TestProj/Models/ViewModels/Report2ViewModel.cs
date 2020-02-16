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
    public class Report2ViewModel 
    {
        public string NameReport { get; set; }
        public int Id { get; set; }
        public string User { get; set; }

        [Display(Name = "Откуда")]
        public string From { get; set; }

        [Display(Name = "Куда")]
        public List<ConsolidatePlan> To { get; set; } = new List<ConsolidatePlan>();

        [Display(Name = "План")]
        public int Plan { get; set; }
        [Display(Name = "Факт")]
        public int Fact { get; set; }

 
    }

   
}