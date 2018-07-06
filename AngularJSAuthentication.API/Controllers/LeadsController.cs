using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeadWomb.Data;
using LeadWomb.Model;
using System.Text;
using System.Text.RegularExpressions;
namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Leads")]
    public class LeadsController : ApiController
    {

     

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

        [Route("LeadSMS")]
        [HttpPost]
        public IHttpActionResult CreateLeadFromSMS([FromBody]LeadSMS leadSMS)
        {

            Leads lead = ParseLeadFromSMS(leadSMS.SMS);
            if (leadSMS.portalType.ToLower() == "mgcbrk")
            {
                lead.Status = 1;
            }
            else if (leadSMS.portalType.ToLower() == "nnacre")
            {
                lead.Status = 2;  
            }
            leadRepository.AddLeadForSMS(lead, leadSMS.userName);
            return Ok();

        }

        private Leads ParseLeadFromSMS(string SMS)
        {
            string phoneNumber = string.Empty;
            const string MatchPhonePattern =
      @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";

            Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            MatchCollection matches = rx.Matches(SMS);

            // Report the number of matches found.
            int noOfMatches = matches.Count;


            //Do something with the matches

            foreach (Match match in matches)
            {
                //Do something with the matches
                phoneNumber = match.Value.ToString(); ;
                break;
            }
            string[] tempSMS = SMS.Split(',');
            return new Leads { CmpctLabel = SMS, PhoneNumber = phoneNumber, Name = tempSMS[0] };
        }
        [Route("LeadMobileStatusCounts")]
        [HttpGet]
        public IHttpActionResult LeadMobileStatusCounts(string userName)
        {
            return Ok(leadRepository.GetStatusCountsByMobileUserName(userName));

        }

        [Route("Company")]
        [HttpGet]
        public IHttpActionResult GetLeadsByCompany(string userName,int? statusID)
        {
            return Ok(leadRepository.GetLeadsByCompany(userName,statusID));

        }
        [Route("CompanyWithPaging")]
        [HttpGet]
        public IHttpActionResult GetLeadsByCompanyPage(string userName,int pageSize,int pageNumber, int? statusID)
        {
            return Ok(leadRepository.GetLeadsByCompanyWithPaging(userName,pageSize,pageNumber, statusID));

        }
        [Route("Recording")]
        [HttpPost]
        public IHttpActionResult CreateRecordings(int leadID, string userName,string fileName)
        {
            return Ok(leadRepository.CreateRecordings(leadID,userName,fileName));

        }
        [Route("Recording")]
        [HttpGet]
        public IHttpActionResult GetRecordings(int leadID)
        {
            return Ok(leadRepository.GetRecordings(leadID));

        }
        [Route("Company/RawLeads")]
        [HttpGet]
        public IHttpActionResult GetRawLeadsByCompany(string userName)
        {
            return Ok(leadRepository.GetRawLeadsByCompany(userName));

        }
        [Route("Company/RawLeadsWithPaging")]
        [HttpGet]
        public IHttpActionResult GetRawLeadsByCompanyWithPaging(string userName,int pageSize,int pageNumber)
        {
            return Ok(leadRepository.GetRawLeadsByCompanyWithPaging(userName,pageSize,pageNumber));

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
        [HttpPut]
        [Route("LeadItem")]
        // PUT api/leads/5
        public void Put([FromBody]LeadItems value)
        {
            leadRepository.UpdateLeadItem(value);
        }
        // DELETE api/leads/5
        public void Delete(int id)
        {   
        }
        [HttpGet]
        [Route("Locations")]
        public IHttpActionResult GetEmployeesLocation(string userName)
        {
            return Ok(leadRepository.GetEmployeesLocation(userName));
        }
        [HttpPost]
        [Route("Location")]
        public IHttpActionResult Location(string userName, string lng, string lat)
        {
            leadRepository.CreateLocation(userName, lng, lat);
            return Ok();
        }

    }
}
