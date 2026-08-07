-- =============================================================================
-- HumanResourcesDB — Audit Triggers
-- Mirrors SBERP.Security's TRG_* pattern: every Insert/Update/Delete copies the
-- row into its *Log table along with PerformedUser and Action.
-- =============================================================================

-- =============================================================================
-- Companies   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertCompanies', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertCompanies;
GO
CREATE TRIGGER dbo.TRG_InsertCompanies ON dbo.Companies AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.CompaniesLog
            (Id, CompanyId, CompanyCode, [Name], LegalName, RegistrationNumber, TaxNumber,
             Address, City, Country, Phone, Email, Website, LogoUrl, CurrencyCode,
             FinancialYearStartMonth, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CompanyCode, I.[Name], I.LegalName, I.RegistrationNumber, I.TaxNumber,
               I.Address, I.City, I.Country, I.Phone, I.Email, I.Website, I.LogoUrl, I.CurrencyCode,
               I.FinancialYearStartMonth, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_UpdateCompanies', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateCompanies;
GO
CREATE TRIGGER dbo.TRG_UpdateCompanies ON dbo.Companies AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.CompaniesLog
            (Id, CompanyId, CompanyCode, [Name], LegalName, RegistrationNumber, TaxNumber,
             Address, City, Country, Phone, Email, Website, LogoUrl, CurrencyCode,
             FinancialYearStartMonth, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CompanyCode, I.[Name], I.LegalName, I.RegistrationNumber, I.TaxNumber,
               I.Address, I.City, I.Country, I.Phone, I.Email, I.Website, I.LogoUrl, I.CurrencyCode,
               I.FinancialYearStartMonth, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_DeleteCompanies', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteCompanies;
GO
CREATE TRIGGER dbo.TRG_DeleteCompanies ON dbo.Companies AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.CompaniesLog
            (Id, CompanyId, CompanyCode, [Name], LegalName, RegistrationNumber, TaxNumber,
             Address, City, Country, Phone, Email, Website, LogoUrl, CurrencyCode,
             FinancialYearStartMonth, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.CompanyCode, D.[Name], D.LegalName, D.RegistrationNumber, D.TaxNumber,
               D.Address, D.City, D.Country, D.Phone, D.Email, D.Website, D.LogoUrl, D.CurrencyCode,
               D.FinancialYearStartMonth, D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate,
               D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

-- =============================================================================
-- Branches   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertBranches', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertBranches;
GO
CREATE TRIGGER dbo.TRG_InsertBranches ON dbo.Branches AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.BranchesLog
            (Id, BranchId, CompanyId, BranchCode, [Name], Address, City, Country, Phone, Email,
             IsHeadOffice, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CompanyId, I.BranchCode, I.[Name], I.Address, I.City, I.Country, I.Phone, I.Email,
               I.IsHeadOffice, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_UpdateBranches', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateBranches;
GO
CREATE TRIGGER dbo.TRG_UpdateBranches ON dbo.Branches AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.BranchesLog
            (Id, BranchId, CompanyId, BranchCode, [Name], Address, City, Country, Phone, Email,
             IsHeadOffice, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CompanyId, I.BranchCode, I.[Name], I.Address, I.City, I.Country, I.Phone, I.Email,
               I.IsHeadOffice, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_DeleteBranches', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteBranches;
GO
CREATE TRIGGER dbo.TRG_DeleteBranches ON dbo.Branches AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.BranchesLog
            (Id, BranchId, CompanyId, BranchCode, [Name], Address, City, Country, Phone, Email,
             IsHeadOffice, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.CompanyId, D.BranchCode, D.[Name], D.Address, D.City, D.Country, D.Phone, D.Email,
               D.IsHeadOffice, D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate, D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

-- =============================================================================
-- Departments
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertDepartments', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertDepartments;
GO
CREATE TRIGGER dbo.TRG_InsertDepartments ON dbo.Departments AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.DepartmentsLog
            (Id, DepartmentId, DepartmentCode, [Name], [Description],
             ParentDepartmentId, HeadEmployeeId, CompanyId, BranchId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.DepartmentCode, I.[Name], I.[Description],
               I.ParentDepartmentId, I.HeadEmployeeId, I.CompanyId, I.BranchId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_UpdateDepartments', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateDepartments;
GO
CREATE TRIGGER dbo.TRG_UpdateDepartments ON dbo.Departments AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.DepartmentsLog
            (Id, DepartmentId, DepartmentCode, [Name], [Description],
             ParentDepartmentId, HeadEmployeeId, CompanyId, BranchId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.DepartmentCode, I.[Name], I.[Description],
               I.ParentDepartmentId, I.HeadEmployeeId, I.CompanyId, I.BranchId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

IF OBJECT_ID('dbo.TRG_DeleteDepartments', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteDepartments;
GO
CREATE TRIGGER dbo.TRG_DeleteDepartments ON dbo.Departments AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.DepartmentsLog
            (Id, DepartmentId, DepartmentCode, [Name], [Description],
             ParentDepartmentId, HeadEmployeeId, CompanyId, BranchId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.DepartmentCode, D.[Name], D.[Description],
               D.ParentDepartmentId, D.HeadEmployeeId, D.CompanyId, D.BranchId, D.CreatedBy, D.CreatedDate,
               D.UpdatedBy, D.UpdatedDate, D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1); ROLLBACK;
    END CATCH
END
GO

-- =============================================================================
-- Designations
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertDesignations', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertDesignations;
GO
CREATE TRIGGER dbo.TRG_InsertDesignations ON dbo.Designations AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DesignationsLog (Id, DesignationId, [Name], [Code], [Description], Grade,
                                      CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive,
                                      PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.[Name], I.[Code], I.[Description], I.Grade,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
           'Created'
    FROM inserted I;
END
GO

IF OBJECT_ID('dbo.TRG_UpdateDesignations', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateDesignations;
GO
CREATE TRIGGER dbo.TRG_UpdateDesignations ON dbo.Designations AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DesignationsLog (Id, DesignationId, [Name], [Code], [Description], Grade,
                                      CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive,
                                      PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.[Name], I.[Code], I.[Description], I.Grade,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Updated'
    FROM inserted I;
END
GO

IF OBJECT_ID('dbo.TRG_DeleteDesignations', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteDesignations;
GO
CREATE TRIGGER dbo.TRG_DeleteDesignations ON dbo.Designations AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DesignationsLog (Id, DesignationId, [Name], [Code], [Description], Grade,
                                      CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive,
                                      PerformedUser, [Action])
    SELECT NEWID(), D.Id, D.[Name], D.[Code], D.[Description], D.Grade,
           D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate, D.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Deleted'
    FROM deleted D;
END
GO

-- =============================================================================
-- Employees
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertEmployees', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertEmployees;
GO
CREATE TRIGGER dbo.TRG_InsertEmployees ON dbo.Employees AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.CompanyId, I.BranchId, I.DepartmentId, I.DesignationId,
           I.EmploymentStatus, I.DateOfJoining, I.DateOfLeaving,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
           'Created'
    FROM inserted I;
END
GO

IF OBJECT_ID('dbo.TRG_UpdateEmployees', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateEmployees;
GO
CREATE TRIGGER dbo.TRG_UpdateEmployees ON dbo.Employees AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.CompanyId, I.BranchId, I.DepartmentId, I.DesignationId,
           I.EmploymentStatus, I.DateOfJoining, I.DateOfLeaving,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Updated'
    FROM inserted I;
END
GO

IF OBJECT_ID('dbo.TRG_DeleteEmployees', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteEmployees;
GO
CREATE TRIGGER dbo.TRG_DeleteEmployees ON dbo.Employees AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), D.Id, D.EmployeeCode, D.FullName, D.OfficialEmail,
           D.CompanyId, D.BranchId, D.DepartmentId, D.DesignationId,
           D.EmploymentStatus, D.DateOfJoining, D.DateOfLeaving,
           D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate, D.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Deleted'
    FROM deleted D;
END
GO

-- =============================================================================
-- OPTIONAL: audit triggers for the lookup tables
-- =============================================================================
-- You asked for triggers "where adding or modification needed." For these
-- five lookup tables my honest recommendation is: DON'T add audit triggers.
--
-- Reasoning:
--   * These are reference tables. Rows change maybe once a year, if ever.
--   * They have no *Log sibling tables (unlike Departments/Employees), and
--     adding them would mean five more tables carrying near-zero data.
--   * The audit value is negligible — there's no "who changed this employee's
--     salary" question here, just "someone added a blood group once."
--   * Triggers on a table that an admin edits by hand in SSMS can surprise
--     them with failures if the *Log table or trigger has any issue.
--
-- What you DO want instead is a guard trigger that PREVENTS deleting or
-- renumbering a row that the Employees table still references — because the
-- Id is a contract shared with the C# enums. That protects data integrity,
-- which is the real risk here, rather than logging.
--
-- Below is that guard for the two tables most likely to be referenced by
-- existing employee rows. Apply the same pattern to the others if you wish.
-- Skip this whole file if you'd rather rely on application-level discipline.
-- =============================================================================


-- Prevent deleting an employment status that employees still use.
IF OBJECT_ID('dbo.TRG_PreventDelete_EmploymentStatuses', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TRG_PreventDelete_EmploymentStatuses;
GO
CREATE TRIGGER dbo.TRG_PreventDelete_EmploymentStatuses
ON dbo.EmploymentStatuses
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM deleted d
        INNER JOIN dbo.Employees e ON e.EmploymentStatus = d.Id
    )
    BEGIN
        RAISERROR('Cannot delete an employment status that is still assigned to employees. Set IsActive = 0 instead.', 16, 1);
        RETURN;
    END
    DELETE FROM dbo.EmploymentStatuses
    WHERE Id IN (SELECT Id FROM deleted);
END
GO


-- Prevent deleting an employment type that employees still use.
IF OBJECT_ID('dbo.TRG_PreventDelete_EmploymentTypes', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TRG_PreventDelete_EmploymentTypes;
GO
CREATE TRIGGER dbo.TRG_PreventDelete_EmploymentTypes
ON dbo.EmploymentTypes
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM deleted d
        INNER JOIN dbo.Employees e ON e.EmploymentType = d.Id
    )
    BEGIN
        RAISERROR('Cannot delete an employment type that is still assigned to employees. Set IsActive = 0 instead.', 16, 1);
        RETURN;
    END
    DELETE FROM dbo.EmploymentTypes
    WHERE Id IN (SELECT Id FROM deleted);
END
GO
