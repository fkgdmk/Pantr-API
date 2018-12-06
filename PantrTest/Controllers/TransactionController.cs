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
        [Route("api/transaction/users/{userId:int}")]
        public HttpResponseMessage GetUserReservation(int userId)
        {
            HttpResponseMessage message = null;
            List<JObject> reservations = new List<JObject>();

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                var userReservationsFromDb = db.tbl_Transaction.Where(c => c.FK_Panter == userId && c.Annulled == false)
                                                               .Select(p => p.tbl_Post).ToList();
                if (userReservationsFromDb != null)
                {
                    foreach (var item in userReservationsFromDb)
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
                } else
                {
                    message = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            return message;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        public async Task<HttpResponseMessage> Put(HttpRequestMessage data)
        {
            HttpResponseMessage message = null;

            var jObject = await data.Content.ReadAsAsync<JObject>();    
            int postId = (int)jObject["postId"];
            int panterId = (int)jObject["panterId"];

            try
            {
                using (PantrDatabaseEntities db = new PantrDatabaseEntities())
                {
                    var transactionToDelete = db.tbl_Transaction.FirstOrDefault(c => c.FK_Panter == panterId && c.FK_Post == postId);

                    transactionToDelete.tbl_Post.Claimed = false;
                    transactionToDelete.Annulled = true;

                    db.SaveChanges();
                }
                message = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                message = Request.CreateResponse(HttpStatusCode.NotModified);
            }
           
            return message;
        }

        
        public void Delete(int id)
        {
            
        }
    }
}