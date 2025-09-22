using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;
using Npgsql;

public sealed class AuthService : IAuthService
{
    private readonly TenantDbFactory _factory;
    private readonly IUserRepository _users;
    private readonly IConfiguration _cfg;

    public AuthService(TenantDbFactory factory, IUserRepository users, IConfiguration cfg)
    { _factory = factory; _users = users; _cfg = cfg; }

    public async Task<LoginResult> LoginAsync(string tenantId, string userId, string password, CancellationToken ct = default)
    {
        var ds = _factory.Get(tenantId);
        var user = await _users.GetByUserIdAsync(ds, userId.Trim(), ct);
        if (user is null) throw new UnauthorizedAccessException();

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException();

        if (!user.FirstLoginCompleted)
        {
            return new LoginResult("", "", true,user.Id);
        }
        // (Optionally enforce first_login_completed here)

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["JWT:Key"] ?? "dev-secret-change-me"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserId),
            new Claim("tenantId", tenantId),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var jwt = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new LoginResult(token, user.Role,false,user.Id);
    }

    public async Task<bool> FirstResetAsync(string tenantId, string userId, string newPassword, CancellationToken ct = default)
    {
        var ds = _factory.Get(tenantId);
        var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 11);
        return await _users.SetPasswordAndCompleteFirstLoginAsync(ds, userId.Trim(), newHash, ct);
    }
}
