using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface IStudentMarkService
    {
        Task<bool?> AddStudentMarkAsync(StudentMark studentMark);
        Task<bool> UpdateStudentMarkAsync(StudentMark studentMark);
        Task<bool?> BulkAddStudentMarksAsync(List<StudentMark> studentMarks);
        Task<List<StudentMark>> GetMarksByAssessmentAsync(Guid assessmentId);
        Task<List<StudentMark>> GetMarksByStudentAsync(string eduNestId, string academicYear);
        Task<StudentMark?> GetMarkByAssessmentAndStudentAsync(Guid assessmentId, string eduNestId);
        Task<bool?> DeleteStudentMarkAsync(Guid id);
    }
}