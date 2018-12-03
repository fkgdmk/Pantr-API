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

                                                 PostQuantity = new PostQuantityViewModel
                                                 {
                                                     QuantityType = new QuantityTypeViewModel
                                                     {
                                                         //QuantityType = post.tbl_PostQuantity.tbl_QuantityType.QuantityType
                                                     },
                                                     //Quantity = (int)post.tbl_PostQuantity.Quantity
                                                 },
                                                 Address = post.Address,
                                                 //StartTime = ConvertIntegerToTimeSpan((int)post.StartTime),
                                                 //EndTime = ConvertIntegerToTimeSpan((int)post.EndTime),
                                                 Claimed = (bool)post.Claimed,
                                                 Completed = (bool)post.Completed,
                                                 //Date = (DateTime)post.Date
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
        public PostViewModel GetUsersPost(int userId)
        {
            PantrDatabaseEntities db = new PantrDatabaseEntities();
            tbl_Post postFromDb = db.tbl_Post.FirstOrDefault(giver => giver.FK_Giver == userId);
            DateTime date = (DateTime)postFromDb.Date;
            PostViewModel post = null;
            if (postFromDb != null)
            {
                TimeSpan startTime = ConvertIntegerToTimeSpan((int)postFromDb.StartTime);
                TimeSpan endTime = ConvertIntegerToTimeSpan((int)postFromDb.EndTime);
                post = new PostViewModel()
                {
                    Id = postFromDb.PK_Post,
                    Material = new MaterialViewModel
                    {
                        Type = postFromDb.tbl_Material.Type
                    },
                    PostQuantity = new PostQuantityViewModel
                    {
                        QuantityType = new QuantityTypeViewModel
                        {
                            //QuantityType = postFromDb.tbl_PostQuantity.tbl_QuantityType.QuantityType
                        },
                        //Quantity = (int)postFromDb.tbl_PostQuantity.Quantity
                    },
                    Address = postFromDb.Address,
                    //StartTime = startTime,
                    //EndTime = endTime,
                    Claimed = (bool)postFromDb.Claimed,
                    Completed = (bool)postFromDb.Completed,
                    Date = date.ToString("dd/MM/yyyy")
                };


            }
            return post;
        }


        // POST api/<controller>
        [HttpPost]
        [Route("api/post")]
        public async Task Post(HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();
            Item item = JsonConvert.DeserializeObject<Item>(jObject.ToString());

            int startTime = ConvertTimeSpanToInteger(item.StartTime);
            int endTime = ConvertTimeSpanToInteger(item.EndTime);

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                tbl_Material material = db.tbl_Material.FirstOrDefault(m => m.Type == item.Material);
                tbl_User giver = db.tbl_User.FirstOrDefault(u => u.PK_User == 1); //Ændres til requests user
                tbl_QuantityType type = db.tbl_QuantityType.FirstOrDefault(t => t.QuantityType == item.QuanityType);

                tbl_PostQuantity postQuantity = new tbl_PostQuantity
                {
                    tbl_QuantityType = type,
                    Quantity = item.Quanity
                };

                tbl_Post post = new tbl_Post
                {
                    tbl_Material = material,
                    //tbl_PostQuantity = postQuantity,
                    tbl_User = giver,
                    Address = item.Address,
                    StartTime = startTime,
                    EndTime = endTime,
                    Claimed = false,
                    Completed = false,
                    Date = DateTime.Today
                };

                db.tbl_Post.Add(post);
                db.SaveChanges();
            }
        }

        public int ConvertTimeSpanToInteger (TimeSpan time)
        {
            int hours = time.Hours;
            int minutes = time.Minutes;
            int minutesAfterMidnight = (hours * 60) + minutes;
            return minutesAfterMidnight;
        }

        public TimeSpan ConvertIntegerToTimeSpan (int minutesAfterMidnight) {

            int hours = minutesAfterMidnight / 60;
            int minutes = minutesAfterMidnight % 60;

            TimeSpan time = new TimeSpan(hours, minutes, 0);


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

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            PantrDatabaseEntities db = new PantrDatabaseEntities();

            tbl_Post post = db.tbl_Post.Find(id);
            db.tbl_Post.Remove(post);
            db.SaveChanges();
        }
    }
}