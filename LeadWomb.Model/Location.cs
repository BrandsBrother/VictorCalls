using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace LeadWomb.Model
{
    public class Location
    {
        [ScriptIgnore]
        public int LocationID { get; set; }

        
        public string title { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }
        [ScriptIgnore]
        public long CompanyID { get; set; }

        public string description { get; set; }

    }
}
