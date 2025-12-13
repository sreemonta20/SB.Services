using Newtonsoft.Json.Serialization;

namespace SBERP.Security.Helper
{
    /// <summary>
    /// It stores all the constants which are currently being used throughout the project.
    /// </summary>
    public class ConstantSupplier
    {
        #region Global Error Messages
        public const string GLOBAL_ERR_AUTH_FAILED_NO_PERMISSION_MSG = "Authentication failed or user does not have permission to access this resource.";
        public const string GLOBAL_ERR_INVALID_INPUT_STATE_MSG = "The request could not be processed due to invalid input or state.";
        public const string GLOBAL_ERR_INVALID_OPERATION_MSG = "The requested operation is invalid or cannot be performed.";
        public const string GLOBAL_ERR_RESOURCE_NOT_FOUND_MSG = "The requested resource was not found.";
        public const string GLOBAL_ERR_UNEXPECTED_MSG = "An unexpected error occurred. Please try again later.";
        #endregion

        #region Common Constants
        public const string SQLSERVER = "SqlServer";
        public const string ORACLE = "Oracle";
        public const string ODBC = "Odbc";
        public const string OLEDB = "Oledb";
        public const string CTRLER_ROUTE_PATH_NAME = "api/[controller]";
        public const string CTRLER_ROUTE_PATH_NAME_VERSION_ONE = "api/v1/[controller]";
        public const string REQUIRED_PARAMETER_NOT_EMPTY = "Required Parameters Should Not Empty Or Null";
        public const string REQ_OR_DATA_NULL = "Request is found null or data not found";
        public const string CORSS_POLICY_NAME = "AllowRedirectOrigin";
        public const string APP_SETTINGS_FILE_NAME = "appsettings.json";
        public const string SECURITY_SQL_DB_CONNECTION_STRING_NAME = "SecurityConectionString";
        public const string AUTHORIZATION_TOKEN_TYPE = "bearer";
        public const string AUTHORIZATION_TOKEN_HEADER_ADD_NAME_01 = "token-expired";
        public const string AUTHORIZATION_TOKEN_HEADER_ADD_VALUE_01 = "true";
        public const string EMAIL_CONFIG_CLASS_KEY = "EmailConfiguration";
        public const string INVALID_CLIENT_REQUEST = "Invalid client request";
        public const string SESSION_EXPIRATION_MSG = "Session expired!. Please login again.";
        public const string LOGGEDIN_USER_CREATED_MSG = "Loggedin user record created successfully.";
        public const string LOGGEDIN_USER_UPDATED_MSG = "Loggedin user record updated successfully.";
        public const string LOGGEDIN_USER_FAILED_MSG = "Creating/Updating loggedin user record failed.";
        public const string UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_SUCCESS_MSG = "Updating login attempt is successful.";
        public const string UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_FAILED_MSG = "Updating login attempt failed.";
        public const string APP_USER_CREATED_MSG = "Application user record created successfully.";
        public const string APP_USER_UPDATED_MSG = "Application user record updated successfully.";
        public const string APP_USER_CREATED_UPDATED_FAILED_MSG = "Creating/updating application user record failed.";
        public const string APP_USER_REFRESH_TOKEN_DURING_LOGIN_SUCCESS_MSG = "Issuing a new refresh token successful.";
        public const string APP_USER_REFRESH_TOKEN_DURING_LOGIN_FAILED_MSG = "Issuing a new refresh token failed.";
        public const string FAILED_MSG = "Failed";
        public const string SUCCESS_MSG = "Success";


        public const string SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME = "v1";
        public const string SWAGGER_SB_API_SERVICE_DOC_TITLE = "SB Services API";
        public const string SWAGGER_SB_API_SERVICE_DOC_DESCRIPTION = "SB Services API Documentation";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_NAME = "Sreemonta Bhowmik ";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_EMAIL = "sreemonta.bhowmik@gmail.com";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_URL = "https://sreemonta.netlify.app/";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_DESC = "Authorization header using the Bearer scheme. Example: {token}";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_NAME = "Authorization";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_SCHEME = "bearer";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID = "Bearer";

        public const string SWAGGER_SB_API_SERVICE_DOC_END_POINT = "/swagger/v1/swagger.json";
        public const string SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME = "SB Services API v1";
        public const string NOT_APPLICABLE = "Not Applicable";

        public const string EMPTY = "Empty";
        #endregion

        #region Application Start Log
        public const string LOG_INFO_APP_START_MSG = "Application is starting";
        public const string LOG_FATAL_APP_FAILED_MSG = "Fatal: Application error";
        public const string LOG_INFO_APPEND_LINE_FIRST = "**********************************************************************";
        public const string LOG_INFO_APPEND_LINE_SECOND_GATEWAY = "**                      SB Services                                **";
        public const string LOG_INFO_APPEND_LINE_THIRD_VERSION = "**                    [Version 1.0.0]                               **";
        public const string LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT = "**  ©2022-2023 Health Care Solutions. All rights reserved           **";
        public const string LOG_INFO_APPEND_LINE_END = "**********************************************************************";
        #endregion

