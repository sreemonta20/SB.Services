namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Separation
        public const string SP_GET_ALL_SEPARATIONS_PAGING = "SP_GetAllSeparationsPagingWithSearch";
        public const string SP_GET_SEPARATION_DETAIL       = "SP_GetSeparationDetail";
        public const string SP_APPROVE_SEPARATION          = "SP_ApproveSeparation";
        public const string SP_SET_CLEARANCE_STATUS        = "SP_SetClearanceStatus";
        public const string SP_SETTLE_SEPARATION           = "SP_SettleSeparation";
        public const string SP_SET_SEPARATION_STATUS       = "SP_SetSeparationStatus";
        #endregion

        #region Routes — Separation
        public const string GET_ALL_SEPARATIONS_PAGING_ROUTE = "getAllSeparationsPagingWithSearch";
        public const string GET_SEPARATION_DETAIL_ROUTE      = "getSeparationDetail";
        public const string CREATE_UPDATE_SEPARATION_ROUTE   = "createUpdateSeparation";
        public const string DELETE_SEPARATION_ROUTE          = "deleteSeparation";
        public const string SET_SEPARATION_STATUS_ROUTE      = "setSeparationStatus";
        public const string APPROVE_SEPARATION_ROUTE         = "approveSeparation";
        public const string SETTLE_SEPARATION_ROUTE          = "settleSeparation";

        public const string SET_CLEARANCE_STATUS_ROUTE       = "setClearanceStatus";

        public const string CREATE_UPDATE_EXIT_INTERVIEW_ROUTE = "createUpdateExitInterview";
        #endregion

        #region Messages — Separation
        public const string SEPARATION_LIST_EMPTY   = "No separations found.";
        public const string SEPARATION_FETCH        = "Separation data fetched successfully.";
        public const string SEPARATION_ACTIVE_EXISTS = "This employee already has an active separation in progress.";
        public const string EXIT_ALREADY_EXISTS     = "An exit interview already exists for this separation.";
        #endregion
    }
}
