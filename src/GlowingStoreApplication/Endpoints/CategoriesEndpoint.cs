using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Extensions;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace GlowingStoreApplication.Endpoints;

public class CategoriesEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var categoriesApiGroup = endpoints.MapGroup("/api/categories");

        categoriesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Administrator")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        categoriesApiGroup.MapGet("{id:guid}", GetAsync)
            .RequireAuthorization("UserActive")
            .WithName("GetCategory")
            .Produces<Category>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        categoriesApiGroup.MapGet(string.Empty, GetListAsync)
            .RequireAuthorization("UserActive")
            .Produces<IEnumerable<Category>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        categoriesApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Administrator")
            .Produces<Category>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesValidationProblem()
            .WithValidation<SaveCategoryRequest>()
            .WithOpenApi();

        categoriesApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Administrator")
            .Produces<Category>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithValidation<SaveCategoryRequest>()
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(ICategoryService categoryService, Guid id, HttpContext context)
    {
        var result = await categoryService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ICategoryService categoryService, Guid id, HttpContext context)
    {
        var result = await categoryService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ICategoryService categoryService, HttpContext context, string name = null)
    {
        var result = await categoryService.GetListAsync(name);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(ICategoryService categoryService, SaveCategoryRequest request, HttpContext context)
    {
        var result = await categoryService.SaveAsync(request);
        return context.CreateResponse(result, "GetCategory", new { result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ICategoryService categoryService, Guid id, SaveCategoryRequest request, HttpContext context)
    {
        var result = await categoryService.SaveAsync(id, request);
        return context.CreateResponse(result);
    }
}