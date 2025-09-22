using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IMapper _mapper;

        public AttendanceController(IAttendanceService attendanceService, IMapper mapper)
        {
            _attendanceService = attendanceService;
            _mapper = mapper;
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> MarkAttendances([FromBody] List<AttendanceDto> attendancesDto)
        {
            if (attendancesDto == null || !attendancesDto.Any())
            {
                return BadRequest("Invalid attendance data.");
            }

            var attendances = _mapper.Map<List<Attendance>>(attendancesDto);

            var result = await _attendanceService.MarkAttendanceAsync(attendances);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error marking attendance.");
            }

            return Ok("Attendance marked successfully.");
        }

        [HttpGet("student")]
        public async Task<IActionResult> GetAttendanceByStudent(
            [FromQuery] string eduNestId,
            [FromQuery] string grade,
            [FromQuery] string section)
        {
            if (string.IsNullOrWhiteSpace(eduNestId) || string.IsNullOrWhiteSpace(grade) || string.IsNullOrWhiteSpace(section))
            {
                return BadRequest("Invalid query parameters.");
            }

            var attendance = await _attendanceService.GetAttendanceByStudentAsync(eduNestId, grade, section);
            if (attendance == null)
            {
                return NotFound("Attendance not found.");
            }

            var attendanceDto = _mapper.Map<List<AttendanceDto>>(attendance);
            return Ok(attendanceDto);
        }
    }
}