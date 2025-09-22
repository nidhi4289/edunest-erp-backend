using EduNestERP.Persistence.Repositories;
using Npgsql;
using System.Threading.Tasks;
using EduNestERP.Persistence.Entities;
using EduNestERP.Application.Interfaces;

namespace EduNestERP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ValidateUserAsync(string userId, string password)
        {
            return await _userRepository.ValidateCredentialsAsync(userId, password);
        }
        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            return await _userRepository.ResetPasswordAsync(userId, newPassword);
        }

    }
}
