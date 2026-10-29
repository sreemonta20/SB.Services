namespace SBERP.HumanResources.Helper
{
    /// <summary>
    /// Organization Setup (Module 0) constants that are not already defined for
    /// Company/Branch (core file) or Position (Recruitment partial).
    /// </summary>
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Organization Setup
        public const string SP_GET_ALL_COSTCENTERS_PAGING      = "SP_GetAllCostCentersPagingWithSearch";
        public const string SP_GET_ALL_JOBGRADES_PAGING        = "SP_GetAllJobGradesPagingWithSearch";
        public const string SP_GET_ALL_HOLIDAYCALENDARS_PAGING = "SP_GetAllHolidayCalendarsPagingWithSearch";
        public const string SP_GET_HOLIDAY_DATES_BY_CALENDAR   = "SP_GetHolidayDatesByCalendar";
        public const string SP_SET_POSITION_FILL               = "SP_SetPositionFill";
        public const string SP_SET_POSITION_STATUS             = "SP_SetPositionStatus";
        public const string SP_GET_POSITION_ORG_TREE           = "SP_GetPositionOrgTree";
        #endregion

        #region Routes — Cost Center
        public const string GET_ALL_COSTCENTERS_ROUTE        = "getAllCostCenters";
        public const string GET_ALL_COSTCENTERS_PAGING_ROUTE = "getAllCostCentersPagingWithSearch";
        public const string CREATE_UPDATE_COSTCENTER_ROUTE   = "createUpdateCostCenter";
        public const string DELETE_COSTCENTER_ROUTE          = "deleteCostCenter";
        #endregion

        #region Routes — Job Grade
        public const string GET_ALL_JOBGRADES_ROUTE        = "getAllJobGrades";
        public const string GET_ALL_JOBGRADES_PAGING_ROUTE = "getAllJobGradesPagingWithSearch";
        public const string CREATE_UPDATE_JOBGRADE_ROUTE   = "createUpdateJobGrade";
        public const string DELETE_JOBGRADE_ROUTE          = "deleteJobGrade";
        #endregion

        #region Routes — Holiday Calendar
        public const string GET_ALL_HOLIDAYCALENDARS_ROUTE        = "getAllHolidayCalendars";
        public const string GET_ALL_HOLIDAYCALENDARS_PAGING_ROUTE = "getAllHolidayCalendarsPagingWithSearch";
        public const string CREATE_UPDATE_HOLIDAYCALENDAR_ROUTE   = "createUpdateHolidayCalendar";
        public const string DELETE_HOLIDAYCALENDAR_ROUTE          = "deleteHolidayCalendar";
        public const string GET_HOLIDAY_DATES_ROUTE               = "getHolidayDatesByCalendar";
        public const string CREATE_UPDATE_HOLIDAY_DATE_ROUTE      = "createUpdateHolidayDate";
        public const string DELETE_HOLIDAY_DATE_ROUTE             = "deleteHolidayDate";
        #endregion

        #region Routes — Position (org-setup extras)
        public const string SET_POSITION_FILL_ROUTE     = "setPositionFill";
        public const string SET_POSITION_STATUS_ROUTE   = "setPositionStatus";
        public const string GET_POSITION_ORG_TREE_ROUTE = "getPositionOrgTree";
        #endregion

        #region Messages — Organization Setup
        public const string ORG_LIST_EMPTY = "No records found.";
        public const string ORG_FETCH      = "Fetched successfully.";
        public const string COSTCENTER_SAVE_SUCCESS   = "Cost center saved successfully.";
        public const string COSTCENTER_DELETE_SUCCESS = "Cost center removed successfully.";
        public const string COSTCENTER_NOT_FOUND      = "Cost center not found.";
        public const string COSTCENTER_DUPLICATE      = "A cost center with the same name already exists.";
        public const string JOBGRADE_SAVE_SUCCESS     = "Job grade saved successfully.";
        public const string JOBGRADE_DELETE_SUCCESS   = "Job grade removed successfully.";
        public const string JOBGRADE_NOT_FOUND        = "Job grade not found.";
        public const string JOBGRADE_DUPLICATE        = "A job grade with the same name or level already exists.";
        public const string CALENDAR_SAVE_SUCCESS     = "Holiday calendar saved successfully.";
        public const string CALENDAR_DELETE_SUCCESS   = "Holiday calendar removed successfully.";
        public const string CALENDAR_NOT_FOUND        = "Holiday calendar not found.";
        public const string HOLIDAY_SAVE_SUCCESS      = "Holiday saved successfully.";
        public const string HOLIDAY_DELETE_SUCCESS    = "Holiday removed successfully.";
        public const string HOLIDAY_NOT_FOUND         = "Holiday not found.";
        public const string CODE_PREFIX_COSTCENTER = "CC";
        public const string CODE_PREFIX_JOBGRADE   = "GR";
        public const string CODE_PREFIX_CALENDAR   = "HC";
        public const string CODE_PREFIX_POSITION   = "POS";
        #endregion
    }
}
