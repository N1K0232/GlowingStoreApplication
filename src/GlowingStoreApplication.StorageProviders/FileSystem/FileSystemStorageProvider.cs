using Microsoft.Extensions.Logging;

namespace GlowingStoreApplication.StorageProviders;

public class FileSystemStorageProvider : IStorageProvider
{
    private readonly FileSystemStorageOptions options;
    private readonly ILogger<FileSystemStorageProvider> logger;

    public FileSystemStorageProvider(FileSystemStorageOptions options, ILogger<FileSystemStorageProvider> logger)
    {
        this.options = options;
        this.logger = logger;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("attempting to delete the stream for the specified path");
        var fullPath = CreatePath(path);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("attempting to retrieve the stream for the specified path");
        var fullPath = CreatePath(path);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        var stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream?>(stream);
    }

    public async Task SaveAsync(string path, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("saving a new file in a the azure storage");
        var fullPath = CreatePath(path);

        if (!overwrite)
        {
            if (File.Exists(fullPath))
            {
                throw new IOException($"The file {path} already exists");
            }
        }

        var directoryName = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        using var outputStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        stream.Position = 0;

        await stream.CopyToAsync(outputStream, cancellationToken);
        await stream.FlushAsync(cancellationToken);

        outputStream.Close();
        stream.Close();
    }

    private string CreatePath(string path)
    {
        var fullPath = Path.Combine(options.StorageFolder, path);
        if (!Path.IsPathRooted(fullPath))
        {
            return Path.Combine(options.SiteRootFolder, fullPath);
        }

        return fullPath;
    }
}