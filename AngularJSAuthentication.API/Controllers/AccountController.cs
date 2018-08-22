using AngularJSAuthentication.API.Models;
using AngularJSAuthentication.API.Results;
using LeadWomb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LeadWomb.Model;
using System.IO;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json;
using System.Text;
using FcmSharp;
using AngularJSAuthentication.API;
using AngularJSAuthentication.API.Notifications;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;
       
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public AccountController()
        {
           
            _repo = new AuthRepository();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult Register(ApplicationUser userModel)
        {

            _repo.RegisterUser(userModel);
           
            List<ApplicationUser> users = _repo.GetProjectUsers(userModel.ProjectId);
            IEnumerable<ITokeneable> tokens = users.Cast<ITokeneable>();
            new VictorCallsNotifications().SendNotification(tokens, "Data updated notifications", false, false,true, false);

            return Ok();
        }
        [Authorize]
        [HttpPost]
        [Route("Project")]
        public IHttpActionResult CreateProject([FromBody]Project project)
        {
            _repo.CreateProject(project);
            List<ApplicationUser> users = _repo.GetCompanyUsers(project.CompanyId);
            IEnumerable<ITokeneable> tokens =  users.Cast<ITokeneable>();
            new VictorCallsNotifications().SendNotification(tokens, "Data update notification", false, true, false, false);
            return Ok();
        }
        [HttpPut]
        [Route("User")]
        public IHttpActionResult UpdateUser(ApplicationUser user)
        {
            _repo.UpdateUser(user);
            return Ok();
        }
        [HttpGet]
        [Route("Users")]
        public List<ApplicationUser> GetUsersOfCompany(string userName)
        {
            return _repo.GetUsersOfCompany(userName, null);
        }
        [HttpGet]
        [Route("Projects")]
        public IHttpActionResult GetProjects(string userName)
        {
            return Ok(_repo.GetProjects(userName));
        }
       

        [HttpGet]
        [Route("Project/{projectID:int}/Documents")]
        public IHttpActionResult GetDocuments(int projectID)
        {

            return Ok(_repo.GetProjectDocuments(projectID));        //return Request.CreateResponse(HttpStatusCode.Created);

        }
        [HttpGet]
        [Route("Project/{projectID:int}/Users")]
        public IHttpActionResult GetProjectUsers(int projectID)
        {
            return Ok(_repo.GetProjectUsers(projectID));        //return Request.CreateResponse(HttpStatusCode.Created);
        }
        [HttpGet]
        [Route("Project/Documents")]
        public List<Document> GetDocuments(string userName)
        {
            return _repo.GetDocuments(userName);
        }



        [HttpGet]
        [Route("path")]
        public string getpath()
        {
            return HttpContext.Current.Server.MapPath("~/UploadFile/");
        }
        [HttpGet]
        [Route("Project/{projectID:int}/Document/{documentID:int}")]
        public async Task<IHttpActionResult> GetDocument(int projectID, int documentID)
        {
            string downloadFilePath = string.Empty;

            string fileName = string.Empty;
            Document document = _repo.GetDocument(projectID, documentID);
            downloadFilePath = HttpContext.Current.Server.MapPath("~/UploadFile/" + document.Name);
            //Check if the file exists. If the file doesn't exist, throw a file not found exception
            if (!System.IO.File.Exists(downloadFilePath))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            var dataBytes = File.ReadAllBytes(downloadFilePath);
            //adding bytes to memory stream   
            var dataStream = new MemoryStream(dataBytes);

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = document.Name;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return ResponseMessage(httpResponseMessage);
        }
        [HttpGet]
        [Route("CompanyIntegrations")]
        public IHttpActionResult GetCompanyIntegrations()
        {
            return Ok(_repo.GetCompanyIntegrations());
        }
        [HttpPost]
        [Route("Company")]        
        public IHttpActionResult AddCompany([FromBody]Company company)
        {
            return Ok(_repo.AddCompany(company));
        }
        [Authorize]
        [HttpPut]
        [Route("Company/{companyId:long}")]
        public IHttpActionResult UpdateCompany(long companyId, Company company)
        {

            company.CompanyId = companyId;
            return Ok(_repo.UpdateCompany(companyId, company));
        }
        [HttpGet]
        [Route("Companies")]
        [Authorize]
        public IHttpActionResult GetCompanies()
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            foreach (Claim claim in identity.Claims)
            {
                string name = claim.Subject.Name;
                string value = claim.Value;
            }
            return Ok(_repo.GetCompanies());
        }
        [HttpGet]
        [Route("Company/{companyId:long}")]
        public IHttpActionResult GetCompanyByCompanyId(long companyId)
        {
            return Ok(_repo.GetCompanybyCompanyId(companyId));
        }
        [HttpGet]
        [Route("Company/{companyId:long}/Integrations")]
        public IHttpActionResult GetIntegrations(long companyID)
        {
            return Ok(_repo.GetIntegrations(companyID));
        }
        [HttpGet]
        [Route("Company/Integrations/{integrationID:int}")]
        public IHttpActionResult GetIntegrationByID(int integrationID)
        {
            return Ok(_repo.GetIntegrationByID(integrationID));
        }
        [HttpPost]
        [Route("Company/{companyId:long}/Integrations")]
        public IHttpActionResult CreateIntegration(Integration integration)
        {
            _repo.CreateIntegration(integration);
            return Ok();

        }
        [HttpPut]
        [Route("Company/{companyID:long}/Integrations/{integrationID:int}")]
        public IHttpActionResult UpdateIntegration(int integrationID,Integration integration)
        {
            integration.ID = integrationID;
            _repo.UpdateIntegration(integration);
            return Ok();
        }

        [HttpPost]
        [Route("Project/{projectID:int}/Document")]
        public async Task<IHttpActionResult> ProjectDocument(int projectID)
        {
           
            string root = HttpContext.Current.Server.MapPath("~/UploadFile");
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
                    var randomFileName = string.Concat(directory,"\\" + fileWithoutQuote );
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
                }
            }
            if (provider.FormData.Count > 0)
            {
                Document document = new Document();
                document.ProjectID = Convert.ToInt32(provider.FormData["projectID"]);
                document.Name = fileWithoutQuote;
                document.Link = provider.FormData["link"];
                _repo.CreateDocument(document);
            }
            List<ApplicationUser> users = _repo.GetProjectUsers(projectID);
            IEnumerable<ITokeneable> tokens = users.Cast<ITokeneable>();
            new VictorCallsNotifications().SendNotification(tokens, "Data updated notifications",
                false, false, false, true);
            return Ok();        //return Request.CreateResponse(HttpStatusCode.Created);
           
        }
        [HttpPut]
        [Route("User/{userName}/Token/{token}")]
        public IHttpActionResult AddToken([FromUri]string userName, [FromUri]string token)
        {
           token = token.Replace("COLON",":");
           return Ok(_repo.AddToken(userName, token));
        }

        [HttpPost]
        [Route("Notify")]
        public IHttpActionResult Notify()
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAA3Mgl_0Y:APA91bFKigkhtGaXIKoGL60v8hTOT-a4u7OwZ_Y98jK8AlRcqQUcLjmtDHMCuY9i5am54h7XMQzgWSpQS5YusFJ5P5Nym2YqccghCf4EMeVtGcGemwKf_bOsXCqM86GK3r2hCSoDt3yAlp5v2UncAh6gQ1h3UF6YnA"));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "948250738502"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = "diox9F1_T4M:APA91bEcwFciZZZIPabKZIVWYSZk5RxfWXTx0nwjnZiz8WWvjThva1BVPGkLQCUBnnaViKcxd0_M40hLCq8q0QEi-PTBZtWjm6T9Px4lzhKr9uIv0dM2or44eB5iG8T_RiDHqGD6tzwtXaoxUtu6rq_v96WFdb4I0A",
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = "Test",
                    title = "Test",
                    badge = 1
                },
            };



            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
            return Ok();
        }

        [HttpDelete]
        [Route("Users")]
        public IHttpActionResult DeleteUser(string userId)
        {
            _repo.DeleteUser(userId);
            return Ok();
        }
        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName);

            return Redirect(redirectUri);

        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = new IdentityUser() { UserName = model.UserName };

            IdentityResult result = await _repo.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            result = await _repo.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName);

            return Ok(accessTokenResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

            return Ok(accessTokenResponse);

        }
        [HttpGet]
        [Route("Roles")]
        public IHttpActionResult Roles()
        {
            return Ok(_repo.GetRoles());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
        [HttpPost]
        [Route("Document")]
        public HttpResponseMessage PostDocument()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            foreach (string file in httpRequest.Files)
            {
                var postedFile = httpRequest.Files[file];
                var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                postedFile.SaveAs(filePath);
                // NOTE: To store in memory use postedFile.InputStream
            }

            return Request.CreateResponse(HttpStatusCode.Created);
        }
        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                var appToken = "xxxxxx";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else if (provider == "Google")
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }

            }

            return parsedToken;
        }
        //public List<ApplicationUser> GetCompanyUsersByRoleId(string userName, string? roleName)
        //{
        //    List<ApplicationUser> users = null;


        //    return users;
        //}
        private JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("role", "user"),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }

        #endregion
    }
}
