using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestProj.Models.ViewModels
{
    public class CreateNewPlanViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Период")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Period { get; set; }
        public string User { get; set; }

        [Display(Name = "Откуда")]
        public string From { get; set; }
        [Display(Name = "Куда")]
        public string To { get; set; }
        [Display(Name = "План")]
        public int Plan { get; set; }
    }
}