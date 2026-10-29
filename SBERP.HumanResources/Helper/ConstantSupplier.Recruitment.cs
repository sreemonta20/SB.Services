namespace SBERP.HumanResources.Helper
{
    /// <summary>
    /// Recruitment constants — merge these regions into the existing
    /// static ConstantSupplier class (partial). Keeps the same grouping the
    /// core file uses (Stored Procedures / Routes / Messages).
    /// </summary>
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Recruitment
        public const string SP_GET_ALL_POSITIONS_PAGING       = "SP_GetAllPositionsPagingWithSearch";
        public const string SP_GET_ALL_REQUISITIONS_PAGING    = "SP_GetAllRequisitionsPagingWithSearch";
        public const string SP_GET_ALL_JOBPOSTINGS_PAGING     = "SP_GetAllJobPostingsPagingWithSearch";
        public const string SP_GET_ALL_CANDIDATES_PAGING      = "SP_GetAllCandidatesPagingWithSearch";
        public const string SP_GET_APPLICATIONS_BY_POSTING     = "SP_GetApplicationsByPosting";
        public const string SP_GET_INTERVIEWS_BY_APPLICATION   = "SP_GetInterviewsByApplication";
        public const string SP_SAVE_CANDIDATE_APPLICATION      = "SP_SaveCandidateApplication";
        public const string SP_MOVE_APPLICATION_STAGE          = "SP_MoveApplicationStage";
        #endregion

        #region Routes — Position
        public const string GET_ALL_POSITIONS_PAGING_ROUTE = "getAllPositionsPagingWithSearch";
        public const string SAVE_UPDATE_POSITION_ROUTE     = "createUpdatePosition";
        public const string DELETE_POSITION_ROUTE          = "deletePosition";
        #endregion

        #region Routes — Requisition
        public const string GET_ALL_REQUISITIONS_PAGING_ROUTE = "getAllRequisitionsPagingWithSearch";
        public const string GET_REQUISITION_BY_ID_ROUTE       = "getRequisitionById";
        public const string SAVE_UPDATE_REQUISITION_ROUTE     = "createUpdateRequisition";
        public const string SUBMIT_REQUISITION_ROUTE          = "submitRequisition";
        public const string APPROVE_REQUISITION_ROUTE         = "approveRequisition";
        public const string REJECT_REQUISITION_ROUTE          = "rejectRequisition";
        #endregion

        #region Routes — Job Posting
        public const string GET_ALL_JOBPOSTINGS_PAGING_ROUTE = "getAllJobPostingsPagingWithSearch";
        public const string GET_JOBPOSTING_BY_ID_ROUTE       = "getJobPostingById";
        public const string SAVE_UPDATE_JOBPOSTING_ROUTE     = "createUpdateJobPosting";
        public const string PUBLISH_JOBPOSTING_ROUTE         = "publishJobPosting";
        public const string CLOSE_JOBPOSTING_ROUTE           = "closeJobPosting";
        public const string GET_APPLICATIONS_BY_POSTING_ROUTE = "getApplicationsByPosting";
        #endregion

        #region Routes — Candidate / Application
        public const string GET_ALL_CANDIDATES_PAGING_ROUTE = "getAllCandidatesPagingWithSearch";
        public const string SAVE_CANDIDATE_APPLICATION_ROUTE = "applyToPosting";
        public const string MOVE_APPLICATION_STAGE_ROUTE     = "moveApplicationStage";
        #endregion

        #region Routes — Interview
        public const string GET_INTERVIEWS_BY_APPLICATION_ROUTE = "getInterviewsByApplication";
        public const string SAVE_UPDATE_INTERVIEW_ROUTE         = "createUpdateInterview";
        #endregion

        #region Routes — Offer
        public const string GET_OFFER_BY_ID_ROUTE = "getOfferById";
        public const string SAVE_UPDATE_OFFER_ROUTE = "createUpdateOffer";
        public const string ISSUE_OFFER_ROUTE       = "issueOffer";
        public const string ACCEPT_OFFER_ROUTE       = "acceptOffer";
        public const string DECLINE_OFFER_ROUTE       = "declineOffer";
        #endregion

        #region Messages — Position
        public const string POSITION_CREATE_SUCCESS = "Position created successfully.";
        public const string POSITION_UPDATE_SUCCESS = "Position updated successfully.";
        public const string POSITION_DELETE_SUCCESS = "Position removed successfully.";
        public const string POSITION_DUPLICATE      = "A position with the same title already exists in this department.";
        public const string POSITION_NOT_FOUND      = "Position not found.";
        public const string POSITION_LIST_EMPTY     = "No position records found.";
        public const string POSITION_HAS_REQUISITION = "Cannot delete — requisitions reference this position.";
        #endregion

        #region Messages — Requisition
        public const string REQUISITION_CREATE_SUCCESS = "Requisition created successfully.";
        public const string REQUISITION_UPDATE_SUCCESS = "Requisition updated successfully.";
        public const string REQUISITION_SUBMIT_SUCCESS = "Requisition submitted for approval.";
        public const string REQUISITION_APPROVE_SUCCESS = "Requisition approved.";
        public const string REQUISITION_REJECT_SUCCESS = "Requisition rejected.";
        public const string REQUISITION_NOT_FOUND      = "Requisition not found.";
        public const string REQUISITION_LIST_EMPTY     = "No requisition records found.";
        public const string REQUISITION_FETCH_SUCCESS  = "Requisition data fetched successfully.";
        public const string REQUISITION_BAD_STATE      = "Requisition is not in a valid state for this action.";
        #endregion

        #region Messages — Job Posting
        public const string JOBPOSTING_CREATE_SUCCESS = "Job posting created successfully.";
        public const string JOBPOSTING_UPDATE_SUCCESS = "Job posting updated successfully.";
        public const string JOBPOSTING_PUBLISH_SUCCESS = "Job posting published.";
        public const string JOBPOSTING_CLOSE_SUCCESS  = "Job posting closed.";
        public const string JOBPOSTING_NOT_FOUND      = "Job posting not found.";
        public const string JOBPOSTING_LIST_EMPTY     = "No job postings found.";
        public const string JOBPOSTING_FETCH_SUCCESS  = "Job posting data fetched successfully.";
        public const string JOBPOSTING_REQ_NOT_APPROVED = "The requisition must be approved before a posting can be created.";
        #endregion

        #region Messages — Candidate / Application
        public const string CANDIDATE_LIST_EMPTY      = "No candidate records found.";
        public const string CANDIDATE_FETCH_SUCCESS   = "Candidate data fetched successfully.";
        public const string APPLICATION_CAPTURE_SUCCESS = "Application captured successfully.";
        public const string APPLICATION_DUPLICATE     = "This candidate has already applied to this posting.";
        public const string APPLICATION_STAGE_UPDATED = "Application stage updated.";
        public const string APPLICATION_FETCH_SUCCESS = "Applications fetched successfully.";
        #endregion

        #region Messages — Interview
        public const string INTERVIEW_SAVE_SUCCESS     = "Interview saved successfully.";
        public const string INTERVIEW_COMPLETE_SUCCESS = "Interview outcome recorded.";
        public const string INTERVIEW_FETCH_SUCCESS    = "Interviews fetched successfully.";
        #endregion

        #region Messages — Offer
        public const string OFFER_CREATE_SUCCESS = "Offer created successfully.";
        public const string OFFER_UPDATE_SUCCESS = "Offer updated successfully.";
        public const string OFFER_ISSUE_SUCCESS  = "Offer issued.";
        public const string OFFER_ACCEPT_SUCCESS = "Offer accepted — ready for onboarding.";
        public const string OFFER_DECLINE_SUCCESS = "Offer declined.";
        public const string OFFER_NOT_FOUND      = "Offer not found.";
        public const string OFFER_FETCH_SUCCESS  = "Offer data fetched successfully.";
        public const string OFFER_BAD_STATE      = "Offer is not in a valid state for this action.";
        #endregion
    }
}
