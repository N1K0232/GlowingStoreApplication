using GlowingStoreApplication.Shared.Models;
using OperationResults;

namespace GlowingStoreApplication.BusinessLayer.Services.Interfaces;

public interface IImageService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Image>> GetAsync(Guid id);

    Task<Result<IEnumerable<Image>>> GetListAsync();

    Task<Result<StreamFileContent>> ReadAsync(Guid id);

    Task<Result<Image>> SaveAsync(string fileName, Stream stream, string description);
}