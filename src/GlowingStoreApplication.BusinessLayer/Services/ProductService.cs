using System.Linq.Dynamic.Core;
using AutoMapper;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.DataAccessLayer;
using GlowingStoreApplication.DataAccessLayer.Extensions;
using GlowingStoreApplication.Shared.Collections;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public ProductService(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var query = applicationDbContext.GetData<Entities.Product>(trackingChanges: true);
        var product = await query.FirstOrDefaultAsync(p => p.Id == id);

        if (product is not null)
        {
            applicationDbContext.Delete(product);
            await applicationDbContext.SaveAsync();

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
    }

    public async Task<Result<Product>> GetAsync(Guid id)
    {
        var query = applicationDbContext.GetData<Entities.Product>(trackingChanges: true);
        var dbProduct = await query.FirstOrDefaultAsync(p => p.Id == id);

        if (dbProduct is not null)
        {
            var product = mapper.Map<Product>(dbProduct);
            return product;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
    }

    public async Task<Result<ListResult<Product>>> GetListAsync(string searchText, string orderBy, int pageIndex, int itemsPerPage)
    {
        var query = applicationDbContext.GetData<Entities.Product>().Include(p => p.Category).AsQueryable();

        if (searchText.HasValue())
        {
            query = query.Where(p => p.Name.Contains(searchText));
        }

        var skip = Skip(pageIndex, itemsPerPage);
        var take = Take(itemsPerPage);

        var totalCount = await query.LongCountAsync();
        var totalPages = await query.TotalPagesAsync(itemsPerPage);

        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);
        var dbProducts = await query.OrderBy(orderBy).Skip(skip).Take(take).ToListAsync();

        var products = mapper.Map<IEnumerable<Product>>(dbProducts).Take(itemsPerPage);
        return new ListResult<Product>(products, totalCount, totalPages, hasNextPage);
    }

    public async Task<Result<Product>> SaveAsync(SaveProductRequest request)
    {
        var categoryExists = await applicationDbContext.GetData<Entities.Category>().AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return Result.Fail(FailureReasons.ClientError, "Category not found", "Category not found. Please specify a new category");
        }

        var dbProduct = mapper.Map<Entities.Product>(request);
        applicationDbContext.Insert(dbProduct);

        var affectedRows = await applicationDbContext.SaveAsync();
        if (affectedRows > 0)
        {
            var product = mapper.Map<Product>(dbProduct);
            return product;
        }

        return Result.Fail(FailureReasons.ClientError, "Error occurred", "An error occurred while saving. No product was added");
    }

    public async Task<Result<Product>> SaveAsync(Guid id, SaveProductRequest request)
    {
        var query = applicationDbContext.GetData<Entities.Product>(trackingChanges: true);
        var dbProduct = await query.FirstOrDefaultAsync(p => p.Id == id);

        if (dbProduct is not null)
        {
            mapper.Map(request, dbProduct);
            var affectedRows = await applicationDbContext.SaveAsync();

            if (affectedRows > 0)
            {
                var product = mapper.Map<Product>(dbProduct);
                return product;
            }

            return Result.Fail(FailureReasons.ClientError, "Error occurred", "An error occurred while saving. No product was updated");
        }

        return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
    }

    private static int Skip(int pageIndex, int itemsPerPage) => pageIndex * itemsPerPage;

    private static int Take(int itemsPerPage) => itemsPerPage + 1;
}