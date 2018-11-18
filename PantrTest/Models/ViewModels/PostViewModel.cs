using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class PostViewModel
    {
        public MaterialViewModel Material { get; set; }
        public UserViewModel Giver { get; set; }
        public PostQuantityViewModel PostQuantity { get; set; }
        public AddressViewModel Address { get; set; } 
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool Claimed { get; set; }
        public bool Completed { get; set; }
        public DateTime Date { get; set; }

    }
}