using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IHomeworkRepository
    {
        Task<bool?> AddAsync(Homework homework);
        Task<bool> UpdateAsync(Homework homework);
        Task<Homework?> GetByIdAsync(Guid id);
    Task<List<Homework>> GetAllAsync(string? grade = null, string? section = null);
        Task<List<Homework>> GetByClassSubjectIdAsync(Guid classSubjectId);
    }
}
