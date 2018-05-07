using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class AssignedUser
    {
        public string ID { get; set; }

        public string AssignedTo { get; set; }

        public bool Enabled { get; set; }
    }
}
