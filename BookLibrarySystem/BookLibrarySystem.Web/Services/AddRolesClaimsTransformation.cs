using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookLibrarySystem.Web.Services
{
    public class AddRolesClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddRolesClaimsTransformation(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Clone current identity
            var clone = principal.Clone();
            
               var newIdentity = (ClaimsIdentity)clone.Identity;

            // Support AD and local accounts
            var emailId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            ApplicationUser user;
            if (emailId == null)
            {
                var nameId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                user = await _userService.GetByIdAsync(nameId.Value);
            }
            else
            {
                // Get user from database
                user = await _userService.GetByUsernameAsync(emailId.Value);
            }
            
            if (user == null)
            {
                return principal;
            }

            // Add role claims to cloned identity
            //foreach (var role in user.Roles)
            var claims = (await _userManager.GetClaimsAsync(user)).Select(c=>c.Value);
            foreach (var role in claims)
            {
                var claim = new Claim(type: newIdentity.RoleClaimType, value: role);//"NormalUser"
                newIdentity.AddClaim(claim);
            }
            var hasUserRole = clone.IsInRole("NormalUser");
            return clone;
        }
    }
}
