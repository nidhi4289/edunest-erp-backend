using System;

namespace EduNestERP.Persistence.Entities
{
    public class Homework
    {
        public Guid Id { get; set; }
        public Guid ClassSubjectId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Details { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
