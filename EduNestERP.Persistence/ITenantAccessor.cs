using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public interface ITenantAccessor
{
    string TenantId { get; }
}

public sealed class HttpTenantAccessor : ITenantAccessor
{
    private readonly IHttpContextAccessor _http;
    public HttpTenantAccessor(IHttpContextAccessor http) => _http = http;

    public string TenantId
    {
        get
        {
            var ctx = _http.HttpContext ?? throw new InvalidOperationException("No HttpContext.");
            // Replace "tenant_id" with the actual claim type used for tenant identification
            return ctx.User.FindFirst("tenantId")?.Value ?? throw new InvalidOperationException("Tenant ID claim not found.");
        }
    }
}
