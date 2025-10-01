using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IMasterDataRepository
    {
        Task<List<Class>> GetAllClassesAsync();
    Task AddClassAsync(Class newClass, List<Guid> subjectIds);
        Task<List<Assessment>> GetAllAssessmentsAsync(string? academicYear, string? grade, string? section, string? assessmentName, string? subjectName);
        Task<bool> AddAssessmentAsync(Assessment newAssessment);
        Task<Assessment?> GetAssessmentByIdAsync(Guid id);
        Task<bool> UpdateAssessmentAsync(Assessment assessment);
        Task<bool> DeleteAssessmentAsync(Guid id);
        Task<List<Subject>> GetSubjectsAsync();
    }
}

