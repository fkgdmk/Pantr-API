using PantrTest.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PantrTest.Controllers
{
    public class PostController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<tbl_Post> Get()
        {
            List<tbl_Post> posts = new List<tbl_Post>();

            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                posts = db.tbl_Post.Select(post => post).ToList();
            }

            return posts;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            using (PantrDatabaseEntities db = new PantrDatabaseEntities())
            {
                DateTime date = DateTime.Today;
                db.tbl_Post.Add(new tbl_Post()
                {
                    StartTime = 60,
                    EndTime = 120,
                    Date = date
                });
                db.SaveChanges();
            }
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