using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;

namespace EduNestERP.Application.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ICommunicationRepository _communicationRepository;

        public CommunicationService(ICommunicationRepository communicationRepository)
        {
            _communicationRepository = communicationRepository;
        }

        public async Task<bool> CreateCommunicationAsync(Communication communication)
        {
            return await _communicationRepository.AddAsync(communication);
        }

        public async Task<bool> UpdateCommunicationAsync(Communication communication)
        {
            return await _communicationRepository.UpdateAsync(communication);
        }

        public async Task<Communication?> GetCommunicationByIdAsync(Guid id)
        {
            return await _communicationRepository.GetByIdAsync(id);
        }

        public async Task<List<Communication>> GetAllCommunicationsAsync(DateTime? modifiedAfter, string? status)
        {
            return await _communicationRepository.GetAllAsync(modifiedAfter, status);
        }

        public async Task<bool> DeleteCommunicationAsync(Guid id)
        {
            return await _communicationRepository.DeleteAsync(id);
        }
    }
}