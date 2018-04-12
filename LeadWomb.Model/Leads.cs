using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
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

        public List<AssignedUser> AssignedUsers { get; set; }
  

    }
}
