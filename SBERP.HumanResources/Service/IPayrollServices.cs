using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IPayComponentService
    {
        Task<PagingResult<PayComponentResponse>?> GetAllPayComponentsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetPayComponentsLookupAsync();
        Task<DataResponse> CreateUpdatePayComponentAsync(PayComponentRequest request);
        Task<DataResponse> DeletePayComponentAsync(string id);
    }

    public interface ISalaryStructureService
    {
        Task<PagingResult<SalaryStructureResponse>?> GetAllSalaryStructuresPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetSalaryStructureDetailAsync(string id);
        Task<DataResponse> CreateUpdateSalaryStructureAsync(SalaryStructureRequest request);
        Task<DataResponse> DeleteSalaryStructureAsync(string id);
    }

    public interface IPayrollRunService
    {
        Task<PagingResult<PayrollRunResponse>?> GetAllPayrollRunsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> ProcessPayrollRunAsync(ProcessPayrollRequest request);
        Task<DataResponse> SetPayrollRunStatusAsync(PayrollRunActionRequest request);
        Task<DataResponse> GetPayslipsByRunAsync(string runId);
        Task<DataResponse> GetPayslipDetailAsync(string id);
    }

    public interface ILoanService
    {
        Task<PagingResult<LoanResponse>?> GetAllLoansPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateLoanAsync(LoanRequest request);
        Task<DataResponse> SetLoanStatusAsync(LoanActionRequest request);
    }
}
