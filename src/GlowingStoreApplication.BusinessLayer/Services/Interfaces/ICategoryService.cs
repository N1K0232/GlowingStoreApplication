using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using OperationResults;

namespace GlowingStoreApplication.BusinessLayer.Services.Interfaces;

public interface ICategoryService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Category>> GetAsync(Guid id);

    Task<Result<IEnumerable<Category>>> GetListAsync(string name);

    Task<Result<Category>> SaveAsync(SaveCategoryRequest request);

    Task<Result<Category>> SaveAsync(Guid id, SaveCategoryRequest request);
}