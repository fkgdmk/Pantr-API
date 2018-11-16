using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class TransactionViewModel
    {
        public PostViewModel Post { get; set; }
        public string Panter { get; set; } //skift type
        public bool Collected { get; set; }
        public bool Annulled { get; set; }
    }
}