using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeadWomb.Data;
using LeadWomb.Model;
namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Leads")]
    public class LeadsController : ApiController
    {

        //[HttpPut]
        //public void Test([FromBody]abc test)
        //{
        //    abc t = test;
        //}

        private LeadsRepository leadRepository = null;
        // GET api/leads
        public LeadsController()
        {
            leadRepository = new LeadsRepository();
        }
        [Route("")]
        public IHttpActionResult Get(string userName, int? statusID)
        {


            return Ok(leadRepository.GetLeads(userName,statusID));
        }


        // GET api/leads/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/leads
        public void Post([FromBody]string value)
        {
        }

        // PUT api/leads/5
        public void Put([FromBody]Leads value)
        {
            leadRepository.UpdateLeads(value);
        }

        // DELETE api/leads/5
        public void Delete(int id)
        {
        }
    }
}
