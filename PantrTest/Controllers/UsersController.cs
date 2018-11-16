using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Sql;
using System.Data.SqlClient;
using PantrTest.Models.DataModels;
using PantrTest.Models.ViewModels;

namespace PantrTest.Controllers
{

    public class UsersController : ApiController
    {
        public List<UserViewModel> Get()
        {
            List<tbl_User> usersFromDb = new List<tbl_User>();
            List<UserViewModel> users = new List<UserViewModel>();
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                usersFromDb = db.tbl_User.Select(c => c).ToList();
                foreach (var user in usersFromDb)
                {
                    users.Add(new UserViewModel()
                    {
                        Firstname = user.Firstname,
                        Surname = user.Surname,
                        Phone = user.Phone,
                        Email = user.Email,
                        IsPanter = (bool)user.IsPanter,
                        Address = new AddressViewModel()
                        {
                            Address = user.tbl_Address.Address,
                            City = new CityViewModel()
                            {
                                City = user.tbl_Address.tbl_City.City,
                                Zip = user.tbl_Address.tbl_City.Zip
                            }
                        }
                    });
                }
            }

            return users;
        }

        public string Get(int id)
        {
            string toReturn = "New user with id " + id;

            return toReturn;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            Console.WriteLine("Yoyo");
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
