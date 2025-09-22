namespace EduNestERP.Persistence.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool FirstLoginCompleted { get; set; } = false;
    // Add other fields as needed
    }
}