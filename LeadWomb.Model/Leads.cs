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

        public string AssignedToUsers
        {
            get {
                string variable = string.Empty;
                string separator = ",";
                if (Items != null && Items.Count > 0)
                {
                    foreach (LeadItems item in Items)
                    {
                        if (variable == string.Empty)
                        {
                            separator = string.Empty;
                        }
                        else
                        {
                            separator = ",";
                        }
                        if (item.IsAssigned)
                        {
                            variable = variable + separator + item.UserName;
                        }
                    }
                }
                return variable;

            }

        }

       // public List<AssignedUser> AssignedUsers { get; set; }
  

    }
}
