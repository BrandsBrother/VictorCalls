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
using System.IO;
using System.Threading.Tasks;
using System.Data;

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
            return Ok(leadRepository.GetLeads(userName, statusID));
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
            if (!string.IsNullOrEmpty(lead.PhoneNumber))
            {
                leadRepository.AddLeadForSMS(lead, leadSMS.userName);
            }
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
        public IHttpActionResult GetLeadsByCompany(string userName, int? statusID)
        {
            return Ok(leadRepository.GetLeadsByCompany(userName, statusID));

        }
        [Route("CompanyWithPaging")]
        [HttpGet]
        public IHttpActionResult GetLeadsByCompanyPage([FromUri]Filter filter)
        {
            return Ok(leadRepository.GetLeadsByCompanyWithPaging(filter.userName, filter.pageSize, 
                filter.pageNumber, filter.statusID,filter.projectId,filter.assignedTo,filter.leadName,
                filter.leadNumber,filter.DateFrom,filter.DateTo));

        }
        [Route("Recording")]
        [HttpPost]
        public IHttpActionResult CreateRecordings(int leadID, string userName, string fileName)
        {
            return Ok(leadRepository.CreateRecordings(leadID, userName, fileName));

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
        public IHttpActionResult GetRawLeadsByCompanyWithPaging(string userName, int pageSize, int pageNumber)
        {
            return Ok(leadRepository.GetRawLeadsByCompanyWithPaging(userName, pageSize, pageNumber));

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
        public IHttpActionResult Put([FromBody]Leads value)
        {
            leadRepository.UpdateLeads(value);
            IEnumerable<ITokeneable> tokens = value.Items.Cast<ITokeneable>();
            //Send notifications to related devices.
            new AngularJSAuthentication.API.Notifications.VictorCallsNotifications().SendNotification(tokens, "Data update notification", true, false, false, false);
            return Ok();
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
        [HttpPost]
        [Route("ExcelUpload")]
        public async Task<IHttpActionResult> BulkUpload()
        {
            
            string root = System.Web.HttpContext.Current.Server.MapPath("~/UploadFile");
            string fileWithoutQuote = string.Empty;
            // Read the form data.
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            if (provider.FileData.Count > 0 && provider.FileData[0] != null)
            {
                MultipartFileData file = provider.FileData[0];

                //clean the file name
                fileWithoutQuote = file.Headers.ContentDisposition.FileName.Substring(1, file.Headers.ContentDisposition.FileName.Length - 2);

                //get current file directory on the server
                var directory = Path.GetDirectoryName(file.LocalFileName);

                if (directory != null)
                {
                    //generate new random file name (not mandatory)
                    var randomFileName = string.Concat(directory, "\\" + fileWithoutQuote);
                    var fileExtension = Path.GetExtension(fileWithoutQuote);
                    var newfilename = Path.ChangeExtension(randomFileName, fileExtension);
                    if (File.Exists(randomFileName))
                    {
                        // Note that no lock is put on the
                        // file and the possibility exists
                        // that another process could do
                        // something with it between
                        // the calls to Exists and Delete.
                        File.Delete(randomFileName);
                    }

                    //Move file to rename existing upload file name with new random filr name
                    File.Move(file.LocalFileName, randomFileName);
                    DataTable table = leadRepository.ReadExcel(randomFileName, fileExtension);
                    int counter = 0;
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    
                    foreach (DataRow row in table.Rows)
                    {
                        if (counter == 0)
                        {
                          dictionary =  FillColumns(row);
                        }
                        else
                        {
                            
                        }
                        counter++;
                    }
                    leadRepository.PostLeadData(table, dictionary);
                    
                }
            }
          

            return Ok();        //return Request.CreateResponse(HttpStatusCode.Created);
        }

        public Dictionary<string,string> FillColumns(DataRow row)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            string f1column = row["F1"].ToString();
            columns = FillRowColumns("F1", row["F1"].ToString(),columns);
            columns = FillRowColumns("F2", row["F2"].ToString(), columns);
            columns = FillRowColumns("F3", row["F3"].ToString(), columns);
            columns = FillRowColumns("F4", row["F4"].ToString(), columns);               
            
            return columns;
        }
        public Dictionary<string, string> FillRowColumns(string columnName,string value,Dictionary<string,string> columns)
        {
            if (value.ToLower().Contains("name"))
            {
                columns.Add("Name", columnName);
            }
            else if (value.ToLower().Contains("phone"))
            {
                columns.Add("PhoneNumber", columnName);
            }
            else if (value.ToLower().Contains("remarks"))
            {
                columns.Add("CmpctLabel", columnName);
            }
            else if (value.ToLower().Contains("email"))
            {
                columns.Add("Email", columnName);
            }
            else
            {

            }
            return columns;
        }

        [HttpGet]
        [Route("Projects")]
        public List<Project> GetProjects(long CompanyId)
        {
            return leadRepository.GetProjects(CompanyId);
            
        }
        [HttpPost]
        [Route("Attendance")]
        public IHttpActionResult CreateAttendance([FromBody]Attendence attendence)
        {
            leadRepository.CreateOrUpdateAttendence(attendence);
            return Ok();
        }
        [HttpGet]
        [Route("Attendance")]
        public List<Attendence> GetAttendance(string userName, DateTime attendanceDate)
        {
            return leadRepository.GetAttendance(userName, attendanceDate);
        }

       
    }
}
