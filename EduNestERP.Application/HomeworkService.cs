using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace EduNestERP.Application
{
    public class HomeworkService : IHomeworkService
    {
        private readonly IHomeworkRepository _repo;
        private readonly Microsoft.Extensions.Logging.ILogger<HomeworkService> _logger;

        public HomeworkService(IHomeworkRepository repo, Microsoft.Extensions.Logging.ILogger<HomeworkService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool?> AddHomeworkAsync(Homework homework)
        {
            try
            {
                homework.CreatedAt = DateTime.UtcNow;
                homework.UpdatedAt = DateTime.UtcNow;
                var result = await _repo.AddAsync(homework);
                _logger.LogInformation("Homework added via service: {@Homework}", homework);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding homework in service: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateHomeworkAsync(Homework homework)
        {
            try
            {
                homework.UpdatedAt = DateTime.UtcNow;
                var result = await _repo.UpdateAsync(homework);
                _logger.LogInformation("Homework updated via service: {@Homework}", homework);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homework in service: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Homework?> GetHomeworkByIdAsync(Guid id)
        {
            try
            {
                var result = await _repo.GetByIdAsync(id);
                _logger.LogInformation("Fetched homework by id in service: {Id}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homework by id in service: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<Homework>> GetAllHomeworkAsync(string? grade = null, string? section = null)
        {
            try
            {
                var result = await _repo.GetAllAsync(grade, section);
                _logger.LogInformation("Fetched {Count} homework records in service for grade={Grade}, section={Section}", result.Count, grade, section);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homework list in service: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<Homework>> GetHomeworkByClassSubjectIdAsync(Guid classSubjectId)
        {
            try
            {
                var result = await _repo.GetByClassSubjectIdAsync(classSubjectId);
                _logger.LogInformation("Fetched {Count} homework records for classSubjectId={ClassSubjectId}", result.Count, classSubjectId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homework by classSubjectId in service: {Message}", ex.Message);
                throw;
            }
        }
    }
}
