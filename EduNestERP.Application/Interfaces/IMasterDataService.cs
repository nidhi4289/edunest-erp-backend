
using EduNestERP.Persistence.Entities;  

namespace EduNestERP.Application.Interfaces
{
    public interface IMasterDataService
    {
    Task<List<Class>> GetClassesAsync();
    Task<List<Subject>> GetSubjectsAsync();
    Task AddClassAsync(Class newClass, List<Guid> subjectIds);
    Task<List<Assessment>> GetAllAssessmentsAsync(string? academicYear, string? grade, string? section, string? assessmentName, string? subjectName);
    Task<bool?> AddAssessmentAsync(Assessment newAssessment);
    Task<Assessment?> GetAssessmentByIdAsync(Guid id);
    Task<bool?> UpdateAssessmentAsync(Assessment assessment);
    Task<bool> DeleteAssessmentAsync(Guid id);

    // Fee Admin methods
    Task<bool?> AddFeeAdminAsync(FeeAdmin feeAdmin);
    Task<bool?> UpdateFeeAdminAsync(FeeAdmin feeAdmin);
    Task<FeeAdmin?> GetFeeAdminByIdAsync(Guid id);
    Task<List<FeeAdmin>> GetAllFeeAdminsAsync(string? academicYear = null, bool? isActive = null);
    Task<FeeAdmin?> GetFeeAdminByClassAndYearAsync(Guid classId, string academicYear);
    Task<bool> DeleteFeeAdminAsync(Guid id);
    Task<List<FeeAdmin>> GetFeeAdminsByAcademicYearAsync(string academicYear);

    }
}
