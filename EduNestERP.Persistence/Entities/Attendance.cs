using System;

namespace EduNestERP.Persistence.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public Guid ClassId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public string Section { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    }
}