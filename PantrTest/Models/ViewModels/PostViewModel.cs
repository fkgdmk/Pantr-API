using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public MaterialViewModel Material { get; set; }
        public UserViewModel Giver { get; set; }
        public PostQuantityViewModel PostQuantity { get; set; }
        public string Address { get; set; } 
<<<<<<< HEAD
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
=======
        public int StartTime { get; set; }
        public int EndTime { get; set; }
>>>>>>> TokeDev
        public bool Claimed { get; set; }
        public bool Completed { get; set; }
        public string Date { get; set; }

    }
}