using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IMasterDataRepository
    {
        Task<List<Class>> GetAllClassesAsync();
    Task AddClassAsync(Class newClass, List<Guid> subjectIds);
        Task<List<Assessment>> GetAllAssessmentsAsync(string? academicYear, string? grade, string? section, string? assessmentName, string? subjectName);
        Task AddAssessmentAsync(Assessment newAssessment);
        Task<List<Subject>> GetSubjectsAsync();
    }
}

