namespace SB.Security.Helper
{
    /// <summary>
    /// It stores all the constants which are currently being used throughout the project.
    /// </summary>
    public class ConstantSupplier
    {
        #region Common Constants
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

        public const string SWAGGER_SB_API_SERVICE_DOC_VERSION_NAME = "v1";
        public const string SWAGGER_SB_API_SERVICE_DOC_TITLE = "SB Services API";
        public const string SWAGGER_SB_API_SERVICE_DOC_DESCRIPTION = "SB Services API";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_NAME = "Healthcare Solutions ";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_EMAIL = "info@sb.com";
        public const string SWAGGER_SB_API_SERVICE_DOC_CONTACT_URL = "https://www.sb.ae/";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_DESC = "Authorization header using the Bearer scheme. Example: {token}";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_NAME = "Authorization";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_SCHEME = "bearer";
        public const string SWAGGER_SB_API_SERVICE_DOC_SCHEME_REF_ID = "Bearer";

        public const string SWAGGER_SB_API_SERVICE_DOC_END_POINT = "/swagger/v1/swagger.json";
        public const string SWAGGER_SB_API_SERVICE_DOC_END_POINT_NAME = "SB Services API v1";
        public const string NOT_APPLICABLE = "Not Applicable";
        #endregion

        #region Serilog Related
        public const string LOG_INFO_APP_START_MSG = "Application is starting";
        public const string LOG_FATAL_APP_FAILED_MSG = "Fatal: Application error";
        public const string LOG_INFO_APPEND_LINE_FIRST = "**********************************************************************";
        public const string LOG_INFO_APPEND_LINE_SECOND_GATEWAY = "**                      SB Services                                **";
        public const string LOG_INFO_APPEND_LINE_THIRD_VERSION = "**                    [Version 1.0.0]                               **";
        public const string LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT = "**  ©2022-2023 Health Care Solutions. All rights reserved           **";
        public const string LOG_INFO_APPEND_LINE_END = "**********************************************************************";

        //api/User/login
        public const string LOGIN_STARTED_INFO_MSG = "Login api method started.\n";
        public const string LOGIN_REQ_MSG = "Login api method request is: \n{0}\n";
        public const string LOGIN_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string SERVICE_LOGIN_REQ_MSG = "Authenticate (User service) method request is: \n{0}\n";
        public const string SERVICE_LOGIN_FAILED_MSG = "Response is: \n{0}\n";
        public const string SERVICE_LOGIN_RES_MSG = "Authenticate (User service) method response is: \n{0}\n";
        public const string LOGIN_RES_MSG = "Login api method response is: \n{0}\n";

        //api/User/refreshtoken
        public const string REFRESHTOKEN_STARTED_INFO_MSG = "RefreshToken api method started.\n";
        public const string REFRESHTOKEN_REQ_MSG = "RefreshToken api method request is: \n{0}\n";
        public const string REFRESHTOKEN_FAILED_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string REFRESHTOKEN_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string SERVICE_REFRESHTOKEN_REQ_MSG = "RefreshToken (User service) method request is: \n{0}\n";
        public const string SERVICE_REFRESHTOKEN_FAILED_MSG = "RefreshToken Failed (User service) : api method response is: \n{0}\n";
        public const string SERVICE_REFRESHTOKEN_RES_MSG = "RefreshToken (User service) method response is: \n{0}\n";
        public const string REFRESHTOKEN_RES_MSG = "RefreshToken api method response is: \n{0}\n";

        //api/User/revoke
        public const string REVOKE_STARTED_INFO_MSG = "Revoke api method started.\n";
        public const string REVOKE_REQ_MSG = "Revoke api method request is: \n{0}\n";
        public const string REVOKE_FAILED_MSG = "Failed Message is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string REVOKE_EXCEPTION_MSG = "Exception is: \t\t\t{0}\nResponse is: \n{1}\n";
        public const string REVOKE_RES_MSG = "Revoke api method response is: \n{0}\n";
        public const string SERVICE_REVOKE_REQ_MSG = "Revoke (User service) method request is: \n{0}\n";
        public const string SERVICE_REVOKE_FAILED_MSG = "Revoke failed (User service) :: api method response is: \n{0}\n";
        public const string SERVICE_REVOKE_RES_MSG = "Revoke (User service) method response is: \n{0}\n";
        

        //api/User/getUserbyId
        public const string GETBYID_STARTED_INFO_MSG = "GetUserbyId api method started.\n";
        public const string GETBYID_REQ_MSG = "GetUserbyId api method request is: \n{0}\n";
        public const string GETBYID_EXCEPTION_MSG = "GetUserbyId Exception below:: \n{0}\n";
        public const string SERVICE_GETBYID_REQ_MSG = "GetUserbyId (User service) method request is: \n{0}\n";
        public const string SERVICE_GETBYID_RES_MSG = "GetUserbyId (User service) method error response is: \n{0}\n";
        public const string GETBYID_RES_MSG = "GetUserbyId api method response is: \n{0}\n";

