using GlowingStoreApplication.DataAccessLayer.Entities.Common;

namespace GlowingStoreApplication.DataAccessLayer.Entities;

public class Product : DeletableEntity
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public double? DiscountPercentage { get; set; }

    public virtual Category Category { get; set; }
}