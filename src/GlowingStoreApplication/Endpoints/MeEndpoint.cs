using System.Security.Claims;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalHelpers.Routing;

namespace GlowingStoreApplication.Endpoints;

public class MeEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var meApiGroup = endpoints.MapGroup("/api/me");

        meApiGroup.MapGet(string.Empty, GetMeAsync)
            .Produces<User>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("UserActive")
            .WithOpenApi();
    }

    private static async Task<Ok<User>> GetMeAsync(IAuthenticatedService authenticatedService, ClaimsPrincipal principal)
    {
        var user = await authenticatedService.GetAsync(principal);
        return TypedResults.Ok(user);
    }
}