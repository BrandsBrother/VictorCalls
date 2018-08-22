using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set;}
        public string Description { get; set; }
        public string Distict { get; set; }
        public int Longtitute { get; set; }
        public int Latitute { get; set; }
        public int CompanyId { get; set; }
    }
}
