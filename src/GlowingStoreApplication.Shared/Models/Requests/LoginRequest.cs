namespace GlowingStoreApplication.Shared.Models.Requests;

public record class LoginRequest(string UserName, string Password, bool IsPersistent);