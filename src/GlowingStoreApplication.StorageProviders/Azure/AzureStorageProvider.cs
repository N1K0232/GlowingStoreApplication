using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace GlowingStoreApplication.StorageProviders;

public class AzureStorageProvider : IStorageProvider
{
    private readonly AzureStorageOptions options;
    private readonly ILogger<AzureStorageProvider> logger;
    private readonly BlobServiceClient blobServiceClient;

    public AzureStorageProvider(AzureStorageOptions options, ILogger<AzureStorageProvider> logger)
    {
        this.options = options;
        this.logger = logger;
        blobServiceClient = new BlobServiceClient(options.ConnectionString);
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("attempting to delete the stream for the specified path");

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(options.ContainerName);
        await blobContainerClient.DeleteBlobIfExistsAsync(path, cancellationToken: cancellationToken);
    }

    public async Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("attempting to retrieve the stream for the specified path");
        var blobClient = await GetBlobClientAsync(path, false, cancellationToken);

        var exists = await blobClient.ExistsAsync(cancellationToken);
        if (!exists)
        {
            logger.LogError("the file doesn't exists");
            return null;
        }

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return stream;
    }

    public async Task SaveAsync(string path, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("saving a new file in a the azure storage");

        var blobClient = await GetBlobClientAsync(path, true, cancellationToken);
        if (!overwrite)
        {
            var exists = await blobClient.ExistsAsync(cancellationToken);
            if (exists)
            {
                logger.LogError("the file already exists");
                throw new IOException($"The file {path} already exists");
            }
        }

        var headers = new BlobHttpHeaders
        {
            ContentType = MimeUtility.GetMimeMapping(path)
        };

        stream.Position = 0;
        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken);
    }

    private async Task<BlobClient> GetBlobClientAsync(string path, bool createIfNotExists = false, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(options.ContainerName);

        if (createIfNotExists)
        {
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        }

        return blobContainerClient.GetBlobClient(path);
    }
}