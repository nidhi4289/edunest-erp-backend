using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduNestERP.Application.Interfaces;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;
using AutoMapper;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentMarksController : ControllerBase
    {
        private readonly IStudentMarkService _studentMarkService;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentMarksController> _logger;

        public StudentMarksController(IStudentMarkService studentMarkService, IMapper mapper, ILogger<StudentMarksController> logger)
        {
            _studentMarkService = studentMarkService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudentMark([FromBody] StudentMarkDto createDto)
        {
            try
            {
                // Check if mark already exists
                var existingMark = await _studentMarkService.GetMarkByAssessmentAndStudentAsync(createDto.AssessmentId, createDto.EduNestId);
                if (existingMark != null)
                {
                    return BadRequest(new { message = "Mark already exists for this student and assessment" });
                }

                var studentMark = _mapper.Map<StudentMark>(createDto);
                var result = await _studentMarkService.AddStudentMarkAsync(studentMark);

                if (result == true)
                {
                    return Ok(new { message = "Student mark created successfully" });
                }

                return BadRequest(new { message = "Failed to create student mark" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student mark");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentMark(Guid id, [FromBody] StudentMarkDto updateDto)
        {
            try
            {
                var existingMark = await _studentMarkService.GetMarkByAssessmentAndStudentAsync(updateDto.AssessmentId, updateDto.EduNestId);
                if (existingMark == null || existingMark.Id != id)
                {
                    return NotFound(new { message = "Student mark not found" });
                }

                existingMark.MarksObtained = updateDto.MarksObtained;
                existingMark.GradeAwarded = updateDto.GradeAwarded;
                existingMark.Remarks = updateDto.Remarks;

                var result = await _studentMarkService.UpdateStudentMarkAsync(existingMark);

                if (result)
                {
                    return Ok(new { message = "Student mark updated successfully" });
                }

                return BadRequest(new { message = "Failed to update student mark" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student mark");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateStudentMarks([FromBody] List<StudentMarkDto> studentMarkDtos)
        {
            try
            {
                var studentMarks = _mapper.Map<List<StudentMark>>(studentMarkDtos);
                var result = await _studentMarkService.BulkAddStudentMarksAsync(studentMarks);

                if (result == true)
                {
                    return Ok(new { message = $"Successfully processed {studentMarks.Count} student marks" });
                }

                return BadRequest(new { message = "Failed to process student marks" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk creating student marks");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("assessment/{assessmentId}")]
        public async Task<IActionResult> GetMarksByAssessment(Guid assessmentId)
        {
            try
            {
                var marks = await _studentMarkService.GetMarksByAssessmentAsync(assessmentId);
                var markDtos = _mapper.Map<List<StudentMarkDto>>(marks);
                return Ok(markDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving marks by assessment");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("student")]
        public async Task<IActionResult> GetMarksByStudent(
        [FromQuery] string eduNestId,
        [FromQuery] string academicYear)
        {
            try
            {
                var marks = await _studentMarkService.GetMarksByStudentAsync(eduNestId, academicYear);
                var markDtos = _mapper.Map<List<StudentMarkDto>>(marks);
                return Ok(markDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving marks by student");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentMark(Guid id)
        {
            try
            {
                var result = await _studentMarkService.DeleteStudentMarkAsync(id);

                if (result == true)
                {
                    return Ok(new { message = "Student mark deleted successfully" });
                }

                return NotFound(new { message = "Student mark not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student mark");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}