using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;
using EduNestERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? grade)
        {
            var students = await _studentService.SearchStudentsAsync(firstName, lastName, grade);
            var studentsDto = _mapper.Map<List<StudentDto>>(students);
            return Ok(studentsDto);
        }

        [HttpGet("{eduNestId}")]
        public async Task<IActionResult> Get(string eduNestId)
        {
            var student = await _studentService.GetStudentByEduNestIdAsync(eduNestId);
            if (student == null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }

        [HttpPost("bulk-add")]
        public async Task<IActionResult> BulkAddStudents([FromBody] List<StudentDto> students)
        {
            if (students == null || students.Count == 0)
                return BadRequest(new { success = false, message = "No students provided." });

            var studentEntities = _mapper.Map<List<Student>>(students);
            var result = await _studentService.AddStudentsBulkAsync(studentEntities);

            if (result)
                return Ok(new { success = true, message = "Students added/updated successfully." });
            else
                return StatusCode(500, new { success = false, message = "Failed to add/update students." });
        }

        [HttpPost("")]
        public async Task<IActionResult> AddStudent([FromBody] StudentDto student)
        {
            if (student == null)
                return BadRequest(new { success = false, message = "No students provided." });

            var studentEntity = _mapper.Map<Student>(student);
            var result = await _studentService.AddStudentAsync(studentEntity);

            if (result == false)
                return Conflict(new { success = false, message = "Student already exists." });
            if (result == true)
                return Ok(new { success = true, message = "Student added successfully." });
            else
                return StatusCode(500, new { success = false, message = "Failed to add student." });
        }

        [HttpPut("{eduNestId}")]
        public async Task<IActionResult> UpdateStudent(string eduNestId, [FromBody] StudentDto studentDto)
        {
            if (studentDto == null)
                return BadRequest(new { success = false, message = "Student data is required." });

            // Ensure the EduNestId from route is set in the DTO
            studentDto.EduNestId = eduNestId;

            var studentEntity = _mapper.Map<Student>(studentDto);
            var result = await _studentService.UpdateStudentAsync(studentEntity);

            if (result == false)
                return NotFound(new { success = false, message = "Student not found." });
            if (result == true)
                return Ok(new { success = true, message = "Student updated successfully." });
            else
                return StatusCode(500, new { success = false, message = "Failed to update student." });
        }
    }
}