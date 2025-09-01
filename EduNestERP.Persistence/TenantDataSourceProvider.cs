using Npgsql;

public interface ITenantDataSourceProvider
{
    NpgsqlDataSource Get();
    Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct = default);
}

public sealed class TenantDataSourceProvider : ITenantDataSourceProvider
{
    private readonly ITenantAccessor _tenant;
    private readonly TenantDbFactory _factory;
    public TenantDataSourceProvider(ITenantAccessor tenant, TenantDbFactory factory)
    { _tenant = tenant; _factory = factory; }

    public NpgsqlDataSource Get() => _factory.Get(_tenant.TenantId);
    public Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct = default) => Get().OpenConnectionAsync(ct).AsTask();
}
