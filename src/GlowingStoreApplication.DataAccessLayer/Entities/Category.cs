using GlowingStoreApplication.DataAccessLayer.Entities.Common;

namespace GlowingStoreApplication.DataAccessLayer.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }
}