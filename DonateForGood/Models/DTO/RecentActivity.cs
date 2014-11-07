using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DonateForGood.Models
{
    public class RecentActivity
    {
        public string ItemName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string EmailAddress { get; set; }

        //public IEnumerable<RecentActivity> recentActivityList { get; set; }
    }
}