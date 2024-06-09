using System.Net.Mime;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Models;
using GlowingStoreApplication.Shared.Models;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace GlowingStoreApplication.Endpoints;

public class ImagesEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var imagesApiGroup = endpoints.MapGroup("/api/images");

        imagesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Administrator")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetImage")
            .RequireAuthorization("UserActive")
            .Produces<Image>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapGet(string.Empty, GetListAsync)
            .RequireAuthorization("UserActive")
            .Produces<IEnumerable<Image>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        imagesApiGroup.MapGet("{id:guid}/content", ReadAsync)
            .RequireAuthorization("UserActive")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapPost(string.Empty, SaveAsync)
            .RequireAuthorization("Administrator")
            .Accepts<FormFileContent>(MediaTypeNames.Multipart.FormData)
            .Produces<Image>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IImageService imageService, HttpContext context)
    {
        var result = await imageService.GetListAsync();
        return context.CreateResponse(result);
    }

    private static async Task<IResult> ReadAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.ReadAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> SaveAsync(IImageService imageService, HttpContext context, FormFileContent content, string description = null)
    {
        var result = await imageService.SaveAsync(content.File.FileName, content.File.OpenReadStream(), description);
        return context.CreateResponse(result, "GetImage", new { id = result.Content?.Id });
    }
}