using GlowingStoreApplication.Shared.Models.Common;

namespace GlowingStoreApplication.Shared.Models;

public class Image : BaseObject
{
    public string Path { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long Length { get; set; }

    public string? Description { get; set; }
}