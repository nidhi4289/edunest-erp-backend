using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface IHomeworkService
    {
        Task<bool?> AddHomeworkAsync(Homework dto);
        Task<bool> UpdateHomeworkAsync(Homework dto);
        Task<Homework?> GetHomeworkByIdAsync(Guid id);
    Task<List<Homework>> GetAllHomeworkAsync(string? grade = null, string? section = null);
        Task<List<Homework>> GetHomeworkByClassSubjectIdAsync(Guid classSubjectId);
    }
}
