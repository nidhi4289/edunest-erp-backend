using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> SearchStudentsAsync(string? firstName, string? lastName, string? grade);

        Task<bool> AddStudentsBulkAsync(List<Student> students);

        Task<bool?> AddStudentAsync(Student student);
        Task<bool?> UpdateStudentAsync(Student student);

        Task<Student?> GetStudentByEduNestIdAsync(string eduNestId);

    }
}