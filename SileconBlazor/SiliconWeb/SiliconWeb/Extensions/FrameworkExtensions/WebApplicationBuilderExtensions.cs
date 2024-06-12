using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
 
using ClassLibrary.Models;

namespace SiliconWeb.Extensions.FrameworkExtensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder ConfigureIdentity(this WebApplicationBuilder builder)
        {
            var cs = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(opt => opt.UseSqlServer(cs));
            builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(cs));
            builder.Services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 5;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.ClaimsIdentity.UserIdClaimType = "siliconAppId";
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<ApplicationDbContext>();
           return builder;
        }
    }
}
 