using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class LeadItems
    {
        public int LeadID { get; set; }
        public string QueryRemarks { get; set; }

        public int TypeOfProperty { get; set; }

        public int Status { get; set; }

        public int RangeFrom { get; set; }

        public int RangeTo { get; set; }

        public string CompactLabel { get; set; }

        public DateTime? RecivedOn { get; set; }

        public string ProjectName { get; set; }

        public string AssignedTo { get; set; }

        public bool? BuilderInterest { get; set; }

        public int StatusId { get; set; }

        public DateTime StatusDate { get; set; }

        public long CompanyId { get; set; }

        public int LeadItemID { get; set; }
    }
}
