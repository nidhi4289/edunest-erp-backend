using System;

namespace EduNestERP.Api.Models
{
    public class StudentFeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string StudentEduNestId { get; set; } = string.Empty;
        public DateTime DateOfCollection { get; set; }
        public decimal FeeCollected { get; set; }
        public decimal FeeWaived { get; set; } = 0;
        public string WaiverReason { get; set; } = string.Empty;
        public string Grade { get; set; }
        public string Section { get; set; }
        public decimal TotalFees { get; set; }
        public decimal FeeRemaining { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
