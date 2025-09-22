using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IFeeAdminRepository
    {
        Task<bool?> AddAsync(FeeAdmin feeAdmin);
        Task<bool?> UpdateAsync(FeeAdmin feeAdmin);
        Task<FeeAdmin?> GetByIdAsync(Guid id);
        Task<List<FeeAdmin>> GetAllAsync(string? academicYear = null, bool? isActive = null);
        Task<FeeAdmin?> GetByClassAndYearAsync(Guid classId, string academicYear);
        Task<bool> DeleteAsync(Guid id);
        Task<List<FeeAdmin>> GetByAcademicYearAsync(string academicYear);
    }
}