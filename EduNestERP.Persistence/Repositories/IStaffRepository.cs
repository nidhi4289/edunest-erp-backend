using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IStaffRepository
    {
    Task<IEnumerable<Staff>> GetAllAsync(string? firstName = null, string? lastName = null, string? staffId = null);
        Task<Staff> GetByIdAsync(Guid id);
        Task AddAsync(Staff staff);
        Task UpdateAsync(Staff staff);
        Task DeleteAsync(Guid id);
    }
}