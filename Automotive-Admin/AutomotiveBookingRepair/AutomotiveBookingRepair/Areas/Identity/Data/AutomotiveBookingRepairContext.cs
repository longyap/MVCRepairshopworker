using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveBookingRepair.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutomotiveBookingRepair.Data
{
    public class AutomotiveBookingRepairContext : IdentityDbContext<AutomotiveBookingRepairUser>
    {
        public AutomotiveBookingRepairContext(DbContextOptions<AutomotiveBookingRepairContext> options)
            : base(options)
        {
        }
        // i want a dbset that follow the structure of AutomotiveBookingRepair
        // linked the class structure and table together
        public DbSet <AutomotiveBookingRepair.Models.UserData>UserData { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
