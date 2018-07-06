using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Leads : VictorCallsBase
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

        public long CompanyId { get; set; }

        public bool IsAssigned { get; set; }
        public string CmpctLabel { get; set; }

        public List<LeadItems> AssignedUsers
        {
            
            get {
                List<LeadItems> items = new List<LeadItems>();
                if (Items != null && Items.Count > 0)
                {
                    foreach (LeadItems item in Items)
                    {
                        if (item.IsAssigned)
                        {
                            items.Add(item);
                        }
                    }
                }
                return items;
            }
        }
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
        public string LeadSource
        {
            get {
                if (Status == 2)
                {
                    return "99acres";
                }
                else if (Status == 1)
                { return "magicbricks"; }
                else
                {
                    return "raw";
                }
            }
        }

        public int Status { get;set;}
        public List<LeadItems> Assignees { get; set; }

       // public List<AssignedUser> AssignedUsers { get; set; }
  

    }
}
