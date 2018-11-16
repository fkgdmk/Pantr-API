using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class ReportViewModel
    {
        public UserViewModel Reporter { get; set; }
        public UserViewModel Reported { get; set; }
        public string Report { get; set; }
        public DateTime Date { get; set; }
    }
}