using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto_Cust.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Auto_Cust.Models;


namespace Auto_Cust.Data
{
    public class Auto_CustContext : IdentityDbContext<Auto_CustUser>
    {
        public Auto_CustContext(DbContextOptions<Auto_CustContext> options)
            : base(options)
        {
        }

        //this part use to define what new table will be generate / refer in the database
        public DbSet<cus_automotive> CustomerAutomotive { get; set; }

        public DbSet<Booking> BookingTable { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
