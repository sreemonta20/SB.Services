using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    public class PayComponentTypeLookup : BaseLookup { }
    public class PayrollRunStatusLookup : BaseLookup { }
    public class LoanStatusLookup       : BaseLookup { }

    public class PayComponent
    {
        [Key] public Guid Id { get; set; }
        public string? ComponentCode { get; set; }
        public string? Name { get; set; }
        public int ComponentTypeId { get; set; }   // 1 Earning / 2 Deduction
        public int CalcMethod { get; set; }         // 1 Fixed / 2 % of basic
        [Column(TypeName = "decimal(18,2)")] public decimal? DefaultValue { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsProrated { get; set; }
        public int? SortOrder { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PayComponentLog
    {
        [Key] public Guid Id { get; set; }
        public Guid PayComponentId { get; set; }
        public string? ComponentCode { get; set; }
        public string? Name { get; set; }
        public int? ComponentTypeId { get; set; }
        public int? CalcMethod { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class SalaryStructure
    {
        [Key] public Guid Id { get; set; }
        public string? StructureCode { get; set; }
        public Guid EmployeeId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal BasicSalary { get; set; }
        public string? Currency { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual ICollection<SalaryStructureLine>? Lines { get; set; }
    }

    public class SalaryStructureLog
    {
        [Key] public Guid Id { get; set; }
        public Guid SalaryStructureId { get; set; }
        public string? StructureCode { get; set; }
        public Guid? EmployeeId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? BasicSalary { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class SalaryStructureLine
    {
        [Key] public Guid Id { get; set; }
        public Guid SalaryStructureId { get; set; }
        public Guid PayComponentId { get; set; }
        public int CalcMethod { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Value { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual PayComponent? PayComponent { get; set; }
    }

    public class PayrollRun
    {
        [Key] public Guid Id { get; set; }
        public string? RunCode { get; set; }
        public int PayMonth { get; set; }
        public int PayYear { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal WorkingDays { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? TotalGross { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? TotalDeductions { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? TotalNet { get; set; }
        public int? EmployeeCount { get; set; }
        public string? ProcessedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PayrollRunLog
    {
        [Key] public Guid Id { get; set; }
        public Guid PayrollRunId { get; set; }
        public string? RunCode { get; set; }
        public int? PayMonth { get; set; }
        public int? PayYear { get; set; }
        public int? StatusId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? TotalNet { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class Payslip
    {
        [Key] public Guid Id { get; set; }
        public string? PayslipCode { get; set; }
        public Guid PayrollRunId { get; set; }
        public Guid EmployeeId { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal PaidDays { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal LopDays { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal BasicEarned { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal GrossEarnings { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalDeductions { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal LoanDeduction { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NetPay { get; set; }
        public string? Currency { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual ICollection<PayslipLine>? Lines { get; set; }
    }

    public class PayslipLine
    {
        [Key] public Guid Id { get; set; }
        public Guid PayslipId { get; set; }
        public Guid? PayComponentId { get; set; }
        public string? ComponentName { get; set; }
        public int ComponentTypeId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class Loan
    {
        [Key] public Guid Id { get; set; }
        public string? LoanCode { get; set; }
        public Guid EmployeeId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal PrincipalAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal InstallmentAmount { get; set; }
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal OutstandingAmount { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int StatusId { get; set; }
        public string? Reason { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class LoanLog
    {
        [Key] public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public string? LoanCode { get; set; }
        public Guid? EmployeeId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? PrincipalAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? OutstandingAmount { get; set; }
        public int? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class LoanInstallment
    {
        [Key] public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public Guid? PayslipId { get; set; }
        public int PayMonth { get; set; }
        public int PayYear { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
