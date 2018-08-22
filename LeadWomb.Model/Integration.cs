using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Integration
    {
        public int ID { get; set; }
        public LeadSourceType SourceType { get; set; }
        public long CompanyId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
