namespace SBERP.Shared.Authorization
{
    /// <summary>
    /// The menu screens that permissions are granted on: AppUserMenus.RouteLink without "/business/",
    /// in lower case. A role's rights on these come from AppUserRoleMenus, the same rows the Angular
    /// guard and the buttons of each screen use, so the server and the UI always agree.
    /// </summary>
    public static class Screens
    {
        public const string Home = "home";
        public const string AppSettings = "appsettings";

        public const string Roles = "security/appuserrole";
        public const string Profiles = "security/appuserprofile";
        public const string Menus = "security/appusermenu";
        public const string RoleMenus = "security/appuserrolemenu";

        private const string Hr = "humanresources/";

        public const string Company = Hr + "corehr/company";
        public const string Branch = Hr + "corehr/branch";
        public const string Department = Hr + "corehr/department";
        public const string Designation = Hr + "corehr/designation";
        public const string Employee = Hr + "corehr/employee";
        public const string HrSettings = Hr + "corehr/settings";

        public const string CostCenter = Hr + "orgsetup/cost-center";
        public const string HolidayCalendar = Hr + "orgsetup/holiday-calendar";
        public const string JobGrade = Hr + "orgsetup/job-grade";
        public const string Position = Hr + "orgsetup/position";

        public const string Requisition = Hr + "recruitment/requisition";
        public const string JobPosting = Hr + "recruitment/job-posting";
        public const string Pipeline = Hr + "recruitment/pipeline";

        public const string Onboarding = Hr + "onboarding/list";

        public const string Shift = Hr + "attendance/shift";
        public const string Overtime = Hr + "attendance/overtime";
        public const string AttendanceSummary = Hr + "attendance/summary";
        public const string Punches = Hr + "attendance/punches";
        public const string BiometricRegistrations = Hr + "attendance/biometric-registrations";
        public const string Devices = Hr + "attendance/devices";
        public const string Zones = Hr + "attendance/zones";

        public const string LeaveApplications = Hr + "leave/applications";
        public const string LeavePolicies = Hr + "leave/policies";
        public const string LeaveTypes = Hr + "leave/types";

        public const string PayComponents = Hr + "payroll/components";
        public const string SalaryStructures = Hr + "payroll/salary-structures";
        public const string PayrollRuns = Hr + "payroll/runs";
        public const string Loans = Hr + "payroll/loans";

        public const string AppraisalCycles = Hr + "performance/cycles";
        public const string Competencies = Hr + "performance/competencies";
        public const string Goals = Hr + "performance/goals";
        public const string Reviews = Hr + "performance/reviews";

        public const string Separation = Hr + "separation";
    }
}
