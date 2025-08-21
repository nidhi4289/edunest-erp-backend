namespace EduNestCrm.Api.Model;

public class DevLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? TenantId { get; set; }
}