namespace EduNestERP.Api.Model
{
    public class StudentMarkDto
    {
        public string EduNestId { get; set; }
        public Guid AssessmentId { get; set; }
        public string? AssessmentName { get; set; } = string.Empty;
        public string? SubjectName { get; set; } = string.Empty;
        public decimal MarksObtained { get; set; }
        public decimal? MaxMarks { get; set; }
        public string? GradeAwarded { get; set; } = string.Empty;
        public string? Remarks { get; set; } = string.Empty;
    }
}