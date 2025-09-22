using System.ComponentModel.DataAnnotations;

namespace EduNestERP.Persistence.Entities
{
    public class Assessment
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GradingType { get; set; } = string.Empty;
        public decimal MaxMarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Term { get; set; } = string.Empty;
        public DateTime AssessmentDate { get; set; }
        public decimal WeightPct { get; set; }
    }
}