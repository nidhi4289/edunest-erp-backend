namespace EduNestERP.Persistence.Entities;

public class StudentFee
{
    public Guid Id { get; set; }
    public string StudentEduNestId { get; set; }
    public DateTime DateOfCollection { get; set; }
    public decimal FeeCollected { get; set; }
    public decimal FeeWaived { get; set; }
    public string WaiverReason { get; set; }
    public string Grade { get; set; }
    public string Section { get; set; }
    public Guid ClassId { get; set; }
    public decimal TotalFees { get; set; }
    public decimal FeeRemaining { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}