        //api/User/getAllUsers
        public const string GETALL_STARTED_INFO_MSG = "GetAllUsers api method started.\n";
        public const string GETALL_REQ_MSG = "GetAllUsers api method request is: \n{0}\n";
        public const string GETALL_EXCEPTION_MSG = "GetAllUsers Exception below:: \n{0}\n";
        public const string SERVICE_GETALL_REQ_MSG = "GetAllUsers (User service) method request is: \n{0}\n";
        public const string SERVICE_GETALL_RES_MSG = "GetAllUsers (User service) method error response is: \n{0}\n";
        public const string GETALL_RES_MSG = "GetAllUsers api method response is: \n{0}\n";


        //api/User/registerUser
        public const string SAVEUP_STARTED_INFO_MSG = "RegisterUser api method started.\n";
        public const string SAVEUP_REQ_MSG = "RegisterUser api method request is: \n{0}\n";
        public const string SAVEUP_EXCEPTION_MSG = "RegisterUser Exception below:: \n{0}\n";
        public const string SERVICE_SAVEUP_REQ_MSG = "RegisterUser (User service) method request is: \n{0}\n";
        public const string SERVICE_SAVEUP_RES_MSG = "RegisterUser (User service) method response is: \n{0}\n";
        public const string SAVEUP_RES_MSG = "RegisterUser api method response is: \n{0}\n";
        public const string SAVEUP_FAILED_RES_MSG = "RegisterUser Failed (User service) : api method response is: \n{0}\n";

        //api/User/deleteUser
        public const string DELUSER_STARTED_INFO_MSG = "DeleteUser api method started.\n";
        public const string DELUSER_REQ_MSG = "DeleteUser api method request is: \n{0}\n";
        public const string DELUSER_EXCEPTION_MSG = "DeleteUser Exception below:: \n{0}\n";
        public const string SERVICE_DELUSER_REQ_MSG = "DeleteUser (User service) method request is: \n{0}\n";
        public const string SERVICE_DELUSER_RES_MSG = "DeleteUser (User service) method response is: \n{0}\n";
        public const string DELUSER_RES_MSG = "DeleteUser api method response is: \n{0}\n";
        public const string DELUSER_FAILED_RES_MSG = "DeleteUser Failed (User service) : api method response is: \n{0}\n";


        //api/RoleMenu/getAllRoles
        public const string GETALLROLES_STARTED_INFO_MSG = "GetAllRoles api method started.\n";
        public const string GETALLROLES_REQ_MSG = "GetAllRoles api method request is: \n{0}\n";
        public const string GETALLROLES_EXCEPTION_MSG = "GetAllRoles Exception below:: \n{0}\n";
        public const string GETALLROLES_INNER_EXCEPTION_MSG = "GetAllRoles Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLROLES_REQ_MSG = "GetAllRolesAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLROLES_RES_MSG = "GetAllRolesAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLROLES_RES_MSG = "GetAllRoles api method response is: \n{0}\n";

        //api/RoleMenu/getAllRolesPagination
        public const string GETALLROLESPAGINATION_STARTED_INFO_MSG = "GetAllRolesPagination api method started.\n";
        public const string GETALLROLESPAGINATION_REQ_MSG = "GetAllRolesPagination api method request is: \n{0}\n";
        public const string GETALLROLESPAGINATION_EXCEPTION_MSG = "GetAllRolesPagination Exception below:: \n{0}\n";
        public const string GETALLROLESPAGINATION_INNER_EXCEPTION_MSG = "GetAllRolesPagination Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLROLESPAGINATION_REQ_MSG = "GetAllRolesPaginationAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLROLESPAGINATION_RES_MSG = "GetAllRolesPaginationAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLROLESPAGINATION_RES_MSG = "GetAllRolesPagination api method response is: \n{0}\n";

        //api/RoleMenu/getRoleById
        public const string GETROLEBYID_STARTED_INFO_MSG = "getRoleById api method started.\n";
        public const string GETROLEBYID_REQ_MSG = "getRoleById api method request is: \n{0}\n";
        public const string GETROLEBYID_EXCEPTION_MSG = "getRoleById Exception below:: \n{0}\n";
        public const string GETROLEBYID_INNER_EXCEPTION_MSG = "getRoleById Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETROLEBYID_REQ_MSG = "getRoleByIdAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETROLEBYID_RES_MSG = "getRoleByIdAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETROLEBYID_RES_MSG = "getRoleById api method response is: \n{0}\n";

