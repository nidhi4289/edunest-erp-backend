using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IAttendanceRepository
    {
        Task<bool> MarkAttendanceAsync(List<Attendance> attendance);
        Task<List<Attendance>> GetAttendanceByStudentAsync(string eduNestId, string grade, string section);
    }
}
