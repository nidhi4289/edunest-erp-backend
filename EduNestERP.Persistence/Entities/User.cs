namespace EduNestERP.Persistence.Entities
{
    public class User
    {
        public string UserId { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool FirstLoginCompleted { get; set; } = false;
    // Add other fields as needed
    }
}