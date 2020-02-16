using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestProj.Models.Entities
{
    public class ConsolidatePlan
    {
        public Delivery FromTo { get; set; }
        public int? PlanNumber { get; set; }
        public int? FactNumber { get; set; }    
    }
}