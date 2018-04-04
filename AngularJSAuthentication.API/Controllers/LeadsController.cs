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
        [Route("LeadStatusCounts")]
        [HttpGet]
        public LeadStatusCounts LeadStatusCounts(string userName)
        {
            LeadStatusCounts obj = new LeadStatusCounts();
            obj.ClosureCount = 23;
            obj.CurrentLeadsCount = 24;
            obj.DeadCount = 25;
            obj.FollowUpsCount = 26;
            obj.NotConnectedCount = 27;
            obj.OtherProjectsCount = 28;
            obj.PendingLeadsCount = 29;
            obj.PlotCount = 30;
            obj.RentCount = 31;
            return obj;

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
