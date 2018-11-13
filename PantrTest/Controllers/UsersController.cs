using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Sql;
using System.Data.SqlClient;
using PantrTest.Models.DataModels;

namespace PantrTest.Controllers
{
    public class UsersController : ApiController
    {
        public IEnumerable<tbl_User> Get()
        {
            List<tbl_User> users = new List<tbl_User>();
            using (PantrEntities db = new PantrEntities())
            {
                users = db.tbl_User.Select(c => c).ToList();
            }

            return users;
        }

        // GET api/values/5
        public tbl_User Get(int id)
        {
            tbl_User user;
            using (PantrEntities db = new PantrEntities())
            {
                user = db.tbl_User.FirstOrDefault(c => c.PK_User == id);
            }

            return user;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
