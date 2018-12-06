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
        public List<PostViewModel> Get()
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
               //List<JObject> lollern = new List<JObject>();

                //var postststs = db.tbl_Post.Select(c => c).ToList();

                //foreach (var item in postststs)
                //{
                //    JObject j = new JObject();
                //    j.Add(item.Claimed);
                //    j.Add(item.Date);
                //    lollern.Add(j);
                //}

                //return lollern;

                List<PostViewModel> posts = (from post in db.tbl_Post  
                             select new PostViewModel
                             {
                                 Id = post.PK_Post,
                                 Material = new MaterialViewModel
                                 {
                                     Type = post.tbl_Material.Type

                                 },
                                 Giver = new UserViewModel
                                 {
                                     Firstname = post.tbl_User.Firstname,
                                     Surname = post.tbl_User.Surname,
                                     Phone = post.tbl_User.Phone,
                                     Email = post.tbl_User.Email,
                                     IsPanter = (bool)post.tbl_User.IsPanter,
                                     Address = new AddressViewModel()
                                     {
                                         Address = post.tbl_User.tbl_Address.Address,
                                         City = new CityViewModel()
                                         {
                                             City = post.tbl_User.tbl_Address.tbl_City.City,
                                             Zip = post.tbl_User.tbl_Address.tbl_City.Zip
                                         }
                                     }
                                 },

                                 Quantity = post.Quantity,
                                 Address = post.Address,
                                 StartTime = (int)post.StartTime,
                                 EndTime = (int)post.EndTime,
                                 Claimed = (bool)post.Claimed,
                                 Completed = (bool)post.Completed,
                                 Date = post.Date.ToString()
                             }).ToList();

                return posts;
            }
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
                } else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, post);
                }
            };
            return response;
        }


        // POST api/<controller>
        [HttpPost]
        [Route("api/post")]
        public async Task<HttpResponseMessage> Post (HttpRequestMessage request)
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
                    Quantity = newPost.Quantity,
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

        public int ConvertTimeSpanToInteger (TimeSpan time)
        {
            int hours = time.Hours;
            int minutes = time.Minutes;
            int minutesAfterMidnight = (hours * 60) + minutes;
            return minutesAfterMidnight;
        }

        public string ConvertTime (int minutesAfterMidnight) {

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
                
                if(ExistingPost != null)
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
        public async Task<HttpResponseMessage> UpdatePost (int id, HttpRequestMessage request)
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