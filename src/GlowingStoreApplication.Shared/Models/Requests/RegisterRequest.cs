namespace GlowingStoreApplication.Shared.Models.Requests;

public record class RegisterRequest(string FirstName, string LastName, string Email, string Password);