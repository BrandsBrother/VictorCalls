using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadWomb.Model
{
    public class LeadStatusCounts
    {
        public int CurrentLeadsCount { get; set; }

        public int NoWorkCount { get; set; }

        public int NotConnectedCount { get; set; }
        public int FollowUpsCount { get; set; }

        public int VisitOnCounts { get; set; }

        public int VisitDoneCount { get; set; }

        public int VisitDeadCount { get; set; }

        public int OtherProjectsCount { get; set; }

        public int ResaleCount { get; set; }

        public int AlreadyBookedCount { get; set; }

        public int BookedDone { get; set; }

        public int DeadCount { get; set; }


        public int RentCount { get; set; }
      
        public int PlotCount { get; set; }

        public int DuplicateCount { get; set; }

    }
}
