namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Performance
        public const string SP_GET_ALL_APPRAISAL_CYCLES_PAGING = "SP_GetAllAppraisalCyclesPagingWithSearch";
        public const string SP_GET_ALL_GOALS_PAGING            = "SP_GetAllGoalsPagingWithSearch";
        public const string SP_GET_ALL_REVIEWS_PAGING          = "SP_GetAllReviewsPagingWithSearch";
        public const string SP_GET_REVIEW_DETAIL               = "SP_GetReviewDetail";
        public const string SP_INITIATE_REVIEWS_FOR_CYCLE       = "SP_InitiateReviewsForCycle";
        public const string SP_SAVE_REVIEW_RATINGS             = "SP_SaveReviewRatings";
        public const string SP_SET_REVIEW_STATUS               = "SP_SetReviewStatus";
        public const string SP_SET_APPRAISAL_CYCLE_STATUS       = "SP_SetAppraisalCycleStatus";
        #endregion

        #region Routes — Performance
        public const string GET_ALL_APPRAISAL_CYCLES_PAGING_ROUTE = "getAllAppraisalCyclesPagingWithSearch";
        public const string CREATE_UPDATE_APPRAISAL_CYCLE_ROUTE   = "createUpdateAppraisalCycle";
        public const string DELETE_APPRAISAL_CYCLE_ROUTE          = "deleteAppraisalCycle";
        public const string SET_CYCLE_STATUS_ROUTE                = "setAppraisalCycleStatus";
        public const string INITIATE_REVIEWS_ROUTE                = "initiateReviews";

        public const string GET_ALL_COMPETENCIES_PAGING_ROUTE     = "getAllCompetenciesPagingWithSearch";
        public const string GET_COMPETENCIES_LOOKUP_ROUTE         = "getCompetenciesLookup";
        public const string CREATE_UPDATE_COMPETENCY_ROUTE        = "createUpdateCompetency";
        public const string DELETE_COMPETENCY_ROUTE               = "deleteCompetency";

        public const string GET_ALL_GOALS_PAGING_ROUTE            = "getAllGoalsPagingWithSearch";
        public const string CREATE_UPDATE_GOAL_ROUTE              = "createUpdateGoal";
        public const string UPDATE_GOAL_PROGRESS_ROUTE            = "updateGoalProgress";
        public const string DELETE_GOAL_ROUTE                     = "deleteGoal";
        public const string GET_CYCLES_LOOKUP_ROUTE               = "getAppraisalCyclesLookup";

        public const string GET_ALL_REVIEWS_PAGING_ROUTE          = "getAllReviewsPagingWithSearch";
        public const string GET_REVIEW_DETAIL_ROUTE               = "getReviewDetail";
        public const string SAVE_REVIEW_RATINGS_ROUTE             = "saveReviewRatings";
        public const string SET_REVIEW_STATUS_ROUTE               = "setReviewStatus";
        #endregion

        #region Messages — Performance
        public const string CYCLE_LIST_EMPTY       = "No appraisal cycles found.";
        public const string CYCLE_FETCH_SUCCESS    = "Appraisal cycle data fetched successfully.";
        public const string COMPETENCY_LIST_EMPTY  = "No competencies found.";
        public const string COMPETENCY_FETCH       = "Competency data fetched successfully.";
        public const string GOAL_LIST_EMPTY        = "No goals found.";
        public const string GOAL_FETCH             = "Goal data fetched successfully.";
        public const string REVIEW_LIST_EMPTY      = "No reviews found.";
        public const string REVIEW_FETCH           = "Review data fetched successfully.";
        public const string CYCLE_IN_USE           = "Cycle has goals or reviews and can't be removed.";
        public const string COMPETENCY_IN_USE      = "Competency is in use and can't be removed.";
        #endregion
    }
}
