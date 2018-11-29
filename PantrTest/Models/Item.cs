using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PantrTest.Models.ViewModels;

namespace PantrTest.Models
{
    public class Item
    {
        public string Giver { get; set; }
        public string Date { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string Quantity { get; set; }
        public MaterialViewModel Material { get; set; }
        public string Address { get; set; }
    }
}