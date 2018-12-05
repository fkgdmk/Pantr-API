using Newtonsoft.Json.Linq;
using PantrTest.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PantrTest.Controllers
{
    public class TransactionController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("api/users/reservation/{userId:int}")]
        public HttpResponseMessage GetUserReservation(int userId)
        {
            HttpResponseMessage message = null;
            List<JObject> reservations = new List<JObject>();

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                var fromDb = db.tbl_Transaction.Where(c => c.FK_Panter == userId).Select(p => p.tbl_Post).ToList();
                if (fromDb != null)
                {
                    foreach (var item in fromDb)
                    {
                        TimeSpan start = TimeSpan.FromMinutes((int)item.StartTime);
                        TimeSpan end = TimeSpan.FromMinutes((int)item.EndTime);
                        DateTime date = (DateTime)item.Date;

                        JObject j = new JObject();
                        j.Add("Quantity", item.Quantity.ToString());
                        j.Add("Address", item.tbl_User.tbl_Address.Address + ", " +
                                         item.tbl_User.tbl_Address.tbl_City.Zip + " " +
                                         item.tbl_User.tbl_Address.tbl_City.City);
                        j.Add("PeriodForPickup", "Kl " + start.ToString("hh':'mm") + " - " + end.ToString("hh':'mm"));
                        j.Add("Date", date.ToString("dd-MM-yyyy"));
                        j.Add("Material", item.tbl_Material.Type);
                        j.Add("Id", item.PK_Post);   
                        reservations.Add(j);
                    }
                    message = Request.CreateResponse(HttpStatusCode.OK, reservations);
                }
            }

            return message;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
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