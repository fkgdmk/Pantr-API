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
        public HttpResponseMessage Post(LoginViewModel login)
        {
            HttpResponseMessage message = null;
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                var userFromDb = db.tbl_Login.FirstOrDefault(c => c.Username == login.username);

                if (userFromDb != null && userFromDb.Password.Equals(login.password))
                    message = Request.CreateResponse(HttpStatusCode.OK, login);
                else
                    message = Request.CreateResponse(HttpStatusCode.NotFound, login);
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