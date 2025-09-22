using System.ComponentModel.DataAnnotations;

namespace EduNestERP.Api.Model
{
    public class AssessmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AcademicYear { get; set; }
        public string Grade { get; set; }
        public string Section { get; set; }
        public string SubjectName { get; set; }
        public decimal MaxMarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class CreateAssessmentDto
    {
        public string Name { get; set; }
        public string AcademicYear { get; set; }
        public Guid ClassId { get; set; }
        public Guid SubjectId { get; set; }
        public string GradingType { get; set; }
        public decimal MaxMarks { get; set; }
    }
}