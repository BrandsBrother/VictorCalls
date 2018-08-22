using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Attendence
    {
        #region Constants
        public const string DB_DistanceIn = "@DistanceIn";
        public const string DB_Attendence = "@Attendence";
        public const string DB_DateIn = "@DateIn";
        public const string DB_DateOut = "@DateOut";
        public const string DB_DistanceOut = "@DistanceOut";
        public const string DB_UserId = "@UserId";
        public const string DB_UserName = "@UserName";
        public const string DB_AttendenceId = "@AttendenceId";
        public const string DB_Date = "@Date";
        #endregion
        public int AttendenceId { get; set; }
        public string UserId { get; set; }
        public float DistanceIn { get; set; }
        public bool Attendance { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public DateTime Date { get; set; }
        public float DistanceOut { get; set; }
        public string UserName { get; set; }

    }
}
