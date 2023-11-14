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
            var emailId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type=="email");
            ApplicationUser user;
            if (emailId == null)
            {
                var nameId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type=="name");
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
            var roles = await _userManager.GetRolesAsync(user);

            //check if user is not blocked, his status, and then give him the role
            if (user.Status != UserStatus.Blocked)
            {
                foreach (var role in roles)
                {
                    var claim = new Claim(type: newIdentity.RoleClaimType, value: role);
                    newIdentity.AddClaim(claim);
                }
            }

            var hasUserRole = clone.IsInRole("NormalUser");
            return clone;
        }
    }
}
