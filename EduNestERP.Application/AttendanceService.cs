using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Entities;
using EduNestERP.Persistence.Repositories;

namespace EduNestERP.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<bool> MarkAttendanceAsync(List<Attendance> attendance)
        {
            return await _attendanceRepository.MarkAttendanceAsync(attendance);
        }

        public async Task<List<Attendance>> GetAttendanceByStudentAsync(string eduNestId, string grade, string section)
        {
            return await _attendanceRepository.GetAttendanceByStudentAsync(eduNestId, grade, section);
        }
    }
}
