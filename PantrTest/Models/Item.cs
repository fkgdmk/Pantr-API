using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models
{
    public class Item
    {
        public string Giver { get; set; }
        public string Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Quanity { get; set; }
        public string QuanityType { get; set; }
        public string Material { get; set; }
        public string Address { get; set; }
    }
}