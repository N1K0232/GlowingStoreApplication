namespace GlowingStoreApplication.Shared.Models.Requests;

public record class SaveProductRequest(Guid CategoryId, string Name, string Description, int Quantity, decimal Price, double? DiscountPercentage);