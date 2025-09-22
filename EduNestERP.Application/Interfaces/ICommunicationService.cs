using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface ICommunicationService
    {
        Task<bool> CreateCommunicationAsync(Communication communication);
        Task<Communication?> GetCommunicationByIdAsync(Guid id);
        Task<List<Communication>> GetAllCommunicationsAsync(DateTime? modifiedAfter, string? status);
        Task<bool> UpdateCommunicationAsync(Communication communication);
        Task<bool> DeleteCommunicationAsync(Guid id);
    }
}