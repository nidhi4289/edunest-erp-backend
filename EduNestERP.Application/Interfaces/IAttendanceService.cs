
using EduNestERP.Persistence.Entities;  

namespace EduNestERP.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> MarkAttendanceAsync(List<Attendance> attendance);

        Task<List<Attendance>> GetAttendanceByStudentAsync(string eduNestId, string grade, string section);
    }
}
