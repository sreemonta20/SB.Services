namespace SBERP.HumanResources.Helper
{
    /// <summary>
    /// All string constants used by HR controllers, services, and logging.
    /// Mirrors SBERP.Security.Helper.ConstantSupplier so future devs find the
    /// same lookup pattern.
    ///
    /// Menu / role-permission constants are NOT here — Security owns that
    /// domain and exposes it via its own controllers.
    /// </summary>
    public static class ConstantSupplier
    {
        #region Common
        public const string SQLSERVER = "SqlServer";
        public const string CTRLER_ROUTE_PATH_NAME_VERSION_ONE = "api/v1/[controller]";
        public const string CORSS_POLICY_NAME = "HRCorsPolicy";
        public const string REQUIRED_PARAMETER_NOT_EMPTY = "Required Parameters Should Not Empty Or Null";
        public const string REQ_OR_DATA_NULL = "Request is found null or data not found";
        public const string SAVE_KEY = "Save";
        public const string UPDATE_KEY = "Update";
        public const string FAILED_MSG = "Operation failed.";
        public const string SUCCESS_MSG = "Operation completed successfully.";
        #endregion

        #region Swagger
        public const string SWAGGER_HR_DOC_END_POINT      = "/swagger/v1/swagger.json";
        public const string SWAGGER_HR_DOC_END_POINT_NAME = "SBERP Human Resources v1";
        public const string SWAGGER_HR_SCHEME_REF_ID      = "Bearer";
        #endregion

        #region Stored Procedures
        public const string SP_GET_ALL_EMPLOYEES_PAGING       = "SP_GetAllEmployeesPagingWithSearch";
        public const string SP_GET_EMPLOYEE_BY_ID             = "SP_GetEmployeeById";
        public const string SP_SAVE_UPDATE_EMPLOYEE           = "SP_SaveUpdateEmployee";
        public const string SP_DELETE_EMPLOYEE                = "SP_DeleteEmployee";
        public const string SP_GET_EMPLOYEE_INITIAL_DATA      = "SP_GetEmployeeInitialData";

        public const string SP_GET_ALL_DEPARTMENTS_PAGING     = "SP_GetAllDepartmentsPagingWithSearch";
        public const string SP_GET_DEPARTMENT_BY_ID           = "SP_GetDepartmentById";
        public const string SP_SAVE_UPDATE_DEPARTMENT         = "SP_SaveUpdateDepartment";
        public const string SP_DELETE_DEPARTMENT              = "SP_DeleteDepartment";

        public const string SP_GET_ALL_DESIGNATIONS_PAGING    = "SP_GetAllDesignationsPagingWithSearch";
        public const string SP_SAVE_UPDATE_DESIGNATION        = "SP_SaveUpdateDesignation";
        public const string SP_DELETE_DESIGNATION             = "SP_DeleteDesignation";

        public const string SP_GET_HR_SETTINGS                = "SP_GetHRSettings";
        public const string SP_SAVE_UPDATE_HR_SETTINGS        = "SP_SaveUpdateHRSettings";
        #endregion

        #region Routes — Employee
        public const string GET_ALL_EMPLOYEES_PAGING_ROUTE   = "getAllEmployeesPagingWithSearch";
        public const string GET_EMPLOYEE_BY_ID_ROUTE         = "getEmployeeById";
        public const string GET_EMPLOYEE_INITIAL_DATA_ROUTE  = "getEmployeeInitialData";
        public const string CREATE_EMPLOYEE_ROUTE            = "createEmployee";
        public const string UPDATE_EMPLOYEE_ROUTE            = "updateEmployee";
        public const string DELETE_EMPLOYEE_ROUTE            = "deleteEmployee";
        #endregion

        #region Routes — Department
        public const string GET_ALL_DEPARTMENTS_ROUTE         = "getAllDepartments";
        public const string GET_ALL_DEPARTMENTS_PAGING_ROUTE  = "getAllDepartmentsPagingWithSearch";
        public const string GET_DEPARTMENT_BY_ID_ROUTE        = "getDepartmentById";
        public const string SAVE_UPDATE_DEPARTMENT_ROUTE      = "createUpdateDepartment";
        public const string DELETE_DEPARTMENT_ROUTE           = "deleteDepartment";
        #endregion

        #region Routes — Designation
        public const string GET_ALL_DESIGNATIONS_ROUTE        = "getAllDesignations";
        public const string GET_ALL_DESIGNATIONS_PAGING_ROUTE = "getAllDesignationsPagingWithSearch";
        public const string SAVE_UPDATE_DESIGNATION_ROUTE     = "createUpdateDesignation";
        public const string DELETE_DESIGNATION_ROUTE          = "deleteDesignation";
        #endregion

        #region Routes — HR Settings
        public const string GET_HR_SETTINGS_ROUTE             = "getHRSettings";
        public const string SAVE_UPDATE_HR_SETTINGS_ROUTE     = "saveUpdateHRSettings";
        public const string UPLOAD_ATTENDANCE_EXCEL_ROUTE     = "uploadAttendanceExcel";
        #endregion

        #region Messages — Employee
        public const string EMPLOYEE_CREATE_SUCCESS  = "Employee record created successfully.";
        public const string EMPLOYEE_UPDATE_SUCCESS  = "Employee record updated successfully.";
        public const string EMPLOYEE_DELETE_SUCCESS  = "Employee record removed successfully.";
        public const string EMPLOYEE_FETCH_SUCCESS   = "Employee data fetched successfully.";
        public const string EMPLOYEE_FETCH_FAILED    = "Employee data not found.";
        public const string EMPLOYEE_EXIST_DUPL_CODE = "An employee with the same Employee Code already exists.";
        public const string EMPLOYEE_NOT_FOUND       = "Employee not found.";
        public const string EMPLOYEE_DELETE_FAILED   = "Failed to remove employee.";
        public const string EMPLOYEE_LIST_EMPTY      = "No employee records found.";
        #endregion

        #region Messages — Department
        public const string DEPARTMENT_CREATE_SUCCESS = "Department created successfully.";
        public const string DEPARTMENT_UPDATE_SUCCESS = "Department updated successfully.";
        public const string DEPARTMENT_DELETE_SUCCESS = "Department removed successfully.";
        public const string DEPARTMENT_FETCH_SUCCESS  = "Department data fetched successfully.";
        public const string DEPARTMENT_NOT_FOUND      = "Department not found.";
        public const string DEPARTMENT_LIST_EMPTY     = "No department records found.";
        public const string DEPARTMENT_DUPLICATE      = "A department with the same code or name already exists.";
        public const string DEPARTMENT_HAS_EMPLOYEES  = "Cannot delete — department still has assigned employees.";
        #endregion

        #region Messages — Designation
        public const string DESIGNATION_CREATE_SUCCESS = "Designation created successfully.";
        public const string DESIGNATION_UPDATE_SUCCESS = "Designation updated successfully.";
        public const string DESIGNATION_DELETE_SUCCESS = "Designation removed successfully.";
        public const string DESIGNATION_DUPLICATE      = "A designation with the same name already exists.";
        public const string DESIGNATION_NOT_FOUND      = "Designation not found.";
        public const string DESIGNATION_LIST_EMPTY     = "No designation records found.";
        #endregion

        #region Messages — HR Settings
        public const string HR_SETTINGS_FETCH_SUCCESS  = "HR settings fetched successfully.";
        public const string HR_SETTINGS_SAVE_SUCCESS   = "HR settings saved successfully.";
        public const string HR_SETTINGS_NOT_FOUND      = "HR settings not configured yet.";
        public const string ATTENDANCE_EXCEL_UPLOAD_OK = "Attendance Excel uploaded and processed successfully.";
        public const string ATTENDANCE_EXCEL_INVALID   = "Uploaded file is invalid or empty.";
        #endregion

        #region Logging templates
        public const string LOG_API_STARTED   = "{0} api method started.";
        public const string LOG_API_REQ       = "{0} api method request: \n{1}\n";
        public const string LOG_API_RES       = "{0} api method response: \n{1}\n";
        public const string LOG_API_EXCEPTION = "{0} api method exception: \n{1}\n";
        public const string LOG_SERVICE_REQ   = "{0} service method request: \n{1}\n";
        public const string LOG_SERVICE_RES   = "{0} service method response: \n{1}\n";
        #endregion
    }
}
