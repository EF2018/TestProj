using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestProj.Models.Entities
{
    public class Delivery
    {
        public City From { get; set; }
        public City To { get; set; }
    }
}