
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AutoMapper;
using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Api.Model;
using System;
using System.Threading.Tasks;


namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _masterDataService;
        private readonly IMapper _mapper;
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(IMasterDataService masterDataService, IMapper mapper, ILogger<MasterDataController> logger)
        {
            _masterDataService = masterDataService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("assessments")]
        public async Task<IActionResult> AddAssessment([FromBody] CreateAssessmentDto assessmentDto)
        {
            try
            {
                _logger.LogInformation("AddAssessment - Request received for assessment: {Name}", assessmentDto.Name);
                
                var assessment = _mapper.Map<Assessment>(assessmentDto);
                var result = await _masterDataService.AddAssessmentAsync(assessment);
                
                if (result == null)
                {
                    _logger.LogError("AddAssessment - Service error occurred");
                    return StatusCode(500, "An error occurred while creating the assessment");
                }

                if (result == false)
                {
                    _logger.LogError("AddAssessment - Failed to add assessment");
                    return StatusCode(500, "Failed to add assessment");
                }
                
                _logger.LogInformation("AddAssessment - Assessment created successfully: {Name}", assessmentDto.Name);
                return Ok(new { success = true, message = "Assessment created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddAssessment - Error adding assessment");
                return StatusCode(500, new { message = "Internal server error occurred while adding assessment" });
            }
        }

        /// <summary>
        /// Get an assessment by ID
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Assessment details</returns>
        [HttpGet("assessments/{id}")]
        public async Task<IActionResult> GetAssessmentById(Guid id)
        {
            try
            {
                _logger.LogInformation("GetAssessmentById - Request received for ID: {Id}", id);

                var assessment = await _masterDataService.GetAssessmentByIdAsync(id);

                if (assessment == null)
                {
                    _logger.LogWarning("GetAssessmentById - Assessment not found for ID: {Id}", id);
                    return NotFound("Assessment not found");
                }

                var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
                _logger.LogInformation("GetAssessmentById - Assessment retrieved successfully for ID: {Id}", id);
                return Ok(assessmentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAssessmentById - Exception occurred while getting assessment with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Update an existing assessment
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <param name="updateAssessmentDto">Updated assessment data</param>
        /// <returns>Success status of the operation</returns>
        [HttpPut("assessments/{id}")]
        public async Task<IActionResult> UpdateAssessment(Guid id, [FromBody] UpdateAssessmentDto updateAssessmentDto)
        {
            try
            {
                _logger.LogInformation("UpdateAssessment - Request received for ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("UpdateAssessment - Invalid model state for ID: {Id}", id);
                    return BadRequest(ModelState);
                }

                if (id != updateAssessmentDto.Id)
                {
                    _logger.LogWarning("UpdateAssessment - ID mismatch. Route ID: {RouteId}, Body ID: {BodyId}", id, updateAssessmentDto.Id);
                    return BadRequest("ID in route does not match ID in request body");
                }

                var assessment = _mapper.Map<Assessment>(updateAssessmentDto);
                var result = await _masterDataService.UpdateAssessmentAsync(assessment);

                if (result == null)
                {
                    _logger.LogError("UpdateAssessment - Service error occurred for ID: {Id}", id);
                    return StatusCode(500, "An error occurred while updating the assessment");
                }

                if (result == false)
                {
                    _logger.LogWarning("UpdateAssessment - Assessment not found or update failed for ID: {Id}", id);
                    return NotFound("Assessment not found or update failed");
                }

                _logger.LogInformation("UpdateAssessment - Assessment updated successfully for ID: {Id}", id);
                return Ok(new { success = true, message = "Assessment updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAssessment - Exception occurred while updating assessment with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Delete an assessment
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Success status of the operation</returns>
        [HttpDelete("assessments/{id}")]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteAssessment - Request received for ID: {Id}", id);

                var result = await _masterDataService.DeleteAssessmentAsync(id);

                if (!result)
                {
                    _logger.LogWarning("DeleteAssessment - Assessment not found or delete failed for ID: {Id}", id);
                    return NotFound("Assessment not found or delete failed");
                }

                _logger.LogInformation("DeleteAssessment - Assessment deleted successfully for ID: {Id}", id);
                return Ok(new { success = true, message = "Assessment deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAssessment - Exception occurred while deleting assessment with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                var classes = await _masterDataService.GetClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving classes");
                return StatusCode(500, new { message = "Internal server error occurred while retrieving classes" });
            }
        }

        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            try
            {
                var subjects = await _masterDataService.GetSubjectsAsync();
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                return StatusCode(500, new { message = "Internal server error occurred while retrieving subjects" });
            }
        }

        [HttpGet("assessments")]
        public async Task<IActionResult> GetAllAssesments([FromQuery] string? academicYear,
            [FromQuery] string? grade,
            [FromQuery] string? section,
            [FromQuery] string? assessmentName,
            [FromQuery] string? subjectName)
        {
            var assessments = await _masterDataService.GetAllAssessmentsAsync(academicYear, grade, section, assessmentName, subjectName);
            var assessmentsDto = _mapper.Map<List<AssessmentDto>>(assessments);
            return Ok(assessmentsDto);
        }

        [HttpGet("fee-admin")]
        public async Task<IActionResult> GetAllFeeAdmins([FromQuery] string? academicYear = null, [FromQuery] bool? isActive = null)
        {
            try
            {
                _logger.LogInformation("GetAllFeeAdmins - Request received. AcademicYear: {AcademicYear}, IsActive: {IsActive}", 
                    academicYear, isActive);

                var feeAdmins = await _masterDataService.GetAllFeeAdminsAsync(academicYear, isActive);
                var feeAdminDtos = _mapper.Map<List<FeeAdminDto>>(feeAdmins);

                _logger.LogInformation("GetAllFeeAdmins - Retrieved {Count} fee admin records", feeAdminDtos.Count);
                return Ok(feeAdminDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllFeeAdmins - Exception occurred while getting fee admins");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Create a new fee admin configuration
        /// </summary>
        /// <param name="createFeeAdminDto">Fee admin data to create</param>
        /// <returns>Success status of the operation</returns>
        [HttpPost("fee-admin")]
        public async Task<IActionResult> CreateFeeAdmin([FromBody] CreateFeeAdminDto createFeeAdminDto)
        {
            try
            {
                _logger.LogInformation("CreateFeeAdmin - Request received for ClassId: {ClassId}, Year: {AcademicYear}", 
                    createFeeAdminDto.ClassId, createFeeAdminDto.AcademicYear);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("CreateFeeAdmin - Invalid model state");
                    return BadRequest(ModelState);
                }

                var feeAdmin = _mapper.Map<FeeAdmin>(createFeeAdminDto);
                var result = await _masterDataService.AddFeeAdminAsync(feeAdmin);

                if (result == null)
                {
                    _logger.LogError("CreateFeeAdmin - Service error occurred");
                    return StatusCode(500, "An error occurred while creating the fee admin");
                }

                if (result == false)
                {
                    _logger.LogWarning("CreateFeeAdmin - Fee admin already exists for ClassId: {ClassId}, Year: {AcademicYear}", 
                        createFeeAdminDto.ClassId, createFeeAdminDto.AcademicYear);
                    return Conflict("Fee admin configuration already exists for this class and academic year");
                }

                _logger.LogInformation("CreateFeeAdmin - Fee admin created successfully for ClassId: {ClassId}, Year: {AcademicYear}", 
                    createFeeAdminDto.ClassId, createFeeAdminDto.AcademicYear);
                
                return Ok(new { success = true, message = "Fee admin created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateFeeAdmin - Exception occurred while creating fee admin");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Get a fee admin configuration by ID
        /// </summary>
        /// <param name="id">Fee admin ID</param>
        /// <returns>Fee admin details</returns>
        [HttpGet("fee-admin/{id}")]
        public async Task<IActionResult> GetFeeAdminById(Guid id)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminById - Request received for ID: {Id}", id);

                var feeAdmin = await _masterDataService.GetFeeAdminByIdAsync(id);

                if (feeAdmin == null)
                {
                    _logger.LogWarning("GetFeeAdminById - Fee admin not found for ID: {Id}", id);
                    return NotFound("Fee admin not found");
                }

                var feeAdminDto = _mapper.Map<FeeAdminDto>(feeAdmin);
                _logger.LogInformation("GetFeeAdminById - Fee admin retrieved successfully for ID: {Id}", id);
                return Ok(feeAdminDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminById - Exception occurred while getting fee admin with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Update an existing fee admin configuration
        /// </summary>
        /// <param name="id">Fee admin ID</param>
        /// <param name="updateFeeAdminDto">Updated fee admin data</param>
        /// <returns>Success status of the operation</returns>
        [HttpPut("fee-admin/{id}")]
        public async Task<IActionResult> UpdateFeeAdmin(Guid id, [FromBody] UpdateFeeAdminDto updateFeeAdminDto)
        {
            try
            {
                _logger.LogInformation("UpdateFeeAdmin - Request received for ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("UpdateFeeAdmin - Invalid model state for ID: {Id}", id);
                    return BadRequest(ModelState);
                }

                if (id != updateFeeAdminDto.Id)
                {
                    _logger.LogWarning("UpdateFeeAdmin - ID mismatch. Route ID: {RouteId}, Body ID: {BodyId}", id, updateFeeAdminDto.Id);
                    return BadRequest("ID in route does not match ID in request body");
                }

                var feeAdmin = _mapper.Map<FeeAdmin>(updateFeeAdminDto);
                var result = await _masterDataService.UpdateFeeAdminAsync(feeAdmin);

                if (result == null)
                {
                    _logger.LogError("UpdateFeeAdmin - Service error occurred for ID: {Id}", id);
                    return StatusCode(500, "An error occurred while updating the fee admin");
                }

                if (result == false)
                {
                    _logger.LogWarning("UpdateFeeAdmin - Fee admin not found or update failed for ID: {Id}", id);
                    return NotFound("Fee admin not found or update failed");
                }

                _logger.LogInformation("UpdateFeeAdmin - Fee admin updated successfully for ID: {Id}", id);
                return Ok(new { success = true, message = "Fee admin updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateFeeAdmin - Exception occurred while updating fee admin with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Get fee admin configuration by class and academic year
        /// </summary>
        /// <param name="classId">Class ID</param>
        /// <param name="academicYear">Academic year</param>
        /// <returns>Fee admin configuration</returns>
        [HttpGet("fee-admin/class/{classId}/year/{academicYear}")]
        public async Task<IActionResult> GetFeeAdminByClassAndYear(Guid classId, string academicYear)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminByClassAndYear - Request received for ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);

                var feeAdmin = await _masterDataService.GetFeeAdminByClassAndYearAsync(classId, academicYear);

                if (feeAdmin == null)
                {
                    _logger.LogWarning("GetFeeAdminByClassAndYear - Fee admin not found for ClassId: {ClassId}, Year: {AcademicYear}", 
                        classId, academicYear);
                    return NotFound("Fee admin not found for the specified class and academic year");
                }

                var feeAdminDto = _mapper.Map<FeeAdminDto>(feeAdmin);
                _logger.LogInformation("GetFeeAdminByClassAndYear - Fee admin retrieved successfully for ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);
                
                return Ok(feeAdminDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminByClassAndYear - Exception occurred. ClassId: {ClassId}, Year: {AcademicYear}", 
                    classId, academicYear);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Get all fee admin configurations for a specific academic year
        /// </summary>
        /// <param name="academicYear">Academic year</param>
        /// <returns>List of fee admin configurations for the year</returns>
        [HttpGet("fee-admin/year/{academicYear}")]
        public async Task<IActionResult> GetFeeAdminsByAcademicYear(string academicYear)
        {
            try
            {
                _logger.LogInformation("GetFeeAdminsByAcademicYear - Request received for year: {AcademicYear}", academicYear);

                var feeAdmins = await _masterDataService.GetFeeAdminsByAcademicYearAsync(academicYear);
                var feeAdminDtos = _mapper.Map<List<FeeAdminDto>>(feeAdmins);

                _logger.LogInformation("GetFeeAdminsByAcademicYear - Retrieved {Count} fee admin records for year {AcademicYear}", 
                    feeAdminDtos.Count, academicYear);
                
                return Ok(feeAdminDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminsByAcademicYear - Exception occurred for year: {AcademicYear}", academicYear);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Delete a fee admin configuration
        /// </summary>
        /// <param name="id">Fee admin ID</param>
        /// <returns>Success status of the operation</returns>
        [HttpDelete("fee-admin/{id}")]
        public async Task<IActionResult> DeleteFeeAdmin(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteFeeAdmin - Request received for ID: {Id}", id);

                var result = await _masterDataService.DeleteFeeAdminAsync(id);

                if (!result)
                {
                    _logger.LogWarning("DeleteFeeAdmin - Fee admin not found or delete failed for ID: {Id}", id);
                    return NotFound("Fee admin not found or delete failed");
                }

                _logger.LogInformation("DeleteFeeAdmin - Fee admin deleted successfully for ID: {Id}", id);
                return Ok(new { success = true, message = "Fee admin deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteFeeAdmin - Exception occurred while deleting fee admin with ID: {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("classes")]
        public async Task<IActionResult> AddClass([FromBody] CreateClassDto classDto)
        {
            try
            {
                var newClass = _mapper.Map<Class>(classDto);
                await _masterDataService.AddClassAsync(newClass, classDto.SubjectIds);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding class");
                return StatusCode(500, new { message = "Internal server error occurred while adding class" });
            }
        }

        //add put method
        [HttpPut("classes/{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, [FromBody] CreateClassDto classDto)
        {
            try
            {
                var existingClasses = await _masterDataService.GetClassesAsync();
                var classToUpdate = existingClasses.FirstOrDefault(c => c.Id == id);
                if (classToUpdate == null)
                {
                    return NotFound();
                }

                // Map updated properties
                _mapper.Map(classDto, classToUpdate);
                await _masterDataService.AddClassAsync(classToUpdate, classDto.SubjectIds);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class");
                return StatusCode(500, new { message = "Internal server error occurred while updating class" });
            }
        }
    }
}