using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.Extensions;
using GlowingStoreApplication.Shared.Collections;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace GlowingStoreApplication.Endpoints;

public class ProductsEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var productsApiGroup = endpoints.MapGroup("/api/products");

        productsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Administrator")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        productsApiGroup.MapGet("{id:guid}", GetAsync)
            .RequireAuthorization("UserActive")
            .WithName("GetProduct")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        productsApiGroup.MapGet(string.Empty, GetListAsync)
            .RequireAuthorization("UserActive")
            .Produces<ListResult<Product>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        productsApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Administrator")
            .Produces<Product>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem()
            .WithValidation<SaveProductRequest>()
            .WithOpenApi();

        productsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Administrator")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithValidation<SaveProductRequest>()
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IProductService productService, Guid id, HttpContext context)
    {
        var result = await productService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IProductService productService, Guid id, HttpContext context)
    {
        var result = await productService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IProductService productService, HttpContext context, string searchText = null, string orderBy = "Name, Price", int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await productService.GetListAsync(searchText, orderBy, pageIndex, itemsPerPage);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(IProductService productService, SaveProductRequest request, HttpContext context)
    {
        var result = await productService.SaveAsync(request);
        return context.CreateResponse(result, "GetProduct", new { result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(IProductService productService, Guid id, SaveProductRequest request, HttpContext context)
    {
        var result = await productService.SaveAsync(id, request);
        return context.CreateResponse(result);
    }
}