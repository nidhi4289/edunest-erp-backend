using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface ICommunicationRepository
    {
        Task<bool> AddAsync(Communication communication);
        Task<bool> UpdateAsync(Communication communication);
        Task<Communication?> GetByIdAsync(Guid id);
        Task<List<Communication>> GetAllAsync(DateTime? modifiedAfter, string? status);
        Task<bool> DeleteAsync(Guid id);
    }
}