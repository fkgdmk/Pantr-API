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

        // GET api/<controller>/5
        public UserViewModel Get(int id)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_User foundUser = new tbl_User();
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
                        Address = foundUser.tbl_Address.Address,
                        City = new CityViewModel()
                        {
                            City = foundUser.tbl_Address.tbl_City.City,
                            Zip = foundUser.tbl_Address.tbl_City.Zip
                        }
                    }
                };
                return user;
            }
        }

        // POST api/values
        public HttpResponseMessage Post(UserViewModel register)
        {
            var message = Request.CreateResponse(HttpStatusCode.Accepted, register);
            return message;
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
