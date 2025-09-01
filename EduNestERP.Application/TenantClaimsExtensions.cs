using System.Security.Claims;

public static class TenantClaimsExtensions
{
    public static string GetTenantId(this ClaimsPrincipal user)
    {
        var v =
            user.FindFirst("tenantId")?.Value ??
            user.FindFirst("tenant_id")?.Value ??
            user.FindFirst("tid")?.Value ??
            user.FindFirst("custom:tenantId")?.Value;

        if (string.IsNullOrWhiteSpace(v))
            throw new UnauthorizedAccessException("Tenant id missing.");
        return v.ToLowerInvariant();
    }

    public static string GetRole(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
}
