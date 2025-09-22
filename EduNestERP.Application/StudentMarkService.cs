using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;

namespace EduNestERP.Application.Services
{
    public class StudentMarkService : IStudentMarkService
    {
        private readonly IStudentMarkRepository _studentMarkRepository;

        public StudentMarkService(IStudentMarkRepository studentMarkRepository)
        {
            _studentMarkRepository = studentMarkRepository;
        }

        public async Task<bool?> AddStudentMarkAsync(StudentMark studentMark)
        {
            return await _studentMarkRepository.AddAsync(studentMark);
        }

        public async Task<bool> UpdateStudentMarkAsync(StudentMark studentMark)
        {
            return await _studentMarkRepository.UpdateAsync(studentMark);
        }

        public async Task<bool?> BulkAddStudentMarksAsync(List<StudentMark> studentMarks)
        {
            return await _studentMarkRepository.BulkAddAsync(studentMarks);
        }

        public async Task<List<StudentMark>> GetMarksByAssessmentAsync(Guid assessmentId)
        {
            return await _studentMarkRepository.GetByAssessmentIdAsync(assessmentId);
        }

        public async Task<List<StudentMark>> GetMarksByStudentAsync(string eduNestId, string academicYear)
        {
            return await _studentMarkRepository.GetByStudentIdAsync(eduNestId, academicYear);
        }

        public async Task<StudentMark?> GetMarkByAssessmentAndStudentAsync(Guid assessmentId, string eduNestId)
        {
            return await _studentMarkRepository.GetByAssessmentAndStudentAsync(assessmentId, eduNestId);
        }

        public async Task<bool?> DeleteStudentMarkAsync(Guid id)
        {
            return await _studentMarkRepository.DeleteAsync(id);
        }
    }
}