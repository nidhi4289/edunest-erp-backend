using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IStudentFeeRepository : IRepository<StudentFee>
    {
        Task<bool?> AddBulkStudentFeesAsync(IEnumerable<StudentFee> studentFees);
        Task<List<StudentFee?>> GetByEduNestIdAsync(string eduNestId);
    }
}