        #region Auth Controller
        public const string POST_AUTH_ROUTE_NAME = "authenticateUser";
        public const string GET_USER_PROFILE_MENU_ROUTE_NAME = "getAppUserProfileMenu";
        public const string REFRESH_TOKEN_ROUTE_NAME = "refreshToken";
        public const string REVOKE_ROUTE_NAME = "revoke";
        #endregion

        #region Auth Methods Execution Log
        //api/v1/Auth/authenticateUser
        public const string LOGIN_STARTED_INFO_MSG = "Login api method started.\n";
        public const string LOGIN_REQ_MSG = "Login api method request is: \n{0}\n";
        public const string LOGIN_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string SERVICE_LOGIN_REQ_MSG = "Authenticate (Auth service) method request is: \n{0}\n";
        public const string SERVICE_LOGIN_FAILED_MSG = "Response is: \n{0}\n";
        public const string SERVICE_LOGIN_RES_MSG = "Authenticate (Auth service) method response is: \n{0}\n";
        public const string LOGIN_RES_MSG = "Login api method response is: \n{0}\n";
        public const string LOGIN_API_EXCEPTION_MSG = "An error occurred while processing your request.";

        //api/v1/Auth/getAppUserProfileMenu
        public const string APP_USER_PROFILE_MENU_STARTED_INFO_MSG = "ProfileMenu api method started.\n";
        public const string APP_USER_PROFILE_MENU_REQ_MSG = "ProfileMenu api method request is: \n{0}\n";
        public const string APP_USER_PROFILE_MENU_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string SERVICE_APP_USER_PROFILE_MENU_REQ_MSG = "ProfileMenu (Auth service) method request is: \n{0}\n";
        public const string SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG = "Response is: \n{0}\n";
        public const string SERVICE_APP_USER_PROFILE_MENU_RES_MSG = "Authenticate (Auth service) method response is: \n{0}\n";
        public const string APP_USER_PROFILE_MENU_RES_MSG = "Login api method response is: \n{0}\n";
        public const string APP_USER_PROFILE_MENU_API_EXCEPTION_MSG = "An error occurred while processing your request.";

        //api/v1/Auth/refreshToken
        public const string REFRESHTOKEN_STARTED_INFO_MSG = "RefreshToken api method started.\n";
        public const string REFRESHTOKEN_REQ_MSG = "RefreshToken api method request is: \n{0}\n";
        public const string REFRESHTOKEN_FAILED_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        //public const string REFRESHTOKEN_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string SERVICE_REFRESHTOKEN_REQ_MSG = "RefreshToken (Auth service) method request is: \n{0}\n";
        public const string SERVICE_REFRESHTOKEN_FAILED_MSG = "RefreshToken Failed (Auth service) : api method response is: \n{0}\n";
        public const string SERVICE_REFRESHTOKEN_RES_MSG = "RefreshToken (Auth service) method response is: \n{0}\n";
        public const string REFRESHTOKEN_RES_MSG = "RefreshToken api method response is: \n{0}\n";
        public const string REFRESH_TOKEN_API_EXCEPTION_MSG = "An error occurred while refreshing the token.";

        //api/v1/Auth/revoke
        public const string REVOKE_STARTED_INFO_MSG = "Revoke api method started.\n";
        public const string REVOKE_REQ_MSG = "Revoke api method request is: \n{0}\n";
        public const string REVOKE_FAILED_MSG = "Failed Message is: \t\t\t{0}\nResponse is: \n{1}\n";
        //public const string REVOKE_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string REVOKE_RES_MSG = "Revoke api method response is: \n{0}\n";
        public const string SERVICE_REVOKE_REQ_MSG = "Revoke (Auth service) method request is: \n{0}\n";
        public const string SERVICE_REVOKE_FAILED_MSG = "Revoke failed (User service) :: api method response is: \n{0}\n";
        public const string SERVICE_REVOKE_RES_MSG = "Revoke (Auth service) method response is: \n{0}\n";
        public const string REVOKE_API_EXCEPTION_MSG = "Error while revoking the token";

        #endregion

        #region Auth Service
        public const string AUTH_FAILED_MSG = "Login failed. Please try again later";
        public const string AUTH_INVALID_CREDENTIAL_MSG = "Invalid username or password.";
        public const string AUTH_SUCCESS_MSG = "Login success!";
        public const string AUTH_FAILED_ATTEMPT_MSG = "Your account was blocked for a {0} minutes, please try again later.";
        public const string INVALID_USER_MSG = "User might be invalid or inactive. Please contact with administrator.";
        public const string AUTH_USER_PROFILE_NOT_FOUND_MSG = "User profile not found. Please contact with administrator.";
        public const string AUTH_FAILED_RETRIEVE_MENU_MSG = "Failed to retrieve user menu. Please contact with administrator.";
        public const string AUTH_FAILED_GENRATE_JWT_MSG = "Failed to generate access token.";
        public const string AUTH_EXCEPTION_MSG = "An error occurred during authentication. Please try again later.";

