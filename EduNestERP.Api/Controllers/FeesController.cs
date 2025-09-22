using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduNestERP.Api.Models;
using EduNestERP.Application.Interfaces;
using AutoMapper;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FeesController : ControllerBase
    {
        private readonly IStudentFeeService _feeService;
        private readonly IMapper _mapper;

        public FeesController(IStudentFeeService feeService, IMapper mapper)
        {
            _feeService = feeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFees()
        {
            var fees = await _feeService.GetFeesAsync();
            var feeDtos = _mapper.Map<List<StudentFeeDto>>(fees);
            return Ok(feeDtos);
        }

        [HttpGet("{eduNestId}")]
        public async Task<IActionResult> Get(string eduNestId)
        {
            var fees = await _feeService.GetFeeByEduNestIdAsync(eduNestId);
            if (fees == null || !fees.Any())
                return NotFound();

            var feeDtos = _mapper.Map<List<StudentFeeDto>>(fees);
            return Ok(feeDtos);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkFees([FromBody] List<StudentFeeDto> fees)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feeEntities = _mapper.Map<List<StudentFee>>(fees);
            var createdFees = await _feeService.CreateBulkFeesAsync(feeEntities);
            return CreatedAtAction(nameof(GetFees), createdFees);

        }

        [HttpPost]
        public async Task<IActionResult> CreateFee([FromBody] StudentFeeDto fee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feeEntity = _mapper.Map<StudentFee>(fee);
            var result = await _feeService.CreateFeeAsync(feeEntity);
            if (result == false)
            {
                return BadRequest(new { success = false, message = "Failed to create fee" });
            }

            return Ok(new { success = true, message = "Fee created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFee(Guid id, [FromBody] StudentFeeDto fee)
        {
            if (id != fee.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feeEntity = _mapper.Map<StudentFee>(fee);
            var updatedFee = await _feeService.UpdateFeeAsync(feeEntity);
            if (updatedFee == false)
            {
                return BadRequest(new { success = false, message = "Failed to update fee" });
            }

            return Ok(new { success = true, message = "Fee updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFee(int id)
        {
            var deleted = await _feeService.DeleteFeeAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}