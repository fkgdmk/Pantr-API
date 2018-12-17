using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantrTest.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsPanter { get; set; }
        public AddressViewModel Address { get; set; }
        public LoginViewModel Login { get; set; }

    }
}