using System;

namespace EduNestERP.Api.Model
{
    public class HomeworkDto
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
    public class CreateHomeworkDto
    {
        public Guid ClassSubjectId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Details { get; set; } = null!;
    }
}
