using GlowingStoreApplication.DataAccessLayer.Entities.Common;

namespace GlowingStoreApplication.DataAccessLayer.Entities;

public class Image : BaseEntity
{
    public string Path { get; set; }

    public string ContentType { get; set; }

    public long Length { get; set; }

    public string Description { get; set; }
}