namespace GlowingStoreApplication.Shared.Models.Requests;

public record class UpdatePasswordRequest(string Email, string Token, string NewPassword);