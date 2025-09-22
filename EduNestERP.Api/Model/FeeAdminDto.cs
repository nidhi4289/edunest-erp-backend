using System;

namespace EduNestERP.Api.Model
{
    public class FeeAdminDto
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

    public class CreateFeeAdminDto
    {
        public Guid ClassId { get; set; }
        public string AcademicYear { get; set; } = null!;
        public decimal? MonthlyFee { get; set; }
        public decimal? AnnualFee { get; set; }
        public decimal? AdmissionFee { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? LibraryFee { get; set; }
        public decimal? SportsFee { get; set; }
        public decimal? MiscellaneousFee { get; set; }
        public bool? IsActive { get; set; } = true;
    }

    public class UpdateFeeAdminDto
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
    }
}