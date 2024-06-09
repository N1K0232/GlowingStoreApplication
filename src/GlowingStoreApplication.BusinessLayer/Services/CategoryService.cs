using AutoMapper;
using AutoMapper.QueryableExtensions;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.DataAccessLayer;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public CategoryService(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var category = await applicationDbContext.GetAsync<Entities.Category>(id);
        if (category is not null)
        {
            applicationDbContext.Delete(category);
            await applicationDbContext.SaveAsync();

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No category found", $"No category found with id {id}");
    }

    public async Task<Result<Category>> GetAsync(Guid id)
    {
        var dbCategory = await applicationDbContext.GetAsync<Entities.Category>(id);
        if (dbCategory is not null)
        {
            var category = mapper.Map<Category>(dbCategory);
            return category;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No category found", $"No category found with id {id}");
    }

    public async Task<Result<IEnumerable<Category>>> GetListAsync(string name)
    {
        var query = applicationDbContext.GetData<Entities.Category>()
            .WhereIf(name.HasValue(), c => c.Name.Contains(name));

        var categories = await query.OrderBy(c => c.Name)
            .ProjectTo<Category>(mapper.ConfigurationProvider)
            .ToListAsync();

        return categories;
    }

    public async Task<Result<Category>> SaveAsync(SaveCategoryRequest request)
    {
        var exists = await applicationDbContext.GetData<Entities.Category>()
            .AnyAsync(c => c.Name == request.Name && c.Description == request.Description);

        if (exists)
        {
            return Result.Fail(FailureReasons.Conflict, "Category already exists", "This category already exists");
        }

        var dbCategory = mapper.Map<Entities.Category>(request);
        applicationDbContext.Insert(dbCategory);

        var affectedRows = await applicationDbContext.SaveAsync();
        if (affectedRows > 0)
        {
            var category = mapper.Map<Category>(dbCategory);
            return category;
        }

        return Result.Fail(FailureReasons.ClientError, "No category added", "An error occurred and no category was added");
    }

    public async Task<Result<Category>> SaveAsync(Guid id, SaveCategoryRequest request)
    {
        var dbCategory = await applicationDbContext.GetData<Entities.Category>(trackingChanges: true)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (dbCategory is not null)
        {
            mapper.Map(request, dbCategory);
            var affectedRows = await applicationDbContext.SaveAsync();

            if (affectedRows > 0)
            {
                var category = mapper.Map<Category>(dbCategory);
                return category;
            }

            return Result.Fail(FailureReasons.ClientError, "No category updated", "An error occurred. No category updated");
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No category found", $"No category found with id {id}");
    }
}