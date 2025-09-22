using System;

namespace EduNestERP.Api.Model
{
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string StaffId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? PersonalEmail { get; set; }
        public string? OfficialEmail { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public DateTime JoiningDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string Status { get; set; } = "Active";
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}