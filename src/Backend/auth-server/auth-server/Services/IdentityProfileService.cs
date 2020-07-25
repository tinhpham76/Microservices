using auth_server.Data.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace auth_server.Services
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IdentityProfileService(
            IUserClaimsPrincipalFactory<User> claimsFactory,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _claimsFactory = claimsFactory;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            if (user == null)
            {
                throw new ArgumentException("");
            }

            var claims = new List<Claim>();
            var userRoles = await _userManager.GetRolesAsync(user);
            var Permissions = new List<string>();
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByIdAsync(userRole);
                var claim = await _roleManager.GetClaimsAsync(role);
                var permission = claim.Select(t => t.Type + "_" + t.Value).ToList();
                Permissions.AddRange(permission);
            }
            var avatar = user.AvatarUri == null ? "https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png" : user.AvatarUri;

            //Add more claims like this
            claims.Add(new Claim("FullName", user.LastName + " " + user.FirstName));
            claims.Add(new Claim("Role", string.Join(";", userRoles)));
            claims.Add(new Claim("Permissions", JsonConvert.SerializeObject(Permissions)));
            claims.Add(new Claim("UserName", user.UserName));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new Claim("Avatar", avatar));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
