using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;
using EduNestERP.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduNestERP.Application.Services
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IFeeAdminRepository _feeAdminRepository;
        private readonly ILogger<MasterDataService> _logger;

        public MasterDataService(IMasterDataRepository masterDataRepository, IFeeAdminRepository feeAdminRepository, ILogger<MasterDataService> logger)
        {
            _masterDataRepository = masterDataRepository;
            _feeAdminRepository = feeAdminRepository;
            _logger = logger;
        }

        public async Task<List<Class>> GetClassesAsync()
        {
            return await _masterDataRepository.GetAllClassesAsync();
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            // Assuming a method GetAllSubjectsAsync exists in the repository
            return await _masterDataRepository.GetSubjectsAsync();
        }

        public async Task AddClassAsync(Class newClass, List<Guid> subjectIds)
        {
            await _masterDataRepository.AddClassAsync(newClass, subjectIds);
        }

        public async Task<bool?> AddAssessmentAsync(Assessment newAssessment)
        {
            try
            {
                _logger.LogInformation("AddAssessmentAsync - Adding assessment: {Name}", newAssessment.Name);
                
                newAssessment.Id = newAssessment.Id == Guid.Empty ? Guid.NewGuid() : newAssessment.Id;
                newAssessment.CreatedAt = DateTime.UtcNow;
                newAssessment.UpdatedAt = DateTime.UtcNow;
                newAssessment.AssessmentDate = DateTime.UtcNow; // You may want to make this configurable
                
                var result = await _masterDataRepository.AddAssessmentAsync(newAssessment);
                _logger.LogInformation("AddAssessmentAsync - Assessment add result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddAssessmentAsync - Error adding assessment");
                return null;
            }
        }

        public async Task<Assessment?> GetAssessmentByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("GetAssessmentByIdAsync - Retrieving assessment with ID: {Id}", id);
                var assessment = await _masterDataRepository.GetAssessmentByIdAsync(id);
                _logger.LogInformation("GetAssessmentByIdAsync - Assessment retrieved: {Found}", assessment != null);
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAssessmentByIdAsync - Error retrieving assessment with ID: {Id}", id);
                return null;
            }
        }

        public async Task<bool?> UpdateAssessmentAsync(Assessment assessment)
        {
            try
            {
                _logger.LogInformation("UpdateAssessmentAsync - Updating assessment with ID: {Id}", assessment.Id);
                
                assessment.UpdatedAt = DateTime.UtcNow;
                var result = await _masterDataRepository.UpdateAssessmentAsync(assessment);
                
                _logger.LogInformation("UpdateAssessmentAsync - Assessment update result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAssessmentAsync - Error updating assessment with ID: {Id}", assessment.Id);
                return null;
            }
        }

        public async Task<bool> DeleteAssessmentAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteAssessmentAsync - Deleting assessment with ID: {Id}", id);
                var result = await _masterDataRepository.DeleteAssessmentAsync(id);
                _logger.LogInformation("DeleteAssessmentAsync - Assessment delete result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAssessmentAsync - Error deleting assessment with ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<Assessment>> GetAllAssessmentsAsync(string? academicYear, string? grade, string? section, string? assessmentName, string? subjectName)
        {
            return await _masterDataRepository.GetAllAssessmentsAsync(academicYear, grade, section, assessmentName, subjectName);
        }

        // Fee Admin methods
        public async Task<bool?> AddFeeAdminAsync(FeeAdmin feeAdmin)
        {
            try
            {
                _logger.LogInformation("AddFeeAdminAsync - Starting to add fee admin for ClassId: {ClassId}, Year: {AcademicYear}", 
                    feeAdmin.ClassId, feeAdmin.AcademicYear);

                // Check if fee admin already exists for this class and academic year
                var existingFeeAdmin = await _feeAdminRepository.GetByClassAndYearAsync(feeAdmin.ClassId, feeAdmin.AcademicYear ?? "");
                if (existingFeeAdmin != null)
                {
                    _logger.LogWarning("AddFeeAdminAsync - Fee admin already exists for ClassId: {ClassId}, Year: {AcademicYear}", 
                        feeAdmin.ClassId, feeAdmin.AcademicYear);
                    return false;
                }

                feeAdmin.Id = Guid.NewGuid();
                // Set audit fields
            feeAdmin.CreatedAt = DateTime.UtcNow;
            feeAdmin.UpdatedAt = DateTime.UtcNow;

                var result = await _feeAdminRepository.AddAsync(feeAdmin);
                
                _logger.LogInformation("AddFeeAdminAsync - Fee admin add operation completed. Success: {Success}, ID: {Id}", 
                    result, feeAdmin.Id);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddFeeAdminAsync - Error adding fee admin for ClassId: {ClassId}, Year: {AcademicYear}", 
                    feeAdmin.ClassId, feeAdmin.AcademicYear);
                return null;
            }
        }

        public async Task<bool?> UpdateFeeAdminAsync(FeeAdmin feeAdmin)
        {
            try
            {
                _logger.LogInformation("UpdateFeeAdminAsync - Starting to update fee admin with ID: {Id}", feeAdmin.Id);

                var existingFeeAdmin = await _feeAdminRepository.GetByIdAsync(feeAdmin.Id);
                if (existingFeeAdmin == null)
                {
                    _logger.LogWarning("UpdateFeeAdminAsync - Fee admin not found with ID: {Id}", feeAdmin.Id);
                    return false;
                }

                // Update properties
                existingFeeAdmin.ClassId = feeAdmin.ClassId;
                existingFeeAdmin.AcademicYear = feeAdmin.AcademicYear;
                existingFeeAdmin.MonthlyFee = feeAdmin.MonthlyFee;
                existingFeeAdmin.AnnualFee = feeAdmin.AnnualFee;
                existingFeeAdmin.AdmissionFee = feeAdmin.AdmissionFee;
                existingFeeAdmin.TransportFee = feeAdmin.TransportFee;
                existingFeeAdmin.LibraryFee = feeAdmin.LibraryFee;
                existingFeeAdmin.SportsFee = feeAdmin.SportsFee;
                existingFeeAdmin.MiscellaneousFee = feeAdmin.MiscellaneousFee;
                existingFeeAdmin.IsActive = feeAdmin.IsActive;
                existingFeeAdmin.UpdatedAt = DateTime.UtcNow;

                var result = await _feeAdminRepository.UpdateAsync(existingFeeAdmin);
                
                _logger.LogInformation("UpdateFeeAdminAsync - Fee admin update operation completed. Success: {Success}, ID: {Id}", 
                    result, feeAdmin.Id);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateFeeAdminAsync - Error updating fee admin with ID: {Id}", feeAdmin.Id);
                return null;
            }
        }

        public async Task<FeeAdmin?> GetFeeAdminByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminByIdAsync - Retrieving fee admin with ID: {Id}", id);

                var feeAdmin = await _feeAdminRepository.GetByIdAsync(id);
                if (feeAdmin == null)
                {
                    _logger.LogInformation("GetFeeAdminByIdAsync - Fee admin not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("GetFeeAdminByIdAsync - Fee admin retrieved successfully. ID: {Id}", id);
                return feeAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminByIdAsync - Error retrieving fee admin with ID: {Id}", id);
                return null;
            }
        }

        public async Task<List<FeeAdmin>> GetAllFeeAdminsAsync(string? academicYear = null, bool? isActive = null)
        {
            try
            {
                _logger.LogInformation("GetAllFeeAdminsAsync - Retrieving fee admins. AcademicYear: {AcademicYear}, IsActive: {IsActive}", 
                    academicYear, isActive);

                var feeAdmins = await _feeAdminRepository.GetAllAsync(academicYear, isActive);
                
                _logger.LogInformation("GetAllFeeAdminsAsync - Retrieved {Count} fee admin records", feeAdmins.Count);
                return feeAdmins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllFeeAdminsAsync - Error retrieving fee admins");
                return new List<FeeAdmin>();
            }
        }

        public async Task<FeeAdmin?> GetFeeAdminByClassAndYearAsync(Guid classId, string academicYear)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminByClassAndYearAsync - Retrieving fee admin for ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);

                var feeAdmin = await _feeAdminRepository.GetByClassAndYearAsync(classId, academicYear);
                if (feeAdmin == null)
                {
                    _logger.LogInformation("GetFeeAdminByClassAndYearAsync - Fee admin not found for ClassId: {ClassId}, Year: {AcademicYear}", 
                        classId, academicYear);
                    return null;
                }

                _logger.LogInformation("GetFeeAdminByClassAndYearAsync - Fee admin retrieved successfully for ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);
                
                return feeAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminByClassAndYearAsync - Error retrieving fee admin for ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);
                return null;
            }
        }

        public async Task<bool> DeleteFeeAdminAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteFeeAdminAsync - Starting to delete fee admin with ID: {Id}", id);

                var existingFeeAdmin = await _feeAdminRepository.GetByIdAsync(id);
                if (existingFeeAdmin == null)
                {
                    _logger.LogWarning("DeleteFeeAdminAsync - Fee admin not found with ID: {Id}", id);
                    return false;
                }

                var result = await _feeAdminRepository.DeleteAsync(id);
                
                _logger.LogInformation("DeleteFeeAdminAsync - Fee admin delete operation completed. Success: {Success}, ID: {Id}", 
                    result, id);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteFeeAdminAsync - Error deleting fee admin with ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<FeeAdmin>> GetFeeAdminsByAcademicYearAsync(string academicYear)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminsByAcademicYearAsync - Retrieving fee admins for year: {AcademicYear}", academicYear);

                var feeAdmins = await _feeAdminRepository.GetByAcademicYearAsync(academicYear);
                
                _logger.LogInformation("GetFeeAdminsByAcademicYearAsync - Retrieved {Count} fee admin records for year {AcademicYear}", 
                    feeAdmins.Count, academicYear);
                
                return feeAdmins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminsByAcademicYearAsync - Error retrieving fee admins for year {AcademicYear}", academicYear);
                return new List<FeeAdmin>();
            }
        }
    }
}
