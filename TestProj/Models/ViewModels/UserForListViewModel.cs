using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestProj.Models.ViewModels
{
    public class UserForListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool Report1 { get; set; }
        public bool Report2 { get; set; }
    }
}