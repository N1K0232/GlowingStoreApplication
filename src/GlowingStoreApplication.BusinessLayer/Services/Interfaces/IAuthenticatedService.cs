using System.Security.Claims;
using GlowingStoreApplication.Shared.Models;

namespace GlowingStoreApplication.BusinessLayer.Services.Interfaces;

public interface IAuthenticatedService
{
    Task<User> GetAsync(ClaimsPrincipal principal);
}