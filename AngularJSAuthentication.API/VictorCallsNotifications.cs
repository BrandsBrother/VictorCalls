using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using LeadWomb.Model;

namespace AngularJSAuthentication.API.Notifications
{
    /// <summary>
    /// Class is used for sending updations 
    /// notifications to devices
    /// </summary>
    public class VictorCallsNotifications
    {
        /// <summary>
        /// 
        /// </summary>
        public VictorCallsNotifications()
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="body"></param>
        /// <param name="leadsUpdated"></param>
        /// <param name="projectsUpdated"></param>
        /// <param name="usersUpdated"></param>
        /// <param name="documentsUpdated"></param>
        public void SendNotification(string token, string body, bool leadsUpdated,
            bool projectsUpdated, bool usersUpdated, bool documentsUpdated)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", ConfigurationSettings.AppSettings["serverKey"]));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationSettings.AppSettings["senderID"]));
            tRequest.ContentType = "application/json";

            var payload = new
            {
                to = token,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = body,
                    title = "Data update notification"
                },
                data = new
                {
                    LeadsUpdated = leadsUpdated,
                    ProjectsUpdated = projectsUpdated,
                    UsersUpdated = usersUpdated,
                    DocumentsUpdated = documentsUpdated
                }
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="body"></param>
        /// <param name="leadsUpdated"></param>
        /// <param name="projectsUpdated"></param>
        /// <param name="usersUpdated"></param>
        /// <param name="documentsUpdated"></param>
        public void SendNotification(IEnumerable<ITokeneable> tokens, string body, bool leadsUpdated,
            bool projectsUpdated, bool usersUpdated, bool documentsUpdated)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ConfigurationSettings.AppSettings["serverKey"]));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationSettings.AppSettings["senderID"]));
                tRequest.ContentType = "application/json";
                foreach (ITokeneable token in tokens)
                {
                    if (!string.IsNullOrEmpty(token.Token))
                    {
                        var payload = new
                        {
                            to = token.Token,
                            priority = "high",
                            content_available = true,
                            notification = new
                            {
                                body = body,
                                title = "VictorCalls",
                                badge = 1
                            },
                            data = new
                            {
                                LeadsUpdated = leadsUpdated,
                                ProjectsUpdated = projectsUpdated,
                                UsersUpdated = usersUpdated,
                                DocumentsUpdated = documentsUpdated
                            }
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
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
