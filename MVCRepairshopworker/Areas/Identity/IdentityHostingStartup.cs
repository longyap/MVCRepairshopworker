using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MVCRepairshopworker.Data;

[assembly: HostingStartup(typeof(MVCRepairshopworker.Areas.Identity.IdentityHostingStartup))]
namespace MVCRepairshopworker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MVCRepairshopworkerContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MVCRepairshopworkerContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<MVCRepairshopworkerContext>();
            });
        }
    }
}