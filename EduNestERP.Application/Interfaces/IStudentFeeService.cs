
using EduNestERP.Persistence.Entities;  

namespace EduNestERP.Application.Interfaces
{
    public interface IStudentFeeService
    {
        Task<List<StudentFee>> GetFeesAsync();
        Task<List<StudentFee>> GetFeeByEduNestIdAsync(string eduNestId);
        Task<bool?> CreateFeeAsync(StudentFee fee);
        Task<bool?> CreateBulkFeesAsync(List<StudentFee> fees);
        Task<bool?> UpdateFeeAsync(StudentFee fee);
        Task<bool> DeleteFeeAsync(int id);
    }
}
