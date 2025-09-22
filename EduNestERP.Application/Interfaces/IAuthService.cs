public record LoginResult(string Token, string Role, bool passwordResetRequired,Guid UserGuid);

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string tenantId, string userId, string password, CancellationToken ct = default);
    Task<bool> FirstResetAsync(string tenantId, string userId, string newPassword, CancellationToken ct = default);
}
