using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class PayComponentRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? ComponentCode { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public int ComponentTypeId { get; set; }   // 1 Earning / 2 Deduction
        public int CalcMethod { get; set; } = 1;              // 1 Fixed / 2 % of basic
        public decimal? DefaultValue { get; set; }
        public bool IsTaxable { get; set; } = true;
        public bool IsProrated { get; set; } = true;
        public int? SortOrder { get; set; }
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class SalaryStructureLineRequest
    {
        public string? PayComponentId { get; set; }
        public int CalcMethod { get; set; }
        public decimal Value { get; set; }
    }

    public class SalaryStructureRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? StructureCode { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public decimal BasicSalary { get; set; }
        public string? Currency { get; set; }
        [Required] public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public List<SalaryStructureLineRequest>? Lines { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ProcessPayrollRequest
    {
        [Required] public int PayMonth { get; set; }
        [Required] public int PayYear { get; set; }
        [Required] public DateTime PeriodStart { get; set; }
        [Required] public DateTime PeriodEnd { get; set; }
        [Required] public decimal WorkingDays { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class PayrollRunActionRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }     // 3 Approved / 4 Paid / 5 Cancelled
        public string? CreateUpdateBy { get; set; }
    }

    public class LoanRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public decimal PrincipalAmount { get; set; }
        [Required] public decimal InstallmentAmount { get; set; }
        [Required] public int TotalInstallments { get; set; }
        [Required] public int StartMonth { get; set; }
        [Required] public int StartYear { get; set; }
        public string? Reason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class LoanActionRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }      // 2 Closed / 3 Cancelled
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class PayComponentResponse
    {
        public Guid Id { get; set; }
        public string? ComponentCode { get; set; }
        public string? Name { get; set; }
        public int ComponentTypeId { get; set; }
        public string? ComponentTypeName { get; set; }
        public int CalcMethod { get; set; }
        public decimal? DefaultValue { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsProrated { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SalaryStructureResponse
    {
        public Guid Id { get; set; }
        public string? StructureCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public decimal BasicSalary { get; set; }
        public string? Currency { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int LineCount { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PayrollRunResponse
    {
        public Guid Id { get; set; }
        public string? RunCode { get; set; }
        public int PayMonth { get; set; }
        public int PayYear { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public decimal WorkingDays { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal? TotalGross { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? TotalNet { get; set; }
        public int? EmployeeCount { get; set; }
    }

    public class LoanResponse
    {
        public Guid Id { get; set; }
        public string? LoanCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InstallmentAmount { get; set; }
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
    }
}
