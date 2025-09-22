using Npgsql;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;

public sealed class UserRepository : IUserRepository
{
   private readonly ITenantDataSourceProvider _dataSource;
   public UserRepository(ITenantDataSourceProvider dataSource) => _dataSource = dataSource;

    public async Task<User?> GetByUserIdAsync(NpgsqlDataSource ds, string userId, CancellationToken ct = default)
    {
        await using var conn = await ds.OpenConnectionAsync(ct);
        const string sql = """
            SELECT id, user_id, password_hash, role, first_login_completed
            FROM users WHERE user_id = @u
            """;
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@u", userId);
        await using var rdr = await cmd.ExecuteReaderAsync(ct);
        if (!await rdr.ReadAsync(ct)) return null;

        return new User
        {
            Id = rdr.GetGuid(0),
            UserId = rdr.GetString(1),
            PasswordHash = rdr.GetString(2),
            Role = rdr.GetString(3),
            FirstLoginCompleted = rdr.GetBoolean(4)
        };
    }

    public async Task<bool> SetPasswordAndCompleteFirstLoginAsync(NpgsqlDataSource ds, string userId, string newHash, CancellationToken ct = default)
    {
        await using var conn = await ds.OpenConnectionAsync(ct);
        const string sql = """
            UPDATE users
               SET password_hash = @h, first_login_completed = true
             WHERE user_id = @u
            """;
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@h", newHash);
        cmd.Parameters.AddWithValue("@u", userId);
        return (await cmd.ExecuteNonQueryAsync(ct)) == 1;
    }


    public async Task<User?> ValidateCredentialsAsync(string userId, string passwordHash)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        const string sql = "SELECT user_id, role, name, id FROM users WHERE user_id = @user_id AND password_hash = @password_hash";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("password_hash", passwordHash); // Use hashed passwords in production!
        await using var rdr = await cmd.ExecuteReaderAsync();
        if (await rdr.ReadAsync())
        {
            return new User
            {
                UserId = rdr.GetString(0),
                Role = rdr.GetString(1),
                Name = rdr.GetString(2),
                Id = rdr.GetGuid(3)
            
            };
        }
        return null;
    }
        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            const string sql = "UPDATE users SET password_hash = @password_hash WHERE user_id = @user_id";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("user_id", userId);
            cmd.Parameters.AddWithValue("password_hash", newPassword); // Hash in production!
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
}
