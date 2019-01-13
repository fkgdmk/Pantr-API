using PantrTest.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PantrTest.Controllers
{
    public class PostController : ApiController
    {
        [Route("api/posts")]
        public HttpResponseMessage Get()
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                HttpResponseMessage message = null;

                List<JObject> posts = new List<JObject>();

                List<tbl_Post> allNonClaimedPosts =  db.tbl_Post.Where(c => c.Claimed == false).ToList();
                foreach (var post in allNonClaimedPosts)
                {

                    JObject postJson = new JObject();
                    postJson.Add("Id", post.PK_Post);

                    string fullAddress = string.Format("{0}, {1} {2}", post.tbl_User.tbl_Address.Address, post.tbl_User.tbl_Address.tbl_City.Zip, post.tbl_User.tbl_Address.tbl_City.City);
                    postJson.Add("Address", fullAddress);

                    var quantity = post.tbl_Quantity;
                    postJson.Add("Quantity", FormatQuantity(quantity));

                    var material = post.tbl_Material;
                    postJson.Add("Material", material.Type);

                    string date = post.Date.Value.ToString("dd-MM-yyyy");
                    postJson.Add("Date", date);

                    string periode = FormatTimeSpan(post);
                    postJson.Add("PeriodForPickup", periode);
                    postJson.Add("StartTime", ConvertTime((int)post.StartTime));
                    postJson.Add("EndTime", ConvertTime((int)post.EndTime));
                    postJson.Add("DateAndPeriod", string.Format("{0}, {2} d. {1}", periode, date, post.Date.Value.DayOfWeek));
                    posts.Add(postJson);
                }
                    message = Request.CreateResponse(HttpStatusCode.OK, posts);
            
                return message;
            }   
        }

        [HttpPut]
        [Route("api/claimpost/{id:int}")]
        public async Task<HttpResponseMessage> ClaimPost(int id, HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            tbl_Post claimedPost = JsonConvert.DeserializeObject<tbl_Post>(jObject.ToString());
            var message = Request.CreateResponse(HttpStatusCode.Accepted);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post post = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == claimedPost.tbl_User.PK_User);

                if (post == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                post.Claimed = true;

                tbl_Transaction transaction = new tbl_Transaction()
                {
                    FK_Post = post.PK_Post,
                    FK_Panter = id,
                    Collected = false,
                    Annulled = false
                };

                db.tbl_Transaction.Add(transaction);
                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.Accepted);

        }

        [HttpGet]
        [Route("api/posts/{zipcode}")]
        public HttpResponseMessage Get(string zipcode)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                List<JObject> posts = new List<JObject>();
                HttpResponseMessage message = null;

                List<tbl_Post> allNonClaimedPosts = db.tbl_Post.Where(c => c.Claimed == false && c.tbl_User.tbl_Address.tbl_City.Zip.ToString().Equals(zipcode)).ToList();
                foreach (var post in allNonClaimedPosts)
                {
                    JObject postJson = new JObject();
                    postJson.Add("Id", post.PK_Post);

                    string fullAddress = string.Format("{0}, {1} {2}", post.tbl_User.tbl_Address.Address, post.tbl_User.tbl_Address.tbl_City.Zip, post.tbl_User.tbl_Address.tbl_City.City);
                    postJson.Add("Address", fullAddress);

                    var quantity = post.tbl_Quantity;
                    postJson.Add("Quantity", FormatQuantity(quantity));

                    var material = post.tbl_Material;
                    postJson.Add("Material", material.Type);

                    string date = post.Date.Value.ToString("dd-MM-yyyy");
                    postJson.Add("Date", date);

                    string periode = FormatTimeSpan(post);
                    postJson.Add("PeriodForPickup", periode);
                    postJson.Add("DateAndPeriod", string.Format("{0}, {2} d. {1}", periode, date, post.Date.Value.DayOfWeek));

                    posts.Add(postJson);

                }
                message = Request.CreateResponse(HttpStatusCode.OK, posts);

                return message;
            }
        }

        [HttpGet]
        [Route("api/post/getuserspost/{userId:int}")]
        public HttpResponseMessage GetUsersPost(int userId)
        {

            JObject post = new JObject();
            HttpResponseMessage response = new HttpResponseMessage();


            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post postFromDb = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == userId);
                tbl_Quantity quantity = db.tbl_Quantity.FirstOrDefault(q => postFromDb.FK_Quantity == q.PK_Quantity);

                if (postFromDb != null)
                {
                    tbl_User user = db.tbl_User.Find(userId);
                    DateTime date = (DateTime)postFromDb.Date;

                    post.Add("Id", postFromDb.PK_Post);
                    post.Add("Material", postFromDb.tbl_Material.Type);
                    post.Add("Quantity", FormatQuantity(quantity));
                    post.Add("Bags", quantity.Bags);
                    post.Add("Sacks", quantity.Sacks);
                    post.Add("Cases", quantity.Cases);
                    post.Add("Date", date.ToString("dd/MM/yyyy"));
                    post.Add("StartTime", ConvertTime((int)postFromDb.StartTime));
                    post.Add("EndTime", ConvertTime((int)postFromDb.EndTime));
                    post.Add("Claimed", postFromDb.Claimed);
                    post.Add("Completed", postFromDb.Completed);
                    post.Add("Address", user.tbl_Address.Address + ", " +
                                     user.tbl_Address.tbl_City.City + " " +
                                     user.tbl_Address.tbl_City.Zip);

                    response = Request.CreateResponse(HttpStatusCode.OK, post);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, post);
                }
            };
            return response;
        }

        [HttpPost]
        [Route("api/post")]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            tbl_Post newPost = JsonConvert.DeserializeObject<tbl_Post>(jObject.ToString());
            var message = Request.CreateResponse(HttpStatusCode.Accepted);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Material material = null;
                if (newPost.tbl_Material.Type != null)
                {
                    material = db.tbl_Material.FirstOrDefault(m => m.Type == newPost.tbl_Material.Type);
                }

                tbl_Quantity quantity = new tbl_Quantity
                {
                    Bags = newPost.tbl_Quantity.Bags,
                    Sacks = newPost.tbl_Quantity.Sacks,
                    Cases = newPost.tbl_Quantity.Cases
                };
                
                tbl_User giver = db.tbl_User.FirstOrDefault(u => u.PK_User == newPost.FK_Giver); 
                DateTime date = newPost.Date.Value;

                tbl_Post post = new tbl_Post
                {
                    tbl_Material = material,
                    tbl_Quantity = quantity,
                    tbl_User = giver,
                    Address = "",
                    StartTime = newPost.StartTime,
                    EndTime = newPost.EndTime,
                    Claimed = false,
                    Completed = false,
                    Date = date
                };

                db.tbl_Post.Add(post);
                db.SaveChanges();
                return request.CreateResponse(HttpStatusCode.OK, newPost);
            }
        }

        [HttpPut]
        [Route("api/updatepost/{id:int}")]
        public async Task<HttpResponseMessage> UpdatePost(int id, HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            tbl_Post updatedPost = JsonConvert.DeserializeObject<tbl_Post>(jObject.ToString());
            var message = Request.CreateResponse(HttpStatusCode.Accepted);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post post = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == id);

                if (post == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                tbl_Material material = null;
                if (updatedPost.tbl_Material.Type != null)
                {
                    material = db.tbl_Material.FirstOrDefault(m => m.Type == updatedPost.tbl_Material.Type);
                }

                tbl_Quantity quantity = new tbl_Quantity
                {
                    Bags = updatedPost.tbl_Quantity.Bags,
                    Sacks = updatedPost.tbl_Quantity.Sacks,
                    Cases = updatedPost.tbl_Quantity.Cases
                };

                DateTime date = updatedPost.Date.Value;
                post.tbl_Material = material;
                post.tbl_Quantity = quantity;
                post.StartTime = updatedPost.StartTime;
                post.EndTime = updatedPost.EndTime;
                post.Date = date;

                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        public HttpResponseMessage Delete(int id)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                //Finder pantopslaget der passer på brugerens id
                tbl_Post post = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == id);
                if (post == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                //Sletter først pantopslagets tilhørenede quanity
                tbl_Quantity quantity = db.tbl_Quantity.Find(post.FK_Quantity);
                if (quantity != null)
                {
                    db.tbl_Quantity.Remove(quantity);
                }

                //Hvis der er en transaktion igang på pantopslaget slettes transaktionen
                tbl_Transaction transaction = db.tbl_Transaction.FirstOrDefault(t => t.FK_Post == post.PK_Post);
                if (transaction != null)
                {
                    db.tbl_Transaction.Remove(transaction);
                }

                //Tilsidst slettes pantopslaget
                db.tbl_Post.Remove(post);
                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public int ConvertTimeSpanToInteger(TimeSpan time)
        {
            int hours = time.Hours;
            int minutes = time.Minutes;
            int minutesAfterMidnight = (hours * 60) + minutes;
            return minutesAfterMidnight;
        }

        public string ConvertTime(int minutesAfterMidnight)
        {

            int hours = minutesAfterMidnight / 60;
            int minutes = minutesAfterMidnight % 60;
            TimeSpan timeSpan = new TimeSpan(hours, minutes, 0);
            string[] timeArr = timeSpan.ToString().Split(':');
            string time = timeArr[0] + ":" + timeArr[1];


            //TimeSpan time = midnight.Add(hours)

            return time;
        }

        private string FormatQuantity(tbl_Quantity quantity)
        {
            //Tjekker om typen skal stå i ental eller flertal
            string bagsForm = quantity.Bags == 1 ? "pose" : "poser";
            string sacksForm = quantity.Sacks == 1 ? "sæk" : "sække";
            string casesForm = quantity.Cases == 1 ? "kasse" : "kasser";

            // Hvis sække og kasser er 0
            if (quantity.Sacks == 0 && quantity.Cases == 0)
                return String.Format("{0} {1}", quantity.Bags, bagsForm);
            // Hvis poser og kasser er 0
            else if (quantity.Bags == 0 && quantity.Cases == 0)
                return String.Format("{0} {1}", quantity.Sacks, sacksForm);
            // Hvis poser og sække er 0
            else if (quantity.Bags == 0 && quantity.Sacks == 0)
                return String.Format("{0} {1}", quantity.Cases, casesForm);

            // Hvis kun poser er 0
            else if (quantity.Bags == 0)
                return String.Format("{0} {1} og {2} {3}", quantity.Sacks, sacksForm, quantity.Cases, casesForm);
            // Hvis kun sække er 0
            else if (quantity.Sacks == 0)
                return String.Format("{0} {1} og {2} {3}", quantity.Bags, bagsForm, quantity.Cases, casesForm);
            // Hvis kun kasser er 0
            else if (quantity.Cases == 0)
                return String.Format("{0} {1} og {2} {3}", quantity.Bags, bagsForm, quantity.Sacks, sacksForm);
            else
                return String.Format("{0} {1}, {2} {3} og {4} {5}", quantity.Bags, bagsForm, quantity.Sacks, sacksForm, quantity.Cases, casesForm);
        }

        public string FormatTimeSpan(tbl_Post post)
        {
            string startTime = TimeSpan.FromMinutes(Convert.ToDouble(post.StartTime)).ToString().Substring(0, 5);
            string endTime = TimeSpan.FromMinutes(Convert.ToDouble(post.EndTime)).ToString().Substring(0, 5);

            return string.Format("{0} - {1}", startTime, endTime);
        }


    }


}