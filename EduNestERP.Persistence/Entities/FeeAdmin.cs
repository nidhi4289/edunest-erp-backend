using System;

namespace EduNestERP.Persistence.Entities
{
    public class FeeAdmin
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string? AcademicYear { get; set; }
        public decimal? MonthlyFee { get; set; }
        public decimal? AnnualFee { get; set; }
        public decimal? AdmissionFee { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? LibraryFee { get; set; }
        public decimal? SportsFee { get; set; }
        public decimal? MiscellaneousFee { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}