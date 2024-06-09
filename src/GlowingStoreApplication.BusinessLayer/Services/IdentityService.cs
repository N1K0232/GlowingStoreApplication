using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GlowingStoreApplication.Authentication;
using GlowingStoreApplication.Authentication.Entities;
using GlowingStoreApplication.Authentication.Extensions;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.BusinessLayer.Settings;
using GlowingStoreApplication.Shared.Models.Requests;
using GlowingStoreApplication.Shared.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OperationResults;

namespace GlowingStoreApplication.BusinessLayer.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly JwtSettings jwtSettings;

    public IdentityService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettingsOptions)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        jwtSettings = jwtSettingsOptions.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.IsPersistent, false);

        if (signInResult.Succeeded)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.SerialNumber, user.SecurityStamp ?? string.Empty)
            }.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = CreateToken(claims);
            return new AuthResponse(accessToken);
        }

        if (signInResult.IsLockedOut)
        {
            return Result.Fail(FailureReasons.ClientError, "you're locked out", $"you're locked out until {user.LockoutEnd}");
        }

        await userManager.AccessFailedAsync(user);
        return Result.Fail(FailureReasons.ClientError, "Login failed", "invalid username or password");
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, RoleNames.User);
        }

        return result.Succeeded ? Result.Ok() : Result.Fail(FailureReasons.ClientError, "Couldn't registrate", result.GetErrors());
    }

    public async Task<Result<ResetPasswordTokenResponse>> ResetPasswordAsync(ResetPasswordTokenRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return new ResetPasswordTokenResponse(token);
        }

        return Result.Fail(FailureReasons.ItemNotFound, "Not existing account", "This account doesn't exists");
    }

    public async Task<Result> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return result.Succeeded ? Result.Ok() : Result.Fail(FailureReasons.ClientError, "Couldn't update password", result.GetErrors());
        }

        return Result.Fail(FailureReasons.ItemNotFound, "Not existing account", "This account doesn't exists");
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var securityKey = Encoding.UTF8.GetBytes(jwtSettings.SecurityKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken
        (
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials
        );

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    }
}