        //api/RoleMenu/saveUpdateRole
        public const string SAVEUPDATEROLE_STARTED_INFO_MSG = "SaveUpdateRole api method started.\n";
        public const string SAVEUPDATEROLE_REQ_MSG = "SaveUpdateRole api method request is: \n{0}\n";
        public const string SAVEUPDATEROLE_EXCEPTION_MSG = "SaveUpdateRole Exception below:: \n{0}\n";
        public const string SAVEUPDATEROLE_INNER_EXCEPTION_MSG = "SaveUpdateRole Inner Exception below:: \n{0}\n";
        public const string SERVICE_SAVEUPDATEROLE_REQ_MSG = "SaveUpdateRoleAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_SAVEUPDATEROLE_RES_MSG = "SaveUpdateRoleAsync (Role Menu service) method response is: \n{0}\n";
        public const string SAVEUPDATEROLE_RES_MSG = "SaveUpdateRole api method response is: \n{0}\n";

        //api/RoleMenu/deleteRole
        public const string DELETEROLE_STARTED_INFO_MSG = "DeleteRole api method started.\n";
        public const string DELETEROLE_REQ_MSG = "DeleteRole api method request is: \n{0}\n";
        public const string DELETEROLE_EXCEPTION_MSG = "DeleteRole Exception below:: \n{0}\n";
        public const string DELETEROLE_INNER_EXCEPTION_MSG = "DeleteRole Inner Exception below:: \n{0}\n";
        public const string SERVICE_DELETEROLE_REQ_MSG = "DeleteRoleAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_DELETEROLE_RES_MSG = "DeleteRoleAsync (Role Menu service) method response is: \n{0}\n";
        public const string DELETEROLE_RES_MSG = "DeleteRole api method response is: \n{0}\n";

        //api/RoleMenu/getAllMenuByUserId
        public const string GETALLMENUBYUSERID_STARTED_INFO_MSG = "GetAllMenuByUserId api method started.\n";
        public const string GETALLMENUBYUSERID_REQ_MSG = "GetAllMenuByUserId api method request is: \n{0}\n";
        public const string GETALLMENUBYUSERID_EXCEPTION_MSG = "GetAllMenuByUserId Exception below:: \n{0}\n";
        public const string GETALLMENUBYUSERID_INNER_EXCEPTION_MSG = "GetAllMenuByUserId Inner Exception below:: \n{0}\n";
        public const string SERVICE_GETALLMENUBYUSERID_REQ_MSG = "GetAllMenuByUserIdAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GETALLMENUBYUSERID_RES_MSG = "GetAllMenuByUserIdAsync (Role Menu service) method response is: \n{0}\n";
        public const string GETALLMENUBYUSERID_RES_MSG = "GetAllMenuByUserId api method response is: \n{0}\n";

        //api/RoleMenu/getAllParentMenus
        public const string GET_ALL_PARENT_MENUS_STARTED_INFO_MSG = "GetAllParentMenus api method started.\n";
        public const string GET_ALL_PARENT_MENUS_REQ_MSG = "GetAllParentMenus api method request is: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_EXCEPTION_MSG = "GetAllParentMenus Exception below:: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_INNER_EXCEPTION_MSG = "GetAllParentMenus Inner Exception below:: \n{0}\n";
        public const string SERVICE_GET_ALL_PARENT_MENUS_REQ_MSG = "GetAllParentMenusAsync (Role Menu service) method request is: \n{0}\n";
        public const string SERVICE_GET_ALL_PARENT_MENUS_RES_MSG = "GetAllParentMenusAsync (Role Menu service) method response is: \n{0}\n";
        public const string GET_ALL_PARENT_MENUS_RES_MSG = "GetAllParentMenus api method response is: \n{0}\n";

        //api/RoleMenu/GetAllUserMenuPagingWithSearch
        public const string GETALL_USER_MENU_PAGING_SEARCH_STARTED_INFO_MSG = "GetAllUserMenuPagingWithSearch api method started.\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG = "GetAllUserMenuPagingWithSearch api method request is: \n{0}\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_EXCEPTION_MSG = "GetAllUserMenuPagingWithSearch Exception below:: \n{0}\n";
        public const string SERVICE_GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG = "GetAllUserMenuPagingWithSearchAsync (RoleMenu service) method request is: \n{0}\n";
        public const string SERVICE_GETALL_USER_MENU_PAGING_SEARCH_RES_MSG = "GetAllUserMenuPagingWithSearchAsync (RoleMenu service) method response is: \n{0}\n";
        public const string GETALL_USER_MENU_PAGING_SEARCH_RES_MSG = "GetAllUserMenuPagingWithSearch api method response is: \n{0}\n";

        #endregion


        #region User Service
        public const string ADMIN = "Admin";
        public const string USER = "User";