        public const string APP_USER_PROFILE_MENU_SUCCESS_MSG = "User profile and menu have been retrieved successfully!";
        public const string APP_USER_PROFILE_MENU_SERVICE_EXCEPTION_MSG = "An error occurred during the retrieval of user profile and menu. Please try again later.";
        public const string APP_USER_PROFILE_MENU_INVALID_TOKEN_MSG = "Invalid user token.";

        public const string REFRESHTOKEN_FAILED = "Refreshing token failed. Please try again later";
        public const string REFRESHTOKEN_INVALID_CREDENTIAL = "Invalid refresh token credential";
        public const string REFRESHTOKEN_SUCCESS = "Token refreshed successfully.";
        public const string NULL_TOKEN = "Token is null";
        public const string REFRESHTOKEN_INVALID_EXPIRED_MSG = "Invalid or expired refresh token.";
        public const string REFRESHTOKEN_EXCEPTION_MSG = "An error occurred during authentication. Please try again later.";

        public const string REVOKE_USER_FAILED = "Revoking user failed. Please try again later";
        public const string REVOKE_USER_SUCCESS = "Revoking user success!";
        public const string REVOKE_USER_NOT_FOUND_MSG = "User not found or already deactivated.";
        public const string REVOKE_SUCCESSFUL_MSG = "User logged out successfully. Refresh token revoked.";
        public const string REVOKE_EXCEPTION_MSG = "An error occurred while revoking the token.";
        #endregion

        #region User Controller
        public const string GET_ALL_APP_USER_PROFILE_ROUTE_NAME = "getAllAppUserProfile";
        public const string GET_ALL_USER_PROFILE_PAGING_WITH_SEARCH_TERM_ROUTE_NAME = "getAllAppUserProfilePagingWithSearch";
        public const string GET_APP_USER_PROFILE_ROUTE_NAME = "getAppUserProfileById";
        public const string POST_APP_USER_PROFILE_ROUTE_NAME = "createUpdateAppUserProfile";
        public const string DEL_APP_USER_PROFILE_ROUTE_NAME = "deleteAppUserProfile";
        public const string POST_APP_USER_ROUTE_NAME = "createUpdateAppUser";

        public const string GET_ALL_USER_PROFILE_PAGING_SEARCH_RESULT_SUCCESS_MSG = "User profile list retrieved successfully.";
        public const string GET_ALL_USER_PROFILE_PAGING_SEARCH_RESULT_FAILED_MSG = "Retrieving user profile list failed.";
        public const string GET_ALL_USER_PROFILE_PAGING_SEARCH_RESULT_EMPTY_MSG = "Retrieving user profile list is found empty.";
        #endregion

        #region User Methods Execution Log
        //api/v1/User/createUpdateAppUser
        public const string SAVEUP_APP_USER_STARTED_INFO_MSG = "CreateUpdateAppUser api method started.\n";
        public const string SAVEUP_APP_USER_REQ_MSG = "CreateUpdateAppUser api method request is: \n{0}\n";
        public const string SAVEUP_APP_USER_EXCEPTION_MSG = "CreateUpdateAppUser Exception below:: \n{0}\n";
        public const string SERVICE_SAVEUP_APP_USER_REQ_MSG = "CreateUpdateAppUser (User service) method request is: \n{0}\n";
        public const string SERVICE_SAVEUP_APP_USER_RES_MSG = "CreateUpdateAppUser (User service) method response is: \n{0}\n";
        public const string SAVEUP_APP_USER_RES_MSG = "CreateUpdateAppUser api method response is: \n{0}\n";
        public const string SAVEUP_APP_USER_FAILED_RES_MSG = "CreateUpdateAppUser Failed (User service) : api method response is: \n{0}\n";

        //api/v1/User/getAllAppUserProfile
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_STARTED_INFO_MSG = "GetAllAppUserProfile api method started.\n";
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_REQ_MSG = "GetAllAppUserProfile api method request is: \n{0}\n";
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_EXCEPTION_MSG = "GetAllAppUserProfile Exception below:: \n{0}\n";
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_INNER_EXCEPTION_MSG = "GetAllAppUserProfile Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_REQ_MSG = "GetAllAppUserProfile (User service) method request is: \n{0}\n";
        public const string SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_RES_MSG = "GetAllAppUserProfile (User service) method error response is: \n{0}\n";
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_RES_MSG = "GetAllAppUserProfile api method response is: \n{0}\n";

        //api/v1/User/getAppUserProfileById
        public const string GET_USER_PROFILE_BY_ID_STARTED_INFO_MSG = "GetAppUserProfileById api method started.\n";
        public const string GET_USER_PROFILE_BY_ID_REQ_MSG = "GetAppUserProfileById api method request is: \n{0}\n";
        public const string GET_USER_PROFILE_BY_ID_EXCEPTION_MSG = "GetAppUserProfileById Exception below:: \n{0}\n";
        public const string GET_USER_PROFILE_BY_ID_INNER_EXCEPTION_MSG = "GetAppUserProfileById Inner Exception below:: \n{0}\n";
        public const string SERVICE_GET_USER_PROFILE_BY_ID_REQ_MSG = "GetAppUserProfileById (User service) method request is: \n{0}\n";
        public const string SERVICE_GET_USER_PROFILE_BY_ID_RES_MSG = "GetAppUserProfileById (User service) method error response is: \n{0}\n";
        public const string GET_USER_PROFILE_BY_ID_RES_MSG = "GetAppUserProfileById api method response is: \n{0}\n";

