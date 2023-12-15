using BugOut.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BugOut.Services.Factories
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public AppUserClaimsPrincipalFactory(IOptions<IdentityOptions> optionsAccessor,
                                                RoleManager<IdentityRole> roleManager,
                                                UserManager<AppUser> userManager)
            :base(userManager, roleManager, optionsAccessor)
        {
           
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));

            return identity;
        }

    }
}
