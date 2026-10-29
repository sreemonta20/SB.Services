namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Payroll
        public const string SP_GET_ALL_PAY_COMPONENTS_PAGING     = "SP_GetAllPayComponentsPagingWithSearch";
        public const string SP_GET_ALL_SALARY_STRUCTURES_PAGING  = "SP_GetAllSalaryStructuresPagingWithSearch";
        public const string SP_GET_SALARY_STRUCTURE_DETAIL       = "SP_GetSalaryStructureDetail";
        public const string SP_PROCESS_PAYROLL_RUN               = "SP_ProcessPayrollRun";
        public const string SP_SET_PAYROLL_RUN_STATUS            = "SP_SetPayrollRunStatus";
        public const string SP_GET_PAYSLIPS_BY_RUN              = "SP_GetPayslipsByRun";
        public const string SP_GET_PAYSLIP_DETAIL               = "SP_GetPayslipDetail";
        public const string SP_GET_ALL_PAYROLL_RUNS_PAGING       = "SP_GetAllPayrollRunsPagingWithSearch";
        public const string SP_GET_ALL_LOANS_PAGING             = "SP_GetAllLoansPagingWithSearch";
        #endregion

        #region Routes — Payroll
        public const string GET_ALL_PAY_COMPONENTS_PAGING_ROUTE   = "getAllPayComponentsPagingWithSearch";
        public const string GET_PAY_COMPONENTS_LOOKUP_ROUTE       = "getPayComponentsLookup";
        public const string CREATE_UPDATE_PAY_COMPONENT_ROUTE     = "createUpdatePayComponent";
        public const string DELETE_PAY_COMPONENT_ROUTE            = "deletePayComponent";

        public const string GET_ALL_SALARY_STRUCTURES_PAGING_ROUTE = "getAllSalaryStructuresPagingWithSearch";
        public const string GET_SALARY_STRUCTURE_DETAIL_ROUTE      = "getSalaryStructureDetail";
        public const string CREATE_UPDATE_SALARY_STRUCTURE_ROUTE   = "createUpdateSalaryStructure";
        public const string DELETE_SALARY_STRUCTURE_ROUTE          = "deleteSalaryStructure";

        public const string GET_ALL_PAYROLL_RUNS_PAGING_ROUTE      = "getAllPayrollRunsPagingWithSearch";
        public const string PROCESS_PAYROLL_RUN_ROUTE              = "processPayrollRun";
        public const string SET_PAYROLL_RUN_STATUS_ROUTE           = "setPayrollRunStatus";
        public const string GET_PAYSLIPS_BY_RUN_ROUTE              = "getPayslipsByRun";
        public const string GET_PAYSLIP_DETAIL_ROUTE               = "getPayslipDetail";

        public const string GET_ALL_LOANS_PAGING_ROUTE             = "getAllLoansPagingWithSearch";
        public const string CREATE_UPDATE_LOAN_ROUTE               = "createUpdateLoan";
        public const string SET_LOAN_STATUS_ROUTE                  = "setLoanStatus";
        #endregion

        #region Messages — Payroll
        public const string PAY_COMPONENT_LIST_EMPTY    = "No pay components found.";
        public const string PAY_COMPONENT_FETCH_SUCCESS = "Pay component data fetched successfully.";
        public const string SALARY_STRUCTURE_LIST_EMPTY = "No salary structures found.";
        public const string SALARY_STRUCTURE_FETCH      = "Salary structure data fetched successfully.";
        public const string PAYROLL_RUN_LIST_EMPTY      = "No payroll runs found.";
        public const string PAYROLL_RUN_FETCH           = "Payroll run data fetched successfully.";
        public const string PAYSLIP_FETCH               = "Payslip data fetched successfully.";
        public const string LOAN_LIST_EMPTY             = "No loans found.";
        public const string LOAN_FETCH                  = "Loan data fetched successfully.";
        public const string PAY_COMPONENT_IN_USE        = "Pay component is in use and can't be removed.";
        public const string SALARY_STRUCTURE_LOCKED     = "Structure can't be changed once payroll has run for the period.";
        #endregion
    }
}
