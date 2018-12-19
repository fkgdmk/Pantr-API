using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PantrTest.Models.DataModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PantrTest.Controllers
{

    public class UsersController : ApiController
    {
        //Registrer bruger
        public async Task<HttpResponseMessage> Post(HttpRequestMessage registerData)
        {
            var jObject = await registerData.Content.ReadAsAsync<JObject>();
            tbl_User registerUser = JsonConvert.DeserializeObject<tbl_User>(jObject.ToString());

            HttpResponseMessage message = null;

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                try
                {
                    //Fordi man kun angiver sit postnummer i xamarin finder vi her den by som matcher postnummeret
                    //og returnerer primary key
                    var city = db.tbl_City.FirstOrDefault(c => c.Zip == registerUser.tbl_Address.tbl_City.Zip).PK_City;

                    //Opretter objecter med de korrekte værdier og tilføjer den nye bruger til databasse
                    tbl_Login registerLogin = new tbl_Login()
                    {
                        Username = registerUser.tbl_Login.Username,
                        Password = registerUser.tbl_Login.Password
                    };

                    tbl_Address registerAddress = new tbl_Address()
                    {
                        Address = registerUser.tbl_Address.Address,
                        FK_City = city
                    };

                    //EF tillader man bare refererer forbindelsen mellem tabeller (tbl_Adress og tbl_Login)
                    //og sætter dem til de nye objekter. Så opdaterer EF selv med de rigtige foreign keys
                    tbl_User newUser = new tbl_User()
                    {
                        Firstname = registerUser.Firstname,
                        Surname = registerUser.Surname,
                        Email = registerUser.Email,
                        Phone = registerUser.Phone,
                        tbl_Address = registerAddress,
                        IsPanter = registerUser.IsPanter,
                        tbl_Login = registerLogin
                    };

                    db.tbl_User.Add(newUser);
                    db.SaveChanges();

                    message = Request.CreateResponse(HttpStatusCode.OK);

                }
                //Hvis fejl, sæt statuscode til andet en OK (her badrequest)
                catch (Exception e)
                {
                    message = Request.CreateResponse(HttpStatusCode.BadRequest);
                }                

            }

            return message;
        }
    }
}
