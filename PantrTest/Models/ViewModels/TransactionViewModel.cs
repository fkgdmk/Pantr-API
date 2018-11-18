using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class TransactionViewModel
    {
        public PostViewModel Post { get; set; }
        public UserViewModel Panter { get; set; }
        public bool Collected { get; set; }
        public bool Annulled { get; set; }
    }
}