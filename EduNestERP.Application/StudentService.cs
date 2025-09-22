using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Application.Interfaces;

namespace EduNestERP.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string? firstName, string? lastName, string? grade, string? section)
        {
            return await _studentRepository.SearchStudentsAsync(firstName, lastName, grade, section);
        }

        public async Task<bool> AddStudentsBulkAsync(List<Student> students)
        {
            return await _studentRepository.AddStudentsBulkAsync(students);
        }

        public async Task<bool?> AddStudentAsync(Student student)
        {

            // Generate eduNestId as 'ST-' + FirstName + LastName + DateOfBirth (yyyymmdd)
            var dobStr = student.DateOfBirth.ToString("yyyyMMdd");
            var generatedEduNestId = $"ST-{student.FirstName}{student.LastName}{dobStr}";

            var existingStudent = await _studentRepository.GetByEduNestIdAsync(generatedEduNestId);
            if (existingStudent != null)
            {
                return false; // Student already exists
            }

            student.EduNestId = generatedEduNestId;
            return await _studentRepository.AddAsync(student);
        }

        public async Task<bool?> UpdateStudentAsync(Student student)
        {
            var existingStudent = await _studentRepository.GetByEduNestIdAsync(student.EduNestId);
            if (existingStudent == null)
            {
                return false; // Student does not exist
            }

            return await _studentRepository.UpdateAsync(student);
        }
        public async Task<Student?> GetStudentByEduNestIdAsync(string eduNestId)
        {
            return await _studentRepository.GetByEduNestIdAsync(eduNestId);
        }
    }
}
