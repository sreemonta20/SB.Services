namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Attendance
        public const string SP_GET_ALL_SHIFTS_PAGING            = "SP_GetAllShiftsPagingWithSearch";
        public const string SP_GET_ALL_OVERTIME_PAGING          = "SP_GetAllOvertimeRequestsPagingWithSearch";
        public const string SP_GET_ALL_REGULARIZATIONS_PAGING   = "SP_GetAllRegularizationsPagingWithSearch";
        public const string SP_GET_EMPLOYEE_ROSTER              = "SP_GetEmployeeRoster";
        public const string SP_SET_OVERTIME_STATUS              = "SP_SetOvertimeStatus";
        public const string SP_SET_REGULARIZATION_STATUS        = "SP_SetRegularizationStatus";
        public const string SP_GET_MONTHLY_ATTENDANCE_SUMMARY   = "SP_GetMonthlyAttendanceSummary";
        #endregion

        #region Routes — Attendance
        public const string GET_ALL_SHIFTS_PAGING_ROUTE          = "getAllShiftsPagingWithSearch";
        public const string CREATE_UPDATE_SHIFT_ROUTE            = "createUpdateShift";
        public const string DELETE_SHIFT_ROUTE                   = "deleteShift";

        public const string CREATE_UPDATE_ROSTER_ROUTE           = "createUpdateRoster";
        public const string GET_EMPLOYEE_ROSTER_ROUTE            = "getEmployeeRoster";
        public const string DELETE_ROSTER_ROUTE                  = "deleteRoster";

        public const string GET_ALL_OVERTIME_PAGING_ROUTE        = "getAllOvertimePagingWithSearch";
        public const string CREATE_UPDATE_OVERTIME_ROUTE         = "createUpdateOvertime";
        public const string SET_OVERTIME_STATUS_ROUTE            = "setOvertimeStatus";

        public const string GET_ALL_REGULARIZATIONS_PAGING_ROUTE = "getAllRegularizationsPagingWithSearch";
        public const string CREATE_UPDATE_REGULARIZATION_ROUTE   = "createUpdateRegularization";
        public const string SET_REGULARIZATION_STATUS_ROUTE      = "setRegularizationStatus";

        public const string GET_MONTHLY_SUMMARY_ROUTE            = "getMonthlyAttendanceSummary";
        #endregion

        #region Messages — Attendance
        public const string SHIFT_LIST_EMPTY        = "No shifts found.";
        public const string SHIFT_FETCH_SUCCESS     = "Shift data fetched successfully.";
        public const string OVERTIME_LIST_EMPTY     = "No overtime requests found.";
        public const string OVERTIME_FETCH_SUCCESS  = "Overtime data fetched successfully.";
        public const string REG_LIST_EMPTY          = "No regularization requests found.";
        public const string REG_FETCH_SUCCESS       = "Regularization data fetched successfully.";
        public const string ROSTER_FETCH_SUCCESS    = "Roster fetched successfully.";
        public const string ROSTER_SAVE_SUCCESS     = "Roster saved successfully.";
        public const string SUMMARY_FETCH_SUCCESS   = "Monthly summary fetched successfully.";
        public const string ATT_TIME_PARSE_ERROR    = "Invalid time value. Use HH:mm or HH:mm:ss.";
        #endregion
    }
}
