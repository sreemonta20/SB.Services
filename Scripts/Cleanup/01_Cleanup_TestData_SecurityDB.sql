/* =============================================================================
   SecurityDB - remove logins and artefacts created for end-to-end TEST employees
   -----------------------------------------------------------------------------
   Run BEFORE 02_Cleanup_TestData_HumanResourcesDB.sql.

   Test employees are identified the same way as in the HR script (candidate
   e-mail pattern). If HumanResourcesDB is on this server their ids/codes are read
   from it; otherwise the profile e-mail pattern alone is used.

   Removes, for those employees only:
     * AppUsers + AppUserProfiles provisioned by the onboarding consumer
       (AppUserProfiles.EmployeeId set) and their audit rows (AppUsersLog holds
       password hashes - removed too);
     * bell Notifications sent to them, and Admin notifications that name them;
     * queued/sent welcome e-mails (EmailOutbox);
     * LoginDeactivations rows created by the offboarding consumer.
   Never touches users without an EmployeeId link (e.g. sree / rohine), roles,
   menus or role-menu rights.

   @Execute = 0 (default) is a DRY RUN: deletes run in a transaction, counts are
   printed, then everything is rolled back.
   ============================================================================= */
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;   -- required for DML on tables with filtered indexes

DECLARE @Execute               BIT           = 0;                       -- 1 = commit
DECLARE @CandidateEmailPattern NVARCHAR(200) = N'aisha.%@example.com';
DECLARE @HrDatabase            SYSNAME       = N'HumanResourcesDB';

IF DB_NAME() <> N'SecurityDB'
BEGIN RAISERROR(N'Run this script in SecurityDB.', 16, 1); RETURN; END

CREATE TABLE #Report (Step INT IDENTITY, TableName SYSNAME, RowsDeleted INT);
CREATE TABLE #Emp (EmployeeId UNIQUEIDENTIFIER PRIMARY KEY, EmployeeCode NVARCHAR(30) NULL, Email NVARCHAR(300) NULL);
DECLARE @n INT;

-- -----------------------------------------------------------------------------
-- 1. Test employees (from HR when available, else from provisioned profiles)
-- -----------------------------------------------------------------------------
IF DB_ID(@HrDatabase) IS NOT NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'
        INSERT #Emp (EmployeeId, EmployeeCode, Email)
        SELECT DISTINCT e.Id, e.EmployeeCode, ISNULL(e.OfficialEmail, e.PersonalEmail)
        FROM ' + QUOTENAME(@HrDatabase) + N'.dbo.Employees e
        JOIN ' + QUOTENAME(@HrDatabase) + N'.dbo.JobOffers o ON o.ConvertedEmployeeId = e.Id
        JOIN ' + QUOTENAME(@HrDatabase) + N'.dbo.Candidates c ON c.Id = o.CandidateId
        WHERE c.Email LIKE @p;';
    EXEC sys.sp_executesql @sql, N'@p NVARCHAR(200)', @p = @CandidateEmailPattern;
END

INSERT #Emp (EmployeeId, EmployeeCode, Email)
SELECT p.EmployeeId, NULL, p.Email FROM dbo.AppUserProfiles p
WHERE p.EmployeeId IS NOT NULL AND p.Email LIKE @CandidateEmailPattern
  AND p.EmployeeId NOT IN (SELECT EmployeeId FROM #Emp);

-- codes for profiles found only locally (needed to match Admin notifications)
UPDATE e SET EmployeeCode = d.EmployeeCode
FROM #Emp e JOIN dbo.LoginDeactivations d ON d.EmployeeId = e.EmployeeId
WHERE e.EmployeeCode IS NULL;

SELECT p.Id INTO #Prof FROM dbo.AppUserProfiles p WHERE p.EmployeeId IN (SELECT EmployeeId FROM #Emp);
SELECT u.Id, u.UserName INTO #User FROM dbo.AppUsers u WHERE u.AppUserProfileId IN (SELECT Id FROM #Prof);

-- guard: never remove an Admin or a profile without the employee link
IF EXISTS (SELECT 1 FROM dbo.AppUserProfiles p JOIN dbo.AppUserRoles r ON r.Id = p.AppUserRoleId
           WHERE p.Id IN (SELECT Id FROM #Prof) AND r.RoleName = N'Admin')
BEGIN RAISERROR(N'A matched profile has the Admin role - not test data. Nothing deleted.', 16, 1); RETURN; END

DECLARE @summary NVARCHAR(MAX) = CONCAT(N'Test employees: ', (SELECT COUNT(*) FROM #Emp), N' | logins: ',
             ISNULL(STUFF((SELECT N', ' + UserName FROM #User ORDER BY UserName FOR XML PATH('')), 1, 2, N''), N'none'));
PRINT @summary;

BEGIN TRANSACTION;

-- Admin notifications naming a test employee code or login ("... (EMP-0006) ...", "Login 'emp-0007' ...")
DELETE n FROM dbo.Notifications n
WHERE n.RecipientProfileId NOT IN (SELECT Id FROM #Prof)
  AND n.[Type] IN (N'Onboarding', N'Offboarding')
  AND (   EXISTS (SELECT 1 FROM #Emp e WHERE e.EmployeeCode IS NOT NULL
                  AND (n.[Message] LIKE N'%' + e.EmployeeCode + N'[^0-9]%' OR n.[Message] LIKE N'%' + e.EmployeeCode))
       OR EXISTS (SELECT 1 FROM #User u WHERE n.[Message] LIKE N'%''' + u.UserName + N'''%'));
                                                                                     SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Notifications (to admins)', @n);
DELETE FROM dbo.Notifications WHERE RecipientProfileId IN (SELECT Id FROM #Prof);    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Notifications (to test users)', @n);

DELETE FROM dbo.EmailOutbox
WHERE ToAddress IN (SELECT Email FROM #Emp WHERE Email IS NOT NULL) OR ToAddress LIKE @CandidateEmailPattern;
                                                                                     SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'EmailOutbox', @n);
DELETE FROM dbo.LoginDeactivations
WHERE EmployeeId IN (SELECT EmployeeId FROM #Emp) OR AppUserId IN (SELECT Id FROM #User);
                                                                                     SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LoginDeactivations', @n);

DELETE FROM dbo.AppUsers WHERE Id IN (SELECT Id FROM #User);                         SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppUsers', @n);
DELETE FROM dbo.AppUserProfiles WHERE Id IN (SELECT Id FROM #Prof);                  SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppUserProfiles', @n);

-- audit rows, incl. the "Deleted" rows the triggers just wrote
DELETE FROM dbo.AppUsersLog WHERE AppUserId IN (SELECT Id FROM #User) OR AppUserProfileId IN (SELECT Id FROM #Prof);
                                                                                     SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppUsersLog', @n);
DELETE FROM dbo.AppUserProfilesLog WHERE AppUserProfileId IN (SELECT Id FROM #Prof); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppUserProfilesLog', @n);

SELECT TableName, RowsDeleted FROM #Report WHERE RowsDeleted > 0 ORDER BY Step;
SELECT SUM(RowsDeleted) AS TotalRows FROM #Report;

IF @Execute = 1
BEGIN
    COMMIT TRANSACTION;
    PRINT N'SecurityDB test data DELETED (committed).';
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    PRINT N'DRY RUN - nothing was changed. Set @Execute = 1 to delete.';
END
