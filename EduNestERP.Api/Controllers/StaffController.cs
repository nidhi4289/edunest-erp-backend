using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EduNestERP.Application.Interfaces;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;
using AutoMapper;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;

        public StaffController(IStaffService staffService, IMapper mapper)
        {
            _staffService = staffService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetAll([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] string? staffId)
        {
            var staff = await _staffService.GetAllStaffAsync(firstName, lastName, staffId);
            var dtos = _mapper.Map<List<StaffDto>>(staff);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetById(Guid id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null) return NotFound();
            return Ok(_mapper.Map<StaffDto>(staff));
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] StaffDto dto)
        {
            var staff = _mapper.Map<Staff>(dto);
            staff.Id = Guid.NewGuid();
            staff.CreatedAt = DateTime.UtcNow;
            await _staffService.AddStaffAsync(staff);
            return CreatedAtAction(nameof(GetById), new { id = staff.Id }, _mapper.Map<StaffDto>(staff));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] StaffDto dto)
        {
            var existing = await _staffService.GetStaffByIdAsync(id);
            if (existing == null) return NotFound();
            var staff = _mapper.Map<Staff>(dto);
            staff.Id = id;
            staff.CreatedAt = existing.CreatedAt;
            await _staffService.UpdateStaffAsync(staff);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existing = await _staffService.GetStaffByIdAsync(id);
            if (existing == null) return NotFound();
            await _staffService.DeleteStaffAsync(id);
            return NoContent();
        }

        // Mapping handled by AutoMapper
    }
}
