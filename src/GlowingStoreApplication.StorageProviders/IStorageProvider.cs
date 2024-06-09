namespace GlowingStoreApplication.StorageProviders;

public interface IStorageProvider
{
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default);

    async Task<string?> ReadAsStringAsync(string path, CancellationToken cancellationToken = default)
    {
        using var stream = await ReadAsStreamAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        stream.Close();
        return content;
    }

    Task SaveAsync(string path, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default);

    async Task SaveAsync(string path, byte[] content, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        await SaveAsync(path, stream, overwrite, cancellationToken);
    }
}