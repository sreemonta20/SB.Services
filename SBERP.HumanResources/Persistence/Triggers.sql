-- =============================================================================
-- HumanResourcesDB — Audit Triggers
-- Mirrors SBERP.Security's TRG_* pattern: every Insert/Update/Delete copies the
-- row into its *Log table along with PerformedUser and Action.
-- =============================================================================

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
             ParentDepartmentId, HeadEmployeeId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.DepartmentCode, I.[Name], I.[Description],
               I.ParentDepartmentId, I.HeadEmployeeId, I.CreatedBy, I.CreatedDate,
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
             ParentDepartmentId, HeadEmployeeId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.DepartmentCode, I.[Name], I.[Description],
               I.ParentDepartmentId, I.HeadEmployeeId, I.CreatedBy, I.CreatedDate,
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
             ParentDepartmentId, HeadEmployeeId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.DepartmentCode, D.[Name], D.[Description],
               D.ParentDepartmentId, D.HeadEmployeeId, D.CreatedBy, D.CreatedDate,
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
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.DepartmentId, I.DesignationId,
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
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.DepartmentId, I.DesignationId,
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
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), D.Id, D.EmployeeCode, D.FullName, D.OfficialEmail,
           D.DepartmentId, D.DesignationId,
           D.EmploymentStatus, D.DateOfJoining, D.DateOfLeaving,
           D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate, D.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Deleted'
    FROM deleted D;
END
GO