        public const string GET_USER_FAILED = "Fetching user details failed.";
        public const string GET_USER_SUCCESS = "Fetching user details successful";

        public const string GET_USER_LIST_FAILED = "Fetching user list failed.";
        public const string GET_USER_LIST_SUCCESS = "Fetching user list successful";

        public const string AUTH_FAILED = "Authentation failed. Please try again later";
        public const string AUTH_INVALID_CREDENTIAL = "Invalid credential";
        public const string AUTH_SUCCESS = "Authentation success!";
        public const string AUTH_FAILED_ATTEMPT = "Your account was blocked for a {0} minutes, please try again later.";

        public const string REFRESHTOKEN_FAILED = "Refreshing token failed. Please try again later";
        public const string REFRESHTOKEN_INVALID_CREDENTIAL = "Invalid refresh token credential";
        public const string REFRESHTOKEN_SUCCESS = "Refreshing token success!";
        public const string NULL_TOKEN = "Token is null";

        public const string REVOKE_USER_FAILED = "Revoking user failed. Please try again later";
        public const string REVOKE_USER_SUCCESS = "Revoking user success!";
        

        public const string SAVE_KEY = "Save";
        public const string UPDATE_KEY = "Update";


        public const string REG_USER_SAVE_FAILED = "Registering user details failed. Please try again later";
        public const string REG_USER_SAVE_SUCCESS = "Registering user details success!";

        public const string REG_USER_UPDATE_FAILED = "Updating user details failed. Please try again later";
        public const string REG_USER_UPDATE_SUCCESS = "Updating user details success!";

        public const string EXIST_USER = "User is already exist. Try unique username or activate/update it.";

        public const string DELETE_FAILED = "Deletion of user failed!. Please try again later";
        public const string DELETE_SUCCESS = "User deleted successfully";

        public const string NO_ROLE_DATA = "No user role data found";
        public const string SUCCESS_ROLE_DATA = "Fething Role data successful!";
        public const string NO_MENU_DATA = "No menu data found";
        public const string SUCCESS_MENU_DATA = "Fething menu data successful!";
        public const string NO_PARENT_MENU_DATA = "No parent menu found";
        public const string SUCCESS_PARENT_MENU_DATA = "Fething parent menu successful!";

        //SP
        public const string GET_USER_BY_ID_SP_NAME = "SP_GetUserById";
        public const string GET_ALL_USER_SP_NAME = "SP_GetAllUser";
        public const string POST_SAVE_UPDATE_USER_SP_NAME = "SP_SaveUpdateUser";
        public const string DELETE_USER_SP_NAME = "SP_DeleteUser";
        #endregion

        public const string CTRLER_ROUTE_PATH_NAME = "api/[controller]";

        #region User Controller
        public const string GET_USER_ROUTE_NAME = "getUserbyId";
        public const string GET_ALL_USER_ROUTE_NAME = "getAllUsers";
        public const string POST_AUTH_ROUTE_NAME = "login";
        public const string POST_PUT_USER_ROUTE_NAME = "registerUser";
        public const string DEL_USER_ROUTE_NAME = "deleteUser";
        public const string REFRESH_TOKEN_ROUTE_NAME = "refreshtoken";
        public const string REVOKE_ROUTE_NAME = "revoke";
        #endregion

        #region Role Menu Controller
        public const string GET_ALL_ROLES_ROUTE_NAME = "getAllRoles";
        public const string GET_ALL_ROLES_PAGINATION_ROUTE_NAME = "getAllRolesPagination";
        public const string GET_ROLE_BY_ID_ROUTE_NAME = "getRoleById";
        public const string POST_SAVE_UPDATE_ROLE_ROUTE_NAME = "saveUpdateRole";
        public const string DELETE_ROLE_ROUTE_NAME = "deleteRole";
        public const string GET_ALL_MENU_BY_USER_ID_ROUTE_NAME = "getAllMenuByUserId";
        public const string GET_ALL_PARENT_MENUS_ROUTE_NAME = "getAllParentMenus";
        public const string GETALL_USER_MENU_PAGING_SEARCH_SP_NAME = "SP_GetAllUserMenuListPagingWithSearch";
        #endregion

        #region Role Menu Service
        public const string EXIST_ROLE = "There is a role with same name. Try different.";
        public const string USER_ROLE_SAVE_SUCCESS = "saving user role success!";
        public const string USER_ROLE_UPDATE_SUCCESS = "Updating user role success!";
        public const string NOT_EXIST_ROLE = "There is no role with such name.";
        public const string DELETE_ROLE_SUCCESS = "Role is successfully removed";
        //SP
        public const string GET_GET_ALL_MENU_BY_USER_ID_SP_NAME = "SP_GetAllMenuByUserId";
        #endregion

    }
}
