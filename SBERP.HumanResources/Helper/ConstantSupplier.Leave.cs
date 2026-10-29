namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Leave
        public const string SP_GET_ALL_LEAVE_TYPES_PAGING        = "SP_GetAllLeaveTypesPagingWithSearch";
        public const string SP_GET_ALL_LEAVE_POLICIES_PAGING     = "SP_GetAllLeavePoliciesPagingWithSearch";
        public const string SP_GET_ALL_LEAVE_APPLICATIONS_PAGING = "SP_GetAllLeaveApplicationsPagingWithSearch";
        public const string SP_GET_EMPLOYEE_LEAVE_BALANCES       = "SP_GetEmployeeLeaveBalances";
        public const string SP_APPLY_LEAVE                       = "SP_ApplyLeave";
        public const string SP_SET_LEAVE_APPLICATION_STATUS      = "SP_SetLeaveApplicationStatus";
        public const string SP_GRANT_LEAVE_ENTITLEMENTS          = "SP_GrantLeaveEntitlements";
        #endregion

        #region Routes — Leave
        public const string GET_ALL_LEAVE_TYPES_PAGING_ROUTE        = "getAllLeaveTypesPagingWithSearch";
        public const string CREATE_UPDATE_LEAVE_TYPE_ROUTE          = "createUpdateLeaveType";
        public const string DELETE_LEAVE_TYPE_ROUTE                 = "deleteLeaveType";

        public const string GET_ALL_LEAVE_POLICIES_PAGING_ROUTE     = "getAllLeavePoliciesPagingWithSearch";
        public const string CREATE_UPDATE_LEAVE_POLICY_ROUTE        = "createUpdateLeavePolicy";
        public const string DELETE_LEAVE_POLICY_ROUTE               = "deleteLeavePolicy";
        public const string GRANT_ENTITLEMENTS_ROUTE                = "grantLeaveEntitlements";

        public const string GET_ALL_LEAVE_APPLICATIONS_PAGING_ROUTE = "getAllLeaveApplicationsPagingWithSearch";
        public const string APPLY_LEAVE_ROUTE                       = "applyLeave";
        public const string SET_LEAVE_STATUS_ROUTE                  = "setLeaveApplicationStatus";
        public const string GET_EMPLOYEE_BALANCES_ROUTE             = "getEmployeeLeaveBalances";
        public const string GET_ALL_LEAVE_TYPES_LOOKUP_ROUTE        = "getLeaveTypesLookup";
        #endregion

        #region Messages — Leave
        public const string LEAVE_TYPE_LIST_EMPTY     = "No leave types found.";
        public const string LEAVE_TYPE_FETCH_SUCCESS  = "Leave type data fetched successfully.";
        public const string LEAVE_POLICY_LIST_EMPTY   = "No leave policies found.";
        public const string LEAVE_POLICY_FETCH_SUCCESS= "Leave policy data fetched successfully.";
        public const string LEAVE_APP_LIST_EMPTY      = "No leave applications found.";
        public const string LEAVE_APP_FETCH_SUCCESS   = "Leave applications fetched successfully.";
        public const string LEAVE_BALANCE_FETCH       = "Leave balances fetched successfully.";
        public const string LEAVE_APPLY_SUCCESS       = "Leave applied successfully.";
        public const string LEAVE_STATUS_SUCCESS      = "Leave application updated.";
        public const string LEAVE_GRANT_SUCCESS       = "Entitlements granted.";
        public const string LEAVE_TYPE_IN_USE         = "Leave type is in use and can't be removed.";
        public const string LEAVE_DATE_PARSE_ERROR    = "Invalid date value.";
        #endregion
    }
}
