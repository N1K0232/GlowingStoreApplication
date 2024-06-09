using GlowingStoreApplication.Shared.Collections;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using OperationResults;

namespace GlowingStoreApplication.BusinessLayer.Services.Interfaces;

public interface IProductService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Product>> GetAsync(Guid id);

    Task<Result<ListResult<Product>>> GetListAsync(string searchText, string orderBy, int pageIndex, int itemsPerPage);

    Task<Result<Product>> SaveAsync(SaveProductRequest request);

    Task<Result<Product>> SaveAsync(Guid id, SaveProductRequest request);
}