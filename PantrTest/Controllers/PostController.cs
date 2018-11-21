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

                var posts = (from post in db.tbl_Post
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
                                           QuantityType = post.tbl_PostQuantity.tbl_QuantityType.QuantityType
                                     },
                                     Quantity = (int)post.tbl_PostQuantity.Quantity
                                 },
                                 Address = post.Address,
                                 //StartTime = ConvertIntegerToTimeSpan((int)post.StartTime),
                                 EndTime = (int)post.EndTime,
                                 Claimed = (bool)post.Claimed,
                                 Completed = (bool)post.Completed,
                                 Date = (DateTime)post.Date
                             }).ToList();

                return posts;
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        [Route("api/post")]
        public async Task Post(HttpRequestMessage request)
        {
            var jObject = await request.Content.ReadAsAsync<JObject>();

            Item item = JsonConvert.DeserializeObject<Item>(jObject.ToString());

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
                    tbl_PostQuantity = postQuantity,
                    tbl_User = giver,
                    Address = item.Address,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    Claimed = false,
                    Completed = false,
                    Date = DateTime.Today
                };

                db.tbl_Post.Add(post);
                db.SaveChanges();
            }
        }

        public string ConvertIntegerToTimeSpan (int minutesAfterMidnight) {

            int hours = minutesAfterMidnight / 60;
            string midnight = new TimeSpan(hours, 0, 0).ToString();


            //TimeSpan time = midnight.Add(hours)

            return midnight;
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