        //api/v1/User/createUpdateAppUserProfile
        public const string SAVEUP_APP_USER_PROFILE_STARTED_INFO_MSG = "CreateUpdateAppUserProfile api method started.\n";
        public const string SAVEUP_APP_USER_PROFILE_REQ_MSG = "CreateUpdateAppUserProfile api method request is: \n{0}\n";
        public const string SAVEUP_APP_USER_PROFILE_EXCEPTION_MSG = "CreateUpdateAppUserProfile Exception below:: \n{0}\n";
        public const string SAVEUP_APP_USER_PROFILE_INNER_EXCEPTION_MSG = "CreateUpdateAppUserProfile Inner Exception below:: \n{0}\n";
        public const string SERVICE_SAVEUP_APP_USER_PROFILE_REQ_MSG = "CreateUpdateAppUserProfile (User service) method request is: \n{0}\n";
        public const string SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG = "CreateUpdateAppUserProfile (User service) method response is: \n{0}\n";
        public const string SAVEUP_APP_USER_PROFILE_RES_MSG = "CreateUpdateAppUserProfile api method response is: \n{0}\n";
        public const string SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG = "CreateUpdateAppUserProfile Failed (User service) : api method response is: \n{0}\n";

        //api/v1/User/deleteAppUserProfile
        public const string DEL_APP_USER_PROFILE_STARTED_INFO_MSG = "DeleteAppUserProfile api method started.\n";
        public const string DEL_APP_USER_PROFILE_REQ_MSG = "DeleteAppUserProfile api method request is: \n{0}\n";
        public const string DEL_APP_USER_PROFILE_EXCEPTION_MSG = "DeleteAppUserProfile Exception below:: \n{0}\n";
        public const string SERVICE_DEL_APP_USER_PROFILE_REQ_MSG = "DeleteAppUserProfile (User service) method request is: \n{0}\n";
        public const string SERVICE_DEL_APP_USER_PROFILE_RES_MSG = "DeleteAppUserProfile (User service) method response is: \n{0}\n";
        public const string DEL_APP_USER_PROFILE_RES_MSG = "DeleteAppUserProfile api method response is: \n{0}\n";
        public const string DEL_APP_USER_PROFILE_FAILED_RES_MSG = "DeleteAppUserProfile Failed (User service) : api method response is: \n{0}\n";

        #endregion

        #region User Service
        public const string ADMIN = "Admin";
        public const string USER = "User";

        public const string GET_APP_USER_PROFILE_LIST_SUCCESS = "Fetching user list successful";
        public const string GET_APP_USER_PROFILE_LIST_FAILED = "Fetching user list failed.";

        public const string GET_APP_USER_PROFILE_SUCCESS = "Fetching user details successful";
        public const string GET_APP_USER_PROFILE_FAILED = "Fetching user details failed.";

        public const string EXIST_APP_USER_PROFILE = "User profile is already exist. Try unique one or activate/update it.";
        public const string CREATE_APP_USER_PROFILE_SAVE_SUCCESS = "Creating user profile success!";
        public const string CREATE_APP_USER_PROFILE_SAVE_FAILED = "Creating user profile failed. Please try again later";
        public const string UPDATE_APP_USER_PROFILE_SUCCESS = "Updating user profile success!";
        public const string UPDATE_APP_USER_PROFILE_FAILED = "Updating user profile failed. Please try again later";
        public const string CREATE_UPDATE_APP_USER_PROFILE_FAILED = "Creating/updating user profile failed. Please try again later";

        public const string DELETE_APP_USER_PROFILE_SUCCESS = "User profile deleted successfully";
        public const string DELETE_APP_USER_PROFILE_FAILED = "Deletion profile of user failed!. Please try again later";

        public const string EXIST_APP_USER = "User is already exist. Try unique username or activate/update it.";
        public const string CREATE_APP_USER_SAVE_SUCCESS = "Creating user success!";
        public const string CREATE_APP_USER_SAVE_FAILED = "Creating user failed. Please try again later";
        public const string UPDATE_APP_USER_SUCCESS = "Updating user success!";
        public const string UPDATE_APP_USER_FAILED = "Updating user failed. Please try again later";
        public const string CREATE_UPDATE_APP_USER_FAILED = "Creating/updating user failed. Please try again later";

        #endregion

        #region Role Menu Controller
        public const string GET_ALL_ROLES_ROUTE_NAME = "getAllAppUserRoles";
        public const string GET_ALL_ROLES_PAGINATION_ROUTE_NAME = "getAllAppUserRolesPagination";
        public const string GET_ROLE_BY_ID_ROUTE_NAME = "getAppUserRolesById";
        public const string POST_SAVE_UPDATE_ROLE_ROUTE_NAME = "createUpdateAppUserRole";
        public const string DELETE_ROLE_ROUTE_NAME = "deleteAppUserRole";
        public const string GET_ALL_MENU_BY_USER_ID_ROUTE_NAME = "getAllAppUserMenuByUserId";
        public const string GET_ALL_USER_MENU_PAGING_WITH_SEARCH_TERM_ROUTE_NAME = "getAllAppUserMenuPagingWithSearch";
        public const string GET_USER_MENU_INITIAL_DATA_ROUTE_NAME = "getAppUserRoleMenuInitialData";
        public const string GET_ALL_PARENT_MENUS_ROUTE_NAME = "getAllParentMenus";

