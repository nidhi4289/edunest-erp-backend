using System.ComponentModel.DataAnnotations;

namespace EduNestERP.Persistence.Entities
{
    public class Class
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Section { get; set; }

        public List<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? GradingType { get; set; }
        public int? MaxMarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class ClassSubject
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid SubjectId { get; set; }

        public string? SubjectName { get; set; } 
       
    }
}
