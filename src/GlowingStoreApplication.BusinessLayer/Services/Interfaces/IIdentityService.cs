using GlowingStoreApplication.Shared.Models.Requests;
using GlowingStoreApplication.Shared.Models.Responses;
using OperationResults;

namespace GlowingStoreApplication.BusinessLayer.Services.Interfaces;

public interface IIdentityService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

    Task<Result> RegisterAsync(RegisterRequest request);

    Task<Result<ResetPasswordTokenResponse>> ResetPasswordAsync(ResetPasswordTokenRequest request);

    Task<Result> UpdatePasswordAsync(UpdatePasswordRequest request);
}