
using EduNestERP.Persistence.Entities;
using Npgsql;
using System.Threading.Tasks;
namespace EduNestERP.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> ResetPasswordAsync(string userId, string newPassword);

    }
}