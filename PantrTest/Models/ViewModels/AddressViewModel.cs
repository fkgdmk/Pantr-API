using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class AddressViewModel
    {
        public string Address { get; set; }
        public CityViewModel City { get; set; }
    }
}