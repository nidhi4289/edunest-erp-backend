namespace EduNestERP.Persistence.Entities
{
    public class Student
    {
        public string EduNestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FatherName { get; set; }
        public string? FatherEmail { get; set; }
        public string MotherName { get; set; }
        public string? MotherEmail { get; set; }
        public string Grade { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public Guid ClassId { get; set; }
        public string ModifiedBy { get; set; } = "System";
        public string AdmissionNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string Email { get; set; }
        
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; } = "India";
        public List<GovernmentIdentifiers> GovernmentIdentifiers { get; set; } = new List<GovernmentIdentifiers>();

    }
}