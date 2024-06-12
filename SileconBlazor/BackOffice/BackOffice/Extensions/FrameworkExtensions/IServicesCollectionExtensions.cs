using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using BackOffice.Services;

namespace BackOffice.Extensions.FrameworkExtensions
{
    public static class IServicesCollectionExtensions
    {

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, AuthStateRevalidation>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            }).AddIdentityCookies();

            services.AddScoped<CookieEvents>();
            services.ConfigureApplicationCookie(options =>
            {
                options.EventsType=typeof(CookieEvents);
            });


            return services;
        }
    }
}
