using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduNestERP.Persistence.Entities
{
    [Table("student_marks")]
    public class StudentMark
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("assessment_id")]
        public Guid AssessmentId { get; set; }

        [Required]
        [Column("edunest_id")]
        public string EduNestId { get; set; } = string.Empty;

        [Column("marks_obtained")]
        public decimal? MarksObtained { get; set; }

        [Column("grade_awarded")]
        public string? GradeAwarded { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string SubjectName { get; set; } = string.Empty;

        public string AssessmentName { get; set; } = string.Empty;

        public decimal? MaxMarks { get; set; }

    }
}