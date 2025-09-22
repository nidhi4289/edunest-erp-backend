using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;

public sealed class TenantDbFactory
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _cfg;
    private readonly string _host, _user, _pwd, _prefix;
    private readonly int _port;

    public TenantDbFactory(IMemoryCache cache, IConfiguration cfg)
    {
        _cache = cache; _cfg = cfg;
        _host   = _cfg["DB_HOST"] ?? throw new("DB_HOST missing");
        _user   = _cfg["DB_USER"] ?? throw new("DB_USER missing");
        _pwd    = _cfg["DB_PASSWORD"] ?? throw new("DB_PASSWORD missing");
        _prefix = _cfg["DB_NAME_PREFIX"] ?? "ed_";
        _port   = int.TryParse(_cfg["DB_PORT"], out var p) ? p : 5432;
        
    }

    public NpgsqlDataSource Get(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId)) throw new("tenantId required");
        var db = $"{_prefix}{tenantId}".ToLowerInvariant();

        return _cache.GetOrCreate($"ds:{db}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

            var csb = new NpgsqlConnectionStringBuilder {
                Host = _host, Port = _port, Database = db,
                Username = _user, Password = _pwd,
                // Pool tuning (keeps RDS happy)
                MaxPoolSize = 5,
                MinPoolSize = 0,
                Timeout = 5,
                ConnectionIdleLifetime = 30,
                ConnectionPruningInterval = 10,
                // Use SSL for production, disable for local development
                SslMode = _host.Contains("localhost") || _host.Contains("127.0.0.1") 
                    ? SslMode.Disable 
                    : SslMode.Require
            };

            return new NpgsqlDataSourceBuilder(csb.ConnectionString).Build();
        })!;
    }
}
