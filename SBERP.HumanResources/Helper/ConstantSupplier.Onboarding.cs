namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Onboarding
        public const string SP_ONBOARD_EMPLOYEE_FROM_OFFER        = "SP_OnboardEmployeeFromOffer";
        public const string SP_GET_ALL_ONBOARDINGS_PAGING         = "SP_GetAllOnboardingsPagingWithSearch";
        public const string SP_GET_ONBOARDING_TASKS               = "SP_GetOnboardingTasks";
        public const string SP_GET_ACCEPTED_OFFERS_AWAITING_ONB   = "SP_GetAcceptedOffersAwaitingOnboarding";
        #endregion

        #region Routes — Onboarding
        public const string GET_ALL_ONBOARDINGS_PAGING_ROUTE = "getAllOnboardingsPagingWithSearch";
        public const string GET_ACCEPTED_OFFERS_ROUTE        = "getAcceptedOffersAwaitingOnboarding";
        public const string START_ONBOARDING_ROUTE           = "startOnboarding";
        public const string GET_ONBOARDING_TASKS_ROUTE       = "getOnboardingTasks";
        public const string UPDATE_ONBOARDING_TASK_ROUTE     = "updateOnboardingTask";
        public const string ADD_ONBOARDING_TASK_ROUTE        = "addOnboardingTask";
        public const string COMPLETE_ONBOARDING_ROUTE        = "completeOnboarding";
        public const string CANCEL_ONBOARDING_ROUTE          = "cancelOnboarding";
        #endregion

        #region Messages — Onboarding
        public const string ONBOARDING_LIST_EMPTY        = "No onboarding records found.";
        public const string ONBOARDING_FETCH_SUCCESS     = "Onboarding data fetched successfully.";
        public const string ONBOARDING_START_SUCCESS     = "Employee onboarded; account creation has been queued.";
        public const string ONBOARDING_TASK_UPDATED      = "Checklist task updated.";
        public const string ONBOARDING_TASK_ADDED        = "Checklist task added.";
        public const string ONBOARDING_COMPLETE_SUCCESS  = "Onboarding marked complete.";
        public const string ONBOARDING_CANCEL_SUCCESS    = "Onboarding cancelled.";
        public const string ONBOARDING_NOT_FOUND         = "Onboarding not found.";
        public const string ONBOARDING_TASKS_FETCH       = "Checklist fetched successfully.";
        public const string ONBOARDING_OFFERS_FETCH      = "Accepted offers fetched successfully.";
        public const string ONBOARDING_BAD_STATE         = "Onboarding is not in a valid state for this action.";
        #endregion
    }
}
