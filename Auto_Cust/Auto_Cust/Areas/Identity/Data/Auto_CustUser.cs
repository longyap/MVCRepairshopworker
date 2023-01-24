using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Auto_Cust.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Auto_CustUser class
    public class Auto_CustUser : IdentityUser
    {
        [PersonalData]

        public string CustomerFullName { get; set; }
        [PersonalData]

        public int CustomerAge { get; set; }
        [PersonalData]

        public string CustomerAddress { get; set; }
        [PersonalData]

        public DateTime CustomerDOB { get; set; }

        [PersonalData]
        public string userrole { get; set; }
    }
}
