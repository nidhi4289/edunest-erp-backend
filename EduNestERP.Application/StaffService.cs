using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;

namespace EduNestERP.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantDataSourceProvider _dataSource;

        public StaffService(IStaffRepository staffRepository, IUserRepository userRepository, ITenantDataSourceProvider dataSource)
        {
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync(string? firstName = null, string? lastName = null, string? staffId = null)
        {
            return await _staffRepository.GetAllAsync(firstName, lastName, staffId);
        }

        public async Task<Staff?> GetStaffByIdAsync(Guid id)
        {
            return await _staffRepository.GetByIdAsync(id);
        }

        public async Task AddStaffAsync(Staff staff)
        {
            await _staffRepository.AddAsync(staff);
        }

        public async Task UpdateStaffAsync(Staff staff)
        {
            await _staffRepository.UpdateAsync(staff);
        }

        public async Task DeleteStaffAsync(Guid id)
        {
            await _staffRepository.DeleteAsync(id);
        }
    }
}