        public const string GETALL_USER_MENU_PAGING_SEARCH_SP_NAME = "SP_GetAllAppUserMenusPagingWithSearch";

        public const string GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_SUCCESS_MSG = "User menu list retrieved successfully.";
        public const string GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_FAILED_MSG = "Retrieving user menu list failed.";
        public const string GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG = "Retrieving user menu list is found empty.";

        public const string POST_SAVE_UPDATE_USER_MENU_ROUTE_NAME = "createUpdateAppUserMenu";
        public const string DELETE_USER_MENU_ROUTE_NAME = "deleteAppUserMenu";

        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_WITH_SEARCH_TERM_ROUTE_NAME = "getAllAppUserRoleMenusPagingWithSearch";

        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_SP_NAME = "SP_GetAllAppUserRoleMenusPagingWithSearch";

        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RESULT_SUCCESS_MSG = "App user role menu list retrieved successfully.";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RESULT_FAILED_MSG = "Retrieving app user role menu list failed.";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG = "Retrieving app user role menu list is found empty.";

        public const string GET_ALL_ACTIVE_ROLES_ROUTE_NAME = "getActiveRoles";
        public const string GET_MENUS_BY_ROLE_ID_ROUTE_NAME = "getMenusByRoleId";

        public const string GET_ALL_USER_ROLE_MENU_PAGING_WITH_SEARCH_ROUTE_NAME = "getRoleMenusPagingWithSearch";
        public const string POST_SAVE_UPDATE_ROLE_MENU_BULK_ROUTE_NAME = "saveUpdateRoleMenuBulk";

        public const string GET_ALL_USER_ROLE_MENU_PAGING_SEARCH_RESULT_SUCCESS_MSG = "Role menu permissions fetched successfully.";
        public const string GET_ALL_USER_ROLE_MENU_PAGING_SEARCH_RESULT_FAILED_MSG = "Retrieving Role menu permissions failed.";
        #endregion

        #region RoleMenu Execution Log
        //api/v1/RoleMenu/getAllAppUserRoles
        public const string GETALLROLES_STARTED_INFO_MSG = "GetAllRoles api method started.\n";
        public const string GETALLROLES_REQ_MSG = "GetAllRoles api method request is: \n{0}\n";
        public const string GETALLROLES_EXCEPTION_MSG = "GetAllRoles Exception below:: \n{0}\n";
        public const string GETALLROLES_INNER_EXCEPTION_MSG = "GetAllRoles Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLROLES_REQ_MSG = "GetAllRolesAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLROLES_RES_MSG = "GetAllRolesAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLROLES_RES_MSG = "GetAllRoles api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAllRolesPagination
        public const string GETALLROLESPAGINATION_STARTED_INFO_MSG = "GetAllRolesPagination api method started.\n";
        public const string GETALLROLESPAGINATION_REQ_MSG = "GetAllRolesPagination api method request is: \n{0}\n";
        public const string GETALLROLESPAGINATION_EXCEPTION_MSG = "GetAllRolesPagination Exception below:: \n{0}\n";
        public const string GETALLROLESPAGINATION_INNER_EXCEPTION_MSG = "GetAllRolesPagination Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLROLESPAGINATION_REQ_MSG = "GetAllRolesPaginationAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLROLESPAGINATION_RES_MSG = "GetAllRolesPaginationAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLROLESPAGINATION_RES_MSG = "GetAllRolesPagination api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAppUserRolesById
        public const string GETROLEBYID_STARTED_INFO_MSG = "getRoleById api method started.\n";
        public const string GETROLEBYID_REQ_MSG = "getRoleById api method request is: \n{0}\n";
        public const string GETROLEBYID_EXCEPTION_MSG = "getRoleById Exception below:: \n{0}\n";
        public const string GETROLEBYID_INNER_EXCEPTION_MSG = "getRoleById Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETROLEBYID_REQ_MSG = "getRoleByIdAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETROLEBYID_RES_MSG = "getRoleByIdAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETROLEBYID_RES_MSG = "getRoleById api method response is: \n{0}\n";

        //api/v1/RoleMenu/createUpdateAppUserRole
        public const string SAVEUPDATEROLE_STARTED_INFO_MSG = "SaveUpdateRole api method started.\n";
        public const string SAVEUPDATEROLE_REQ_MSG = "SaveUpdateRole api method request is: \n{0}\n";
        public const string SAVEUPDATEROLE_EXCEPTION_MSG = "SaveUpdateRole Exception below:: \n{0}\n";
        public const string SAVEUPDATEROLE_INNER_EXCEPTION_MSG = "SaveUpdateRole Inner Exception below:: \n{0}\n";
        public const string SERVICE_SAVEUPDATEROLE_REQ_MSG = "SaveUpdateRoleAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_SAVEUPDATEROLE_RES_MSG = "SaveUpdateRoleAsync (Role Menu service) method response is: \n{0}\n";
        public const string SAVEUPDATEROLE_RES_MSG = "SaveUpdateRole api method response is: \n{0}\n";

