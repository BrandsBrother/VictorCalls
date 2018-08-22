using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Filter
    {
        public string userName { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public int? statusID { get; set; }
        public int? leadNumber { get; set; }
        public int? projectId { get; set; }
        public string leadName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
           public string assignedTo { get; set; }
    }
}
