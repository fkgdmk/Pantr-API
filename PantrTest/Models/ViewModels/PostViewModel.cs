using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class PostViewModel
    {
        public MaterialViewModel Material { get; set; }
        public string Giver { get; set; } //Type
        public string PostQuantity { get; set; } //Type
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool Claimed { get; set; }
        public bool Completed { get; set; }
        public string Address { get; set; } //Skift type
        public DateTime Date { get; set; }

    }
}