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
        public int FollowUpsCount { get; set; }

        public int VisitOnCounts { get; set; }

        public int VisitDoneCount { get; set; }

        public int NotConnectedCount { get; set; }

        public int DeadCount { get; set; }

        public int PendingLeadsCount { get; set; }

        public int ResaleCount { get; set; }

        public int OtherProjectsCount { get; set; }

        public int ClosureCount { get; set; }

        public int RentCount { get; set; }

        public int PlotCount { get; set; }
    }
}
