using PantrTest.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PantrTest.Models.ViewModels;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using PantrTest.Models;

namespace PantrTest.Controllers
{
    public class PostController : ApiController
    {
        // GET api/<controller>
        [Route("api/posts")]
        public List<JObject> Get()
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                List<JObject> posts = new List<JObject>();

                List<tbl_Post> allNonClaimedPosts = db.tbl_Post.Where(c => c.Claimed == false).ToList();
                foreach (var post in allNonClaimedPosts)
                {
                    JObject postJson = new JObject();
                    postJson.Add("Id", post.PK_Post);
                    var giver = db.tbl_User.FirstOrDefault(user => user.PK_User == post.FK_Giver);
                    var address = db.tbl_Address.FirstOrDefault(giverAddress => giverAddress.PK_Address == giver.FK_Address);
                    var zipcode = db.tbl_City.FirstOrDefault(giverAdressZipcode => giverAdressZipcode.PK_City == address.FK_City);
                    postJson.Add("Address", address.Address + ", " + zipcode.Zip + " " + zipcode.City);
                    var quantity = db.tbl_Quantity.FirstOrDefault(postsQuantity => post.FK_Quantity == postsQuantity.PK_Quantity);
                    postJson.Add("Quantity", FormatQuantity(quantity));
                    var material = db.tbl_Material.FirstOrDefault(postsMaterial => post.FK_Material == postsMaterial.PK_Material);
                    postJson.Add("Material", material.Type);
                    string date = post.Date.Value.ToString("dd-MM-yyyy");
                    postJson.Add("Date", date);
                    string periode = FormatTimeSpan(post);
                    postJson.Add("PeriodForPickup", periode);
                    postJson.Add("DateAndPeriod", string.Format("{0}, {2} d. {1}", periode, date, post.Date.Value.DayOfWeek));


                    posts.Add(postJson);
                }
                return posts;
            }
        }
        /**
        [HttpGet]
        [Route("api/posts/{zipcode:string}")]
        public List<JObject> Get(string zipcode)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                List<JObject> posts = new List<JObject>();

                List<tbl_Post> allNonClaimedPosts = db.tbl_Post.Where(c => c.Claimed == false).ToList();
                foreach (var post in allNonClaimedPosts)
                {
                    JObject postJson = new JObject();
                    postJson.Add("Id", post.PK_Post);
                    var giver = db.tbl_User.FirstOrDefault(user => user.PK_User == post.FK_Giver);
                    var address = db.tbl_Address.FirstOrDefault(giverAddress => giverAddress.PK_Address == giver.FK_Address);
                    var city = db.tbl_City.FirstOrDefault(giverAdressZipcode => giverAdressZipcode.PK_City == address.FK_City);
                    string fullAddress = string.Format("{0}, {1} {2}", address.Address, city.Zip, city.City);
                    postJson.Add("Address", fullAddress);
                    var quantity = db.tbl_Quantity.FirstOrDefault(postsQuantity => post.FK_Quantity == postsQuantity.PK_Quantity);
                    postJson.Add("Quantity", FormatQuantity(quantity));
                    var material = db.tbl_Material.FirstOrDefault(postsMaterial => post.FK_Material == postsMaterial.PK_Material);
                    postJson.Add("Material", material.Type);
                    string date = post.Date.Value.ToString("dd-MM-yyyy");
                    postJson.Add("Date", date);
                    string periode = FormatTimeSpan(post);
                    postJson.Add("PeriodForPickup", periode);
                    postJson.Add("DateAndPeriod", string.Format("{0}, {2} d. {1}", periode, date, post.Date.Value.DayOfWeek));

                    if (city.Zip.Equals(zipcode));
                        posts.Add(postJson);
                }
                return posts;
            }
        }
    */
        private string FormatQuantity(tbl_Quantity quantity)
        {
            if (quantity == null)
            {
                throw new Exception("Quantity object er uventet tomt!");
            }

            // Hvis sække og kasser er 0
            if (quantity.Sacks == 0 && quantity.Cases == 0)
                return String.Format("{0} pose(r)", quantity.Bags);
            // Hvis poser og kasser er 0
            else if (quantity.Bags == 0 && quantity.Cases == 0)
                return String.Format("{0} sæk(ke)", quantity.Sacks);
            // Hvis poser og sække er 0
            else if (quantity.Bags == 0 && quantity.Sacks == 0)
                return String.Format("{0} kasse(r)", quantity.Cases);

            // Hvis kun poser er 0
            else if (quantity.Bags == 0)
                return String.Format("{0} sæk(ke) og {1} kasse(r)", quantity.Sacks, quantity.Cases);
            // Hvis kun sække er 0
            else if (quantity.Sacks == 0)
                return String.Format("{0} pose(r) og {1} kasse(r)", quantity.Bags, quantity.Cases);
            // Hvis kun kasser er 0
            else if (quantity.Cases == 0)
                return String.Format("{0} pose(r) og {1} sæk(ke)", quantity.Bags, quantity.Sacks);
            else
                return String.Format("{0} pose(r), {1} sæk(ke) og {2} kasse(r)", quantity.Bags, quantity.Sacks, quantity.Cases);
        }

        public string FormatTimeSpan(tbl_Post post)
        {
            string startTime = TimeSpan.FromMinutes(Convert.ToDouble(post.StartTime)).ToString();
            string endTime = TimeSpan.FromMinutes(Convert.ToDouble(post.EndTime)).ToString();

            return string.Format("{0} - {1}", startTime, endTime);
        }

        // GET api/<controller>/5
        public PostViewModel Get(int id)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post foundPost = new tbl_Post();
                foundPost = db.tbl_Post.Find(id);
                PostViewModel post = new PostViewModel()
                {
                    Giver = new UserViewModel()
                    {
                        Firstname = foundPost.tbl_User.Firstname,
                        Surname = foundPost.tbl_User.Surname,
                        Phone = foundPost.tbl_User.Phone,
                        Email = foundPost.tbl_User.Email,
                        IsPanter = (bool)foundPost.tbl_User.IsPanter,
                        Address = new AddressViewModel()
                        {
                            Address = foundPost.tbl_User.tbl_Address.Address,
                            City = new CityViewModel()
                            {
                                City = foundPost.tbl_User.tbl_Address.tbl_City.City,
                                Zip = foundPost.tbl_User.tbl_Address.tbl_City.Zip
                            },
                        },
                    },
                    Material = new MaterialViewModel()
                    {
                        Type = foundPost.tbl_Material.Type
                    },
                    //StartTime = ConvertIntegerToTimeSpan((int)foundPost.StartTime),
                    //EndTime = ConvertIntegerToTimeSpan((int)foundPost.EndTime),
                    Claimed = (bool)foundPost.Claimed,
                    Address = foundPost.Address,
                    //Date = (DateTime)foundPost.Date

                };

                return post;
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

                if (postFromDb != null)
                {
                    tbl_User user = db.tbl_User.Find(userId);
                    DateTime date = (DateTime)postFromDb.Date;

                    post.Add("Id", postFromDb.PK_Post);
                    post.Add("Material", postFromDb.tbl_Material.Type);
                    post.Add("Material", postFromDb.tbl_Material.Type);
                    post.Add("Quantity", postFromDb.Quantity);
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


        // POST api/<controller>
        [HttpPost]
        [Route("api/post")]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            PostViewModel newPost = JsonConvert.DeserializeObject<PostViewModel>(jObject.ToString());
            var message = Request.CreateResponse(HttpStatusCode.Accepted);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Material material = null;
                if (newPost.Material.Type != null)
                {
                    material = db.tbl_Material.FirstOrDefault(m => m.Type == newPost.Material.Type);
                }

                tbl_User giver = db.tbl_User.FirstOrDefault(u => u.PK_User == newPost.Giver.Id); //Ændres til requests user
                DateTime date = DateTime.Parse(newPost.Date);

                tbl_Post post = new tbl_Post
                {
                    tbl_Material = material,
                    //Quantity = newPost.Quantity,
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

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                var ExistingPost = db.tbl_Post.FirstOrDefault(p => id == p.PK_Post);

                if (ExistingPost != null)
                {
                    ExistingPost.Claimed = true;

                    db.SaveChanges();
                    Console.WriteLine("Vi kom sgu til enden!");

                }
                else
                {
                    return NotFound();
                }

                return Ok();


            }
        }

        [HttpPut]
        [Route("api/updatepost/{id:int}")]
        public async Task<HttpResponseMessage> UpdatePost(int id, HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            Item item = JsonConvert.DeserializeObject<Item>(jObject.ToString());
            var message = Request.CreateResponse(HttpStatusCode.Accepted);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post post = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == id);

                if (post == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                tbl_Material material = null;
                if (item.Material.Type != null)
                {
                    material = db.tbl_Material.FirstOrDefault(m => m.Type == item.Material.Type);
                }

                DateTime date = DateTime.Parse(item.Date);
                post.tbl_Material = material;
                post.Quantity = item.Quantity;
                post.StartTime = item.StartTime;
                post.EndTime = item.EndTime;
                post.Date = date;

                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Post post = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == id);
                if (post == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                tbl_Transaction transaction = db.tbl_Transaction.FirstOrDefault(t => t.FK_Panter == post.PK_Post);

                if (transaction != null)
                {
                    transaction.Annulled = true;
                }
                db.tbl_Post.Remove(post);
                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}