using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Project
    {
        #region constants
        public const string DB_PROJECT_ID = "@ProjectId";
        public const string DB_PROJECT_NAME = "@Name";
        public const string DB_PRO_DESCRIPTION = "@Description";
        public const string DB_LONGITUDE = "@Longtitude";
        public const string DB_LATTITUDE = "@Lattitude";
        public const string DB_COMPANYID = "@CompanyId";
        public const string DB_DISTRICT = "@District";
        #endregion

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Longitude { get; set; }
        public string Lattitude { get; set; }
        public long CompanyId { get; set; }
        public string District { get; set; }
        public List<Document> Documents { get; set; }
    }
}
