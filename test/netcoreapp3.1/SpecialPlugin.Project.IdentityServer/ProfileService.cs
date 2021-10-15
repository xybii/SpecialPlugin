using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault()?.Value;

            if (subjectId == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            var user = await _userManager.FindByIdAsync(subjectId);

            var claims = await GetClaimsFromUser(user);

            context.IssuedClaims.AddRange(claims);
        }

        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault()?.Value;

            if (subjectId == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();

                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);

                        if (db_security_stamp != security_stamp)
                        {
                            return;
                        }
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            return claims;
        }
    }
}
