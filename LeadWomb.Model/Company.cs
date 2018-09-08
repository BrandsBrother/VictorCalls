using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Company
    {
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public DateTime ActivatedTill { get; set; }
        public bool IsActivated { get; set; }
        public int CompanyType { get; set; }
        public string Logopath { get; set; }
        public List<Integration> Integrations { get; set; }
        public List<Project> Projects { get; set; }
    }
}
