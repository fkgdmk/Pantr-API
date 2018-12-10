using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PantrTest.Models.ViewModels;
using PantrTest.Models.DataModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PantrTest.Controllers
{
    public class LoginController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "yo" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "this is the id: " + id;
        }

        // POST api/<controller>
        public async Task<HttpResponseMessage> Post(HttpRequestMessage data)
        {
            var jObject = await data.Content.ReadAsAsync<JObject>();
            tbl_Login login = JsonConvert.DeserializeObject<tbl_Login>(jObject.ToString());
            HttpResponseMessage message = null;
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                //forsøger at finde bruger i DB, der har det brugernavn der logges ind med
                var userFromDb = db.tbl_User.FirstOrDefault(c => c.tbl_Login.Username == login.Username);

                //hvis brugeren findes, tjekkes der om passwordet matcher og sætter derefter variable i JObject
                //som skal bruges i xamarin
                if (userFromDb != null && userFromDb.tbl_Login.Password.Equals(login.Password))
                {
                    JObject authenticatedUser = new JObject();
                    authenticatedUser.Add("ID", userFromDb.PK_User);
                    authenticatedUser.Add("Username", userFromDb.tbl_Login.Username);

                    message = Request.CreateResponse(HttpStatusCode.OK, authenticatedUser);

                }
                //Hvis ikke den findes returneres NotFound
                else
                    message = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return message;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}