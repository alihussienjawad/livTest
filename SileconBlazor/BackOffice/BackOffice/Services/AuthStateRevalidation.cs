using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
 
using System.Security.Claims;
 
using ClassLibrary.Models;

namespace BackOffice.Services
{
    public class AuthStateRevalidation (ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<IdentityOptions> options
        
        ): RevalidatingServerAuthenticationStateProvider(loggerFactory)
    {
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(20);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var userMaganger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            return await ValidateSecurityTimeStampAsync(userMaganger,
                authenticationState.User
                );
        }

        private async Task<bool> ValidateSecurityTimeStampAsync(UserManager<ApplicationUser> userManager,ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null) return false;

            var principalStamp= principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);

            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp==userStamp;


        }
    }
}
