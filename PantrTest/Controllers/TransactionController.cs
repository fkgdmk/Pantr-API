using Newtonsoft.Json.Linq;
using PantrTest.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PantrTest.Controllers
{
    public class TransactionController : ApiController
    {
        //Returnerer alle pantopslag en panter har resereveret baseret på panterens id
        [HttpGet]
        [Route("api/transaction/users/{userId:int}")]
        public HttpResponseMessage GetUserReservation(int userId)
        {
            HttpResponseMessage message = null;
            //Listen som sendes tilbage med responseMessage
            List<JObject> reservations = new List<JObject>();

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                //Returnerer en liste af posts gennem oprettede transaktioner, 
                //hvor ID passer på brugeren og transaktionen ikke er annuleret
                var userReservationsFromDb = db.tbl_Transaction.Where(c => c.FK_Panter == userId && c.Annulled == false)
                                                               .Select(p => p.tbl_Post).ToList();
                if (userReservationsFromDb != null)
                {
                    //Tilføjer JObjects til listen som returneres til xamarin med den korrekte data
                    foreach (var item in userReservationsFromDb)
                    {
                        TimeSpan start = TimeSpan.FromMinutes((int)item.StartTime);
                        TimeSpan end = TimeSpan.FromMinutes((int)item.EndTime);
                        DateTime date = (DateTime)item.Date;

                        JObject j = new JObject();
                        j.Add("Bags", item.tbl_Quantity.Bags);
                        j.Add("Sacks", item.tbl_Quantity.Sacks);
                        j.Add("Cases", item.tbl_Quantity.Cases);
                        j.Add("Quantity", FormatQuantity(item.tbl_Quantity));
                        j.Add("Address", item.tbl_User.tbl_Address.Address + ", " +
                                         item.tbl_User.tbl_Address.tbl_City.Zip + " " +
                                         item.tbl_User.tbl_Address.tbl_City.City);
                        j.Add("StartTime", start.ToString("hh':'mm"));
                        j.Add("EndTime", end.ToString("hh':'mm"));
                        j.Add("Date", date.ToString("dd-MM-yyyy"));
                        j.Add("Material", item.tbl_Material.Type);
                        j.Add("Id", item.PK_Post);
                        
                        reservations.Add(j);
                    }
                    //slutteligt sættes statuscode til OK
                    message = Request.CreateResponse(HttpStatusCode.OK, reservations);
                } else
                {
                    //Hvis ikke der findes nogle transactioner sættes statuscode til badrequest
                    message = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            return message;
        }

        //Bruges til at afmelde en reservation baseret på postId og panterId i transaction tabellen
        public async Task<HttpResponseMessage> Put(HttpRequestMessage data)
        {
            HttpResponseMessage message = null;

            //data.Content indeholder post- og panterid
            var jObject = await data.Content.ReadAsAsync<JObject>();    
            int postId = (int)jObject["postId"];
            int panterId = (int)jObject["panterId"];

            try
            {
                using (PantrDatabaseEntities db = new PantrDatabaseEntities())
                {
                    //Henter transaction hvor panter- og postid matcher
                    var transactionToDelete = db.tbl_Transaction.FirstOrDefault(c => c.FK_Panter == panterId && c.FK_Post == postId);

                    //Sætter claimed til false (afmelder) og annuulled til true (da den pågældende panter 
                    //ikke er tilknyttet transactionen)
                    transactionToDelete.tbl_Post.Claimed = false;
                    transactionToDelete.Annulled = true;

                    db.SaveChanges();
                }
                //Sætter statuscode til ok
                message = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                //sætter statuscode til notmodified
                message = Request.CreateResponse(HttpStatusCode.NotModified);
            }
           
            return message;
        }


        private string FormatQuantity(tbl_Quantity quantity)
        {
            if (quantity == null)
            {
                throw new Exception("Quantity object er uventet tomt!");
            }

            //Tjekker om typen skal stå i ental eller flertal
            string bagsForm = quantity.Bags == 1 ? "pose" : "poser";
            string sacksForm = quantity.Sacks == 1 ? "sæk" : "sække";
            string casesForm = quantity.Cases == 1 ? "kasse" : "kasser";

            Dictionary<int, string> keyvalueFormat = new Dictionary<int, string>()
            {
                {quantity.Bags, bagsForm},
                {quantity.Sacks, sacksForm},
                {quantity.Cases, casesForm}
            };

            string formattedQuantities = "";
            foreach (KeyValuePair<int, string> item in keyvalueFormat)
            {
                if (item.Key > 0)
                    formattedQuantities += item.Key + " " + item.Value + ", ";
            }
            return formattedQuantities = formattedQuantities.Substring(0, formattedQuantities.Length - 2);
        }

    }

}