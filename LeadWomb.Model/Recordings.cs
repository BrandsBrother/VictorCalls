using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Recordings
    {
        public int ID { get; set; }
        public int Lead_ID { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
