using AutoMapper;
using AutoMapper.QueryableExtensions;
using GlowingStoreApplication.BusinessLayer.Internal;
using GlowingStoreApplication.BusinessLayer.Services.Interfaces;
using GlowingStoreApplication.DataAccessLayer;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.StorageProviders;
using Microsoft.EntityFrameworkCore;
using MimeMapping;
using OperationResults;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Services;

public class ImageService : IImageService
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IStorageProvider storageProvider;
    private readonly IMapper mapper;

    public ImageService(IApplicationDbContext applicationDbContext, IStorageProvider storageProvider, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.storageProvider = storageProvider;
        this.mapper = mapper;
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var image = await applicationDbContext.GetAsync<Entities.Image>(id);
        if (image is not null)
        {
            applicationDbContext.Delete(image);
            await applicationDbContext.SaveAsync();
            await storageProvider.DeleteAsync(image.Path);

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with id {id}");
    }

    public async Task<Result<Image>> GetAsync(Guid id)
    {
        var dbImage = await applicationDbContext.GetAsync<Entities.Image>(id);
        if (dbImage is not null)
        {
            var image = mapper.Map<Image>(dbImage);
            return image;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with id {id}");
    }

    public async Task<Result<IEnumerable<Image>>> GetListAsync()
    {
        var query = applicationDbContext.GetData<Entities.Image>();
        var images = await query.OrderBy(i => i.Path)
            .ProjectTo<Image>(mapper.ConfigurationProvider)
            .ToListAsync();

        return images;
    }

    public async Task<Result<StreamFileContent>> ReadAsync(Guid id)
    {
        var image = await applicationDbContext.GetAsync<Entities.Image>(id);
        if (image is not null)
        {
            var stream = await storageProvider.ReadAsStreamAsync(image.Path);
            if (stream is not null)
            {
                return new StreamFileContent(stream, image.ContentType);
            }

            return Result.Fail(FailureReasons.ItemNotFound, "No stream found", "No stream found for the specified image");
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with id {id}");
    }

    public async Task<Result<Image>> SaveAsync(string fileName, Stream stream, string description)
    {
        var path = PathGenerator.CreatePath(fileName);
        await storageProvider.SaveAsync(path, stream);

        var image = new Entities.Image
        {
            Path = path,
            ContentType = MimeUtility.GetMimeMapping(fileName),
            Length = stream.Length,
            Description = description
        };

        applicationDbContext.Insert(image);
        await applicationDbContext.SaveAsync();

        var savedImage = mapper.Map<Image>(image);
        return savedImage;
    }
}