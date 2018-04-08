using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{


    public class abc
    {
        public string test { get; set; }

        public string testa{get;set;}

        public string testb { get; set; }
    }


    public class Leads
    {
        
        public int LeadId { get; set; }
        public string CreateUserID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string EditUserId { get; set; }

        public DateTime? EditDateTime { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public List<LeadItems> Items { get; set; }
  

    }
}