        //api/v1/RoleMenu/deleteAppUserRole
        public const string DELETEROLE_STARTED_INFO_MSG = "DeleteRole api method started.\n";
        public const string DELETEROLE_REQ_MSG = "DeleteRole api method request is: \n{0}\n";
        public const string DELETEROLE_EXCEPTION_MSG = "DeleteRole Exception below:: \n{0}\n";
        public const string DELETEROLE_INNER_EXCEPTION_MSG = "DeleteRole Inner Exception below:: \n{0}\n";
        public const string SERVICE_DELETEROLE_REQ_MSG = "DeleteRoleAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_DELETEROLE_RES_MSG = "DeleteRoleAsync (Role Menu service) method response is: \n{0}\n";
        public const string DELETEROLE_RES_MSG = "DeleteRole api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAllAppUserMenuPagingWithSearch
        public const string GETALL_USER_MENU_PAGING_SEARCH_STARTED_INFO_MSG = "GetAllUserMenuPagingWithSearchTerm api method started.\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG = "GetAllUserMenuPagingWithSearchTerm api method request is: \n{0}\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_EXCEPTION_MSG = "GetAllUserMenuPagingWithSearchTerm Exception below:: \n{0}\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_INNER_EXCEPTION_MSG = "GetAllUserMenuPagingWithSearchTerm Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG = "GetAllUserMenuPagingWithSearchAsync (RoleMenu service) method request is: \n{0}\n";
        public const string SERVICE_GETALL_USER_MENU_PAGING_SEARCH_RES_MSG = "GetAllUserMenuPagingWithSearchAsync (RoleMenu service) method response is: \n{0}\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_RES_MSG = "GetAllUserMenuPagingWithSearchTerm api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAllAppUserMenuByUserId
        public const string GETALLMENUBYUSERID_STARTED_INFO_MSG = "GetAllMenuByUserId api method started.\n";
        public const string GETALLMENUBYUSERID_REQ_MSG = "GetAllMenuByUserId api method request is: \n{0}\n";
        public const string GETALLMENUBYUSERID_EXCEPTION_MSG = "GetAllMenuByUserId Exception below:: \n{0}\n";
        public const string GETALLMENUBYUSERID_INNER_EXCEPTION_MSG = "GetAllMenuByUserId Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLMENUBYUSERID_REQ_MSG = "GetAllMenuByUserIdAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLMENUBYUSERID_RES_MSG = "GetAllMenuByUserIdAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLMENUBYUSERID_RES_MSG = "GetAllMenuByUserId api method response is: \n{0}\n";

        //api/v1/RoleMenu/saveUpdateUserMenu
        public const string SAVE_UPDATE_USER_MENU_STARTED_INFO_MSG = "SaveUpdateUserMenu api method started.\n";
        public const string SAVE_UPDATE_USER_MENU_REQ_MSG = "SaveUpdateUserMenu api method request is: \n{0}\n";
        public const string SAVE_UPDATE_USER_MENU_EXCEPTION_MSG = "SaveUpdateUserMenu Exception below:: \n{0}\n";
        public const string SAVE_UPDATE_USER_MENU_INNER_EXCEPTION_MSG = "SaveUpdateUserMenu Inner Exception below:: \n{0}\n";
        public const string SERVICE_SAVE_UPDATE_USER_MENU_REQ_MSG = "SaveUpdateUserMenuAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_SAVE_UPDATE_USER_MENU_RES_MSG = "SaveUpdateUserMenuAsync (Role Menu service) method response is: \n{0}\n";
        public const string SAVE_UPDATE_USER_MENU_RES_MSG = "SaveUpdateUserMenu api method response is: \n{0}\n";

        //api/v1/RoleMenu/deleteUserMenu
        public const string DELETE_APP_USER_MENU_STARTED_INFO_MSG = "DeleteUserMenu api method started.\n";
        public const string DELETE_APP_USER_MENU_REQ_MSG = "DeleteUserMenu api method request is: \n{0}\n";
        public const string DELETE_APP_USER_MENU_EXCEPTION_MSG = "DeleteUserMenu Exception below:: \n{0}\n";
        public const string DELETE_APP_USER_MENU_INNER_EXCEPTION_MSG = "DeleteUserMenu Inner Exception below:: \n{0}\n";
        public const string SERVICE_DELETE_APP_USER_MENU_REQ_MSG = "DeleteUserMenuAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_DELETE_APP_USER_MENU_RES_MSG = "DeleteUserMenuAsync (Role Menu service) method response is: \n{0}\n";
        public const string DELETE_APP_USER_MENU_RES_MSG = "DeleteUserMenu api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAllParentMenus
        public const string GET_ALL_PARENT_MENUS_STARTED_INFO_MSG = "GetAllParentMenus api method started.\n";
        public const string GET_ALL_PARENT_MENUS_REQ_MSG = "GetAllParentMenus api method request is: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_EXCEPTION_MSG = "GetAllParentMenus Exception below:: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_INNER_EXCEPTION_MSG = "GetAllParentMenus Inner Exception below:: \n{0}\n";
        public const string SERVICE_GET_ALL_PARENT_MENUS_REQ_MSG = "GetAllParentMenusAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GET_ALL_PARENT_MENUS_RES_MSG = "GetAllParentMenusAsync (Role Menu service) method response is: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_RES_MSG = "GetAllParentMenus api method response is: \n{0}\n";

