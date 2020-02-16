using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestProj.Models.ViewModels
{
    public class CreateNewFactRecordViewModel
    {
        public int Id { get; set; }
        public int IdPlanRecord { get; set; }
        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Факт.количество перевозок")]
        public int Number { get; set; }
    }
}