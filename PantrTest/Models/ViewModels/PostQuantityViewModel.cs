using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class PostQuantityViewModel
    {
        public QuantityTypeViewModel QuantityType { get; set; }
        public int Quantity { get; set; }
    }
}