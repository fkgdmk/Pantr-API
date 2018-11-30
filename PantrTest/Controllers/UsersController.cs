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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
        public async Task<HttpResponseMessage> Post(HttpRequestMessage registerData)
        {
            var jObject = await registerData.Content.ReadAsAsync<JObject>();
            UserViewModel registerUser = JsonConvert.DeserializeObject<UserViewModel>(jObject.ToString());

            HttpResponseMessage message = null;

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                var city = db.tbl_City.FirstOrDefault(c => c.Zip == registerUser.Address.City.Zip).PK_City;
                tbl_Login registerLogin = new tbl_Login()
                {
                    Username = registerUser.Login.Username,
                    Password = registerUser.Login.Password
                };

                tbl_Address registerAddress = new tbl_Address()
                {
                    Address = registerUser.Address.Address,
                    FK_City = city
                };

                db.tbl_Login.Add(registerLogin);
                db.tbl_Address.Add(registerAddress);
                db.SaveChanges();

                tbl_User newUser = new tbl_User()
                {
                    Firstname = registerUser.Firstname,
                    Surname = registerUser.Surname,
                    Email = registerUser.Email,
                    Phone = registerUser.Phone,
                    FK_Address = db.tbl_Address.FirstOrDefault(c => c.Address == registerAddress.Address).PK_Address,
                    IsPanter = registerUser.IsPanter,
                    FK_Login = db.tbl_Login.FirstOrDefault(c => c.Username == registerLogin.Username).PK_Login
                };

                db.tbl_User.Add(newUser);
                db.SaveChanges();
                //I stedet for at returnere newUser som poco, skal det måske laves som JObject(se logincontroller)
                message = Request.CreateResponse(HttpStatusCode.OK, newUser);
            }

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
