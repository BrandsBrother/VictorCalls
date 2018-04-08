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
        private AccountContext accountContext = null;
        // GET api/leads
        public LeadsController()
        {
            leadRepository = new LeadsRepository();
            accountContext = new AccountContext();
        }
        [Route("")]
        public IHttpActionResult Get(string userName, int? statusID)
        {


            return Ok(leadRepository.GetLeads(userName,statusID));
        }
        [Route("LeadStatusCounts")]
        [HttpGet]
        public IHttpActionResult LeadStatusCounts(string userName)
        {
            return Ok(accountContext.GetStatusCountsByUserName(userName));

        }
        //[Route("")]
        //public IHttpActionResult Get(long CompanyId, int statusID, int? AssignedTo)
        //{


        //    return Ok(leadRepository.GetLeads(CompanyId, statusID, AssignedTo));
        //}
        // GET api/leads/5
        public string Get(int id)
        {
            return "value";
        }
           [Route("")]
        // POST api/leads
        public IHttpActionResult Post([FromBody]Leads value)
        {
            leadRepository.AddLead(value);
            return Ok();
        }
        [HttpPut]
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
