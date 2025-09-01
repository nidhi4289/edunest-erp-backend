using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    // Add methods specific to StudentRepository if needed
    Task<bool> AddStudentsBulkAsync(List<Student> students);

    Task<Student?> GetByEduNestIdAsync(string eduNestId);

    Task<IEnumerable<Student>> SearchStudentsAsync(string? firstName, string? lastName, string? grade);

}