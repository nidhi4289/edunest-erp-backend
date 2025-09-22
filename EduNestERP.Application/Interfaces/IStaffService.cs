using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllStaffAsync(string? firstName = null, string? lastName = null, string? staffId = null);
        Task<Staff?> GetStaffByIdAsync(Guid id);
        Task AddStaffAsync(Staff staff);
        Task UpdateStaffAsync(Staff staff);
        Task DeleteStaffAsync(Guid id);
    }
}