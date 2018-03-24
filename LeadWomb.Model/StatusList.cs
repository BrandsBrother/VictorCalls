using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class StatusList
    {
        private Dictionary<int, string> statusList = new Dictionary<int, string>();
        private Dictionary<int, string> typeOfProperty = new Dictionary<int, string>();
       
        /// <summary>
        /// 
        /// </summary>
        public StatusList()
        {            
            statusList.Add(1, "Raw");
            statusList.Add(2, "Current");
            statusList.Add(3, "FollowUps");
            statusList.Add(4, "Pending");
            statusList.Add(5, "Visits On");
            statusList.Add(6, "Visits Done");
            statusList.Add(7, "Dead");
            statusList.Add(8, "Not Connected");
            statusList.Add(9, "Rent");
            statusList.Add(10, "Other projects");
            statusList.Add(11, "Plot");
            statusList.Add(12, "Resale");
            statusList.Add(13, "Lead trans to Closure");

            typeOfProperty.Add(1, "1BHK");
            typeOfProperty.Add(2, "1BHK + Study");
            typeOfProperty.Add(3,"2BHK");
            typeOfProperty.Add(4, "2BHK + Study");
            typeOfProperty.Add(5, "3BHK");
            typeOfProperty.Add(6, "3BHK + Study");
            typeOfProperty.Add(7, "Villa");
            typeOfProperty.Add(8, "Plot");           

        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> Status
        {
            get{
            return statusList;   
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> TypeOfProperty
        {
            get{
            return typeOfProperty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public string GetStatus(int statusId)
        {
            return statusList[statusId];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topID"></param>
        /// <returns></returns>
        public string GetTypeOfProperty(int topID)
        {
            return typeOfProperty[topID];
        }
    }
}
