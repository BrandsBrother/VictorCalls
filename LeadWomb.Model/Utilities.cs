using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class Utilities
    {
        public static DateTimeOffset ChangeToOffset(DateTime dateTime)
        {
            DateTimeOffset localTime = DateTimeOffset.MinValue;
            if (dateTime != null && dateTime != DateTime.MinValue)
            {
                DateTime localTime1 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                localTime = DateTime.SpecifyKind(localTime1, DateTimeKind.Local);
            }
            return localTime;
        }

      

        public static DateTimeOffset? CheckDateTimeOffsetIfNullOrEmpty(DateTime datetime)
        {
            if (datetime == null || datetime == DateTime.MinValue)
            {
                return null;
            }
            return ChangeToOffset(datetime);
        }

        public static DateTime? CheckDateTimeIfNullOrEmpty(DateTime datetime)
        {
            if (datetime == null || datetime == DateTime.MinValue)
            {
                return null;
            }
            return datetime;
        }
    }
}
