using System;
using AutomotiveBookingRepair.Areas.Identity.Data;
using AutomotiveBookingRepair.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AutomotiveBookingRepair.Areas.Identity.IdentityHostingStartup))]
namespace AutomotiveBookingRepair.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AutomotiveBookingRepairContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AutomotiveBookingRepairContextConnection")));

                services.AddDefaultIdentity<AutomotiveBookingRepairUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AutomotiveBookingRepairContext>();
            });
        }
    }
}