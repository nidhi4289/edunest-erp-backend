using EduNestERP.Persistence.Entities;
using Npgsql;

namespace EduNestERP.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUserIdAsync(NpgsqlDataSource ds, string userId, CancellationToken ct = default);

        Task<User?> ValidateCredentialsAsync(string userId, string passwordHash);

         Task<bool> SetPasswordAndCompleteFirstLoginAsync(NpgsqlDataSource ds, string userId, string newHash, CancellationToken ct = default);

    }
}