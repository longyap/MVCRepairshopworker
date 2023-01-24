using System;
using Auto_Cust.Areas.Identity.Data;
using Auto_Cust.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Auto_Cust.Areas.Identity.IdentityHostingStartup))]
namespace Auto_Cust.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Auto_CustContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Auto_CustContextConnection")));

                services.AddDefaultIdentity<Auto_CustUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()    
                .AddEntityFrameworkStores<Auto_CustContext>();
            });
        }
    }
}