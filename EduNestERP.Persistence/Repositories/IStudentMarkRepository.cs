using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IStudentMarkRepository 
    {
        Task<bool?> AddAsync(StudentMark studentMark);
        Task<bool> UpdateAsync(StudentMark studentMark);
        Task<bool?> BulkAddAsync(List<StudentMark> studentMarks);
        Task<List<StudentMark>> GetByAssessmentIdAsync(Guid assessmentId);
        Task<List<StudentMark>> GetByStudentIdAsync(string eduNestId, string academicYear);
        Task<StudentMark?> GetByAssessmentAndStudentAsync(Guid assessmentId, string eduNestId);
        Task<bool?> DeleteAsync(Guid id);
    }
}