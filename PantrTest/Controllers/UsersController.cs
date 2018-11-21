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

        public UserViewModel Get(int id)
        {
            tbl_User foundUser = new tbl_User();
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                foundUser = db.tbl_User.Find(id);
                UserViewModel user = new UserViewModel()
                {
                    Firstname = foundUser.Firstname,
                    Surname = foundUser.Surname,
                    Phone = foundUser.Phone,
                    Email = foundUser.Email,
                    IsPanter = (bool)foundUser.IsPanter,
                    Address = new AddressViewModel()
                    {
                        Address = db.tbl_Address.Find(foundUser.FK_Address).Address,
                        City = new CityViewModel()
                        {
                            City = db.tbl_City.Find(db.tbl_Address.Find(foundUser.FK_Address).FK_City).City,
                            //Zip = db.tbl_City.Find(db.tbl_Address.Find(foundUser.FK_Address)).Zip
                        }
                    }
                };
                return user;
            }
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
