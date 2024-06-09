using GlowingStoreApplication.Shared.Models.Common;

namespace GlowingStoreApplication.Shared.Models;

public class Product : BaseObject
{
    public string Category { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public double? DiscountPercentage { get; set; }
}