        //api/v1/RoleMenu/getAppUserRoleMenuInitialData
        public const string GETUSERMENUINITIALDATA_STARTED_INFO_MSG = "GetUserMenuInitialData api method started.\n";
        public const string GETUSERMENUINITIALDATA_REQ_MSG = "GetUserMenuInitialData api method request is: \n{0}\n";
        public const string GETUSERMENUINITIALDATA_EXCEPTION_MSG = "GetUserMenuInitialData Exception below:: \n{0}\n";
        public const string GETUSERMENUINITIALDATA_INNER_EXCEPTION_MSG = "GetUserMenuInitialData Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETUSERMENUINITIALDATA_REQ_MSG = "GetUserMenuInitialDataAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETUSERMENUINITIALDATA_RES_MSG = "GetUserMenuInitialDataAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETUSERMENUINITIALDATA_RES_MSG = "GetUserMenuInitialData api method response is: \n{0}\n";

        //api/v1/RoleMenu/getMenusByRoleId
        public const string GETACTIVEMENUSBYROLEID_STARTED_INFO_MSG = "GetActiveMenusByRoleId api method started.\n";
        public const string GETACTIVEMENUSBYROLEID_REQ_MSG = "GetActiveMenusByRoleId api method request is: \n{0}\n";
        public const string GETACTIVEMENUSBYROLEID_EXCEPTION_MSG = "GetActiveMenusByRoleId Exception below:: \n{0}\n";
        public const string GETACTIVEMENUSBYROLEID_INNER_EXCEPTION_MSG = "GetActiveMenusByRoleId Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETACTIVEMENUSBYROLEID_REQ_MSG = "GetActiveMenusByRoleId (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETACTIVEMENUSBYROLEID_RES_MSG = "GetActiveMenusByRoleId (Role Menu service) method response is: \n{0}\n";
        public const string GETACTIVEMENUSBYROLEID_RES_MSG = "GetActiveMenusByRoleId api method response is: \n{0}\n";

        //api/v1/RoleMenu/getRoleMenusPagingWithSearch
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_STARTED_INFO_MSG = "GetRoleMenusPagingWithSearch api method started.\n";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_REQ_MSG = "GetRoleMenusPagingWithSearch api method request is: \n{0}\n";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_EXCEPTION_MSG = "GetRoleMenusPagingWithSearch Exception below:: \n{0}\n";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_INNER_EXCEPTION_MSG = "GetRoleMenusPagingWithSearch Inner Exception below:: \n{0}\n";
        public const string SERVICE_GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_REQ_MSG = "GetRoleMenusPagingWithSearchAsync (RoleMenu service) method request is: \n{0}\n";
        public const string SERVICE_GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RES_MSG = "GetRoleMenusPagingWithSearchAsync (RoleMenu service) method response is: \n{0}\n";
        public const string GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RES_MSG = "GetRoleMenusPagingWithSearch api method response is: \n{0}\n";

        //api/v1/RoleMenu/saveUpdateRoleMenuBulk
        public const string SAVE_UPDATE_APP_USER_ROLE_MENU_STARTED_INFO_MSG = "SaveUpdateRoleMenuBulk api method started.\n";
        public const string SAVE_UPDATE_APP_USER_ROLE_MENU_REQ_MSG = "SaveUpdateRoleMenuBulk api method request is: \n{0}\n";
        public const string SAVE_UPDATE_APP_USER_ROLE_MENU_EXCEPTION_MSG = "SaveUpdateRoleMenuBulk Exception below:: \n{0}\n";
        public const string SAVE_UPDATE_APP_USER_ROLE_MENU_INNER_EXCEPTION_MSG = "SaveUpdateRoleMenuBulk Inner Exception below:: \n{0}\n";
        public const string SERVICE_SAVE_UPDATE_APP_USER_ROLE_MENU_REQ_MSG = "SaveUpdateRoleMenuBulkAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_SAVE_UPDATE_APP_USER_ROLE_MENU_RES_MSG = "SaveUpdateRoleMenuBulkAsync (Role Menu service) method response is: \n{0}\n";
        public const string SAVE_UPDATE_APP_USER_ROLE_MENU_RES_MSG = "SaveUpdateRoleMenuBulk api method response is: \n{0}\n";
        #endregion

        #region Role Menu Service
        public const string SAVE_KEY = "Save";
        public const string UPDATE_KEY = "Update";
        public const string ERROR_DELETE_MSG = "We are experiencing a problem. {0} cannot be removed at this moment.";
        public const string NO_ROLE_DATA = "No user role data found";
        public const string SUCCESS_ROLE_DATA = "Fething Role data successful!";
        public const string NO_MENU_DATA = "No menu data found";
        public const string SUCCESS_MENU_DATA = "Fetching menu data successful!";
        public const string NO_PARENT_MENU_DATA = "No parent menu found";
        public const string SUCCESS_PARENT_MENU_DATA = "Fething parent menu successful!";
        public const string GET_ALL_ROLES_PAGINATION_FOUND = "List of Roles retrieved successfully.";
        public const string GET_ALL_ROLES_PAGINATION_NOT_FOUND = "Empty roles.";
        public const string EXIST_ROLE = "There is a role with same name. Activate it or try different.";
        public const string USER_ROLE_SAVE_SUCCESS = "saving user role success!";
        public const string USER_ROLE_UPDATE_SUCCESS = "Updating user role success!";
        public const string NOT_EXIST_ROLE = "There is no role with such name.";
        public const string DELETE_ROLE_SUCCESS = "Role is successfully removed";
        public const string GET_APP_USER_ROLE_SUCCESS = "Fetching app user role details successful";
        public const string GET_APP_USER_ROLE_FAILED = "Fetching app user role details failed.";
        public const string NO_USER_MENU_FORM_INITIAL_DATA = "User menu form initialized data not found. You can proceed.";
        public const string SUCCESS_LOAD_USER_MENU_FORM_INITIAL_DATA = "User menu form initialized data retieved successfully.";
        public const string EXIST_USER_MENU_WITH_SAME_NAME = "There is a user menu with same name. Activate it or try different.";
        public const string EXIST_USER_MENU = "User menu exist. Activate it or try different.";
        public const string USER_MENU_SAVE_SUCCESS = "User menu is created successfully!";
        public const string USER_MENU_UPDATE_SUCCESS = "User menu is updated successfully!";
        public const string NOT_EXIST_USER_MENU = "There is no such user menu exist.";
        public const string DELETE_USER_MENU_SUCCESS = "User menu is successfully removed";
        public const string INVALID_USERNAME_MSG = "Invalid request: Username cannot be null or empty";

        public const string GET_USER_MENU_INITIAL_DATA_SP_NAME = "SP_GetAppUserRoleMenuInitialData";
        public const string GET_GET_ALL_MENU_BY_USER_ID_SP_NAME = "SP_GetAllAppUserMenusByUserId";
        public const string POST_SAVE_UPDATE_USER_MENU_SP_NAME = "SP_CreateUpdateAppUserMenu";
        public const string DELETE_USER_MENU_SP_NAME = "SP_DeleteAppUserMenu";

        public const string GET_ALL_ROLE_MENUS_BY_ROLE_ID_PAGING_SEARCH_SP_NAME = "SP_GetRoleMenusPagingWithSearch";
        public const string POST_SAVE_UPDATE_ROLE_MENU_BULK_SP_NAME = "SP_SaveUpdateRoleMenuInBulk";


        public const string DELETE_APP_USER_MENU_SUCCESS = "User menu is successfully removed";
        public const string EXIST_BUT_DEACTIVATED_APP_USER_MENU = "The user menu is already deactivated.";
        public const string NOT_EXIST_APP_USER_MENU = "There is no user menu with such name.";
        public const string DELETE_APP_USER_MENU_BUT_EXIST_ROLE_MENU = "This User menu already used for a role";
        public const string DELETE_APP_USER_MENU_INACTIVATED_SUCCESS = "User menu is successfully inactivated";
        public const string DELETE_APP_USER_MENU_REMOVED_SUCCESS = "User menu is successfully removed";

        public const string NO_ACTIVE_MENU_BY_ROLE_DATA = "No active menu data found for the selected role";
        public const string SUCCESS_ACTIVE_MENU_BY_ROLE_DATA = "Fething menu data successful!";

        public const string USER_ROLE_MENU_CREATE_UPDATE_SUCCESS = "User role menu permission saved successfully!";
        public const string USER_ROLE_MENU_CREATE_UPDATE_FAILED = "Saving user role menu permission failed!";

        #endregion

        #region Data Analytics Controller

        #endregion

        #region Data Analytics Service

        #endregion

        #region SP
        public const string GETALL_USER_PROFILE_PAGING_SEARCH_SP_NAME = "SP_GetAllAppUserProfilesPagingWithSearch";
        public const string GET_USER_PROFILE_BY_ID_SP_NAME = "SP_GetAppUserProfileById";
        public const string POST_SAVE_UPDATE_APP_USER_PROFILE_SP_NAME = "SP_SaveUpdateAppUserProfile";
        public const string POST_SAVE_UPDATE_APP_USER_SP_NAME = "SP_SaveUpdateAppUser";
        public const string DELETE_USER_SP_NAME = "SP_DeleteAppUserProfile";
        #endregion

        #region Query Identifier
        public const string GET_ACTIVE_ROLES = "GET_ACTIVE_ROLES";
        public const string GET_ACTIVE_MENUS_BY_ROLE_ID = "GET_ACTIVE_MENUS_BY_ROLE_ID";
        #endregion

    }
}
