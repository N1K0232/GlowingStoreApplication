using System.Security.Claims;
using GlowingStoreApplication.Authentication.Entities;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace GlowingStoreApplication.BusinessLayer.Services;

public class AuthenticatedService(UserManager<ApplicationUser> userManager) : IAuthenticatedService
{
    public async Task<User> GetAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        return new User(user.FirstName, user.LastName, user.Email);
    }
}