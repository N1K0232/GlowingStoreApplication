using GlowingStoreApplication.Shared.Models.Common;

namespace GlowingStoreApplication.Shared.Models;

public class Category : BaseObject
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}