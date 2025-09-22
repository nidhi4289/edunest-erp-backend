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
    public class CommunicationController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public CommunicationController(ICommunicationService communicationService, IMapper mapper)
        {
            _communicationService = communicationService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunication([FromBody] CommunicationCreateDto communicationDto)
        {
            if (communicationDto == null)
            {
                return BadRequest("Invalid communication data.");
            }

            var communication = _mapper.Map<Communication>(communicationDto);
            var result = await _communicationService.CreateCommunicationAsync(communication);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating communication.");
            }

            return Ok(new { message = "Communication created successfully.", id = communication.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommunicationById(Guid id)
        {
            var communication = await _communicationService.GetCommunicationByIdAsync(id);
            if (communication == null)
            {
                return NotFound("Communication not found.");
            }

            var communicationDto = _mapper.Map<CommunicationDto>(communication);
            return Ok(communicationDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommunications([FromQuery] DateTime? modifiedAfter = null, [FromQuery] string? status = null)
        {
            var communications = await _communicationService.GetAllCommunicationsAsync(modifiedAfter, status);
            var communicationDtos = _mapper.Map<List<CommunicationDto>>(communications);
            return Ok(communicationDtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommunication(Guid id, [FromBody] CommunicationUpdateDto communicationDto)
        {
            if (communicationDto == null)
            {
                return BadRequest("Invalid communication data.");
            }

            var existing = await _communicationService.GetCommunicationByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Communication not found.");
            }

            _mapper.Map(communicationDto, existing);
            existing.Id = id; // Ensure the ID is set
            var result = await _communicationService.UpdateCommunicationAsync(existing);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating communication.");
            }

            return Ok(new { message = "Communication updated successfully.", id = id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommunication(Guid id)
        {
            var existing = await _communicationService.GetCommunicationByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Communication not found.");
            }

            var result = await _communicationService.DeleteCommunicationAsync(id);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting communication.");
            }

            return Ok(new { message = "Communication deleted successfully.", id = id });
        }

    }
}