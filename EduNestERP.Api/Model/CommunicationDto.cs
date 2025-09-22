namespace EduNestERP.Api.Model
{
    public class CommunicationCreateDto
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public string? Status { get; set; }
    }

    public class CommunicationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    public class CommunicationUpdateDto
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string? Status { get; set; }
        public Guid ModifiedById { get; set; } // The user's Id who is modifying

    }
}