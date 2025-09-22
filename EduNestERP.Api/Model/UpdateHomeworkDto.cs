using System;

namespace EduNestERP.Api.Model
{
    public class UpdateHomeworkDto
    {
        public Guid Id { get; set; }
        public Guid ClassSubjectId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Details { get; set; } = null!;
    }
}
