using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Service;
using System.Net;

namespace SBERP.HumanResources.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class PayComponentController : ControllerBase
    {
        private readonly IPayComponentService _service;
        private readonly IHRLogService _log;
        public PayComponentController(IPayComponentService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_PAY_COMPONENTS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllPayComponentsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllPayComponentsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.PAY_COMPONENT_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.PAY_COMPONENT_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllPayComponentsPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_PAY_COMPONENTS_LOOKUP_ROUTE)]
        public async Task<object> GetPayComponentsLookupAsync()
        {
            try { return await _service.GetPayComponentsLookupAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayComponentsLookupAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_PAY_COMPONENT_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdatePayComponentAsync([FromBody] PayComponentRequest request)
        {
            try { return await _service.CreateUpdatePayComponentAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdatePayComponentAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_PAY_COMPONENT_ROUTE)]
        public async Task<object> DeletePayComponentAsync([FromQuery] string id)
        {
            try { return await _service.DeletePayComponentAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeletePayComponentAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class SalaryStructureController : ControllerBase
    {
        private readonly ISalaryStructureService _service;
        private readonly IHRLogService _log;
        public SalaryStructureController(ISalaryStructureService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_SALARY_STRUCTURES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllSalaryStructuresPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllSalaryStructuresPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.SALARY_STRUCTURE_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.SALARY_STRUCTURE_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllSalaryStructuresPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_SALARY_STRUCTURE_DETAIL_ROUTE)]
        public async Task<object> GetSalaryStructureDetailAsync([FromQuery] string id)
        {
            try { return await _service.GetSalaryStructureDetailAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetSalaryStructureDetailAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_SALARY_STRUCTURE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateSalaryStructureAsync([FromBody] SalaryStructureRequest request)
        {
            try { return await _service.CreateUpdateSalaryStructureAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateSalaryStructureAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_SALARY_STRUCTURE_ROUTE)]
        public async Task<object> DeleteSalaryStructureAsync([FromQuery] string id)
        {
            try { return await _service.DeleteSalaryStructureAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteSalaryStructureAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class PayrollRunController : ControllerBase
    {
        private readonly IPayrollRunService _service;
        private readonly IHRLogService _log;
        public PayrollRunController(IPayrollRunService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_PAYROLL_RUNS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllPayrollRunsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllPayrollRunsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.PAYROLL_RUN_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.PAYROLL_RUN_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllPayrollRunsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.PROCESS_PAYROLL_RUN_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> ProcessPayrollRunAsync([FromBody] ProcessPayrollRequest request)
        {
            try { return await _service.ProcessPayrollRunAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ProcessPayrollRunAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_PAYROLL_RUN_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetPayrollRunStatusAsync([FromBody] PayrollRunActionRequest request)
        {
            try { return await _service.SetPayrollRunStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetPayrollRunStatusAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_PAYSLIPS_BY_RUN_ROUTE)]
        public async Task<object> GetPayslipsByRunAsync([FromQuery] string id)
        {
            try { return await _service.GetPayslipsByRunAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayslipsByRunAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_PAYSLIP_DETAIL_ROUTE)]
        public async Task<object> GetPayslipDetailAsync([FromQuery] string id)
        {
            try { return await _service.GetPayslipDetailAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayslipDetailAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _service;
        private readonly IHRLogService _log;
        public LoanController(ILoanService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_LOANS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllLoansPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllLoansPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.LOAN_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.LOAN_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllLoansPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_LOAN_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateLoanAsync([FromBody] LoanRequest request)
        {
            try { return await _service.CreateUpdateLoanAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateLoanAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_LOAN_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetLoanStatusAsync([FromBody] LoanActionRequest request)
        {
            try { return await _service.SetLoanStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetLoanStatusAsync)); }
        }
    }
}
