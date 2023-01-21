using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AutomotiveBookingRepair.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AutomotiveBookingRepairUser class
    public class AutomotiveBookingRepairUser : IdentityUser // model use for database
    {
        [PersonalData] 
        public string CustomerFullName { get; set; }

        [PersonalData] 
        public int CustomerAge{ get; set; }

        [PersonalData] 
        public string CustomerAddress { get; set; }

        [PersonalData] 
        public DateTime CustomerDOB { get; set; }

        [PersonalData]
        public string userrole { get; set; }
    }
}
