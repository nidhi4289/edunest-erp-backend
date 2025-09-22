namespace EduNestERP.Api.Model
{
    public class AttendanceDto
    {
        public string StudentId { get; set; } 
        public string Grade { get; set; } 
        public string Section { get; set; } 
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        
    }
}