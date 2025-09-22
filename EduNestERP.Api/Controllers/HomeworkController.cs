using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using System;
using System.Threading.Tasks;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HomeworkController : ControllerBase
    {
        private readonly IHomeworkService _homeworkService;
        private readonly ILogger<HomeworkController> _logger;

        public HomeworkController(IHomeworkService homeworkService, ILogger<HomeworkController> logger)
        {
            _homeworkService = homeworkService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddHomework([FromBody] Homework homework)
        {
            try
            {
                var result = await _homeworkService.AddHomeworkAsync(homework);
                return result == true ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding homework");
                return StatusCode(500, new { message = "Internal server error occurred while adding homework" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHomework(Guid id, [FromBody] Homework homework)
        {
            try
            {
                if (id != homework.Id) return BadRequest("ID mismatch");
                var result = await _homeworkService.UpdateHomeworkAsync(homework);
                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homework");
                return StatusCode(500, new { message = "Internal server error occurred while updating homework" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHomework([FromQuery] string? grade, [FromQuery] string? section)
        {
            var result = await _homeworkService.GetAllHomeworkAsync(grade, section);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeworkById(Guid id)
        {
            var result = await _homeworkService.GetHomeworkByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-class-subject/{classSubjectId}")]
        public async Task<IActionResult> GetHomeworkByClassSubjectId(Guid classSubjectId)
        {
            var result = await _homeworkService.GetHomeworkByClassSubjectIdAsync(classSubjectId);
            return Ok(result);
        }
    }
}
