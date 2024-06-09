using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Shared.Models.Requests;
using GlowingStoreApplication.Shared.Models.Responses;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace GlowingStoreApplication.Endpoints;

public class AuthEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authEndpointGroup = endpoints.MapGroup("/api/auth");

        authEndpointGroup.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest) //this response is triggered when the login fails
            .ProducesValidationProblem() //this response is triggered when the validation fails
            .WithOpenApi();

        authEndpointGroup.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .WithOpenApi();

        authEndpointGroup.MapPost("resetpassword", ResetPasswordAsync)
            .AllowAnonymous()
            .Produces<ResetPasswordTokenResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithOpenApi();

        authEndpointGroup.MapPost("updatepassword", UpdatePasswordAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithOpenApi();
    }

    private static async Task<IResult> LoginAsync(IIdentityService identityService, LoginRequest request, HttpContext context)
    {
        var result = await identityService.LoginAsync(request);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> RegisterAsync(IIdentityService identityService, RegisterRequest request, HttpContext context)
    {
        var result = await identityService.RegisterAsync(request);
        return context.CreateResponse(result, StatusCodes.Status200OK);
    }

    private static async Task<IResult> ResetPasswordAsync(IIdentityService identityService, ResetPasswordTokenRequest request, HttpContext context)
    {
        var result = await identityService.ResetPasswordAsync(request);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> UpdatePasswordAsync(IIdentityService identityService, UpdatePasswordRequest request, HttpContext context)
    {
        var result = await identityService.UpdatePasswordAsync(request);
        return context.CreateResponse(result);
    }
}