-- =====================================================================================================
-- HumanResourcesDB — all SQL objects (functions, stored procedures, triggers, column defaults)
-- -----------------------------------------------------------------------------------------------------
-- The only SQL file of SBERP.HumanResources. Tables, keys, indexes and seed data are not here: they come from the
-- EF Core model (HumanResourcesDBContext, HasData). This file is an Embedded Resource and is applied by the newest
-- migration through SqlScriptHelper.ApplyAll, after the tables exist:
--     dotnet ef database update      (fresh database at a customer, or an upgrade)
-- Every object is dropped if it exists and created again, so the file can run any number of times.
-- SQL Server 2016 RTM compatible: no CREATE OR ALTER, no STRING_AGG.
--
-- Sections (in run order):
--    1. CoreHR Functions
--    2. CoreHR StoredProcedures
--    3. CoreHR Triggers
--    4. ColumnDefaults
--    5. OrgSetup Objects
--    6. Recruitment StoredProcedures
--    7. Recruitment FunctionsTriggers
--    8. Onboarding StoredProcedures
--    9. Onboarding FunctionsTriggers
--   10. Attendance StoredProcedures
--   11. Attendance FunctionsTriggers
--   12. Leave StoredProcedures
--   13. Leave FunctionsTriggers
--   14. Payroll StoredProcedures
--   15. Payroll FunctionsTriggers
--   16. Performance StoredProcedures
--   17. Performance FunctionsTriggers
--   18. Separation StoredProcedures
--   19. Separation FunctionsTriggers
--   20. Biometric Attendance (mobile app / door terminal)
-- =====================================================================================================
GO


-- #####################################################################################################
-- SECTION 1: CoreHR Functions   (was 01_CoreHR_Functions.sql)
-- #####################################################################################################
-- Core HR (+ Company/Branch) functions

-- =============================================================================
-- HumanResourcesDB — Functions
-- Author: Sreemonta Bhowmik
-- =============================================================================

-- =============================================================================
-- fn_GetNextEmployeeCode
-- Returns the next employee code in the form EMP-0001, EMP-0002, ...
-- Pure function so it can be inlined in DEFAULT constraints if desired.
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextEmployeeCode', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetNextEmployeeCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextEmployeeCode;
GO
CREATE FUNCTION dbo.fn_GetNextEmployeeCode()
RETURNS NVARCHAR(30)
AS
BEGIN
    DECLARE @next INT;

    SELECT @next = ISNULL(MAX(
        TRY_CAST(SUBSTRING(EmployeeCode, 5, 50) AS INT)
    ), 0) + 1
    FROM dbo.Employees
    WHERE EmployeeCode LIKE 'EMP-%';

    RETURN 'EMP-' + RIGHT('0000' + CAST(@next AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- fn_GetEmployeeFullName
-- Composes a full name out of first/middle/last, handling NULL middle.
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetEmployeeFullName', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetEmployeeFullName;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetEmployeeFullName;
GO
CREATE FUNCTION dbo.fn_GetEmployeeFullName(
    @First  NVARCHAR(100),
    @Middle NVARCHAR(100),
    @Last   NVARCHAR(100)
)
RETURNS NVARCHAR(300)
AS
BEGIN
    RETURN LTRIM(RTRIM(
        ISNULL(@First, '') +
        CASE WHEN @Middle IS NULL OR LTRIM(RTRIM(@Middle)) = ''
             THEN '' ELSE ' ' + @Middle END +
        CASE WHEN @Last IS NULL OR LTRIM(RTRIM(@Last)) = ''
             THEN '' ELSE ' ' + @Last END
    ));
END
GO
-- =============================================================================
-- fn_CountEmployeesInDepartment
-- Used by the Department list view to show counts without an extra round-trip.
-- =============================================================================
IF OBJECT_ID('dbo.fn_CountEmployeesInDepartment', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CountEmployeesInDepartment;
GO
DROP FUNCTION IF EXISTS dbo.fn_CountEmployeesInDepartment;
GO
CREATE FUNCTION dbo.fn_CountEmployeesInDepartment(@DeptId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @cnt INT;
    SELECT @cnt = COUNT(1)
    FROM dbo.Employees
    WHERE DepartmentId = @DeptId
      AND ISNULL(IsActive, 0) = 1;
    RETURN ISNULL(@cnt, 0);
END
GO
-- =============================================================================
-- fn_CountBranchesInCompany   (Module 0)
-- Used by the Company list view, same purpose/shape as
-- fn_CountEmployeesInDepartment above.
-- =============================================================================
IF OBJECT_ID('dbo.fn_CountBranchesInCompany', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CountBranchesInCompany;
GO
DROP FUNCTION IF EXISTS dbo.fn_CountBranchesInCompany;
GO
CREATE FUNCTION dbo.fn_CountBranchesInCompany(@CompanyId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @cnt INT;
    SELECT @cnt = COUNT(1)
    FROM dbo.Branches
    WHERE CompanyId = @CompanyId
      AND ISNULL(IsActive, 0) = 1;
    RETURN ISNULL(@cnt, 0);
END
GO
-- =============================================================================
-- fn_CountDepartmentsInCompany   (Module 0)
-- Same shape, used if the Company detail view wants a department count too.
-- =============================================================================
IF OBJECT_ID('dbo.fn_CountDepartmentsInCompany', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CountDepartmentsInCompany;
GO
DROP FUNCTION IF EXISTS dbo.fn_CountDepartmentsInCompany;
GO
CREATE FUNCTION dbo.fn_CountDepartmentsInCompany(@CompanyId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @cnt INT;
    SELECT @cnt = COUNT(1)
    FROM dbo.Departments
    WHERE CompanyId = @CompanyId
      AND ISNULL(IsActive, 0) = 1;
    RETURN ISNULL(@cnt, 0);
END
GO
GO

-- #####################################################################################################
-- SECTION 2: CoreHR StoredProcedures   (was 02_CoreHR_StoredProcedures.sql)
-- #####################################################################################################
-- Core HR (+ Company/Branch) stored procedures

-- =============================================================================
-- HumanResourcesDB — Stored Procedures
-- Author: Sreemonta Bhowmik
-- Style matches SBERP.Security's SP suite (JSON paging output, action-flag SP)
-- =============================================================================

-- =============================================================================
-- SP_GetAllCompaniesPagingWithSearch   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllCompaniesPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllCompaniesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllCompaniesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllCompaniesPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #CoTBL
    (
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX)
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Companies C
    WHERE (@SearchTerm = ''
           OR C.[Name] LIKE '%' + @SearchTerm + '%'
           OR C.CompanyCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH SortedCo AS
    (
        SELECT
            C.Id, C.CompanyCode, C.[Name], C.LegalName, C.RegistrationNumber, C.TaxNumber,
            C.Address, C.City, C.Country, C.Phone, C.Email, C.Website, C.LogoUrl,
            C.CurrencyCode, C.FinancialYearStartMonth,
            dbo.fn_CountBranchesInCompany(C.Id) AS BranchCount,
            C.IsActive, C.CreatedDate, C.UpdatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'ASC'  THEN C.[Name] END ASC,
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'DESC' THEN C.[Name] END DESC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'ASC'  THEN C.[Name] END ASC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'DESC' THEN C.[Name] END DESC
            ) AS RowNum
        FROM dbo.Companies C
        WHERE (@SearchTerm = ''
               OR C.[Name] LIKE '%' + @SearchTerm + '%'
               OR C.CompanyCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT
            Id, CompanyCode, [Name], LegalName, RegistrationNumber, TaxNumber, Address, City,
            Country, Phone, Email, Website, LogoUrl, CurrencyCode, FinancialYearStartMonth,
            BranchCount, IsActive, CreatedDate, UpdatedDate
        FROM SortedCo
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1
                         AND  @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #CoTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #CoTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #CoTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetCompanyById   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetCompanyById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetCompanyById;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetCompanyById;
GO
CREATE PROCEDURE dbo.SP_GetCompanyById @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        C.Id, C.CompanyCode, C.[Name], C.LegalName, C.RegistrationNumber, C.TaxNumber,
        C.Address, C.City, C.Country, C.Phone, C.Email, C.Website, C.LogoUrl,
        C.CurrencyCode, C.FinancialYearStartMonth,
        dbo.fn_CountBranchesInCompany(C.Id) AS BranchCount,
        C.IsActive, C.CreatedDate, C.UpdatedDate
    FROM dbo.Companies C
    WHERE C.Id = @Id;
END
GO
-- =============================================================================
-- SP_SaveUpdateCompany   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveUpdateCompany', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_SaveUpdateCompany;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveUpdateCompany;
GO
CREATE PROCEDURE dbo.SP_SaveUpdateCompany
    @ActionName              VARCHAR(10),
    @Id                      UNIQUEIDENTIFIER,
    @CompanyCode             NVARCHAR(20),
    @Name                    NVARCHAR(200),
    @LegalName               NVARCHAR(300)  = NULL,
    @RegistrationNumber      NVARCHAR(100)  = NULL,
    @TaxNumber               NVARCHAR(100)  = NULL,
    @Address                 NVARCHAR(500)  = NULL,
    @City                    NVARCHAR(100)  = NULL,
    @Country                 NVARCHAR(100)  = NULL,
    @Phone                   NVARCHAR(30)   = NULL,
    @Email                   NVARCHAR(200)  = NULL,
    @Website                 NVARCHAR(200)  = NULL,
    @LogoUrl                 NVARCHAR(500)  = NULL,
    @CurrencyCode            NVARCHAR(10)   = NULL,
    @FinancialYearStartMonth INT            = NULL,
    @CreatedBy               NVARCHAR(MAX),
    @UpdatedBy                NVARCHAR(MAX),
    @IsActive                BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ActionName = 'Save'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Companies WHERE CompanyCode = @CompanyCode)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Duplicate company code' AS 'Message';
            RETURN;
        END

        INSERT INTO dbo.Companies
            (Id, CompanyCode, [Name], LegalName, RegistrationNumber, TaxNumber, Address, City,
             Country, Phone, Email, Website, LogoUrl, CurrencyCode, FinancialYearStartMonth,
             CreatedBy, CreatedDate, IsActive)
        VALUES
            (@Id, @CompanyCode, @Name, @LegalName, @RegistrationNumber, @TaxNumber, @Address, @City,
             @Country, @Phone, @Email, @Website, @LogoUrl, @CurrencyCode, @FinancialYearStartMonth,
             @CreatedBy, GETUTCDATE(), @IsActive);

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update'
    BEGIN
        UPDATE dbo.Companies
        SET CompanyCode             = @CompanyCode,
            [Name]                  = @Name,
            LegalName               = @LegalName,
            RegistrationNumber      = @RegistrationNumber,
            TaxNumber               = @TaxNumber,
            Address                 = @Address,
            City                    = @City,
            Country                 = @Country,
            Phone                   = @Phone,
            Email                   = @Email,
            Website                 = @Website,
            LogoUrl                 = @LogoUrl,
            CurrencyCode            = @CurrencyCode,
            FinancialYearStartMonth = @FinancialYearStartMonth,
            UpdatedBy               = @UpdatedBy,
            UpdatedDate             = GETUTCDATE(),
            IsActive                = @IsActive
        WHERE Id = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
        RAISERROR('Invalid action flag. Must be ''Save'' or ''Update''.', 16, 1);
END
GO
-- =============================================================================
-- SP_DeleteCompany   (Module 0)
-- Soft-deletes (IsActive = 0) when active branches/departments/employees exist.
-- =============================================================================
IF OBJECT_ID('dbo.SP_DeleteCompany', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_DeleteCompany;
GO
DROP PROCEDURE IF EXISTS dbo.SP_DeleteCompany;
GO
CREATE PROCEDURE dbo.SP_DeleteCompany
    @Id        UNIQUEIDENTIFIER,
    @IsDelete  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Branches WHERE CompanyId = @Id AND ISNULL(IsActive, 0) = 1)
       OR EXISTS (SELECT 1 FROM dbo.Departments WHERE CompanyId = @Id AND ISNULL(IsActive, 0) = 1)
       OR EXISTS (SELECT 1 FROM dbo.Employees WHERE CompanyId = @Id AND ISNULL(IsActive, 0) = 1)
    BEGIN
        UPDATE dbo.Companies SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated — still has active branches, departments, or employees' AS 'Message';
        RETURN;
    END

    IF @IsDelete = 1
    BEGIN
        DELETE FROM dbo.Companies WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Removed' AS 'Message';
    END
    ELSE
    BEGIN
        UPDATE dbo.Companies SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated' AS 'Message';
    END
END
GO
-- =============================================================================
-- SP_GetAllBranchesPagingWithSearch   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllBranchesPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllBranchesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllBranchesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllBranchesPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #BrTBL
    (
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX)
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Branches B
    WHERE (@SearchTerm = ''
           OR B.[Name] LIKE '%' + @SearchTerm + '%'
           OR B.BranchCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH SortedBr AS
    (
        SELECT
            B.Id, B.CompanyId, C.[Name] AS CompanyName, B.BranchCode, B.[Name],
            B.Address, B.City, B.Country, B.Phone, B.Email, B.IsHeadOffice,
            B.IsActive, B.CreatedDate, B.UpdatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'ASC'  THEN B.[Name] END ASC,
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'DESC' THEN B.[Name] END DESC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'ASC'  THEN B.[Name] END ASC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'DESC' THEN B.[Name] END DESC
            ) AS RowNum
        FROM dbo.Branches B
        LEFT JOIN dbo.Companies C ON C.Id = B.CompanyId
        WHERE (@SearchTerm = ''
               OR B.[Name] LIKE '%' + @SearchTerm + '%'
               OR B.BranchCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT
            Id, CompanyId, CompanyName, BranchCode, [Name], Address, City, Country,
            Phone, Email, IsHeadOffice, IsActive, CreatedDate, UpdatedDate
        FROM SortedBr
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1
                         AND  @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #BrTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #BrTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #BrTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllBranchesByCompanyId   (Module 0)
-- Feeds the Branch dropdown once a Company is picked, on Department/Employee
-- forms and the Branch form itself.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllBranchesByCompanyId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllBranchesByCompanyId;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllBranchesByCompanyId;
GO
CREATE PROCEDURE dbo.SP_GetAllBranchesByCompanyId @CompanyId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, BranchCode, [Name], IsHeadOffice
    FROM dbo.Branches
    WHERE CompanyId = @CompanyId
      AND ISNULL(IsActive, 0) = 1
    ORDER BY [Name];
END
GO
-- =============================================================================
-- SP_GetBranchById   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetBranchById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetBranchById;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetBranchById;
GO
CREATE PROCEDURE dbo.SP_GetBranchById @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        B.Id, B.CompanyId, C.[Name] AS CompanyName, B.BranchCode, B.[Name],
        B.Address, B.City, B.Country, B.Phone, B.Email, B.IsHeadOffice,
        B.IsActive, B.CreatedDate, B.UpdatedDate
    FROM dbo.Branches B
    LEFT JOIN dbo.Companies C ON C.Id = B.CompanyId
    WHERE B.Id = @Id;
END
GO
-- =============================================================================
-- SP_SaveUpdateBranch   (Module 0)
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveUpdateBranch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_SaveUpdateBranch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveUpdateBranch;
GO
CREATE PROCEDURE dbo.SP_SaveUpdateBranch
    @ActionName    VARCHAR(10),
    @Id            UNIQUEIDENTIFIER,
    @CompanyId     UNIQUEIDENTIFIER,
    @BranchCode    NVARCHAR(20),
    @Name          NVARCHAR(200),
    @Address       NVARCHAR(500) = NULL,
    @City          NVARCHAR(100) = NULL,
    @Country       NVARCHAR(100) = NULL,
    @Phone         NVARCHAR(30)  = NULL,
    @Email         NVARCHAR(200) = NULL,
    @IsHeadOffice  BIT           = 0,
    @CreatedBy     NVARCHAR(MAX),
    @UpdatedBy     NVARCHAR(MAX),
    @IsActive      BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND ISNULL(IsActive, 0) = 1)
    BEGIN
        SELECT 0 AS 'RowsAffected', 'Selected company does not exist or is inactive' AS 'Message';
        RETURN;
    END

    IF @ActionName = 'Save'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Branches WHERE BranchCode = @BranchCode)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Duplicate branch code' AS 'Message';
            RETURN;
        END

        INSERT INTO dbo.Branches
            (Id, CompanyId, BranchCode, [Name], Address, City, Country, Phone, Email,
             IsHeadOffice, CreatedBy, CreatedDate, IsActive)
        VALUES
            (@Id, @CompanyId, @BranchCode, @Name, @Address, @City, @Country, @Phone, @Email,
             @IsHeadOffice, @CreatedBy, GETUTCDATE(), @IsActive);

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update'
    BEGIN
        UPDATE dbo.Branches
        SET CompanyId     = @CompanyId,
            BranchCode    = @BranchCode,
            [Name]        = @Name,
            Address       = @Address,
            City          = @City,
            Country       = @Country,
            Phone         = @Phone,
            Email         = @Email,
            IsHeadOffice  = @IsHeadOffice,
            UpdatedBy     = @UpdatedBy,
            UpdatedDate   = GETUTCDATE(),
            IsActive      = @IsActive
        WHERE Id = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
        RAISERROR('Invalid action flag. Must be ''Save'' or ''Update''.', 16, 1);
END
GO
-- =============================================================================
-- SP_DeleteBranch   (Module 0)
-- Soft-deletes (IsActive = 0) when active departments/employees still use it.
-- =============================================================================
IF OBJECT_ID('dbo.SP_DeleteBranch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_DeleteBranch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_DeleteBranch;
GO
CREATE PROCEDURE dbo.SP_DeleteBranch
    @Id        UNIQUEIDENTIFIER,
    @IsDelete  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Departments WHERE BranchId = @Id AND ISNULL(IsActive, 0) = 1)
       OR EXISTS (SELECT 1 FROM dbo.Employees WHERE BranchId = @Id AND ISNULL(IsActive, 0) = 1)
    BEGIN
        UPDATE dbo.Branches SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated — still has active departments or employees' AS 'Message';
        RETURN;
    END

    IF @IsDelete = 1
    BEGIN
        DELETE FROM dbo.Branches WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Removed' AS 'Message';
    END
    ELSE
    BEGIN
        UPDATE dbo.Branches SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated' AS 'Message';
    END
END
GO
-- =============================================================================
-- SP_GetAllDepartmentsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllDepartmentsPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllDepartmentsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllDepartmentsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllDepartmentsPagingWithSearch
    @SearchTerm           NVARCHAR(200) = '',
    @SortColumnName       NVARCHAR(50)  = '',
    @SortColumnDirection  NVARCHAR(5)   = 'ASC',
    @PageNumber           INT           = 1,
    @PageSize             INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #DeptTBL
    (
        [RowCount]    INT,
        [CurrentPage] INT,
        [PageSize]    INT,
        [PageCount]   INT,
        [Items]       NVARCHAR(MAX)
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Departments D
    WHERE (@SearchTerm = ''
           OR D.[Name] LIKE '%' + @SearchTerm + '%'
           OR D.[DepartmentCode] LIKE '%' + @SearchTerm + '%'
           OR D.[Description] LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH SortedDept AS
    (
        SELECT
            D.[Id],
            D.[DepartmentCode],
            D.[Name],
            D.[Description],
            D.[ParentDepartmentId],
            PD.[Name]                AS ParentDepartmentName,
            D.[HeadEmployeeId],
            HE.[FullName]            AS HeadEmployeeName,
            D.[CompanyId],
            CO.[Name]                AS CompanyName,
            D.[BranchId],
            BR.[Name]                AS BranchName,
            dbo.fn_CountEmployeesInDepartment(D.[Id]) AS EmployeeCount,
            D.[IsActive],
            D.[CreatedDate],
            D.[UpdatedDate],
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName = 'Name'           AND @SortColumnDirection = 'ASC'  THEN D.[Name] END ASC,
                CASE WHEN @SortColumnName = 'Name'           AND @SortColumnDirection = 'DESC' THEN D.[Name] END DESC,
                CASE WHEN @SortColumnName = 'DepartmentCode' AND @SortColumnDirection = 'ASC'  THEN D.[DepartmentCode] END ASC,
                CASE WHEN @SortColumnName = 'DepartmentCode' AND @SortColumnDirection = 'DESC' THEN D.[DepartmentCode] END DESC,
                CASE WHEN @SortColumnName = ''               AND @SortColumnDirection = 'ASC'  THEN D.[Name] END ASC,
                CASE WHEN @SortColumnName = ''               AND @SortColumnDirection = 'DESC' THEN D.[Name] END DESC
            ) AS RowNum
        FROM dbo.Departments D
        LEFT JOIN dbo.Departments PD ON PD.Id = D.ParentDepartmentId
        LEFT JOIN dbo.Employees   HE ON HE.Id = D.HeadEmployeeId
        LEFT JOIN dbo.Companies   CO ON CO.Id = D.CompanyId
        LEFT JOIN dbo.Branches    BR ON BR.Id = D.BranchId
        WHERE (@SearchTerm = ''
               OR D.[Name] LIKE '%' + @SearchTerm + '%'
               OR D.[DepartmentCode] LIKE '%' + @SearchTerm + '%'
               OR D.[Description] LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT
            [Id], [DepartmentCode], [Name], [Description],
            [ParentDepartmentId], [ParentDepartmentName],
            [HeadEmployeeId], [HeadEmployeeName],
            [CompanyId], [CompanyName], [BranchId], [BranchName],
            [EmployeeCount], [IsActive], [CreatedDate], [UpdatedDate]
        FROM SortedDept
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1
                         AND  @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #DeptTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);

    SET @Result =
    (
        SELECT * FROM #DeptTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DROP TABLE #DeptTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetDepartmentById
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetDepartmentById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetDepartmentById;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetDepartmentById;
GO
CREATE PROCEDURE dbo.SP_GetDepartmentById @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        D.Id, D.DepartmentCode, D.[Name], D.[Description],
        D.ParentDepartmentId, PD.[Name]  AS ParentDepartmentName,
        D.HeadEmployeeId,    HE.FullName AS HeadEmployeeName,
        D.CompanyId,         CO.[Name]   AS CompanyName,
        D.BranchId,          BR.[Name]   AS BranchName,
        dbo.fn_CountEmployeesInDepartment(D.Id) AS EmployeeCount,
        D.IsActive, D.CreatedDate, D.UpdatedDate
    FROM dbo.Departments D
    LEFT JOIN dbo.Departments PD ON PD.Id = D.ParentDepartmentId
    LEFT JOIN dbo.Employees   HE ON HE.Id = D.HeadEmployeeId
    LEFT JOIN dbo.Companies   CO ON CO.Id = D.CompanyId
    LEFT JOIN dbo.Branches    BR ON BR.Id = D.BranchId
    WHERE D.Id = @Id;
END
GO
-- =============================================================================
-- SP_SaveUpdateDepartment
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveUpdateDepartment', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_SaveUpdateDepartment;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveUpdateDepartment;
GO
CREATE PROCEDURE dbo.SP_SaveUpdateDepartment
    @ActionName          VARCHAR(10),
    @Id                  UNIQUEIDENTIFIER,
    @DepartmentCode      NVARCHAR(20),
    @Name                NVARCHAR(150),
    @Description         NVARCHAR(500),
    @ParentDepartmentId  UNIQUEIDENTIFIER = NULL,
    @HeadEmployeeId      UNIQUEIDENTIFIER = NULL,
    @CompanyId           UNIQUEIDENTIFIER,          -- Module 0, required
    @BranchId            UNIQUEIDENTIFIER = NULL,   -- Module 0, optional
    @CreatedBy           NVARCHAR(MAX),
    @UpdatedBy           NVARCHAR(MAX),
    @IsActive            BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ActionName = 'Save'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentCode = @DepartmentCode)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Duplicate department code' AS 'Message';
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND ISNULL(IsActive, 0) = 1)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Selected company does not exist or is inactive' AS 'Message';
            RETURN;
        END

        INSERT INTO dbo.Departments
            (Id, DepartmentCode, [Name], [Description], ParentDepartmentId,
             HeadEmployeeId, CompanyId, BranchId, CreatedBy, CreatedDate, IsActive)
        VALUES
            (@Id, @DepartmentCode, @Name, @Description, @ParentDepartmentId,
             @HeadEmployeeId, @CompanyId, @BranchId, @CreatedBy, GETUTCDATE(), @IsActive);

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND ISNULL(IsActive, 0) = 1)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Selected company does not exist or is inactive' AS 'Message';
            RETURN;
        END

        UPDATE dbo.Departments
        SET DepartmentCode     = @DepartmentCode,
            [Name]             = @Name,
            [Description]      = @Description,
            ParentDepartmentId = @ParentDepartmentId,
            HeadEmployeeId     = @HeadEmployeeId,
            CompanyId          = @CompanyId,
            BranchId           = @BranchId,
            UpdatedBy          = @UpdatedBy,
            UpdatedDate        = GETUTCDATE(),
            IsActive           = @IsActive
        WHERE Id = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
        RAISERROR('Invalid action flag. Must be ''Save'' or ''Update''.', 16, 1);
END
GO
-- =============================================================================
-- SP_DeleteDepartment
-- Soft-deletes (IsActive = 0) when assigned employees exist.
-- =============================================================================
IF OBJECT_ID('dbo.SP_DeleteDepartment', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_DeleteDepartment;
GO
DROP PROCEDURE IF EXISTS dbo.SP_DeleteDepartment;
GO
CREATE PROCEDURE dbo.SP_DeleteDepartment
    @Id        UNIQUEIDENTIFIER,
    @IsDelete  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @empCount INT = dbo.fn_CountEmployeesInDepartment(@Id);

    IF @empCount > 0
    BEGIN
        UPDATE dbo.Departments SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated — has employees' AS 'Message';
        RETURN;
    END

    IF @IsDelete = 1
    BEGIN
        DELETE FROM dbo.Departments WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Removed' AS 'Message';
    END
    ELSE
    BEGIN
        UPDATE dbo.Departments SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated' AS 'Message';
    END
END
GO
-- =============================================================================
-- SP_GetAllDesignationsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllDesignationsPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllDesignationsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllDesignationsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllDesignationsPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #DesignTBL
    (
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX)
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Designations
    WHERE (@SearchTerm = ''
           OR [Name] LIKE '%' + @SearchTerm + '%'
           OR [Code] LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            Id, [Name], [Code], [Description], Grade, IsActive,
            CreatedDate, UpdatedDate,
            (SELECT COUNT(1) FROM dbo.Employees E
              WHERE E.DesignationId = D.Id AND ISNULL(E.IsActive, 0) = 1
            ) AS EmployeeCount,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'ASC'  THEN [Name] END ASC,
                CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'DESC' THEN [Name] END DESC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'ASC'  THEN [Name] END ASC,
                CASE WHEN @SortColumnName = ''     AND @SortColumnDirection = 'DESC' THEN [Name] END DESC
            ) AS RowNum
        FROM dbo.Designations D
        WHERE (@SearchTerm = ''
               OR [Name] LIKE '%' + @SearchTerm + '%'
               OR [Code] LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, [Name], [Code], [Description], Grade, EmployeeCount, IsActive, CreatedDate, UpdatedDate
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1
                         AND  @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #DesignTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #DesignTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #DesignTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_SaveUpdateDesignation
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveUpdateDesignation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_SaveUpdateDesignation;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveUpdateDesignation;
GO
CREATE PROCEDURE dbo.SP_SaveUpdateDesignation
    @ActionName  VARCHAR(10),
    @Id          UNIQUEIDENTIFIER,
    @Name        NVARCHAR(150),
    @Code        NVARCHAR(20),
    @Description NVARCHAR(500),
    @Grade       INT,
    @CreatedBy   NVARCHAR(MAX),
    @UpdatedBy   NVARCHAR(MAX),
    @IsActive    BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ActionName = 'Save'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Designations WHERE [Name] = @Name)
        BEGIN
            SELECT 0 AS 'RowsAffected', 'Duplicate designation name' AS 'Message';
            RETURN;
        END
        INSERT INTO dbo.Designations (Id, [Name], [Code], [Description], Grade, CreatedBy, CreatedDate, IsActive)
        VALUES (@Id, @Name, @Code, @Description, @Grade, @CreatedBy, GETUTCDATE(), @IsActive);
        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update'
    BEGIN
        UPDATE dbo.Designations
        SET [Name]        = @Name,
            [Code]        = @Code,
            [Description] = @Description,
            Grade         = @Grade,
            UpdatedBy     = @UpdatedBy,
            UpdatedDate   = GETUTCDATE(),
            IsActive      = @IsActive
        WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
        RAISERROR('Invalid action flag. Must be ''Save'' or ''Update''.', 16, 1);
END
GO
-- =============================================================================
-- SP_DeleteDesignation
-- =============================================================================
IF OBJECT_ID('dbo.SP_DeleteDesignation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_DeleteDesignation;
GO
DROP PROCEDURE IF EXISTS dbo.SP_DeleteDesignation;
GO
CREATE PROCEDURE dbo.SP_DeleteDesignation
    @Id       UNIQUEIDENTIFIER,
    @IsDelete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @used INT;
    SELECT @used = COUNT(1) FROM dbo.Employees WHERE DesignationId = @Id;

    IF @used > 0
    BEGIN
        UPDATE dbo.Designations SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected', 'Inactivated — designation in use' AS 'Message';
        RETURN;
    END

    IF @IsDelete = 1
    BEGIN
        DELETE FROM dbo.Designations WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
    BEGIN
        UPDATE dbo.Designations SET IsActive = 0 WHERE Id = @Id;
        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
END
GO
-- =============================================================================
-- SP_GetEmployeeInitialData
-- Dropdowns for the Employee create/edit form.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetEmployeeInitialData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetEmployeeInitialData;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetEmployeeInitialData;
GO
CREATE PROCEDURE dbo.SP_GetEmployeeInitialData
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #InitTBL
    (
        companies          NVARCHAR(MAX),
        branches           NVARCHAR(MAX),
        departments        NVARCHAR(MAX),
        designations       NVARCHAR(MAX),
        reportingManagers  NVARCHAR(MAX),
        genders            NVARCHAR(MAX),
        maritalStatuses    NVARCHAR(MAX),
        bloodGroups        NVARCHAR(MAX),
        employmentTypes    NVARCHAR(MAX),
        employmentStatuses NVARCHAR(MAX),
        nextEmployeeCode   NVARCHAR(30)
    );

    DECLARE @Company NVARCHAR(MAX), @Branch NVARCHAR(MAX), @Dept NVARCHAR(MAX), @Desg NVARCHAR(MAX), @Mgr NVARCHAR(MAX), @Gender NVARCHAR(MAX),
            @Marital NVARCHAR(MAX), @Blood NVARCHAR(MAX), @EmpType NVARCHAR(MAX), @EmpStatus NVARCHAR(MAX),
            @Next NVARCHAR(30);

    -- Module 0 — companies and branches for the form's dropdown pair.
    -- Branch carries companyId so the frontend can filter client-side the
    -- same way it already narrows dropdowns from this single payload.
    SET @Company = (
        SELECT LOWER(CAST(Id AS NVARCHAR(36))) AS id, [Name] AS [name]
        FROM dbo.Companies
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY [Name]
        FOR JSON AUTO
    );

    SET @Branch = (
        SELECT LOWER(CAST(Id AS NVARCHAR(36))) AS id,
               LOWER(CAST(CompanyId AS NVARCHAR(36))) AS companyId,
               [Name] AS [name]
        FROM dbo.Branches
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY [Name]
        FOR JSON AUTO
    );

    SET @Dept = (
        SELECT LOWER(CAST(Id AS NVARCHAR(36))) AS id, [Name] AS [name]
        FROM dbo.Departments
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY [Name]
        FOR JSON AUTO
    );

    SET @Desg = (
        SELECT LOWER(CAST(Id AS NVARCHAR(36))) AS id, [Name] AS [name]
        FROM dbo.Designations
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY [Name]
        FOR JSON AUTO
    );

    SET @Mgr = (
        SELECT LOWER(CAST(Id AS NVARCHAR(36))) AS id, FullName AS [name]
        FROM dbo.Employees
        WHERE ISNULL(IsActive, 0) = 1
          AND ISNULL(EmploymentStatus, 1) = 1
        ORDER BY FullName
        FOR JSON AUTO
    );

    -- Integer-based lookups — id cast to NVARCHAR, no casing concern
    SET @Gender = (
        SELECT CAST(Id AS NVARCHAR(10)) AS id, [Name] AS [name]
        FROM dbo.Genders
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY SortOrder
        FOR JSON AUTO
    );

    SET @Marital = (
        SELECT CAST(Id AS NVARCHAR(10)) AS id, [Name] AS [name]
        FROM dbo.MaritalStatuses
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY SortOrder
        FOR JSON AUTO
    );

    SET @Blood = (
        SELECT CAST(Id AS NVARCHAR(10)) AS id, [Name] AS [name]
        FROM dbo.BloodGroups
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY SortOrder
        FOR JSON AUTO
    );

    SET @EmpType = (
        SELECT CAST(Id AS NVARCHAR(10)) AS id, [Name] AS [name]
        FROM dbo.EmploymentTypes
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY SortOrder
        FOR JSON AUTO
    );

    SET @EmpStatus = (
        SELECT CAST(Id AS NVARCHAR(10)) AS id, [Name] AS [name]
        FROM dbo.EmploymentStatuses
        WHERE ISNULL(IsActive, 0) = 1
        ORDER BY SortOrder
        FOR JSON AUTO
    );

    SET @Next = dbo.fn_GetNextEmployeeCode();

    INSERT INTO #InitTBL VALUES (
        @Company, @Branch, @Dept, @Desg, @Mgr, @Gender, @Marital,
        @Blood, @EmpType, @EmpStatus, @Next
    );

    SELECT (SELECT * FROM #InitTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS result;

    DROP TABLE #InitTBL;
END
GO
-- =============================================================================
-- SP_GetAllEmployeesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllEmployeesPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllEmployeesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllEmployeesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllEmployeesPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #EmpTBL
    (
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX)
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Employees E
    LEFT JOIN dbo.Departments  D ON D.Id = E.DepartmentId
    LEFT JOIN dbo.Designations DG ON DG.Id = E.DesignationId
    WHERE (@SearchTerm = ''
           OR E.EmployeeCode  LIKE '%' + @SearchTerm + '%'
           OR E.FullName      LIKE '%' + @SearchTerm + '%'
           OR E.OfficialEmail LIKE '%' + @SearchTerm + '%'
           OR E.MobileNumber  LIKE '%' + @SearchTerm + '%'
           OR D.[Name]        LIKE '%' + @SearchTerm + '%'
           OR DG.[Name]       LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            E.Id, E.EmployeeCode, E.FullName, E.OfficialEmail, E.MobileNumber,
            D.[Name]   AS DepartmentName,
            DG.[Name]  AS DesignationName,
            CO.[Name]  AS CompanyName,
            BR.[Name]  AS BranchName,
            E.DateOfJoining, E.EmploymentStatus,
            CASE E.EmploymentStatus
                WHEN 1 THEN 'Active'
                WHEN 2 THEN 'On Leave'
                WHEN 3 THEN 'Suspended'
                WHEN 4 THEN 'Resigned'
                WHEN 5 THEN 'Terminated'
                WHEN 6 THEN 'Retired'
                ELSE NULL END AS EmploymentStatusName,
            E.PhotoId,
            E.IsActive,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName = 'FullName'      AND @SortColumnDirection = 'ASC'  THEN E.FullName END ASC,
                CASE WHEN @SortColumnName = 'FullName'      AND @SortColumnDirection = 'DESC' THEN E.FullName END DESC,
                CASE WHEN @SortColumnName = 'EmployeeCode'  AND @SortColumnDirection = 'ASC'  THEN E.EmployeeCode END ASC,
                CASE WHEN @SortColumnName = 'EmployeeCode'  AND @SortColumnDirection = 'DESC' THEN E.EmployeeCode END DESC,
                CASE WHEN @SortColumnName = ''              AND @SortColumnDirection = 'ASC'  THEN E.EmployeeCode END ASC,
                CASE WHEN @SortColumnName = ''              AND @SortColumnDirection = 'DESC' THEN E.EmployeeCode END DESC
            ) AS RowNum
        FROM dbo.Employees E
        LEFT JOIN dbo.Departments  D  ON D.Id  = E.DepartmentId
        LEFT JOIN dbo.Designations DG ON DG.Id = E.DesignationId
        LEFT JOIN dbo.Companies    CO ON CO.Id = E.CompanyId
        LEFT JOIN dbo.Branches     BR ON BR.Id = E.BranchId
        WHERE (@SearchTerm = ''
               OR E.EmployeeCode  LIKE '%' + @SearchTerm + '%'
               OR E.FullName      LIKE '%' + @SearchTerm + '%'
               OR E.OfficialEmail LIKE '%' + @SearchTerm + '%'
               OR E.MobileNumber  LIKE '%' + @SearchTerm + '%'
               OR D.[Name]        LIKE '%' + @SearchTerm + '%'
               OR DG.[Name]       LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, EmployeeCode, FullName, OfficialEmail, MobileNumber,
               DepartmentName, DesignationName, CompanyName, BranchName, DateOfJoining,
               EmploymentStatus, EmploymentStatusName, PhotoId, IsActive
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1
                         AND  @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #EmpTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #EmpTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #EmpTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetEmployeeById — returns the full nested employee record as JSON
-- The HR service deserializes the JSON to EmployeeDetailResponse.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetEmployeeById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetEmployeeById;
GO
DROP PROCEDURE IF EXISTS [dbo].[SP_GetEmployeeById];
GO
CREATE PROCEDURE [dbo].[SP_GetEmployeeById] @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare a variable to hold the entire massive JSON payload safely
    DECLARE @EmployeeJson NVARCHAR(MAX);

    SET @EmployeeJson = (
        SELECT
            E.Id, E.EmployeeCode, E.AppUserId, E.OfficialEmail,
            E.FirstName, E.MiddleName, E.LastName, E.FullName,
            E.DateOfBirth, E.Gender, E.MaritalStatus, E.BloodGroup,
            E.Nationality, E.Religion, E.NationalId,
            E.PassportNumber, E.PassportExpiryDate,
            E.PersonalEmail, E.MobileNumber, E.AlternatePhoneNumber,
            E.CompanyId,      CO.[Name]  AS CompanyName,
            E.BranchId,       BR.[Name]  AS BranchName,
            E.DepartmentId,   D.[Name]   AS DepartmentName,
            E.DesignationId,  DG.[Name]  AS DesignationName,
            E.ReportingManagerId, RM.FullName AS ReportingManagerName,
            E.DateOfJoining, E.ProbationEndDate, E.ConfirmationDate, E.DateOfLeaving,
            E.EmploymentType, E.EmploymentStatus, E.WorkLocation,
            E.BasicSalary, E.SalaryCurrency,
            E.PhotoUrl, E.PhotoId, E.SignatureUrl, E.IsActive,

            Addresses        = ISNULL((SELECT Id, AddressType, Line1, Line2, City, State, Country, PostalCode, IsPrimary, IsActive
                                       FROM dbo.EmployeeAddresses           WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Educations       = ISNULL((SELECT Id, Qualification, Institution, University, Specialization, StartYear, EndYear, Grade, DocumentUrl, IsActive
                                       FROM dbo.EmployeeEducations          WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Experiences      = ISNULL((SELECT Id, CompanyName, Designation, StartDate, EndDate, Responsibilities, Location, IsActive
                                       FROM dbo.EmployeeExperiences         WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Skills           = ISNULL((SELECT Id, SkillName, ProficiencyLevel, YearsOfExperience, IsActive
                                       FROM dbo.EmployeeSkills              WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Trainings        = ISNULL((SELECT Id, TrainingName, Provider, StartDate, EndDate, Outcome, CertificateUrl, IsActive
                                       FROM dbo.EmployeeTrainings           WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Certifications   = ISNULL((SELECT Id, CertificationName, IssuingAuthority, CertificationNumber, IssueDate, ExpiryDate, CertificateUrl, IsActive
                                       FROM dbo.EmployeeCertifications      WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            Documents        = ISNULL((SELECT Id, DocumentType, DocumentNumber, FileUrl, IssueDate, ExpiryDate, Remark, IsActive
                                       FROM dbo.EmployeeDocuments           WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            EmergencyContacts= ISNULL((SELECT Id, [Name], Relationship, PrimaryPhone, AlternatePhone, Email, Address, IsPrimary, IsActive
                                       FROM dbo.EmployeeEmergencyContacts   WHERE EmployeeId = E.Id FOR JSON PATH), '[]'),
            BankInfo         = (SELECT TOP 1 Id, BankName, BranchName, AccountHolderName, AccountNumber, IbanNumber, SwiftCode, ISNULL(RoutingNumber,'') AS RoutingNumber, Currency, IsActive
                                       FROM dbo.EmployeeBanks               WHERE EmployeeId = E.Id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)

        FROM dbo.Employees E
        LEFT JOIN dbo.Departments  D  ON D.Id  = E.DepartmentId
        LEFT JOIN dbo.Designations DG ON DG.Id = E.DesignationId
        LEFT JOIN dbo.Employees    RM ON RM.Id = E.ReportingManagerId
        LEFT JOIN dbo.Companies    CO ON CO.Id = E.CompanyId
        LEFT JOIN dbo.Branches     BR ON BR.Id = E.BranchId
        WHERE E.Id = @Id
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    -- Return the complete JSON string cleanly as a single record field
    SELECT @EmployeeJson AS EmployeeData;
END
GO
-- =============================================================================
-- SP_DeleteEmployee — soft delete unless explicitly told to hard delete
-- =============================================================================
IF OBJECT_ID('dbo.SP_DeleteEmployee', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_DeleteEmployee;
GO
DROP PROCEDURE IF EXISTS dbo.SP_DeleteEmployee;
GO
CREATE PROCEDURE dbo.SP_DeleteEmployee
    @Id       UNIQUEIDENTIFIER,
    @IsDelete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @IsDelete = 1
        BEGIN
            -- Cascade FKs will clean up subtables
            DELETE FROM dbo.Employees WHERE Id = @Id;
            SELECT @@ROWCOUNT AS 'RowsAffected';
        END
        ELSE
        BEGIN
            UPDATE dbo.Employees
            SET IsActive = 0,
                EmploymentStatus = 4   -- Resigned
            WHERE Id = @Id;
            SELECT @@ROWCOUNT AS 'RowsAffected';
        END
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        SELECT 0 AS 'RowsAffected', @msg AS 'Message';
    END CATCH
END
GO
-- =============================================================================
-- SP_GetHRSettings — single active row
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetHRSettings', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetHRSettings;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetHRSettings;
GO
CREATE PROCEDURE dbo.SP_GetHRSettings
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        Id, AttendanceSource,
        CASE AttendanceSource WHEN 1 THEN 'Manual Excel Upload'
                              WHEN 2 THEN 'Biometric' END AS AttendanceSourceName,
        BiometricProvider,
        CASE BiometricProvider WHEN 0 THEN 'None'
                               WHEN 1 THEN 'Fingerprint'
                               WHEN 2 THEN 'Face'
                               WHEN 3 THEN 'Iris'
                               WHEN 4 THEN 'Multi'   END AS BiometricProviderName,
        BiometricConnectionString, BiometricSourceObject,
        OfficeStartTime, OfficeEndTime, GracePeriodMinutes,
        WeeklyOffDays, AnnualLeaveDays, SickLeaveDays, CasualLeaveDays,
        AutoProcessTime, AutoProcessEnabled, IsActive, IsHardDelete
    FROM dbo.HRSettings
    WHERE ISNULL(IsActive, 0) = 1
    ORDER BY CreatedDate DESC;
END
GO
GO

-- #####################################################################################################
-- SECTION 3: CoreHR Triggers   (was 03_CoreHR_Triggers.sql)
-- #####################################################################################################
-- Core HR (+ Company/Branch) audit triggers

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
DROP TRIGGER IF EXISTS dbo.TRG_InsertCompanies;
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
DROP TRIGGER IF EXISTS dbo.TRG_UpdateCompanies;
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
DROP TRIGGER IF EXISTS dbo.TRG_DeleteCompanies;
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
DROP TRIGGER IF EXISTS dbo.TRG_InsertBranches;
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
DROP TRIGGER IF EXISTS dbo.TRG_UpdateBranches;
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
DROP TRIGGER IF EXISTS dbo.TRG_DeleteBranches;
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
DROP TRIGGER IF EXISTS dbo.TRG_InsertDepartments;
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
DROP TRIGGER IF EXISTS dbo.TRG_UpdateDepartments;
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
DROP TRIGGER IF EXISTS dbo.TRG_DeleteDepartments;
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
DROP TRIGGER IF EXISTS dbo.TRG_InsertDesignations;
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
DROP TRIGGER IF EXISTS dbo.TRG_UpdateDesignations;
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
DROP TRIGGER IF EXISTS dbo.TRG_DeleteDesignations;
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
DROP TRIGGER IF EXISTS dbo.TRG_InsertEmployees;
GO
CREATE TRIGGER dbo.TRG_InsertEmployees ON dbo.Employees AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving, PhotoId, PhotoLocation,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.CompanyId, I.BranchId, I.DepartmentId, I.DesignationId,
           I.EmploymentStatus, I.DateOfJoining, I.DateOfLeaving, I.PhotoId, I.PhotoLocation,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
           'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateEmployees', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateEmployees;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateEmployees;
GO
CREATE TRIGGER dbo.TRG_UpdateEmployees ON dbo.Employees AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving, PhotoId, PhotoLocation,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), I.Id, I.EmployeeCode, I.FullName, I.OfficialEmail,
           I.CompanyId, I.BranchId, I.DepartmentId, I.DesignationId,
           I.EmploymentStatus, I.DateOfJoining, I.DateOfLeaving, I.PhotoId, I.PhotoLocation,
           I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
           'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_DeleteEmployees', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteEmployees;
GO
DROP TRIGGER IF EXISTS dbo.TRG_DeleteEmployees;
GO
CREATE TRIGGER dbo.TRG_DeleteEmployees ON dbo.Employees AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeesLog
        (Id, EmployeeId, EmployeeCode, FullName, OfficialEmail, CompanyId, BranchId, DepartmentId, DesignationId,
         EmploymentStatus, DateOfJoining, DateOfLeaving, PhotoId, PhotoLocation,
         CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
    SELECT NEWID(), D.Id, D.EmployeeCode, D.FullName, D.OfficialEmail,
           D.CompanyId, D.BranchId, D.DepartmentId, D.DesignationId,
           D.EmploymentStatus, D.DateOfJoining, D.DateOfLeaving, D.PhotoId, D.PhotoLocation,
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
DROP TRIGGER IF EXISTS dbo.TRG_PreventDelete_EmploymentStatuses;
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
DROP TRIGGER IF EXISTS dbo.TRG_PreventDelete_EmploymentTypes;
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
GO

-- #####################################################################################################
-- SECTION 4: ColumnDefaults   (was 09_ColumnDefaults.sql)
-- #####################################################################################################
-- Column DEFAULT constraints the stored procedures rely on (INSERTs that omit
-- these columns). EF maps the columns but does not own the defaults, so EF
-- inserts are unaffected (it always sends explicit values). Idempotent.

IF COL_LENGTH('dbo.ManpowerRequisitions','NumberOfVacancies') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.ManpowerRequisitions') AND c.name = 'NumberOfVacancies')
    ALTER TABLE dbo.ManpowerRequisitions ADD CONSTRAINT DF_MRF_Vacancies DEFAULT (1) FOR [NumberOfVacancies];
IF COL_LENGTH('dbo.ManpowerRequisitions','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.ManpowerRequisitions') AND c.name = 'StatusId')
    ALTER TABLE dbo.ManpowerRequisitions ADD CONSTRAINT DF_MRF_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.JobPostings','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.JobPostings') AND c.name = 'StatusId')
    ALTER TABLE dbo.JobPostings ADD CONSTRAINT DF_JobPostings_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.JobApplications','StageId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.JobApplications') AND c.name = 'StageId')
    ALTER TABLE dbo.JobApplications ADD CONSTRAINT DF_JobApplications_Stage DEFAULT (1) FOR [StageId];
IF COL_LENGTH('dbo.Interviews','RoundNumber') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Interviews') AND c.name = 'RoundNumber')
    ALTER TABLE dbo.Interviews ADD CONSTRAINT DF_Interviews_Round DEFAULT (1) FOR [RoundNumber];
IF COL_LENGTH('dbo.JobOffers','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.JobOffers') AND c.name = 'StatusId')
    ALTER TABLE dbo.JobOffers ADD CONSTRAINT DF_JobOffers_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Shifts','IsNightShift') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Shifts') AND c.name = 'IsNightShift')
    ALTER TABLE dbo.Shifts ADD CONSTRAINT DF_Shifts_Night DEFAULT (0) FOR [IsNightShift];
IF COL_LENGTH('dbo.Shifts','BreakMinutes') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Shifts') AND c.name = 'BreakMinutes')
    ALTER TABLE dbo.Shifts ADD CONSTRAINT DF_Shifts_Break DEFAULT (0) FOR [BreakMinutes];
IF COL_LENGTH('dbo.Shifts','GraceInMinutes') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Shifts') AND c.name = 'GraceInMinutes')
    ALTER TABLE dbo.Shifts ADD CONSTRAINT DF_Shifts_GraceIn DEFAULT (0) FOR [GraceInMinutes];
IF COL_LENGTH('dbo.Shifts','GraceOutMinutes') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Shifts') AND c.name = 'GraceOutMinutes')
    ALTER TABLE dbo.Shifts ADD CONSTRAINT DF_Shifts_GraceOut DEFAULT (0) FOR [GraceOutMinutes];
IF COL_LENGTH('dbo.OvertimeRequests','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.OvertimeRequests') AND c.name = 'StatusId')
    ALTER TABLE dbo.OvertimeRequests ADD CONSTRAINT DF_OT_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.AttendanceRegularizationRequests','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.AttendanceRegularizationRequests') AND c.name = 'StatusId')
    ALTER TABLE dbo.AttendanceRegularizationRequests ADD CONSTRAINT DF_Reg_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.LeaveTypes','IsPaid') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveTypes') AND c.name = 'IsPaid')
    ALTER TABLE dbo.LeaveTypes ADD CONSTRAINT DF_LeaveTypes_Paid DEFAULT (1) FOR [IsPaid];
IF COL_LENGTH('dbo.LeaveTypes','AllowHalfDay') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveTypes') AND c.name = 'AllowHalfDay')
    ALTER TABLE dbo.LeaveTypes ADD CONSTRAINT DF_LeaveTypes_Half DEFAULT (1) FOR [AllowHalfDay];
IF COL_LENGTH('dbo.LeaveTypes','RequiresAttachment') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveTypes') AND c.name = 'RequiresAttachment')
    ALTER TABLE dbo.LeaveTypes ADD CONSTRAINT DF_LeaveTypes_Att DEFAULT (0) FOR [RequiresAttachment];
IF COL_LENGTH('dbo.LeaveTypes','IsCarryForward') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveTypes') AND c.name = 'IsCarryForward')
    ALTER TABLE dbo.LeaveTypes ADD CONSTRAINT DF_LeaveTypes_CF DEFAULT (0) FOR [IsCarryForward];
IF COL_LENGTH('dbo.LeaveBalances','Entitled') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveBalances') AND c.name = 'Entitled')
    ALTER TABLE dbo.LeaveBalances ADD CONSTRAINT DF_LB_Entitled DEFAULT (0) FOR [Entitled];
IF COL_LENGTH('dbo.LeaveBalances','CarriedForward') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveBalances') AND c.name = 'CarriedForward')
    ALTER TABLE dbo.LeaveBalances ADD CONSTRAINT DF_LB_CF DEFAULT (0) FOR [CarriedForward];
IF COL_LENGTH('dbo.LeaveBalances','Used') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveBalances') AND c.name = 'Used')
    ALTER TABLE dbo.LeaveBalances ADD CONSTRAINT DF_LB_Used DEFAULT (0) FOR [Used];
IF COL_LENGTH('dbo.LeaveBalances','Pending') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveBalances') AND c.name = 'Pending')
    ALTER TABLE dbo.LeaveBalances ADD CONSTRAINT DF_LB_Pending DEFAULT (0) FOR [Pending];
IF COL_LENGTH('dbo.LeaveApplications','IsHalfDay') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveApplications') AND c.name = 'IsHalfDay')
    ALTER TABLE dbo.LeaveApplications ADD CONSTRAINT DF_LA_Half DEFAULT (0) FOR [IsHalfDay];
IF COL_LENGTH('dbo.LeaveApplications','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.LeaveApplications') AND c.name = 'StatusId')
    ALTER TABLE dbo.LeaveApplications ADD CONSTRAINT DF_LA_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.PayComponents','CalcMethod') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.PayComponents') AND c.name = 'CalcMethod')
    ALTER TABLE dbo.PayComponents ADD CONSTRAINT DF_PC_Calc DEFAULT (1) FOR [CalcMethod];
IF COL_LENGTH('dbo.PayComponents','IsTaxable') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.PayComponents') AND c.name = 'IsTaxable')
    ALTER TABLE dbo.PayComponents ADD CONSTRAINT DF_PC_Tax DEFAULT (1) FOR [IsTaxable];
IF COL_LENGTH('dbo.PayComponents','IsProrated') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.PayComponents') AND c.name = 'IsProrated')
    ALTER TABLE dbo.PayComponents ADD CONSTRAINT DF_PC_Prorate DEFAULT (1) FOR [IsProrated];
IF COL_LENGTH('dbo.PayrollRuns','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.PayrollRuns') AND c.name = 'StatusId')
    ALTER TABLE dbo.PayrollRuns ADD CONSTRAINT DF_PR_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Payslips','LopDays') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Payslips') AND c.name = 'LopDays')
    ALTER TABLE dbo.Payslips ADD CONSTRAINT DF_PS_Lop DEFAULT (0) FOR [LopDays];
IF COL_LENGTH('dbo.Payslips','LoanDeduction') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Payslips') AND c.name = 'LoanDeduction')
    ALTER TABLE dbo.Payslips ADD CONSTRAINT DF_PS_Loan DEFAULT (0) FOR [LoanDeduction];
IF COL_LENGTH('dbo.Loans','PaidInstallments') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Loans') AND c.name = 'PaidInstallments')
    ALTER TABLE dbo.Loans ADD CONSTRAINT DF_Loan_Paid DEFAULT (0) FOR [PaidInstallments];
IF COL_LENGTH('dbo.Loans','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Loans') AND c.name = 'StatusId')
    ALTER TABLE dbo.Loans ADD CONSTRAINT DF_Loan_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.AppraisalCycles','RatingScaleMax') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.AppraisalCycles') AND c.name = 'RatingScaleMax')
    ALTER TABLE dbo.AppraisalCycles ADD CONSTRAINT DF_AC_Scale DEFAULT (5) FOR [RatingScaleMax];
IF COL_LENGTH('dbo.AppraisalCycles','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.AppraisalCycles') AND c.name = 'StatusId')
    ALTER TABLE dbo.AppraisalCycles ADD CONSTRAINT DF_AC_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Goals','Weight') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Goals') AND c.name = 'Weight')
    ALTER TABLE dbo.Goals ADD CONSTRAINT DF_Goal_Weight DEFAULT (0) FOR [Weight];
IF COL_LENGTH('dbo.Goals','ProgressPercent') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Goals') AND c.name = 'ProgressPercent')
    ALTER TABLE dbo.Goals ADD CONSTRAINT DF_Goal_Prog DEFAULT (0) FOR [ProgressPercent];
IF COL_LENGTH('dbo.Goals','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Goals') AND c.name = 'StatusId')
    ALTER TABLE dbo.Goals ADD CONSTRAINT DF_Goal_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Reviews','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Reviews') AND c.name = 'StatusId')
    ALTER TABLE dbo.Reviews ADD CONSTRAINT DF_Rev_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.ReviewRatings','Weight') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.ReviewRatings') AND c.name = 'Weight')
    ALTER TABLE dbo.ReviewRatings ADD CONSTRAINT DF_RR_Weight DEFAULT (0) FOR [Weight];
IF COL_LENGTH('dbo.Separations','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Separations') AND c.name = 'StatusId')
    ALTER TABLE dbo.Separations ADD CONSTRAINT DF_Sep_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Separations','IsNoticeWaived') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Separations') AND c.name = 'IsNoticeWaived')
    ALTER TABLE dbo.Separations ADD CONSTRAINT DF_Sep_Waived DEFAULT (0) FOR [IsNoticeWaived];
IF COL_LENGTH('dbo.Separations','ExitInterviewDone') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Separations') AND c.name = 'ExitInterviewDone')
    ALTER TABLE dbo.Separations ADD CONSTRAINT DF_Sep_Exit DEFAULT (0) FOR [ExitInterviewDone];
IF COL_LENGTH('dbo.SeparationClearances','ClearanceStatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.SeparationClearances') AND c.name = 'ClearanceStatusId')
    ALTER TABLE dbo.SeparationClearances ADD CONSTRAINT DF_Clr_Status DEFAULT (1) FOR [ClearanceStatusId];
IF COL_LENGTH('dbo.Onboardings','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Onboardings') AND c.name = 'StatusId')
    ALTER TABLE dbo.Onboardings ADD CONSTRAINT DF_Onboardings_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.Onboardings','ChecklistTotal') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Onboardings') AND c.name = 'ChecklistTotal')
    ALTER TABLE dbo.Onboardings ADD CONSTRAINT DF_Onboardings_Total DEFAULT (0) FOR [ChecklistTotal];
IF COL_LENGTH('dbo.Onboardings','ChecklistDone') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.Onboardings') AND c.name = 'ChecklistDone')
    ALTER TABLE dbo.Onboardings ADD CONSTRAINT DF_Onboardings_Done DEFAULT (0) FOR [ChecklistDone];
IF COL_LENGTH('dbo.OnboardingTasks','StatusId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.OnboardingTasks') AND c.name = 'StatusId')
    ALTER TABLE dbo.OnboardingTasks ADD CONSTRAINT DF_OnboardingTasks_Status DEFAULT (1) FOR [StatusId];
IF COL_LENGTH('dbo.IntegrationOutbox','OccurredOn') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.IntegrationOutbox') AND c.name = 'OccurredOn')
    ALTER TABLE dbo.IntegrationOutbox ADD CONSTRAINT DF_Outbox_Occurred DEFAULT (GETUTCDATE()) FOR [OccurredOn];
IF COL_LENGTH('dbo.IntegrationOutbox','Attempts') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id WHERE dc.parent_object_id = OBJECT_ID('dbo.IntegrationOutbox') AND c.name = 'Attempts')
    ALTER TABLE dbo.IntegrationOutbox ADD CONSTRAINT DF_Outbox_Attempts DEFAULT (0) FOR [Attempts];
GO
GO

-- #####################################################################################################
-- SECTION 5: OrgSetup Objects   (was 10_OrgSetup_Objects.sql)
-- #####################################################################################################
-- Module 0 Organization Setup — Cost Center / Job Grade / Holiday / Position objects

-- =============================================================================
-- FUNCTIONS — code generators (prefix + zero-padded next number)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextOrgCode','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetNextOrgCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextOrgCode;
GO
CREATE FUNCTION dbo.fn_GetNextOrgCode(@Prefix NVARCHAR(10), @CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- fn_IsHoliday — is a given date a holiday in a given calendar? (used by Leave/Attendance)
IF OBJECT_ID('dbo.fn_IsHoliday','FN') IS NOT NULL DROP FUNCTION dbo.fn_IsHoliday;
GO
DROP FUNCTION IF EXISTS dbo.fn_IsHoliday;
GO
CREATE FUNCTION dbo.fn_IsHoliday(@CalendarId UNIQUEIDENTIFIER, @Date DATETIME)
RETURNS BIT
AS
BEGIN
    DECLARE @hit BIT = 0;
    IF EXISTS (
        SELECT 1 FROM dbo.HolidayDates
        WHERE HolidayCalendarId = @CalendarId AND IsActive = 1
          AND ( CAST(HolidayDate AS DATE) = CAST(@Date AS DATE)
                OR (IsRecurring = 1
                    AND MONTH(HolidayDate) = MONTH(@Date)
                    AND DAY(HolidayDate)   = DAY(@Date)) )
    )
        SET @hit = 1;
    RETURN @hit;
END
GO
IF OBJECT_ID('dbo.TRG_InsertCostCenters','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertCostCenters;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertCostCenters;
GO
CREATE TRIGGER dbo.TRG_InsertCostCenters ON dbo.CostCenters AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CostCentersLog (Id,CostCenterId,CostCenterCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CostCenterCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created' FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateCostCenters','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateCostCenters;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateCostCenters;
GO
CREATE TRIGGER dbo.TRG_UpdateCostCenters ON dbo.CostCenters AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CostCentersLog (Id,CostCenterId,CostCenterCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CostCenterCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated' FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertJobGrades','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertJobGrades;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertJobGrades;
GO
CREATE TRIGGER dbo.TRG_InsertJobGrades ON dbo.JobGrades AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.JobGradesLog (Id,JobGradeId,GradeCode,[Name],GradeLevel,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.GradeCode,I.[Name],I.GradeLevel,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created' FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateJobGrades','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateJobGrades;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateJobGrades;
GO
CREATE TRIGGER dbo.TRG_UpdateJobGrades ON dbo.JobGrades AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.JobGradesLog (Id,JobGradeId,GradeCode,[Name],GradeLevel,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.GradeCode,I.[Name],I.GradeLevel,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated' FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertHolidayCalendars','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertHolidayCalendars;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertHolidayCalendars;
GO
CREATE TRIGGER dbo.TRG_InsertHolidayCalendars ON dbo.HolidayCalendars AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.HolidayCalendarsLog (Id,HolidayCalendarId,CalendarCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CalendarCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created' FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateHolidayCalendars','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateHolidayCalendars;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateHolidayCalendars;
GO
CREATE TRIGGER dbo.TRG_UpdateHolidayCalendars ON dbo.HolidayCalendars AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.HolidayCalendarsLog (Id,HolidayCalendarId,CalendarCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CalendarCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated' FROM inserted I;
END
GO

-- =============================================================================
-- SP_GetAllCostCentersPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllCostCentersPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllCostCentersPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllCostCentersPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllCostCentersPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC', @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.CostCenters CC WHERE CC.IsActive=1 AND (@SearchTerm='' OR CC.CostCenterCode LIKE '%'+@SearchTerm+'%' OR CC.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT CC.Id,CC.CostCenterCode,CC.[Name],CC.CompanyId,co.[Name] AS CompanyName,CC.BranchId,br.[Name] AS BranchName,
               CC.ParentCostCenterId,p.[Name] AS ParentName,CC.OwnerEmployeeId,e.FullName AS OwnerName,CC.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='CostCenterCode' AND @SortColumnDirection='ASC'  THEN CC.CostCenterCode END ASC,
                 CASE WHEN @SortColumnName='CostCenterCode' AND @SortColumnDirection='DESC' THEN CC.CostCenterCode END DESC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='ASC'  THEN CC.[Name] END ASC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='DESC' THEN CC.[Name] END DESC) AS RowNum
        FROM dbo.CostCenters CC
        LEFT JOIN dbo.Companies co ON co.Id=CC.CompanyId
        LEFT JOIN dbo.Branches br ON br.Id=CC.BranchId
        LEFT JOIN dbo.CostCenters p ON p.Id=CC.ParentCostCenterId
        LEFT JOIN dbo.Employees e ON e.Id=CC.OwnerEmployeeId
        WHERE CC.IsActive=1 AND (@SearchTerm='' OR CC.CostCenterCode LIKE '%'+@SearchTerm+'%' OR CC.[Name] LIKE '%'+@SearchTerm+'%'))
    SELECT @Items=(SELECT Id,CostCenterCode,[Name],CompanyId,CompanyName,BranchId,BranchName,ParentCostCenterId,ParentName,OwnerEmployeeId,OwnerName,IsActive FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER); DROP TABLE #T; SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllJobGradesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllJobGradesPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllJobGradesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllJobGradesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllJobGradesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC', @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.JobGrades G WHERE G.IsActive=1 AND (@SearchTerm='' OR G.GradeCode LIKE '%'+@SearchTerm+'%' OR G.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT G.Id,G.GradeCode,G.[Name],G.GradeLevel,G.Band,G.RefMinSalary,G.RefMidSalary,G.RefMaxSalary,G.CurrencyCode,G.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='GradeLevel' AND @SortColumnDirection='ASC'  THEN G.GradeLevel END ASC,
                 CASE WHEN @SortColumnName='GradeLevel' AND @SortColumnDirection='DESC' THEN G.GradeLevel END DESC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='ASC'  THEN G.GradeLevel END ASC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='DESC' THEN G.GradeLevel END DESC) AS RowNum
        FROM dbo.JobGrades G WHERE G.IsActive=1 AND (@SearchTerm='' OR G.GradeCode LIKE '%'+@SearchTerm+'%' OR G.[Name] LIKE '%'+@SearchTerm+'%'))
    SELECT @Items=(SELECT Id,GradeCode,[Name],GradeLevel,Band,RefMinSalary,RefMidSalary,RefMaxSalary,CurrencyCode,IsActive FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER); DROP TABLE #T; SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllHolidayCalendarsPagingWithSearch  (+ holiday count)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllHolidayCalendarsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllHolidayCalendarsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllHolidayCalendarsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllHolidayCalendarsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC', @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.HolidayCalendars H WHERE H.IsActive=1 AND (@SearchTerm='' OR H.CalendarCode LIKE '%'+@SearchTerm+'%' OR H.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT H.Id,H.CalendarCode,H.[Name],H.[Year],H.Country,H.IsActive,
               (SELECT COUNT(1) FROM dbo.HolidayDates D WHERE D.HolidayCalendarId=H.Id AND D.IsActive=1) AS HolidayCount,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='CalendarCode' AND @SortColumnDirection='ASC'  THEN H.CalendarCode END ASC,
                 CASE WHEN @SortColumnName='CalendarCode' AND @SortColumnDirection='DESC' THEN H.CalendarCode END DESC,
                 CASE WHEN @SortColumnName=''             AND @SortColumnDirection='ASC'  THEN H.[Year] END ASC,
                 CASE WHEN @SortColumnName=''             AND @SortColumnDirection='DESC' THEN H.[Year] END DESC) AS RowNum
        FROM dbo.HolidayCalendars H WHERE H.IsActive=1 AND (@SearchTerm='' OR H.CalendarCode LIKE '%'+@SearchTerm+'%' OR H.[Name] LIKE '%'+@SearchTerm+'%'))
    SELECT @Items=(SELECT Id,CalendarCode,[Name],[Year],Country,HolidayCount,IsActive FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER); DROP TABLE #T; SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetHolidayDatesByCalendar — child list for a calendar (JSON array).
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetHolidayDatesByCalendar','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetHolidayDatesByCalendar;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetHolidayDatesByCalendar;
GO
CREATE PROCEDURE dbo.SP_GetHolidayDatesByCalendar @CalendarId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT D.Id,D.[Name],D.HolidayDate,D.HolidayTypeId,HT.[Name] AS HolidayTypeName,D.IsRecurring,D.[Description]
        FROM dbo.HolidayDates D INNER JOIN dbo.HolidayTypes HT ON HT.Id=D.HolidayTypeId
        WHERE D.HolidayCalendarId=@CalendarId AND D.IsActive=1
        ORDER BY D.HolidayDate FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_GetAllPositionsPagingWithSearch — with joined names + headcount summary.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllPositionsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllPositionsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllPositionsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllPositionsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC', @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.Positions P WHERE P.IsActive=1 AND (@SearchTerm='' OR P.PositionCode LIKE '%'+@SearchTerm+'%' OR P.Title LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT P.Id,P.PositionCode,P.Title,P.DesignationId,dsg.[Name] AS DesignationName,
               P.CompanyId,co.[Name] AS CompanyName,P.BranchId,br.[Name] AS BranchName,
               P.CostCenterId,cc.[Name] AS CostCenterName,P.JobGradeId,g.[Name] AS GradeName,
               P.DepartmentId,dp.[Name] AS DepartmentName,P.ReportsToPositionId,rp.Title AS ReportsToTitle,
               P.BudgetedHeadcount,P.FilledHeadcount,(P.BudgetedHeadcount-P.FilledHeadcount) AS OpenHeadcount,
               P.StatusId,ps.[Name] AS StatusName,P.Grade,P.EmploymentType,P.WorkLocation,P.MinSalary,P.MaxSalary,P.SalaryCurrency,P.EffectiveFrom,P.EffectiveTo,P.[Description],P.IsActive,P.CreatedDate,P.UpdatedDate,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='PositionCode' AND @SortColumnDirection='ASC'  THEN P.PositionCode END ASC,
                 CASE WHEN @SortColumnName='PositionCode' AND @SortColumnDirection='DESC' THEN P.PositionCode END DESC,
                 CASE WHEN @SortColumnName=''             AND @SortColumnDirection='ASC'  THEN P.Title END ASC,
                 CASE WHEN @SortColumnName=''             AND @SortColumnDirection='DESC' THEN P.Title END DESC) AS RowNum
        FROM dbo.Positions P
        LEFT JOIN dbo.Designations dsg ON dsg.Id=P.DesignationId
        LEFT JOIN dbo.Companies co ON co.Id=P.CompanyId
        LEFT JOIN dbo.Branches br ON br.Id=P.BranchId
        LEFT JOIN dbo.CostCenters cc ON cc.Id=P.CostCenterId
        LEFT JOIN dbo.JobGrades g ON g.Id=P.JobGradeId
        LEFT JOIN dbo.Departments dp ON dp.Id=P.DepartmentId
        LEFT JOIN dbo.Positions rp ON rp.Id=P.ReportsToPositionId
        INNER JOIN dbo.PositionStatuses ps ON ps.Id=P.StatusId
        WHERE P.IsActive=1 AND (@SearchTerm='' OR P.PositionCode LIKE '%'+@SearchTerm+'%' OR P.Title LIKE '%'+@SearchTerm+'%'))
    SELECT @Items=(SELECT Id,PositionCode,Title,DesignationId,DesignationName,CompanyId,CompanyName,BranchId,BranchName,CostCenterId,CostCenterName,JobGradeId,GradeName,DepartmentId,DepartmentName,ReportsToPositionId,ReportsToTitle,BudgetedHeadcount,FilledHeadcount,OpenHeadcount,StatusId,StatusName,Grade,EmploymentType,WorkLocation,MinSalary,MaxSalary,SalaryCurrency,EffectiveFrom,EffectiveTo,[Description],IsActive,CreatedDate,UpdatedDate FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER); DROP TABLE #T; SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_SetPositionFill — assign (+1) or release (-1) headcount on a position and
-- auto-flip status: Filled when FilledHeadcount >= Budgeted, else Open. Guards
-- against over-fill and negative. @Delta = +1 assign / -1 release.
-- Recruitment/Onboarding call +1 when an employee occupies the seat; Separation
-- calls -1 on exit.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetPositionFill','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetPositionFill;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetPositionFill;
GO
CREATE PROCEDURE dbo.SP_SetPositionFill @PositionId UNIQUEIDENTIFIER, @Delta INT, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @Budget INT,@Filled INT,@Status INT;
        SELECT @Budget=BudgetedHeadcount,@Filled=FilledHeadcount,@Status=StatusId FROM dbo.Positions WHERE Id=@PositionId;
        IF @Budget IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Position not found.' AS [Message]; RETURN; END

        DECLARE @NewFilled INT = @Filled + @Delta;
        IF @NewFilled < 0 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Filled count cannot go below zero.' AS [Message]; RETURN; END
        IF @NewFilled > @Budget BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Assigning would exceed budgeted headcount.' AS [Message]; RETURN; END

        DECLARE @NewStatus INT = CASE
            WHEN @Status IN (4,5) THEN @Status                 -- respect OnHold/Closed
            WHEN @NewFilled >= @Budget THEN 3                  -- Filled
            ELSE 2 END;                                        -- Open

        UPDATE dbo.Positions SET FilledHeadcount=@NewFilled, StatusId=@NewStatus, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE() WHERE Id=@PositionId;
        COMMIT;
        SELECT 1 AS RowsAffected, CONCAT('Position headcount updated (',@NewFilled,'/',@Budget,').') AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetPositionStatus — Draft/Open/OnHold/Closed transitions (manual).
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetPositionStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetPositionStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetPositionStatus;
GO
CREATE PROCEDURE dbo.SP_SetPositionStatus @Id UNIQUEIDENTIFIER, @StatusId INT, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Positions WHERE Id=@Id) BEGIN SELECT 0 AS RowsAffected,'Position not found.' AS [Message]; RETURN; END
        UPDATE dbo.Positions SET StatusId=@StatusId, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE() WHERE Id=@Id;
        SELECT 1 AS RowsAffected,'Position status updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_GetPositionOrgTree — position-to-position reporting tree (flat, depth-ordered
-- JSON array; the SPA nests it). @RootPositionId NULL => top-level positions.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetPositionOrgTree','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetPositionOrgTree;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetPositionOrgTree;
GO
CREATE PROCEDURE dbo.SP_GetPositionOrgTree @RootPositionId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH Tree AS (
        SELECT P.Id,P.PositionCode,P.Title,P.ReportsToPositionId,P.BudgetedHeadcount,P.FilledHeadcount,P.StatusId,0 AS Depth,
               CAST(RIGHT('0000000000'+CAST(ROW_NUMBER() OVER (ORDER BY P.Title) AS VARCHAR(10)),10) AS VARCHAR(MAX)) AS SortPath
        FROM dbo.Positions P
        WHERE P.IsActive=1 AND ((@RootPositionId IS NULL AND (P.ReportsToPositionId IS NULL
                    OR NOT EXISTS (SELECT 1 FROM dbo.Positions M WHERE M.Id=P.ReportsToPositionId AND M.IsActive=1)))
               OR (@RootPositionId IS NOT NULL AND P.Id=@RootPositionId))
        UNION ALL
        SELECT C.Id,C.PositionCode,C.Title,C.ReportsToPositionId,C.BudgetedHeadcount,C.FilledHeadcount,C.StatusId,T.Depth+1,
               T.SortPath+'.'+RIGHT('0000000000'+CAST(ROW_NUMBER() OVER (ORDER BY C.Title) AS VARCHAR(10)),10)
        FROM dbo.Positions C INNER JOIN Tree T ON C.ReportsToPositionId=T.Id WHERE C.IsActive=1)
    SELECT (SELECT T.Id,T.PositionCode,T.Title,T.Depth,T.ReportsToPositionId AS ManagerId,
                   T.BudgetedHeadcount,T.FilledHeadcount,ps.[Name] AS StatusName
            FROM Tree T INNER JOIN dbo.PositionStatuses ps ON ps.Id=T.StatusId
            ORDER BY T.SortPath FOR JSON PATH) AS result
    OPTION (MAXRECURSION 100);
END
GO
GO

-- #####################################################################################################
-- SECTION 6: Recruitment StoredProcedures   (was 11b_Recruitment_StoredProcedures.sql)
-- #####################################################################################################
-- Recruitment: stored procedures

-- =============================================================================
-- SP_GetAllRequisitionsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllRequisitionsPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllRequisitionsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllRequisitionsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllRequisitionsPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #MrfTBL ([RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX));
    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.ManpowerRequisitions R
    WHERE (@SearchTerm = '' OR R.RequisitionCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            R.Id, R.RequisitionCode, R.NumberOfVacancies, R.RequiredByDate, R.IsReplacement,
            R.StatusId, RS.[Name] AS StatusName,
            P.Title AS PositionTitle, D.[Name] AS DepartmentName, DG.[Name] AS DesignationName,
            (SELECT COUNT(1) FROM dbo.JobPostings JP WHERE JP.ManpowerRequisitionId = R.Id) AS PostingCount,
            R.CreatedDate, R.UpdatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName='RequisitionCode' AND @SortColumnDirection='ASC'  THEN R.RequisitionCode END ASC,
                CASE WHEN @SortColumnName='RequisitionCode' AND @SortColumnDirection='DESC' THEN R.RequisitionCode END DESC,
                CASE WHEN @SortColumnName=''                AND @SortColumnDirection='ASC'  THEN R.CreatedDate END ASC,
                CASE WHEN @SortColumnName=''                AND @SortColumnDirection='DESC' THEN R.CreatedDate END DESC
            ) AS RowNum
        FROM dbo.ManpowerRequisitions R
        INNER JOIN dbo.RequisitionStatuses RS ON RS.Id = R.StatusId
        LEFT JOIN dbo.Positions    P  ON P.Id  = R.PositionId
        LEFT JOIN dbo.Departments  D  ON D.Id  = R.DepartmentId
        LEFT JOIN dbo.Designations DG ON DG.Id = R.DesignationId
        WHERE (@SearchTerm = '' OR R.RequisitionCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, RequisitionCode, NumberOfVacancies, RequiredByDate, IsReplacement,
               StatusId, StatusName, PositionTitle, DepartmentName, DesignationName, PostingCount
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
        FOR JSON AUTO
    );

    INSERT INTO #MrfTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #MrfTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #MrfTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllJobPostingsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllJobPostingsPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllJobPostingsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllJobPostingsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllJobPostingsPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #JpTBL ([RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX));
    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.JobPostings JP
    WHERE (@SearchTerm = '' OR JP.Title LIKE '%' + @SearchTerm + '%' OR JP.PostingCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            JP.Id, JP.PostingCode, JP.Title, JP.WorkLocation, JP.IsRemote, JP.PostedDate, JP.ClosingDate,
            JP.StatusId, PS.[Name] AS StatusName,
            dbo.fn_CountApplicantsByPosting(JP.Id) AS ApplicantCount,
            JP.CreatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName='Title' AND @SortColumnDirection='ASC'  THEN JP.Title END ASC,
                CASE WHEN @SortColumnName='Title' AND @SortColumnDirection='DESC' THEN JP.Title END DESC,
                CASE WHEN @SortColumnName=''      AND @SortColumnDirection='ASC'  THEN JP.CreatedDate END ASC,
                CASE WHEN @SortColumnName=''      AND @SortColumnDirection='DESC' THEN JP.CreatedDate END DESC
            ) AS RowNum
        FROM dbo.JobPostings JP
        INNER JOIN dbo.JobPostingStatuses PS ON PS.Id = JP.StatusId
        WHERE (@SearchTerm = '' OR JP.Title LIKE '%' + @SearchTerm + '%' OR JP.PostingCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, PostingCode, Title, WorkLocation, IsRemote, PostedDate, ClosingDate,
               StatusId, StatusName, ApplicantCount
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
        FOR JSON AUTO
    );

    INSERT INTO #JpTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #JpTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #JpTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllCandidatesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllCandidatesPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllCandidatesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllCandidatesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllCandidatesPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #CanTBL ([RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX));
    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Candidates C
    WHERE (@SearchTerm = '' OR C.FullName LIKE '%' + @SearchTerm + '%'
           OR C.Email LIKE '%' + @SearchTerm + '%' OR C.CandidateCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            C.Id, C.CandidateCode, C.FullName, C.Email, C.Phone, C.TotalExperienceYears,
            C.CurrentEmployer, C.ExpectedSalary, C.Source, C.IsBlacklisted, C.ResumeUrl,
            (SELECT COUNT(1) FROM dbo.JobApplications A WHERE A.CandidateId = C.Id) AS ApplicationCount,
            C.CreatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName='FullName' AND @SortColumnDirection='ASC'  THEN C.FullName END ASC,
                CASE WHEN @SortColumnName='FullName' AND @SortColumnDirection='DESC' THEN C.FullName END DESC,
                CASE WHEN @SortColumnName=''         AND @SortColumnDirection='ASC'  THEN C.CreatedDate END ASC,
                CASE WHEN @SortColumnName=''         AND @SortColumnDirection='DESC' THEN C.CreatedDate END DESC
            ) AS RowNum
        FROM dbo.Candidates C
        WHERE (@SearchTerm = '' OR C.FullName LIKE '%' + @SearchTerm + '%'
               OR C.Email LIKE '%' + @SearchTerm + '%' OR C.CandidateCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, CandidateCode, FullName, Email, Phone, TotalExperienceYears,
               CurrentEmployer, ExpectedSalary, Source, IsBlacklisted, ResumeUrl, ApplicationCount
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
        FOR JSON AUTO
    );

    INSERT INTO #CanTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #CanTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #CanTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetApplicationsByPosting — pipeline cards (single JSON array in 'result')
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetApplicationsByPosting', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetApplicationsByPosting;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetApplicationsByPosting;
GO
CREATE PROCEDURE dbo.SP_GetApplicationsByPosting
    @JobPostingId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
    (
        SELECT A.Id, A.ApplicationCode, A.StageId, ST.[Name] AS StageName, A.AppliedDate, A.ScreeningScore,
               C.Id AS CandidateId, C.FullName AS CandidateName, C.Email, C.Phone,
               C.TotalExperienceYears, C.ExpectedSalary, C.ResumeUrl, C.Source
        FROM dbo.JobApplications A
        INNER JOIN dbo.Candidates C        ON C.Id  = A.CandidateId
        INNER JOIN dbo.ApplicationStages ST ON ST.Id = A.StageId
        WHERE A.JobPostingId = @JobPostingId
        ORDER BY A.AppliedDate DESC
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_GetInterviewsByApplication — rounds + panel
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetInterviewsByApplication', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetInterviewsByApplication;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetInterviewsByApplication;
GO
CREATE PROCEDURE dbo.SP_GetInterviewsByApplication
    @JobApplicationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
    (
        SELECT I.Id, I.RoundNumber, I.RoundName, I.InterviewModeId, IM.[Name] AS ModeName,
               I.ScheduledAt, I.DurationMinutes, I.[Location], I.MeetingLink,
               I.OverallRating, I.Recommendation, I.IsCompleted,
               (SELECT PM.InterviewerEmployeeId, E.FullName AS InterviewerName, PM.IsPanelLead, PM.IndividualRating
                FROM dbo.InterviewPanelMembers PM
                LEFT JOIN dbo.Employees E ON E.Id = PM.InterviewerEmployeeId
                WHERE PM.InterviewId = I.Id FOR JSON PATH) AS Panel
        FROM dbo.Interviews I
        LEFT JOIN dbo.InterviewModes IM ON IM.Id = I.InterviewModeId
        WHERE I.JobApplicationId = @JobApplicationId
        ORDER BY I.RoundNumber
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_SaveCandidateApplication — capture applicant + application in one tx.
-- Reuses candidate by email; blocks duplicate application to same posting.
-- Multi-table workflow, so it stays an SP (CRUD on single tables uses EF).
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveCandidateApplication', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_SaveCandidateApplication;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveCandidateApplication;
GO
CREATE PROCEDURE dbo.SP_SaveCandidateApplication
    @JobPostingId         UNIQUEIDENTIFIER,
    @FirstName            NVARCHAR(100),
    @LastName             NVARCHAR(100),
    @Email                NVARCHAR(200),
    @Phone                NVARCHAR(30)     = NULL,
    @TotalExperienceYears DECIMAL(4,1)     = NULL,
    @CurrentEmployer      NVARCHAR(200)    = NULL,
    @ExpectedSalary       DECIMAL(18,2)    = NULL,
    @ResumeUrl            NVARCHAR(500)    = NULL,
    @Source               NVARCHAR(100)    = NULL,
    @ReferredByEmployeeId UNIQUEIDENTIFIER = NULL,
    @CreatedBy            NVARCHAR(450)    = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        DECLARE @CandidateId UNIQUEIDENTIFIER;
        SELECT TOP 1 @CandidateId = Id FROM dbo.Candidates WHERE Email = @Email AND ISNULL(IsActive,1) = 1;

        IF @CandidateId IS NULL
        BEGIN
            SET @CandidateId = NEWID();
            DECLARE @ccode NVARCHAR(30) =
                dbo.fn_GetNextRecruitmentCode('CAN', (SELECT MAX(CandidateCode) FROM dbo.Candidates WHERE CandidateCode LIKE 'CAN-%'));
            INSERT INTO dbo.Candidates
                (Id, CandidateCode, FirstName, LastName, FullName, Email, Phone, TotalExperienceYears,
                 CurrentEmployer, ExpectedSalary, ResumeUrl, Source, ReferredByEmployeeId,
                 CreatedBy, CreatedDate, IsActive)
            VALUES
                (@CandidateId, @ccode, @FirstName, @LastName,
                 LTRIM(RTRIM(@FirstName + ' ' + ISNULL(@LastName,''))), @Email, @Phone, @TotalExperienceYears,
                 @CurrentEmployer, @ExpectedSalary, @ResumeUrl, @Source, @ReferredByEmployeeId,
                 @CreatedBy, GETUTCDATE(), 1);
        END

        IF EXISTS (SELECT 1 FROM dbo.JobApplications WHERE JobPostingId = @JobPostingId AND CandidateId = @CandidateId)
        BEGIN
            ROLLBACK;
            SELECT 0 AS RowsAffected, 'Candidate already applied to this posting.' AS [Message];
            RETURN;
        END

        DECLARE @AppId UNIQUEIDENTIFIER = NEWID();
        DECLARE @acode NVARCHAR(30) =
            dbo.fn_GetNextRecruitmentCode('APP', (SELECT MAX(ApplicationCode) FROM dbo.JobApplications WHERE ApplicationCode LIKE 'APP-%'));
        INSERT INTO dbo.JobApplications
            (Id, ApplicationCode, JobPostingId, CandidateId, StageId, AppliedDate, CreatedBy, CreatedDate, IsActive)
        VALUES
            (@AppId, @acode, @JobPostingId, @CandidateId, 1, GETUTCDATE(), @CreatedBy, GETUTCDATE(), 1);

        COMMIT;
        SELECT 1 AS RowsAffected, 'Application captured successfully.' AS [Message],
               @acode AS ApplicationCode, @CandidateId AS CandidateId, @AppId AS ApplicationId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message];
    END CATCH
END
GO
-- =============================================================================
-- SP_MoveApplicationStage — advance/reject a pipeline card
-- =============================================================================
IF OBJECT_ID('dbo.SP_MoveApplicationStage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_MoveApplicationStage;
GO
DROP PROCEDURE IF EXISTS dbo.SP_MoveApplicationStage;
GO
CREATE PROCEDURE dbo.SP_MoveApplicationStage
    @Id              UNIQUEIDENTIFIER,
    @StageId         INT,
    @ScreeningScore  DECIMAL(5,2)  = NULL,
    @ScreeningNotes  NVARCHAR(1000) = NULL,
    @RejectionReason NVARCHAR(500)  = NULL,
    @UpdatedBy       NVARCHAR(450)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.JobApplications
    SET StageId         = @StageId,
        ScreeningScore  = COALESCE(@ScreeningScore, ScreeningScore),
        ScreeningNotes  = COALESCE(@ScreeningNotes, ScreeningNotes),
        RejectionReason = CASE WHEN @StageId = 7 THEN @RejectionReason ELSE RejectionReason END,
        RejectedDate    = CASE WHEN @StageId = 7 THEN GETUTCDATE()     ELSE RejectedDate    END,
        UpdatedBy       = @UpdatedBy,
        UpdatedDate     = GETUTCDATE()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected, 'Stage updated.' AS [Message];
END
GO
GO

-- #####################################################################################################
-- SECTION 7: Recruitment FunctionsTriggers   (was 11_Recruitment_FunctionsTriggers.sql)
-- #####################################################################################################
-- Recruitment: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Recruitment Module : Seed + Functions + Triggers
-- Triggers feed the *Log tables on INSERT/UPDATE/DELETE, mirroring
-- TRG_InsertDepartments etc. PerformedUser resolves the actor's FullName from
-- dbo.Employees via TRY_CAST on CreatedBy/UpdatedBy (same as existing triggers).
-- =============================================================================

-- =============================================================================
-- SEED — lookups (ids mirror the C# enums in SBERP.HumanResources.Enum)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.RequisitionStatuses)
INSERT INTO dbo.RequisitionStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),(2,'PENDING','Pending Approval',2,1),(3,'APPROVED','Approved',3,1),
 (4,'REJECTED','Rejected',4,1),(5,'ONHOLD','On Hold',5,1),(6,'CLOSED','Closed',6,1),(7,'FILLED','Filled',7,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.JobPostingStatuses)
INSERT INTO dbo.JobPostingStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),(2,'PUBLISHED','Published',2,1),(3,'PAUSED','Paused',3,1),
 (4,'CLOSED','Closed',4,1),(5,'CANCELLED','Cancelled',5,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationStages)
INSERT INTO dbo.ApplicationStages (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'APPLIED','Applied',1,1),(2,'SCREENING','Screening',2,1),(3,'SHORTLISTED','Shortlisted',3,1),
 (4,'INTERVIEW','Interviewing',4,1),(5,'OFFER','Offer',5,1),(6,'HIRED','Hired',6,1),
 (7,'REJECTED','Rejected',7,1),(8,'WITHDRAWN','Withdrawn',8,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.InterviewModes)
INSERT INTO dbo.InterviewModes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'INPERSON','In Person',1,1),(2,'VIDEO','Video Call',2,1),(3,'PHONE','Phone',3,1),(4,'TEST','Assessment Test',4,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.OfferStatuses)
INSERT INTO dbo.OfferStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),(2,'ISSUED','Issued',2,1),(3,'ACCEPTED','Accepted',3,1),
 (4,'DECLINED','Declined',4,1),(5,'EXPIRED','Expired',5,1),(6,'WITHDRAWN','Withdrawn',6,1);
GO
-- =============================================================================
-- fn_GetNextRecruitmentCode — generic prefixed sequence (mirrors fn_GetNextEmployeeCode)
-- Usage: dbo.fn_GetNextRecruitmentCode('MRF', (SELECT MAX(RequisitionCode) ...))
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextRecruitmentCode', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetNextRecruitmentCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextRecruitmentCode;
GO
CREATE FUNCTION dbo.fn_GetNextRecruitmentCode(@Prefix NVARCHAR(10), @Current NVARCHAR(30))
RETURNS NVARCHAR(30)
AS
BEGIN
    DECLARE @next INT = ISNULL(TRY_CAST(SUBSTRING(@Current, LEN(@Prefix) + 2, 50) AS INT), 0) + 1;
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(@next AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- fn_CountApplicantsByPosting — list-view count without an extra round trip
-- (mirrors fn_CountEmployeesInDepartment)
-- =============================================================================
IF OBJECT_ID('dbo.fn_CountApplicantsByPosting', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CountApplicantsByPosting;
GO
DROP FUNCTION IF EXISTS dbo.fn_CountApplicantsByPosting;
GO
CREATE FUNCTION dbo.fn_CountApplicantsByPosting(@JobPostingId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @cnt INT;
    SELECT @cnt = COUNT(1) FROM dbo.JobApplications WHERE JobPostingId = @JobPostingId;
    RETURN ISNULL(@cnt, 0);
END
GO
-- =============================================================================
-- TRIGGERS — Positions
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertPositions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertPositions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertPositions;
GO
CREATE TRIGGER dbo.TRG_InsertPositions ON dbo.Positions AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.PositionsLog
            (Id, PositionId, PositionCode, Title, DesignationId, DepartmentId,
             BudgetedHeadcount, FilledHeadcount, StatusId, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.PositionCode, I.Title, I.DesignationId, I.DepartmentId,
               I.BudgetedHeadcount, I.FilledHeadcount, I.StatusId, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdatePositions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdatePositions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdatePositions;
GO
CREATE TRIGGER dbo.TRG_UpdatePositions ON dbo.Positions AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.PositionsLog
            (Id, PositionId, PositionCode, Title, DesignationId, DepartmentId,
             BudgetedHeadcount, FilledHeadcount, StatusId, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.PositionCode, I.Title, I.DesignationId, I.DepartmentId,
               I.BudgetedHeadcount, I.FilledHeadcount, I.StatusId, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_DeletePositions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeletePositions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_DeletePositions;
GO
CREATE TRIGGER dbo.TRG_DeletePositions ON dbo.Positions AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.PositionsLog
            (Id, PositionId, PositionCode, Title, DesignationId, DepartmentId,
             BudgetedHeadcount, FilledHeadcount, StatusId, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.PositionCode, D.Title, D.DesignationId, D.DepartmentId,
               D.BudgetedHeadcount, D.FilledHeadcount, D.StatusId, D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate,
               D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
-- =============================================================================
-- TRIGGERS — ManpowerRequisitions
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertManpowerRequisitions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertManpowerRequisitions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertManpowerRequisitions;
GO
CREATE TRIGGER dbo.TRG_InsertManpowerRequisitions ON dbo.ManpowerRequisitions AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.ManpowerRequisitionsLog
            (Id, ManpowerRequisitionId, RequisitionCode, PositionId, NumberOfVacancies, StatusId,
             ApprovedById, ApprovalRemark, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.RequisitionCode, I.PositionId, I.NumberOfVacancies, I.StatusId,
               I.ApprovedById, I.ApprovalRemark, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateManpowerRequisitions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateManpowerRequisitions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateManpowerRequisitions;
GO
CREATE TRIGGER dbo.TRG_UpdateManpowerRequisitions ON dbo.ManpowerRequisitions AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.ManpowerRequisitionsLog
            (Id, ManpowerRequisitionId, RequisitionCode, PositionId, NumberOfVacancies, StatusId,
             ApprovedById, ApprovalRemark, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.RequisitionCode, I.PositionId, I.NumberOfVacancies, I.StatusId,
               I.ApprovedById, I.ApprovalRemark, I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate,
               I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_DeleteManpowerRequisitions', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteManpowerRequisitions;
GO
DROP TRIGGER IF EXISTS dbo.TRG_DeleteManpowerRequisitions;
GO
CREATE TRIGGER dbo.TRG_DeleteManpowerRequisitions ON dbo.ManpowerRequisitions AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.ManpowerRequisitionsLog
            (Id, ManpowerRequisitionId, RequisitionCode, PositionId, NumberOfVacancies, StatusId,
             ApprovedById, ApprovalRemark, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
             IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.RequisitionCode, D.PositionId, D.NumberOfVacancies, D.StatusId,
               D.ApprovedById, D.ApprovalRemark, D.CreatedBy, D.CreatedDate, D.UpdatedBy, D.UpdatedDate,
               D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
-- =============================================================================
-- TRIGGERS — JobPostings
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertJobPostings', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertJobPostings;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertJobPostings;
GO
CREATE TRIGGER dbo.TRG_InsertJobPostings ON dbo.JobPostings AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobPostingsLog
            (Id, JobPostingId, PostingCode, Title, StatusId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.PostingCode, I.Title, I.StatusId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateJobPostings', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateJobPostings;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateJobPostings;
GO
CREATE TRIGGER dbo.TRG_UpdateJobPostings ON dbo.JobPostings AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobPostingsLog
            (Id, JobPostingId, PostingCode, Title, StatusId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.PostingCode, I.Title, I.StatusId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_DeleteJobPostings', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteJobPostings;
GO
DROP TRIGGER IF EXISTS dbo.TRG_DeleteJobPostings;
GO
CREATE TRIGGER dbo.TRG_DeleteJobPostings ON dbo.JobPostings AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobPostingsLog
            (Id, JobPostingId, PostingCode, Title, StatusId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), D.Id, D.PostingCode, D.Title, D.StatusId, D.CreatedBy, D.CreatedDate,
               D.UpdatedBy, D.UpdatedDate, D.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(D.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Deleted'
        FROM deleted D;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
-- =============================================================================
-- TRIGGERS — Candidates
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertCandidates', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertCandidates;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertCandidates;
GO
CREATE TRIGGER dbo.TRG_InsertCandidates ON dbo.Candidates AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.CandidatesLog
            (Id, CandidateId, CandidateCode, FullName, Email, IsBlacklisted, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CandidateCode, I.FullName, I.Email, I.IsBlacklisted, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateCandidates', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateCandidates;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateCandidates;
GO
CREATE TRIGGER dbo.TRG_UpdateCandidates ON dbo.Candidates AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.CandidatesLog
            (Id, CandidateId, CandidateCode, FullName, Email, IsBlacklisted, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.CandidateCode, I.FullName, I.Email, I.IsBlacklisted, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
-- =============================================================================
-- TRIGGERS — JobApplications
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertJobApplications', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertJobApplications;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertJobApplications;
GO
CREATE TRIGGER dbo.TRG_InsertJobApplications ON dbo.JobApplications AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobApplicationsLog
            (Id, JobApplicationId, ApplicationCode, StageId, RejectionReason, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.ApplicationCode, I.StageId, I.RejectionReason, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateJobApplications', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateJobApplications;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateJobApplications;
GO
CREATE TRIGGER dbo.TRG_UpdateJobApplications ON dbo.JobApplications AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobApplicationsLog
            (Id, JobApplicationId, ApplicationCode, StageId, RejectionReason, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.ApplicationCode, I.StageId, I.RejectionReason, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
-- =============================================================================
-- TRIGGERS — JobOffers
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertJobOffers', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertJobOffers;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertJobOffers;
GO
CREATE TRIGGER dbo.TRG_InsertJobOffers ON dbo.JobOffers AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobOffersLog
            (Id, JobOfferId, OfferCode, StatusId, OfferedSalary, ConvertedEmployeeId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.OfferCode, I.StatusId, I.OfferedSalary, I.ConvertedEmployeeId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateJobOffers', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateJobOffers;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateJobOffers;
GO
CREATE TRIGGER dbo.TRG_UpdateJobOffers ON dbo.JobOffers AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.JobOffersLog
            (Id, JobOfferId, OfferCode, StatusId, OfferedSalary, ConvertedEmployeeId, CreatedBy, CreatedDate,
             UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.OfferCode, I.StatusId, I.OfferedSalary, I.ConvertedEmployeeId, I.CreatedBy, I.CreatedDate,
               I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
GO

-- #####################################################################################################
-- SECTION 8: Onboarding StoredProcedures   (was 12b_Onboarding_StoredProcedures.sql)
-- #####################################################################################################
-- Onboarding: stored procedures

-- =============================================================================
-- HumanResourcesDB — Onboarding Module : Stored Procedures
-- The centrepiece is SP_OnboardEmployeeFromOffer: it converts an ACCEPTED offer
-- into a real Employee, seeds a default checklist, and writes the
-- EmployeeOnboardedEvent into dbo.IntegrationOutbox — all in one transaction.
-- =============================================================================

-- =============================================================================
-- SP_OnboardEmployeeFromOffer
-- Guards: offer must exist and be Accepted (status 3); not already onboarded.
-- Creates Employee (AppUserId = NULL — Security fills it after consuming the
-- event), links Onboarding.EmployeeId, advances the offer's ConvertedEmployeeId,
-- seeds checklist tasks, and enqueues the integration event atomically.
-- =============================================================================
IF OBJECT_ID('dbo.SP_OnboardEmployeeFromOffer', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_OnboardEmployeeFromOffer;
GO
DROP PROCEDURE IF EXISTS dbo.SP_OnboardEmployeeFromOffer;
GO
CREATE PROCEDURE dbo.SP_OnboardEmployeeFromOffer
    @JobOfferId        UNIQUEIDENTIFIER,
    @ReportingManagerId UNIQUEIDENTIFIER = NULL,
    @WorkLocation      NVARCHAR(200)    = NULL,
    @CreatedBy         NVARCHAR(450)    = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        -- ---- validate offer ------------------------------------------------
        DECLARE @StatusId INT, @CandidateId UNIQUEIDENTIFIER, @AppId UNIQUEIDENTIFIER,
                @PositionId UNIQUEIDENTIFIER, @DesignationId UNIQUEIDENTIFIER,
                @DepartmentId UNIQUEIDENTIFIER, @OfferedSalary DECIMAL(18,2),
                @Currency NVARCHAR(10), @JoiningDate DATETIME, @ConvertedEmployeeId UNIQUEIDENTIFIER;

        SELECT @StatusId = StatusId, @CandidateId = CandidateId, @AppId = JobApplicationId,
               @PositionId = PositionId, @DesignationId = DesignationId, @DepartmentId = DepartmentId,
               @OfferedSalary = OfferedSalary, @Currency = SalaryCurrency, @JoiningDate = JoiningDate,
               @ConvertedEmployeeId = ConvertedEmployeeId
        FROM dbo.JobOffers WHERE Id = @JobOfferId;

        IF @CandidateId IS NULL
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected, 'Offer not found.' AS [Message]; RETURN; END

        IF @StatusId <> 3   -- EnumOfferStatus.Accepted
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected, 'Offer must be Accepted before onboarding.' AS [Message]; RETURN; END

        IF @ConvertedEmployeeId IS NOT NULL OR EXISTS (SELECT 1 FROM dbo.Onboardings WHERE JobOfferId = @JobOfferId)
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected, 'This offer has already been onboarded.' AS [Message]; RETURN; END

        -- ---- candidate snapshot -------------------------------------------
        DECLARE @FirstName NVARCHAR(100), @LastName NVARCHAR(100), @Email NVARCHAR(200),
                @Phone NVARCHAR(30), @Gender INT, @DOB DATETIME, @Nationality NVARCHAR(100);
        SELECT @FirstName = FirstName, @LastName = LastName, @Email = Email, @Phone = Phone,
               @Gender = Gender, @DOB = DateOfBirth, @Nationality = Nationality
        FROM dbo.Candidates WHERE Id = @CandidateId;

        -- ---- create the Employee (AppUserId NULL — the onboarding seam) -----
        DECLARE @EmployeeId UNIQUEIDENTIFIER = NEWID();
        DECLARE @EmpCode NVARCHAR(30) = dbo.fn_GetNextEmployeeCode();
        DECLARE @FullName NVARCHAR(300) = dbo.fn_GetEmployeeFullName(@FirstName, NULL, @LastName);

        -- ---- org placement: inherit the seat's company/branch/cost center/grade.
        --      Employees.CompanyId is NOT NULL, so fall back to the department's
        --      company, then to the first active company.
        DECLARE @CompanyId UNIQUEIDENTIFIER, @BranchId UNIQUEIDENTIFIER,
                @CostCenterId UNIQUEIDENTIFIER, @JobGradeId UNIQUEIDENTIFIER,
                @Budget INT, @Filled INT, @PosStatus INT;
        IF @PositionId IS NOT NULL
            SELECT @CompanyId = CompanyId, @BranchId = BranchId, @CostCenterId = CostCenterId,
                   @JobGradeId = JobGradeId, @Budget = BudgetedHeadcount, @Filled = FilledHeadcount,
                   @PosStatus = StatusId,
                   @DepartmentId = ISNULL(@DepartmentId, DepartmentId),
                   @DesignationId = ISNULL(@DesignationId, DesignationId),
                   @WorkLocation = ISNULL(@WorkLocation, WorkLocation)
            FROM dbo.Positions WHERE Id = @PositionId;
        IF @CompanyId IS NULL AND @DepartmentId IS NOT NULL
            SELECT @CompanyId = CompanyId, @BranchId = ISNULL(@BranchId, BranchId) FROM dbo.Departments WHERE Id = @DepartmentId;
        IF @CompanyId IS NULL
            SELECT TOP 1 @CompanyId = Id FROM dbo.Companies WHERE IsActive = 1 ORDER BY CreatedDate;
        IF @CompanyId IS NULL
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected, 'No active company is configured — set one up in Organization Setup first.' AS [Message]; RETURN; END

        IF @PositionId IS NOT NULL AND @Budget IS NOT NULL AND @Filled >= @Budget
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected, 'The offered position has no vacant seat left (raise its budgeted headcount first).' AS [Message]; RETURN; END

        -- official e-mail is unique; keep the candidate address as personal e-mail if already taken
        DECLARE @OfficialEmail NVARCHAR(200) =
            CASE WHEN EXISTS (SELECT 1 FROM dbo.Employees WHERE OfficialEmail = @Email) THEN NULL ELSE @Email END;

        INSERT INTO dbo.Employees
            (Id, EmployeeCode, AppUserId, OfficialEmail, FirstName, LastName, FullName,
             DateOfBirth, Gender, Nationality, PersonalEmail, MobileNumber,
             CompanyId, BranchId, CostCenterId, PositionId, JobGradeId,
             DepartmentId, DesignationId, ReportingManagerId, DateOfJoining,
             EmploymentType, EmploymentStatus, WorkLocation, BasicSalary, SalaryCurrency,
             CreatedBy, CreatedDate, IsActive)
        VALUES
            (@EmployeeId, @EmpCode, NULL, @OfficialEmail, @FirstName, @LastName, @FullName,
             @DOB, @Gender, @Nationality, @Email, @Phone,
             @CompanyId, @BranchId, @CostCenterId, @PositionId, @JobGradeId,
             @DepartmentId, @DesignationId, @ReportingManagerId, @JoiningDate,
             6 /* Probation */, 1 /* Active */, @WorkLocation, @OfferedSalary, @Currency,
             @CreatedBy, GETUTCDATE(), 1);

        -- ---- occupy the seat (same rules as SP_SetPositionFill) ---------------
        IF @PositionId IS NOT NULL AND @Budget IS NOT NULL
            UPDATE dbo.Positions
            SET FilledHeadcount = @Filled + 1,
                StatusId = CASE WHEN @PosStatus IN (4,5) THEN @PosStatus
                                WHEN @Filled + 1 >= @Budget THEN 3 ELSE 2 END,
                UpdatedBy = @CreatedBy, UpdatedDate = GETUTCDATE()
            WHERE Id = @PositionId;

        -- ---- close the loop on the offer -----------------------------------
        UPDATE dbo.JobOffers
        SET ConvertedEmployeeId = @EmployeeId, UpdatedBy = @CreatedBy, UpdatedDate = GETUTCDATE()
        WHERE Id = @JobOfferId;

        -- ---- create the Onboarding record ----------------------------------
        DECLARE @OnbId UNIQUEIDENTIFIER = NEWID();
        DECLARE @OnbCode NVARCHAR(30) = dbo.fn_GetNextOnboardingCode();
        INSERT INTO dbo.Onboardings
            (Id, OnboardingCode, JobOfferId, CandidateId, EmployeeId, PositionId,
             DepartmentId, DesignationId, ReportingManagerId, JoiningDate, StatusId,
             ChecklistTotal, ChecklistDone, CreatedBy, CreatedDate, IsActive)
        VALUES
            (@OnbId, @OnbCode, @JobOfferId, @CandidateId, @EmployeeId, @PositionId,
             @DepartmentId, @DesignationId, @ReportingManagerId, @JoiningDate, 1 /* Initiated */,
             5, 0, @CreatedBy, GETUTCDATE(), 1);

        -- ---- seed a default checklist --------------------------------------
        INSERT INTO dbo.OnboardingTasks (Id, OnboardingId, Title, OwnerRole, StatusId, SortOrder, CreatedBy, CreatedDate, IsActive)
        VALUES
            (NEWID(), @OnbId, 'Create system user account',     'IT',      1, 1, @CreatedBy, GETUTCDATE(), 1),
            (NEWID(), @OnbId, 'Collect signed documents',       'HR',      1, 2, @CreatedBy, GETUTCDATE(), 1),
            (NEWID(), @OnbId, 'Assign workstation & assets',    'IT',      1, 3, @CreatedBy, GETUTCDATE(), 1),
            (NEWID(), @OnbId, 'Payroll & bank setup',           'Finance', 1, 4, @CreatedBy, GETUTCDATE(), 1),
            (NEWID(), @OnbId, 'Team intro & orientation',       'Manager', 1, 5, @CreatedBy, GETUTCDATE(), 1);

        -- ---- enqueue the integration event (transactional outbox) ----------
        -- Payload mirrors EmployeeOnboardedEvent. Security consumes this, creates
        -- the AppUser, sends the welcome email, and writes AppUserId back.
        DECLARE @Payload NVARCHAR(MAX) =
        (
            SELECT
                @EmployeeId    AS EmployeeId,
                @EmpCode       AS EmployeeCode,
                @FullName      AS FullName,
                @Email         AS OfficialEmail,
                @DepartmentId  AS DepartmentId,
                @DesignationId AS DesignationId,
                (SELECT TOP 1 [Name] FROM dbo.Designations WHERE Id = @DesignationId) AS DesignationName,
                @JoiningDate   AS DateOfJoining,
                @OnbId         AS OnboardingId
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        INSERT INTO dbo.IntegrationOutbox
            (Id, EventType, RoutingKey, AggregateType, AggregateId, Payload, OccurredOn)
        VALUES
            (NEWID(), 'EmployeeOnboardedEvent', 'hr.employee.onboarded', 'Employee', @EmployeeId, @Payload, GETUTCDATE());

        COMMIT;
        SELECT 1 AS RowsAffected, 'Employee onboarded; event queued.' AS [Message],
               @EmployeeId AS EmployeeId, @EmpCode AS EmployeeCode, @OnbId AS OnboardingId, @OnbCode AS OnboardingCode;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message];
    END CATCH
END
GO
-- =============================================================================
-- SP_GetAllOnboardingsPagingWithSearch — list view (JSON envelope)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllOnboardingsPagingWithSearch', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAllOnboardingsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllOnboardingsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllOnboardingsPagingWithSearch
    @SearchTerm          NVARCHAR(200) = '',
    @SortColumnName      NVARCHAR(50)  = '',
    @SortColumnDirection NVARCHAR(5)   = 'ASC',
    @PageNumber          INT           = 1,
    @PageSize            INT           = 10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #OnbTBL ([RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] NVARCHAR(MAX));
    DECLARE @TotalRecords INT, @TotalPages INT, @Items NVARCHAR(MAX), @Result NVARCHAR(MAX);

    SELECT @TotalRecords = COUNT(1)
    FROM dbo.Onboardings O
    WHERE (@SearchTerm = '' OR O.OnboardingCode LIKE '%' + @SearchTerm + '%');

    SET @TotalPages = CEILING(@TotalRecords * 1.0 / @PageSize);

    ;WITH Sorted AS
    (
        SELECT
            O.Id, O.OnboardingCode, O.JoiningDate, O.StatusId, OS.[Name] AS StatusName,
            O.ChecklistTotal, O.ChecklistDone,
            C.FullName AS CandidateName, E.EmployeeCode, E.FullName AS EmployeeName,
            D.[Name] AS DepartmentName, DG.[Name] AS DesignationName,
            O.CreatedDate,
            ROW_NUMBER() OVER (
                ORDER BY
                CASE WHEN @SortColumnName='OnboardingCode' AND @SortColumnDirection='ASC'  THEN O.OnboardingCode END ASC,
                CASE WHEN @SortColumnName='OnboardingCode' AND @SortColumnDirection='DESC' THEN O.OnboardingCode END DESC,
                CASE WHEN @SortColumnName=''               AND @SortColumnDirection='ASC'  THEN O.CreatedDate END ASC,
                CASE WHEN @SortColumnName=''               AND @SortColumnDirection='DESC' THEN O.CreatedDate END DESC
            ) AS RowNum
        FROM dbo.Onboardings O
        INNER JOIN dbo.OnboardingStatuses OS ON OS.Id = O.StatusId
        LEFT JOIN dbo.Candidates   C  ON C.Id  = O.CandidateId
        LEFT JOIN dbo.Employees    E  ON E.Id  = O.EmployeeId
        LEFT JOIN dbo.Departments  D  ON D.Id  = O.DepartmentId
        LEFT JOIN dbo.Designations DG ON DG.Id = O.DesignationId
        WHERE (@SearchTerm = '' OR O.OnboardingCode LIKE '%' + @SearchTerm + '%')
    )
    SELECT @Items =
    (
        SELECT Id, OnboardingCode, JoiningDate, StatusId, StatusName, ChecklistTotal, ChecklistDone,
               CandidateName, EmployeeCode, EmployeeName, DepartmentName, DesignationName
        FROM Sorted
        WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
        FOR JSON AUTO
    );

    INSERT INTO #OnbTBL VALUES (@TotalRecords, @PageNumber, @PageSize, @TotalPages, @Items);
    SET @Result = (SELECT * FROM #OnbTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #OnbTBL;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetOnboardingTasks — checklist for one onboarding (JSON array in result)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetOnboardingTasks', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetOnboardingTasks;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetOnboardingTasks;
GO
CREATE PROCEDURE dbo.SP_GetOnboardingTasks
    @OnboardingId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
    (
        SELECT T.Id, T.Title, T.[Description], T.OwnerRole, T.DueDate,
               T.StatusId, TS.[Name] AS StatusName, T.CompletedDate, T.SortOrder
        FROM dbo.OnboardingTasks T
        INNER JOIN dbo.OnboardingTaskStatuses TS ON TS.Id = T.StatusId
        WHERE T.OnboardingId = @OnboardingId
        ORDER BY T.SortOrder
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_GetAcceptedOffersAwaitingOnboarding — feeds the "start onboarding" picker
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAcceptedOffersAwaitingOnboarding', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetAcceptedOffersAwaitingOnboarding;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAcceptedOffersAwaitingOnboarding;
GO
CREATE PROCEDURE dbo.SP_GetAcceptedOffersAwaitingOnboarding
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
    (
        SELECT JO.Id, JO.OfferCode, JO.JoiningDate, JO.OfferedSalary,
               C.FullName AS CandidateName, C.Email,
               D.[Name] AS DepartmentName, DG.[Name] AS DesignationName
        FROM dbo.JobOffers JO
        INNER JOIN dbo.Candidates    C  ON C.Id  = JO.CandidateId
        LEFT JOIN dbo.Departments    D  ON D.Id  = JO.DepartmentId
        LEFT JOIN dbo.Designations   DG ON DG.Id = JO.DesignationId
        WHERE JO.StatusId = 3                 -- Accepted
          AND JO.ConvertedEmployeeId IS NULL  -- not yet onboarded
        ORDER BY JO.JoiningDate
        FOR JSON PATH
    ) AS result;
END
GO
GO

-- #####################################################################################################
-- SECTION 9: Onboarding FunctionsTriggers   (was 12_Onboarding_FunctionsTriggers.sql)
-- #####################################################################################################
-- Onboarding: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Onboarding Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups (ids mirror C# enums in SBERP.HumanResources.Enum)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.OnboardingStatuses)
INSERT INTO dbo.OnboardingStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'INITIATED','Initiated',1,1),
 (2,'INPROGRESS','In Progress',2,1),
 (3,'COMPLETED','Completed',3,1),
 (4,'CANCELLED','Cancelled',4,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.OnboardingTaskStatuses)
INSERT INTO dbo.OnboardingTaskStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'PENDING','Pending',1,1),
 (2,'INPROGRESS','In Progress',2,1),
 (3,'DONE','Done',3,1),
 (4,'NA','Not Applicable',4,1);
GO
-- =============================================================================
-- fn_GetNextOnboardingCode — ONB-#### (mirrors fn_GetNextEmployeeCode style)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextOnboardingCode', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetNextOnboardingCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextOnboardingCode;
GO
CREATE FUNCTION dbo.fn_GetNextOnboardingCode()
RETURNS NVARCHAR(30)
AS
BEGIN
    DECLARE @next INT;
    SELECT @next = ISNULL(MAX(TRY_CAST(SUBSTRING(OnboardingCode, 5, 50) AS INT)), 0) + 1
    FROM dbo.Onboardings WHERE OnboardingCode LIKE 'ONB-%';
    RETURN 'ONB-' + RIGHT('0000' + CAST(@next AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- TRIGGERS — Onboardings (audit to OnboardingsLog, same shape as Departments)
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertOnboardings', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertOnboardings;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertOnboardings;
GO
CREATE TRIGGER dbo.TRG_InsertOnboardings ON dbo.Onboardings AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.OnboardingsLog
            (Id, OnboardingId, OnboardingCode, JobOfferId, EmployeeId, StatusId,
             CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.OnboardingCode, I.JobOfferId, I.EmployeeId, I.StatusId,
               I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),
               'Created'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
IF OBJECT_ID('dbo.TRG_UpdateOnboardings', 'TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateOnboardings;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateOnboardings;
GO
CREATE TRIGGER dbo.TRG_UpdateOnboardings ON dbo.Onboardings AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.OnboardingsLog
            (Id, OnboardingId, OnboardingCode, JobOfferId, EmployeeId, StatusId,
             CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive, PerformedUser, [Action])
        SELECT NEWID(), I.Id, I.OnboardingCode, I.JobOfferId, I.EmployeeId, I.StatusId,
               I.CreatedBy, I.CreatedDate, I.UpdatedBy, I.UpdatedDate, I.IsActive,
               (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
               'Updated'
        FROM inserted I;
    END TRY
    BEGIN CATCH
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@msg,16,1); ROLLBACK;
    END CATCH
END
GO
GO

-- #####################################################################################################
-- SECTION 10: Attendance StoredProcedures   (was 14b_Attendance_StoredProcedures.sql)
-- #####################################################################################################
-- Attendance: stored procedures

-- =============================================================================
-- HumanResourcesDB — Time & Attendance Module : Stored Procedures
-- Paging SPs (JSON envelope) + workflow procs. The regularization-approve proc
-- writes the corrected punch back onto the existing dbo.Attendances row.
-- =============================================================================

-- =============================================================================
-- SP_GetAllShiftsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllShiftsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllShiftsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllShiftsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllShiftsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.Shifts S
    WHERE (@SearchTerm='' OR S.ShiftCode LIKE '%'+@SearchTerm+'%' OR S.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT S.Id,S.ShiftCode,S.[Name],S.StartTime,S.EndTime,S.IsNightShift,S.BreakMinutes,
               S.GraceInMinutes,S.GraceOutMinutes,S.FullDayHours,S.HalfDayHours,S.WeekOffDays,S.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='ShiftCode' AND @SortColumnDirection='ASC'  THEN S.ShiftCode END ASC,
                 CASE WHEN @SortColumnName='ShiftCode' AND @SortColumnDirection='DESC' THEN S.ShiftCode END DESC,
                 CASE WHEN @SortColumnName='Name'      AND @SortColumnDirection='ASC'  THEN S.[Name] END ASC,
                 CASE WHEN @SortColumnName='Name'      AND @SortColumnDirection='DESC' THEN S.[Name] END DESC,
                 CASE WHEN @SortColumnName=''          AND @SortColumnDirection='ASC'  THEN S.ShiftCode END ASC,
                 CASE WHEN @SortColumnName=''          AND @SortColumnDirection='DESC' THEN S.ShiftCode END DESC
               ) AS RowNum
        FROM dbo.Shifts S
        WHERE (@SearchTerm='' OR S.ShiftCode LIKE '%'+@SearchTerm+'%' OR S.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,ShiftCode,[Name],StartTime,EndTime,IsNightShift,BreakMinutes,GraceInMinutes,
                          GraceOutMinutes,FullDayHours,HalfDayHours,WeekOffDays,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllOvertimeRequestsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllOvertimeRequestsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllOvertimeRequestsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllOvertimeRequestsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllOvertimeRequestsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.OvertimeRequests O INNER JOIN dbo.Employees E ON E.Id=O.EmployeeId
    WHERE (@SearchTerm='' OR O.RequestCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT O.Id,O.RequestCode,O.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               O.OvertimeDate,O.FromTime,O.ToTime,O.[Hours],O.Reason,O.StatusId,OS.[Name] AS StatusName,
               O.ApproverRemarks,O.ApprovedDate,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='RequestCode' AND @SortColumnDirection='ASC'  THEN O.RequestCode END ASC,
                 CASE WHEN @SortColumnName='RequestCode' AND @SortColumnDirection='DESC' THEN O.RequestCode END DESC,
                 CASE WHEN @SortColumnName=''            AND @SortColumnDirection='ASC'  THEN O.OvertimeDate END ASC,
                 CASE WHEN @SortColumnName=''            AND @SortColumnDirection='DESC' THEN O.OvertimeDate END DESC
               ) AS RowNum
        FROM dbo.OvertimeRequests O
        INNER JOIN dbo.Employees E ON E.Id=O.EmployeeId
        INNER JOIN dbo.OvertimeStatuses OS ON OS.Id=O.StatusId
        WHERE (@SearchTerm='' OR O.RequestCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,RequestCode,EmployeeId,EmployeeName,EmployeeCode,OvertimeDate,FromTime,ToTime,
                          [Hours],Reason,StatusId,StatusName,ApproverRemarks,ApprovedDate
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllRegularizationsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllRegularizationsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllRegularizationsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllRegularizationsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllRegularizationsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.AttendanceRegularizationRequests R INNER JOIN dbo.Employees E ON E.Id=R.EmployeeId
    WHERE (@SearchTerm='' OR R.RequestCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT R.Id,R.RequestCode,R.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               R.AttendanceDate,R.RegTypeId,RT.[Name] AS RegTypeName,
               R.RequestedCheckIn,R.RequestedCheckOut,R.Reason,R.StatusId,RS.[Name] AS StatusName,
               R.ApproverRemarks,R.ApprovedDate,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='RequestCode' AND @SortColumnDirection='ASC'  THEN R.RequestCode END ASC,
                 CASE WHEN @SortColumnName='RequestCode' AND @SortColumnDirection='DESC' THEN R.RequestCode END DESC,
                 CASE WHEN @SortColumnName=''            AND @SortColumnDirection='ASC'  THEN R.AttendanceDate END ASC,
                 CASE WHEN @SortColumnName=''            AND @SortColumnDirection='DESC' THEN R.AttendanceDate END DESC
               ) AS RowNum
        FROM dbo.AttendanceRegularizationRequests R
        INNER JOIN dbo.Employees E ON E.Id=R.EmployeeId
        INNER JOIN dbo.RegularizationTypes RT ON RT.Id=R.RegTypeId
        INNER JOIN dbo.RegularizationStatuses RS ON RS.Id=R.StatusId
        WHERE (@SearchTerm='' OR R.RequestCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,RequestCode,EmployeeId,EmployeeName,EmployeeCode,AttendanceDate,RegTypeId,RegTypeName,
                          RequestedCheckIn,RequestedCheckOut,Reason,StatusId,StatusName,ApproverRemarks,ApprovedDate
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetEmployeeRoster — current/!future roster rows for an employee (JSON array)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetEmployeeRoster','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetEmployeeRoster;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetEmployeeRoster;
GO
CREATE PROCEDURE dbo.SP_GetEmployeeRoster @EmployeeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT R.Id,R.EmployeeId,R.ShiftId,S.ShiftCode,S.[Name] AS ShiftName,S.StartTime,S.EndTime,
               R.EffectiveFrom,R.EffectiveTo,R.Remarks,R.IsActive
        FROM dbo.ShiftRosters R
        INNER JOIN dbo.Shifts S ON S.Id=R.ShiftId
        WHERE R.EmployeeId=@EmployeeId AND R.IsActive=1
        ORDER BY R.EffectiveFrom DESC
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_ApproveOvertime / shared status setter (StatusId: 2 Approved, 3 Rejected, 4 Cancelled)
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetOvertimeStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetOvertimeStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetOvertimeStatus;
GO
CREATE PROCEDURE dbo.SP_SetOvertimeStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @ApproverId UNIQUEIDENTIFIER=NULL,
    @ApproverRemarks NVARCHAR(500)=NULL, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.OvertimeRequests WHERE Id=@Id)
        BEGIN SELECT 0 AS RowsAffected, 'Overtime request not found.' AS [Message]; RETURN; END
        IF EXISTS (SELECT 1 FROM dbo.OvertimeRequests WHERE Id=@Id AND StatusId IN (2,3,4))
        BEGIN SELECT 0 AS RowsAffected, 'Request is already finalised.' AS [Message]; RETURN; END

        UPDATE dbo.OvertimeRequests
        SET StatusId=@StatusId, ApproverId=@ApproverId, ApproverRemarks=@ApproverRemarks,
            ApprovedDate=CASE WHEN @StatusId=2 THEN GETUTCDATE() ELSE ApprovedDate END,
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;
        SELECT 1 AS RowsAffected, 'Overtime request updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_ApproveRegularization — approve writes the corrected punch onto the
-- existing dbo.Attendances row (creating it if there wasn't one). Recomputes
-- WorkingHours. Reject/Cancel just set status.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetRegularizationStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetRegularizationStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetRegularizationStatus;
GO
CREATE PROCEDURE dbo.SP_SetRegularizationStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @ApproverId UNIQUEIDENTIFIER=NULL,
    @ApproverRemarks NVARCHAR(500)=NULL, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @EmpId UNIQUEIDENTIFIER, @AttDate DATETIME, @AttId UNIQUEIDENTIFIER,
                @In TIME(0), @Out TIME(0), @Cur INT;
        SELECT @Cur=StatusId, @EmpId=EmployeeId, @AttDate=AttendanceDate, @AttId=AttendanceId,
               @In=RequestedCheckIn, @Out=RequestedCheckOut
        FROM dbo.AttendanceRegularizationRequests WHERE Id=@Id;

        IF @EmpId IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Regularization not found.' AS [Message]; RETURN; END
        IF @Cur IN (2,3,4) BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Request is already finalised.' AS [Message]; RETURN; END

        UPDATE dbo.AttendanceRegularizationRequests
        SET StatusId=@StatusId, ApproverId=@ApproverId, ApproverRemarks=@ApproverRemarks,
            ApprovedDate=CASE WHEN @StatusId=2 THEN GETUTCDATE() ELSE ApprovedDate END,
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        IF @StatusId=2  -- Approved: apply the correction to dbo.Attendances
        BEGIN
            DECLARE @Hours DECIMAL(5,2) =
                CASE WHEN @In IS NOT NULL AND @Out IS NOT NULL
                     THEN CAST(DATEDIFF(MINUTE,@In,@Out)/60.0 AS DECIMAL(5,2)) END;

            IF @AttId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Attendances WHERE Id=@AttId)
            BEGIN
                UPDATE dbo.Attendances
                SET CheckInTime = COALESCE(@In, CheckInTime),
                    CheckOutTime= COALESCE(@Out, CheckOutTime),
                    WorkingHours= COALESCE(@Hours, WorkingHours),
                    Status = 1,  -- Present (EnumAttendanceStatus)
                    Remarks = LEFT(ISNULL(Remarks,'') + ' [Regularized]',490),
                    UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
                WHERE Id=@AttId;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.Attendances
                    (Id,EmployeeId,AttendanceDate,CheckInTime,CheckOutTime,WorkingHours,Status,SourceType,Remarks,CreatedBy,CreatedDate,IsActive)
                VALUES
                    (NEWID(),@EmpId,@AttDate,@In,@Out,@Hours,1,1,'Created via regularization',@UpdatedBy,GETUTCDATE(),1);
            END
        END

        COMMIT;
        SELECT 1 AS RowsAffected, 'Regularization updated.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_GetMonthlyAttendanceSummary — per-employee day grid + totals for a month.
-- Reads the existing dbo.Attendances table. @Month is any date in the month.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetMonthlyAttendanceSummary','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetMonthlyAttendanceSummary;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetMonthlyAttendanceSummary;
GO
CREATE PROCEDURE dbo.SP_GetMonthlyAttendanceSummary
    @Month DATETIME, @DepartmentId UNIQUEIDENTIFIER=NULL, @EmployeeId UNIQUEIDENTIFIER=NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Start DATE = DATEFROMPARTS(YEAR(@Month),MONTH(@Month),1);
    DECLARE @End   DATE = EOMONTH(@Start);

    SELECT (
        SELECT
            E.Id AS EmployeeId, E.EmployeeCode, E.FullName, D.[Name] AS DepartmentName,
            SUM(CASE WHEN A.[Status]=1 THEN 1 ELSE 0 END) AS PresentDays,
            SUM(CASE WHEN A.[Status]=2 THEN 1 ELSE 0 END) AS AbsentDays,
            SUM(CASE WHEN A.[Status]=3 THEN 1 ELSE 0 END) AS LeaveDays,
            SUM(CASE WHEN A.[Status]=6 THEN 1 ELSE 0 END) AS HalfDays,
            SUM(CASE WHEN A.[Status]=7 THEN 1 ELSE 0 END) AS LateDays,
            CAST(ISNULL(SUM(A.WorkingHours),0) AS DECIMAL(8,2)) AS TotalHours
        FROM dbo.Employees E
        LEFT JOIN dbo.Attendances A
               ON A.EmployeeId=E.Id AND A.AttendanceDate BETWEEN @Start AND @End AND A.IsActive=1
        LEFT JOIN dbo.Departments D ON D.Id=E.DepartmentId
        WHERE E.IsActive=1
          AND (@DepartmentId IS NULL OR E.DepartmentId=@DepartmentId)
          AND (@EmployeeId   IS NULL OR E.Id=@EmployeeId)
        GROUP BY E.Id,E.EmployeeCode,E.FullName,D.[Name]
        ORDER BY E.FullName
        FOR JSON PATH
    ) AS result;
END
GO
GO

-- #####################################################################################################
-- SECTION 11: Attendance FunctionsTriggers   (was 14_Attendance_FunctionsTriggers.sql)
-- #####################################################################################################
-- Attendance: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Time & Attendance Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups (ids mirror enums in SBERP.HumanResources.Enum)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.OvertimeStatuses)
INSERT INTO dbo.OvertimeStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'PENDING','Pending',1,1),
 (2,'APPROVED','Approved',2,1),
 (3,'REJECTED','Rejected',3,1),
 (4,'CANCELLED','Cancelled',4,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.RegularizationStatuses)
INSERT INTO dbo.RegularizationStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'PENDING','Pending',1,1),
 (2,'APPROVED','Approved',2,1),
 (3,'REJECTED','Rejected',3,1),
 (4,'CANCELLED','Cancelled',4,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.RegularizationTypes)
INSERT INTO dbo.RegularizationTypes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'MISSED_IN','Missed Check-In',1,1),
 (2,'MISSED_OUT','Missed Check-Out',2,1),
 (3,'WRONG_TIME','Wrong Punch Time',3,1),
 (4,'FORGOT','Forgot to Punch',4,1),
 (5,'ON_DUTY','On Duty / Field Work',5,1);
GO
-- =============================================================================
-- fn_GetNextAttendanceCode — generic prefixed sequence over a target table.
-- (Mirrors the fn_GetNextRecruitmentCode style — caller passes prefix + current max.)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextAttendanceCode', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetNextAttendanceCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextAttendanceCode;
GO
CREATE FUNCTION dbo.fn_GetNextAttendanceCode(@Prefix NVARCHAR(10), @CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- fn_GetEmployeeShiftOn — the effective shift for an employee on a given date.
-- Picks the roster row whose [EffectiveFrom, EffectiveTo] window contains @On
-- (most recent EffectiveFrom wins). Returns the ShiftId or NULL.
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetEmployeeShiftOn', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetEmployeeShiftOn;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetEmployeeShiftOn;
GO
CREATE FUNCTION dbo.fn_GetEmployeeShiftOn(@EmployeeId UNIQUEIDENTIFIER, @On DATETIME)
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
    DECLARE @ShiftId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ShiftId = ShiftId
    FROM dbo.ShiftRosters
    WHERE EmployeeId = @EmployeeId
      AND IsActive = 1
      AND EffectiveFrom <= @On
      AND (EffectiveTo IS NULL OR EffectiveTo >= @On)
    ORDER BY EffectiveFrom DESC;
    RETURN @ShiftId;
END
GO
-- =============================================================================
-- TRIGGER FACTORY — audit triggers for the four workflow tables.
-- Same shape as TRG_InsertDepartments: PerformedUser resolved from dbo.Employees.
-- =============================================================================

-- ---- Shifts ----
IF OBJECT_ID('dbo.TRG_InsertShifts','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertShifts;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertShifts;
GO
CREATE TRIGGER dbo.TRG_InsertShifts ON dbo.Shifts AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ShiftsLog (Id,ShiftId,ShiftCode,[Name],StartTime,EndTime,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ShiftCode,I.[Name],I.StartTime,I.EndTime,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateShifts','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateShifts;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateShifts;
GO
CREATE TRIGGER dbo.TRG_UpdateShifts ON dbo.Shifts AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ShiftsLog (Id,ShiftId,ShiftCode,[Name],StartTime,EndTime,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ShiftCode,I.[Name],I.StartTime,I.EndTime,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_DeleteShifts','TR') IS NOT NULL DROP TRIGGER dbo.TRG_DeleteShifts;
GO
DROP TRIGGER IF EXISTS dbo.TRG_DeleteShifts;
GO
CREATE TRIGGER dbo.TRG_DeleteShifts ON dbo.Shifts AFTER DELETE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ShiftsLog (Id,ShiftId,ShiftCode,[Name],StartTime,EndTime,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),D.Id,D.ShiftCode,D.[Name],D.StartTime,D.EndTime,D.CreatedBy,D.CreatedDate,D.UpdatedBy,D.UpdatedDate,D.IsActive,NULL,'Deleted'
    FROM deleted D;
END
GO
-- ---- ShiftRosters ----
IF OBJECT_ID('dbo.TRG_InsertShiftRosters','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertShiftRosters;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertShiftRosters;
GO
CREATE TRIGGER dbo.TRG_InsertShiftRosters ON dbo.ShiftRosters AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ShiftRostersLog (Id,ShiftRosterId,EmployeeId,ShiftId,EffectiveFrom,EffectiveTo,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.EmployeeId,I.ShiftId,I.EffectiveFrom,I.EffectiveTo,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateShiftRosters','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateShiftRosters;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateShiftRosters;
GO
CREATE TRIGGER dbo.TRG_UpdateShiftRosters ON dbo.ShiftRosters AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ShiftRostersLog (Id,ShiftRosterId,EmployeeId,ShiftId,EffectiveFrom,EffectiveTo,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.EmployeeId,I.ShiftId,I.EffectiveFrom,I.EffectiveTo,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- OvertimeRequests ----
IF OBJECT_ID('dbo.TRG_InsertOvertimeRequests','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertOvertimeRequests;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertOvertimeRequests;
GO
CREATE TRIGGER dbo.TRG_InsertOvertimeRequests ON dbo.OvertimeRequests AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OvertimeRequestsLog (Id,OvertimeRequestId,RequestCode,EmployeeId,OvertimeDate,[Hours],StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RequestCode,I.EmployeeId,I.OvertimeDate,I.[Hours],I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateOvertimeRequests','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateOvertimeRequests;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateOvertimeRequests;
GO
CREATE TRIGGER dbo.TRG_UpdateOvertimeRequests ON dbo.OvertimeRequests AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OvertimeRequestsLog (Id,OvertimeRequestId,RequestCode,EmployeeId,OvertimeDate,[Hours],StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RequestCode,I.EmployeeId,I.OvertimeDate,I.[Hours],I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- AttendanceRegularizationRequests ----
IF OBJECT_ID('dbo.TRG_InsertRegularization','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertRegularization;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertRegularization;
GO
CREATE TRIGGER dbo.TRG_InsertRegularization ON dbo.AttendanceRegularizationRequests AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AttendanceRegularizationRequestsLog (Id,RegularizationRequestId,RequestCode,EmployeeId,AttendanceDate,RegTypeId,StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RequestCode,I.EmployeeId,I.AttendanceDate,I.RegTypeId,I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateRegularization','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateRegularization;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateRegularization;
GO
CREATE TRIGGER dbo.TRG_UpdateRegularization ON dbo.AttendanceRegularizationRequests AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AttendanceRegularizationRequestsLog (Id,RegularizationRequestId,RequestCode,EmployeeId,AttendanceDate,RegTypeId,StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RequestCode,I.EmployeeId,I.AttendanceDate,I.RegTypeId,I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
GO

-- #####################################################################################################
-- SECTION 12: Leave StoredProcedures   (was 15b_Leave_StoredProcedures.sql)
-- #####################################################################################################
-- Leave: stored procedures

-- =============================================================================
-- HumanResourcesDB — Leave Module : Stored Procedures
-- Balance-moving SPs are transactional and return RowsAffected / Message,
-- matching the onboarding/attendance status-SP shape the services already read.
-- =============================================================================

-- =============================================================================
-- SP_GetAllLeaveTypesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllLeaveTypesPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllLeaveTypesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllLeaveTypesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllLeaveTypesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.LeaveTypes T
    WHERE (@SearchTerm='' OR T.LeaveTypeCode LIKE '%'+@SearchTerm+'%' OR T.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT T.Id,T.LeaveTypeCode,T.[Name],T.IsPaid,T.AllowHalfDay,T.RequiresAttachment,T.IsCarryForward,
               T.MaxCarryForward,T.AccrualPerMonth,T.ColorHex,T.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='LeaveTypeCode' AND @SortColumnDirection='ASC'  THEN T.LeaveTypeCode END ASC,
                 CASE WHEN @SortColumnName='LeaveTypeCode' AND @SortColumnDirection='DESC' THEN T.LeaveTypeCode END DESC,
                 CASE WHEN @SortColumnName='Name'          AND @SortColumnDirection='ASC'  THEN T.[Name] END ASC,
                 CASE WHEN @SortColumnName='Name'          AND @SortColumnDirection='DESC' THEN T.[Name] END DESC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='ASC'  THEN T.LeaveTypeCode END ASC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='DESC' THEN T.LeaveTypeCode END DESC
               ) AS RowNum
        FROM dbo.LeaveTypes T
        WHERE (@SearchTerm='' OR T.LeaveTypeCode LIKE '%'+@SearchTerm+'%' OR T.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,LeaveTypeCode,[Name],IsPaid,AllowHalfDay,RequiresAttachment,IsCarryForward,
                          MaxCarryForward,AccrualPerMonth,ColorHex,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllLeavePoliciesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllLeavePoliciesPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllLeavePoliciesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllLeavePoliciesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllLeavePoliciesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.LeavePolicies P
    WHERE (@SearchTerm='' OR P.PolicyCode LIKE '%'+@SearchTerm+'%' OR P.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT P.Id,P.PolicyCode,P.[Name],P.LeaveTypeId,LT.[Name] AS LeaveTypeName,
               P.DepartmentId,D.[Name] AS DepartmentName,P.Grade,P.AnnualEntitlement,
               P.MinServiceMonths,P.MaxConsecutiveDays,P.EffectiveYear,P.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='PolicyCode' AND @SortColumnDirection='ASC'  THEN P.PolicyCode END ASC,
                 CASE WHEN @SortColumnName='PolicyCode' AND @SortColumnDirection='DESC' THEN P.PolicyCode END DESC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='ASC'  THEN P.PolicyCode END ASC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='DESC' THEN P.PolicyCode END DESC
               ) AS RowNum
        FROM dbo.LeavePolicies P
        INNER JOIN dbo.LeaveTypes LT ON LT.Id=P.LeaveTypeId
        LEFT JOIN dbo.Departments D ON D.Id=P.DepartmentId
        WHERE (@SearchTerm='' OR P.PolicyCode LIKE '%'+@SearchTerm+'%' OR P.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,PolicyCode,[Name],LeaveTypeId,LeaveTypeName,DepartmentId,DepartmentName,Grade,
                          AnnualEntitlement,MinServiceMonths,MaxConsecutiveDays,EffectiveYear,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllLeaveApplicationsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllLeaveApplicationsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllLeaveApplicationsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllLeaveApplicationsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllLeaveApplicationsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.LeaveApplications A INNER JOIN dbo.Employees E ON E.Id=A.EmployeeId
    WHERE (@SearchTerm='' OR A.ApplicationCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT A.Id,A.ApplicationCode,A.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               A.LeaveTypeId,LT.[Name] AS LeaveTypeName,LT.ColorHex,
               A.FromDate,A.ToDate,A.IsHalfDay,A.TotalDays,A.Reason,A.StatusId,LS.[Name] AS StatusName,
               A.ApproverRemarks,A.ApprovedDate,A.[Year],
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='ApplicationCode' AND @SortColumnDirection='ASC'  THEN A.ApplicationCode END ASC,
                 CASE WHEN @SortColumnName='ApplicationCode' AND @SortColumnDirection='DESC' THEN A.ApplicationCode END DESC,
                 CASE WHEN @SortColumnName=''                AND @SortColumnDirection='ASC'  THEN A.FromDate END ASC,
                 CASE WHEN @SortColumnName=''                AND @SortColumnDirection='DESC' THEN A.FromDate END DESC
               ) AS RowNum
        FROM dbo.LeaveApplications A
        INNER JOIN dbo.Employees E ON E.Id=A.EmployeeId
        INNER JOIN dbo.LeaveTypes LT ON LT.Id=A.LeaveTypeId
        INNER JOIN dbo.LeaveApplicationStatuses LS ON LS.Id=A.StatusId
        WHERE (@SearchTerm='' OR A.ApplicationCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,ApplicationCode,EmployeeId,EmployeeName,EmployeeCode,LeaveTypeId,LeaveTypeName,ColorHex,
                          FromDate,ToDate,IsHalfDay,TotalDays,Reason,StatusId,StatusName,ApproverRemarks,ApprovedDate,[Year]
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetEmployeeLeaveBalances — all balances for an employee/year (JSON array).
-- Computes Available = Entitled + CarriedForward - Used - Pending.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetEmployeeLeaveBalances','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetEmployeeLeaveBalances;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetEmployeeLeaveBalances;
GO
CREATE PROCEDURE dbo.SP_GetEmployeeLeaveBalances @EmployeeId UNIQUEIDENTIFIER, @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT B.Id,B.LeaveTypeId,LT.[Name] AS LeaveTypeName,LT.ColorHex,B.[Year],
               B.Entitled,B.CarriedForward,B.Used,B.Pending,
               CAST(B.Entitled + B.CarriedForward - B.Used - B.Pending AS DECIMAL(6,1)) AS Available
        FROM dbo.LeaveBalances B
        INNER JOIN dbo.LeaveTypes LT ON LT.Id=B.LeaveTypeId
        WHERE B.EmployeeId=@EmployeeId AND B.[Year]=@Year AND B.IsActive=1
        ORDER BY LT.[Name]
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_ApplyLeave — create a pending application + move Pending on the balance.
-- Validates available balance for paid types. One transaction.
-- =============================================================================
IF OBJECT_ID('dbo.SP_ApplyLeave','P') IS NOT NULL DROP PROCEDURE dbo.SP_ApplyLeave;
GO
DROP PROCEDURE IF EXISTS dbo.SP_ApplyLeave;
GO
CREATE PROCEDURE dbo.SP_ApplyLeave
    @EmployeeId UNIQUEIDENTIFIER, @LeaveTypeId UNIQUEIDENTIFIER,
    @FromDate DATETIME, @ToDate DATETIME, @IsHalfDay BIT,
    @Reason NVARCHAR(1000)=NULL, @AttachmentUrl NVARCHAR(500)=NULL,
    @ContactDuringLeave NVARCHAR(100)=NULL, @CreatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF @ToDate < @FromDate BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'To-date is before from-date.' AS [Message]; RETURN; END

        DECLARE @Days DECIMAL(5,1) = dbo.fn_CountLeaveDays(@FromDate,@ToDate,@IsHalfDay);
        IF @Days <= 0 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Invalid leave duration.' AS [Message]; RETURN; END

        DECLARE @Year INT = YEAR(@FromDate);
        DECLARE @IsPaid BIT = (SELECT IsPaid FROM dbo.LeaveTypes WHERE Id=@LeaveTypeId);

        -- overlap guard: any non-final application covering these dates?
        IF EXISTS (
            SELECT 1 FROM dbo.LeaveApplications
            WHERE EmployeeId=@EmployeeId AND StatusId IN (1,2)
              AND NOT (@ToDate < FromDate OR @FromDate > ToDate))
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Overlapping leave already exists for these dates.' AS [Message]; RETURN; END

        -- ensure a balance row exists for paid types and check availability
        IF @IsPaid = 1
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.LeaveBalances WHERE EmployeeId=@EmployeeId AND LeaveTypeId=@LeaveTypeId AND [Year]=@Year)
                INSERT INTO dbo.LeaveBalances (Id,EmployeeId,LeaveTypeId,[Year],Entitled,CarriedForward,Used,Pending,CreatedBy,CreatedDate,IsActive)
                VALUES (NEWID(),@EmployeeId,@LeaveTypeId,@Year,0,0,0,0,@CreatedBy,GETUTCDATE(),1);

            DECLARE @Available DECIMAL(6,1) =
                (SELECT Entitled + CarriedForward - Used - Pending FROM dbo.LeaveBalances
                 WHERE EmployeeId=@EmployeeId AND LeaveTypeId=@LeaveTypeId AND [Year]=@Year);
            IF @Available < @Days
            BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Insufficient leave balance.' AS [Message]; RETURN; END
        END

        -- code
        DECLARE @max INT = (SELECT MAX(TRY_CAST(SUBSTRING(ApplicationCode,5,50) AS INT)) FROM dbo.LeaveApplications WHERE ApplicationCode LIKE 'LAP-%');
        DECLARE @Code NVARCHAR(30) = dbo.fn_GetNextLeaveCode('LAP', @max);
        DECLARE @AppId UNIQUEIDENTIFIER = NEWID();

        INSERT INTO dbo.LeaveApplications
            (Id,ApplicationCode,EmployeeId,LeaveTypeId,FromDate,ToDate,IsHalfDay,TotalDays,Reason,AttachmentUrl,ContactDuringLeave,StatusId,[Year],CreatedBy,CreatedDate,IsActive)
        VALUES
            (@AppId,@Code,@EmployeeId,@LeaveTypeId,@FromDate,@ToDate,@IsHalfDay,@Days,@Reason,@AttachmentUrl,@ContactDuringLeave,1,@Year,@CreatedBy,GETUTCDATE(),1);

        -- move Pending for paid types
        IF @IsPaid = 1
            UPDATE dbo.LeaveBalances SET Pending = Pending + @Days, UpdatedBy=@CreatedBy, UpdatedDate=GETUTCDATE()
            WHERE EmployeeId=@EmployeeId AND LeaveTypeId=@LeaveTypeId AND [Year]=@Year;

        COMMIT;
        SELECT 1 AS RowsAffected,'Leave applied; pending approval.' AS [Message], @AppId AS ApplicationId, @Code AS ApplicationCode;
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetLeaveApplicationStatus — approve / reject / cancel / withdraw.
-- Approve: Pending -> Used + DEDUCTION transaction.
-- Reject/Cancel/Withdraw from Pending: release Pending.
-- Cancel after Approve: reverse Used + REVERSAL transaction.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetLeaveApplicationStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetLeaveApplicationStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetLeaveApplicationStatus;
GO
CREATE PROCEDURE dbo.SP_SetLeaveApplicationStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @ApproverId UNIQUEIDENTIFIER=NULL,
    @ApproverRemarks NVARCHAR(500)=NULL, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @Cur INT,@Emp UNIQUEIDENTIFIER,@Type UNIQUEIDENTIFIER,@Year INT,@Days DECIMAL(5,1),@IsPaid BIT;
        SELECT @Cur=A.StatusId,@Emp=A.EmployeeId,@Type=A.LeaveTypeId,@Year=A.[Year],@Days=A.TotalDays,@IsPaid=LT.IsPaid
        FROM dbo.LeaveApplications A INNER JOIN dbo.LeaveTypes LT ON LT.Id=A.LeaveTypeId
        WHERE A.Id=@Id;

        IF @Emp IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Application not found.' AS [Message]; RETURN; END
        IF @Cur IN (3,4,5) BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Application is already finalised.' AS [Message]; RETURN; END
        IF @Cur=2 AND @StatusId=2 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Application is already approved.' AS [Message]; RETURN; END

        UPDATE dbo.LeaveApplications
        SET StatusId=@StatusId, ApproverId=@ApproverId, ApproverRemarks=@ApproverRemarks,
            ApprovedDate=CASE WHEN @StatusId=2 THEN GETUTCDATE() ELSE ApprovedDate END,
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        IF @IsPaid = 1
        BEGIN
            IF @Cur=1 AND @StatusId=2   -- Pending -> Approved
            BEGIN
                UPDATE dbo.LeaveBalances SET Pending = Pending - @Days, Used = Used + @Days, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
                WHERE EmployeeId=@Emp AND LeaveTypeId=@Type AND [Year]=@Year;
                INSERT INTO dbo.LeaveTransactions (Id,EmployeeId,LeaveTypeId,[Year],TransactionTypeId,Days,LeaveApplicationId,Notes,CreatedBy,CreatedDate)
                VALUES (NEWID(),@Emp,@Type,@Year,4,-@Days,@Id,'Approved leave',@UpdatedBy,GETUTCDATE());
            END
            ELSE IF @Cur=1 AND @StatusId IN (3,4,5)  -- Pending -> Rejected/Cancelled/Withdrawn
            BEGIN
                UPDATE dbo.LeaveBalances SET Pending = Pending - @Days, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
                WHERE EmployeeId=@Emp AND LeaveTypeId=@Type AND [Year]=@Year;
            END
            ELSE IF @Cur=2 AND @StatusId=4           -- Approved -> Cancelled (reverse)
            BEGIN
                UPDATE dbo.LeaveBalances SET Used = Used - @Days, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
                WHERE EmployeeId=@Emp AND LeaveTypeId=@Type AND [Year]=@Year;
                INSERT INTO dbo.LeaveTransactions (Id,EmployeeId,LeaveTypeId,[Year],TransactionTypeId,Days,LeaveApplicationId,Notes,CreatedBy,CreatedDate)
                VALUES (NEWID(),@Emp,@Type,@Year,5,@Days,@Id,'Reversal of cancelled leave',@UpdatedBy,GETUTCDATE());
            END
        END

        COMMIT;
        SELECT 1 AS RowsAffected,'Leave application updated.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_GrantLeaveEntitlements — from policies, seed/refresh balances for a year.
-- For each active policy in @Year, set Entitled on the matching employees'
-- balance rows (respecting department/grade scope) + a GRANT transaction.
-- Idempotent: re-running updates Entitled rather than stacking.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GrantLeaveEntitlements','P') IS NOT NULL DROP PROCEDURE dbo.SP_GrantLeaveEntitlements;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GrantLeaveEntitlements;
GO
CREATE PROCEDURE dbo.SP_GrantLeaveEntitlements @Year INT, @CreatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @count INT = 0;

        DECLARE @PolicyId UNIQUEIDENTIFIER,@TypeId UNIQUEIDENTIFIER,@Dept UNIQUEIDENTIFIER,@Grade INT,@Ent DECIMAL(5,1),@MinSvc INT;
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT Id,LeaveTypeId,DepartmentId,Grade,AnnualEntitlement,MinServiceMonths
            FROM dbo.LeavePolicies WHERE EffectiveYear=@Year AND IsActive=1;
        OPEN cur; FETCH NEXT FROM cur INTO @PolicyId,@TypeId,@Dept,@Grade,@Ent,@MinSvc;
        WHILE @@FETCH_STATUS=0
        BEGIN
            -- target employees in scope and eligible by service length
            DECLARE @emps TABLE (EmpId UNIQUEIDENTIFIER);
            INSERT INTO @emps (EmpId)
            SELECT E.Id FROM dbo.Employees E
            WHERE E.IsActive=1
              AND (@Dept IS NULL OR E.DepartmentId=@Dept)
              AND (@MinSvc IS NULL OR E.DateOfJoining IS NULL OR DATEDIFF(MONTH,E.DateOfJoining,DATEFROMPARTS(@Year,1,1)) >= @MinSvc);

            -- upsert balance Entitled
            MERGE dbo.LeaveBalances AS B
            USING (SELECT EmpId FROM @emps) AS S
            ON (B.EmployeeId=S.EmpId AND B.LeaveTypeId=@TypeId AND B.[Year]=@Year)
            WHEN MATCHED THEN UPDATE SET Entitled=@Ent, UpdatedBy=@CreatedBy, UpdatedDate=GETUTCDATE()
            WHEN NOT MATCHED THEN
                INSERT (Id,EmployeeId,LeaveTypeId,[Year],Entitled,CarriedForward,Used,Pending,CreatedBy,CreatedDate,IsActive)
                VALUES (NEWID(),S.EmpId,@TypeId,@Year,@Ent,0,0,0,@CreatedBy,GETUTCDATE(),1);

            -- ledger: a GRANT row per employee (skip if already granted this year for this type)
            INSERT INTO dbo.LeaveTransactions (Id,EmployeeId,LeaveTypeId,[Year],TransactionTypeId,Days,Notes,CreatedBy,CreatedDate)
            SELECT NEWID(),S.EmpId,@TypeId,@Year,1,@Ent,'Annual entitlement grant',@CreatedBy,GETUTCDATE()
            FROM @emps S
            WHERE NOT EXISTS (SELECT 1 FROM dbo.LeaveTransactions X
                              WHERE X.EmployeeId=S.EmpId AND X.LeaveTypeId=@TypeId AND X.[Year]=@Year AND X.TransactionTypeId=1);

            SET @count = @count + 1;
            DELETE FROM @emps;
            FETCH NEXT FROM cur INTO @PolicyId,@TypeId,@Dept,@Grade,@Ent,@MinSvc;
        END
        CLOSE cur; DEALLOCATE cur;

        COMMIT;
        SELECT 1 AS RowsAffected, CONCAT('Entitlements granted for ',@count,' policy(ies).') AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
GO

-- #####################################################################################################
-- SECTION 13: Leave FunctionsTriggers   (was 15_Leave_FunctionsTriggers.sql)
-- #####################################################################################################
-- Leave: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Leave Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups + a starter set of leave types
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.LeaveApplicationStatuses)
INSERT INTO dbo.LeaveApplicationStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'PENDING','Pending',1,1),
 (2,'APPROVED','Approved',2,1),
 (3,'REJECTED','Rejected',3,1),
 (4,'CANCELLED','Cancelled',4,1),
 (5,'WITHDRAWN','Withdrawn',5,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.LeaveTransactionTypes)
INSERT INTO dbo.LeaveTransactionTypes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'GRANT','Annual Grant',1,1),
 (2,'ACCRUAL','Monthly Accrual',2,1),
 (3,'CARRY_FORWARD','Carry Forward',3,1),
 (4,'DEDUCTION','Deduction (Approved Leave)',4,1),
 (5,'REVERSAL','Reversal (Cancelled Leave)',5,1),
 (6,'ADJUSTMENT','Manual Adjustment',6,1),
 (7,'ENCASHMENT','Encashment',7,1);
GO
-- starter leave types (codes map to EnumLeaveType ids)
IF NOT EXISTS (SELECT 1 FROM dbo.LeaveTypes)
INSERT INTO dbo.LeaveTypes (Id, LeaveTypeCode, [Name], IsPaid, AllowHalfDay, RequiresAttachment, IsCarryForward, MaxCarryForward, AccrualPerMonth, ColorHex, IsActive, CreatedDate)
VALUES
 (NEWID(),'ANNUAL','Annual Leave',1,1,0,1,10,2.5,'#28a745',1,GETUTCDATE()),
 (NEWID(),'SICK','Sick Leave',1,1,1,0,NULL,NULL,'#dc3545',1,GETUTCDATE()),
 (NEWID(),'CASUAL','Casual Leave',1,1,0,0,NULL,NULL,'#17a2b8',1,GETUTCDATE()),
 (NEWID(),'UNPAID','Unpaid Leave',0,1,0,0,NULL,NULL,'#6c757d',1,GETUTCDATE()),
 (NEWID(),'MATERNITY','Maternity Leave',1,0,1,0,NULL,NULL,'#e83e8c',1,GETUTCDATE()),
 (NEWID(),'PATERNITY','Paternity Leave',1,0,0,0,NULL,NULL,'#fd7e14',1,GETUTCDATE());
GO
-- =============================================================================
-- fn_GetNextLeaveCode — generic prefixed sequence (e.g. LAP-####, LT-####)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextLeaveCode','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetNextLeaveCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextLeaveCode;
GO
CREATE FUNCTION dbo.fn_GetNextLeaveCode(@Prefix NVARCHAR(10), @CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- fn_CountLeaveDays — inclusive day count between two dates, half-day aware.
-- (Calendar days; weekend/holiday exclusion can be layered on later using the
--  employee's shift WeekOffDays via fn_GetEmployeeShiftOn.)
-- =============================================================================
IF OBJECT_ID('dbo.fn_CountLeaveDays','FN') IS NOT NULL DROP FUNCTION dbo.fn_CountLeaveDays;
GO
DROP FUNCTION IF EXISTS dbo.fn_CountLeaveDays;
GO
CREATE FUNCTION dbo.fn_CountLeaveDays(@From DATETIME, @To DATETIME, @IsHalfDay BIT)
RETURNS DECIMAL(5,1)
AS
BEGIN
    IF @From IS NULL OR @To IS NULL OR @To < @From RETURN 0;
    DECLARE @days INT = DATEDIFF(DAY, @From, @To) + 1;
    IF @IsHalfDay = 1 AND @days = 1 RETURN 0.5;
    RETURN @days;
END
GO
-- =============================================================================
-- TRIGGERS — LeaveTypes / LeavePolicies / LeaveApplications (audit to *Log).
-- Same shape as TRG_InsertDepartments: PerformedUser from dbo.Employees.
-- =============================================================================

-- ---- LeaveTypes ----
IF OBJECT_ID('dbo.TRG_InsertLeaveTypes','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertLeaveTypes;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertLeaveTypes;
GO
CREATE TRIGGER dbo.TRG_InsertLeaveTypes ON dbo.LeaveTypes AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeaveTypesLog (Id,LeaveTypeId,LeaveTypeCode,[Name],IsPaid,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.LeaveTypeCode,I.[Name],I.IsPaid,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateLeaveTypes','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateLeaveTypes;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateLeaveTypes;
GO
CREATE TRIGGER dbo.TRG_UpdateLeaveTypes ON dbo.LeaveTypes AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeaveTypesLog (Id,LeaveTypeId,LeaveTypeCode,[Name],IsPaid,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.LeaveTypeCode,I.[Name],I.IsPaid,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- LeavePolicies ----
IF OBJECT_ID('dbo.TRG_InsertLeavePolicies','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertLeavePolicies;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertLeavePolicies;
GO
CREATE TRIGGER dbo.TRG_InsertLeavePolicies ON dbo.LeavePolicies AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeavePoliciesLog (Id,LeavePolicyId,PolicyCode,LeaveTypeId,AnnualEntitlement,EffectiveYear,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.PolicyCode,I.LeaveTypeId,I.AnnualEntitlement,I.EffectiveYear,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateLeavePolicies','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateLeavePolicies;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateLeavePolicies;
GO
CREATE TRIGGER dbo.TRG_UpdateLeavePolicies ON dbo.LeavePolicies AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeavePoliciesLog (Id,LeavePolicyId,PolicyCode,LeaveTypeId,AnnualEntitlement,EffectiveYear,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.PolicyCode,I.LeaveTypeId,I.AnnualEntitlement,I.EffectiveYear,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- LeaveApplications ----
IF OBJECT_ID('dbo.TRG_InsertLeaveApplications','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertLeaveApplications;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertLeaveApplications;
GO
CREATE TRIGGER dbo.TRG_InsertLeaveApplications ON dbo.LeaveApplications AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeaveApplicationsLog (Id,LeaveApplicationId,ApplicationCode,EmployeeId,LeaveTypeId,FromDate,ToDate,TotalDays,StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ApplicationCode,I.EmployeeId,I.LeaveTypeId,I.FromDate,I.ToDate,I.TotalDays,I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateLeaveApplications','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateLeaveApplications;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateLeaveApplications;
GO
CREATE TRIGGER dbo.TRG_UpdateLeaveApplications ON dbo.LeaveApplications AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LeaveApplicationsLog (Id,LeaveApplicationId,ApplicationCode,EmployeeId,LeaveTypeId,FromDate,ToDate,TotalDays,StatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ApplicationCode,I.EmployeeId,I.LeaveTypeId,I.FromDate,I.ToDate,I.TotalDays,I.StatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
GO

-- #####################################################################################################
-- SECTION 14: Payroll StoredProcedures   (was 16b_Payroll_StoredProcedures.sql)
-- #####################################################################################################
-- Payroll: stored procedures

-- =============================================================================
-- HumanResourcesDB — Payroll Module : Stored Procedures
-- =============================================================================

-- =============================================================================
-- SP_GetAllPayComponentsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllPayComponentsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllPayComponentsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllPayComponentsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllPayComponentsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.PayComponents C
    WHERE (@SearchTerm='' OR C.ComponentCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT C.Id,C.ComponentCode,C.[Name],C.ComponentTypeId,CT.[Name] AS ComponentTypeName,
               C.CalcMethod,C.DefaultValue,C.IsTaxable,C.IsProrated,C.SortOrder,C.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='ComponentCode' AND @SortColumnDirection='ASC'  THEN C.ComponentCode END ASC,
                 CASE WHEN @SortColumnName='ComponentCode' AND @SortColumnDirection='DESC' THEN C.ComponentCode END DESC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='ASC'  THEN C.SortOrder END ASC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='DESC' THEN C.SortOrder END DESC
               ) AS RowNum
        FROM dbo.PayComponents C
        INNER JOIN dbo.PayComponentTypes CT ON CT.Id=C.ComponentTypeId
        WHERE (@SearchTerm='' OR C.ComponentCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,ComponentCode,[Name],ComponentTypeId,ComponentTypeName,CalcMethod,DefaultValue,IsTaxable,IsProrated,SortOrder,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllSalaryStructuresPagingWithSearch  (current structures, latest first)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllSalaryStructuresPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllSalaryStructuresPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllSalaryStructuresPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllSalaryStructuresPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.SalaryStructures S INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
    WHERE (@SearchTerm='' OR S.StructureCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT S.Id,S.StructureCode,S.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               S.BasicSalary,S.Currency,S.EffectiveFrom,S.EffectiveTo,S.IsActive,
               (SELECT COUNT(1) FROM dbo.SalaryStructureLines L WHERE L.SalaryStructureId=S.Id) AS LineCount,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='StructureCode' AND @SortColumnDirection='ASC'  THEN S.StructureCode END ASC,
                 CASE WHEN @SortColumnName='StructureCode' AND @SortColumnDirection='DESC' THEN S.StructureCode END DESC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='ASC'  THEN S.EffectiveFrom END ASC,
                 CASE WHEN @SortColumnName=''              AND @SortColumnDirection='DESC' THEN S.EffectiveFrom END DESC
               ) AS RowNum
        FROM dbo.SalaryStructures S
        INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
        WHERE (@SearchTerm='' OR S.StructureCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,StructureCode,EmployeeId,EmployeeName,EmployeeCode,BasicSalary,Currency,EffectiveFrom,EffectiveTo,LineCount,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetSalaryStructureDetail — header + lines as one JSON object.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetSalaryStructureDetail','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetSalaryStructureDetail;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetSalaryStructureDetail;
GO
CREATE PROCEDURE dbo.SP_GetSalaryStructureDetail @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT S.Id,S.StructureCode,S.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               S.BasicSalary,S.Currency,S.EffectiveFrom,S.EffectiveTo,S.Remarks,
               (SELECT L.Id,L.PayComponentId,PC.ComponentCode,PC.[Name] AS ComponentName,
                       PC.ComponentTypeId,L.CalcMethod,L.[Value]
                FROM dbo.SalaryStructureLines L
                INNER JOIN dbo.PayComponents PC ON PC.Id=L.PayComponentId
                WHERE L.SalaryStructureId=S.Id
                FOR JSON PATH) AS Lines
        FROM dbo.SalaryStructures S
        INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
        WHERE S.Id=@Id
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) AS result;
END
GO
-- =============================================================================
-- SP_ProcessPayrollRun — the engine.
-- Creates (or re-processes a Draft) run, then for every active employee with a
-- current salary structure: prorates earnings by paid days, computes deductions,
-- applies the next loan installment, writes payslip + lines, advances the loan.
-- Returns RowsAffected / Message / RunId.
-- =============================================================================
IF OBJECT_ID('dbo.SP_ProcessPayrollRun','P') IS NOT NULL DROP PROCEDURE dbo.SP_ProcessPayrollRun;
GO
DROP PROCEDURE IF EXISTS dbo.SP_ProcessPayrollRun;
GO
CREATE PROCEDURE dbo.SP_ProcessPayrollRun
    @PayMonth INT, @PayYear INT,
    @PeriodStart DATETIME, @PeriodEnd DATETIME, @WorkingDays DECIMAL(5,1),
    @CreatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF @WorkingDays IS NULL OR @WorkingDays <= 0
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Working days must be greater than zero.' AS [Message]; RETURN; END

        -- find or create the run; block if already approved/paid
        DECLARE @RunId UNIQUEIDENTIFIER, @Status INT;
        SELECT @RunId=Id,@Status=StatusId FROM dbo.PayrollRuns WHERE PayMonth=@PayMonth AND PayYear=@PayYear AND IsActive=1;

        IF @RunId IS NOT NULL AND @Status >= 3
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Payroll for this period is already approved or paid.' AS [Message]; RETURN; END

        IF @RunId IS NULL
        BEGIN
            DECLARE @max INT = (SELECT MAX(TRY_CAST(SUBSTRING(RunCode,6,50) AS INT)) FROM dbo.PayrollRuns WHERE RunCode LIKE 'PRUN-%');
            SET @RunId = NEWID();
            INSERT INTO dbo.PayrollRuns (Id,RunCode,PayMonth,PayYear,PeriodStart,PeriodEnd,WorkingDays,StatusId,CreatedBy,CreatedDate,IsActive)
            VALUES (@RunId, dbo.fn_GetNextPayrollCode('PRUN',@max), @PayMonth,@PayYear,@PeriodStart,@PeriodEnd,@WorkingDays,1,@CreatedBy,GETUTCDATE(),1);
        END
        ELSE
        BEGIN
            -- re-process a Draft: clear previous payslips + reverse their loan installments
            UPDATE L SET L.PaidInstallments = L.PaidInstallments - 1,
                         L.OutstandingAmount = L.OutstandingAmount + LI.Amount,
                         L.StatusId = CASE WHEN L.StatusId=2 THEN 1 ELSE L.StatusId END
            FROM dbo.Loans L
            INNER JOIN dbo.LoanInstallments LI ON LI.LoanId=L.Id
            INNER JOIN dbo.Payslips P ON P.Id=LI.PayslipId
            WHERE P.PayrollRunId=@RunId;

            DELETE LI FROM dbo.LoanInstallments LI INNER JOIN dbo.Payslips P ON P.Id=LI.PayslipId WHERE P.PayrollRunId=@RunId;
            DELETE PL FROM dbo.PayslipLines PL INNER JOIN dbo.Payslips P ON P.Id=PL.PayslipId WHERE P.PayrollRunId=@RunId;
            DELETE FROM dbo.Payslips WHERE PayrollRunId=@RunId;

            UPDATE dbo.PayrollRuns SET PeriodStart=@PeriodStart,PeriodEnd=@PeriodEnd,WorkingDays=@WorkingDays,
                   StatusId=1,UpdatedBy=@CreatedBy,UpdatedDate=GETUTCDATE() WHERE Id=@RunId;
        END

        DECLARE @psMax INT = (SELECT MAX(TRY_CAST(SUBSTRING(PayslipCode,5,50) AS INT)) FROM dbo.Payslips WHERE PayslipCode LIKE 'PSL-%');
        SET @psMax = ISNULL(@psMax,0);

        -- cursor over employees who have a current salary structure for the period
        DECLARE @Emp UNIQUEIDENTIFIER,@StructId UNIQUEIDENTIFIER,@Basic DECIMAL(18,2),@Curr NVARCHAR(10);
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT S.EmployeeId, S.Id, S.BasicSalary, S.Currency
            FROM dbo.SalaryStructures S
            INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId AND E.IsActive=1
            WHERE S.IsActive=1 AND S.EffectiveFrom <= @PeriodEnd AND (S.EffectiveTo IS NULL OR S.EffectiveTo >= @PeriodStart);
        OPEN cur; FETCH NEXT FROM cur INTO @Emp,@StructId,@Basic,@Curr;
        WHILE @@FETCH_STATUS=0
        BEGIN
            DECLARE @PaidDays DECIMAL(5,1) = dbo.fn_GetPaidDays(@Emp,@PeriodStart,@PeriodEnd,@WorkingDays);
            DECLARE @Lop DECIMAL(5,1) = @WorkingDays - @PaidDays;
            DECLARE @Factor DECIMAL(18,6) = @PaidDays / @WorkingDays;

            DECLARE @BasicEarned DECIMAL(18,2) = ROUND(@Basic * @Factor, 2);
            DECLARE @Gross DECIMAL(18,2) = @BasicEarned;     -- basic is an earning
            DECLARE @Ded DECIMAL(18,2) = 0;

            SET @psMax = @psMax + 1;
            DECLARE @PsId UNIQUEIDENTIFIER = NEWID();
            INSERT INTO dbo.Payslips (Id,PayslipCode,PayrollRunId,EmployeeId,PaidDays,LopDays,BasicEarned,GrossEarnings,TotalDeductions,LoanDeduction,NetPay,Currency,CreatedBy,CreatedDate,IsActive)
            VALUES (@PsId, dbo.fn_GetNextPayrollCode('PSL',@psMax-1), @RunId,@Emp,@PaidDays,@Lop,@BasicEarned,0,0,0,0,@Curr,@CreatedBy,GETUTCDATE(),1);

            -- Basic as a line
            INSERT INTO dbo.PayslipLines (Id,PayslipId,PayComponentId,ComponentName,ComponentTypeId,Amount,CreatedDate)
            VALUES (NEWID(),@PsId,NULL,'Basic',1,@BasicEarned,GETUTCDATE());

            -- component lines from the salary structure
            DECLARE @PcId UNIQUEIDENTIFIER,@PcName NVARCHAR(100),@PcType INT,@Calc INT,@Val DECIMAL(18,2),@Prorate BIT,@LineAmt DECIMAL(18,2);
            DECLARE lc CURSOR LOCAL FAST_FORWARD FOR
                SELECT L.PayComponentId,PC.[Name],PC.ComponentTypeId,L.CalcMethod,L.[Value],PC.IsProrated
                FROM dbo.SalaryStructureLines L
                INNER JOIN dbo.PayComponents PC ON PC.Id=L.PayComponentId
                WHERE L.SalaryStructureId=@StructId AND PC.IsActive=1;
            OPEN lc; FETCH NEXT FROM lc INTO @PcId,@PcName,@PcType,@Calc,@Val,@Prorate;
            WHILE @@FETCH_STATUS=0
            BEGIN
                -- base amount: percentage of basic, or fixed
                SET @LineAmt = CASE WHEN @Calc=2 THEN ROUND(@Basic * @Val/100.0,2) ELSE @Val END;
                -- prorate earnings/deductions flagged as prorated
                IF @Prorate=1 SET @LineAmt = ROUND(@LineAmt * @Factor,2);

                INSERT INTO dbo.PayslipLines (Id,PayslipId,PayComponentId,ComponentName,ComponentTypeId,Amount,CreatedDate)
                VALUES (NEWID(),@PsId,@PcId,@PcName,@PcType,@LineAmt,GETUTCDATE());

                IF @PcType=1 SET @Gross = @Gross + @LineAmt; ELSE SET @Ded = @Ded + @LineAmt;
                FETCH NEXT FROM lc INTO @PcId,@PcName,@PcType,@Calc,@Val,@Prorate;
            END
            CLOSE lc; DEALLOCATE lc;

            -- active loan: take this month's installment (capped at outstanding)
            DECLARE @LoanDed DECIMAL(18,2) = 0;
            DECLARE @LoanId UNIQUEIDENTIFIER,@Inst DECIMAL(18,2),@Out DECIMAL(18,2);
            DECLARE ln CURSOR LOCAL FAST_FORWARD FOR
                SELECT Id,InstallmentAmount,OutstandingAmount FROM dbo.Loans
                WHERE EmployeeId=@Emp AND StatusId=1 AND OutstandingAmount > 0
                  AND (StartYear < @PayYear OR (StartYear=@PayYear AND StartMonth <= @PayMonth));
            OPEN ln; FETCH NEXT FROM ln INTO @LoanId,@Inst,@Out;
            WHILE @@FETCH_STATUS=0
            BEGIN
                DECLARE @Take DECIMAL(18,2) = CASE WHEN @Inst > @Out THEN @Out ELSE @Inst END;
                INSERT INTO dbo.LoanInstallments (Id,LoanId,PayslipId,PayMonth,PayYear,Amount,CreatedDate)
                VALUES (NEWID(),@LoanId,@PsId,@PayMonth,@PayYear,@Take,GETUTCDATE());
                UPDATE dbo.Loans SET PaidInstallments=PaidInstallments+1, OutstandingAmount=OutstandingAmount-@Take,
                       StatusId = CASE WHEN OutstandingAmount-@Take <= 0 THEN 2 ELSE 1 END,
                       UpdatedBy=@CreatedBy, UpdatedDate=GETUTCDATE()
                WHERE Id=@LoanId;
                SET @LoanDed = @LoanDed + @Take;
                FETCH NEXT FROM ln INTO @LoanId,@Inst,@Out;
            END
            CLOSE ln; DEALLOCATE ln;

            IF @LoanDed > 0
                INSERT INTO dbo.PayslipLines (Id,PayslipId,PayComponentId,ComponentName,ComponentTypeId,Amount,CreatedDate)
                VALUES (NEWID(),@PsId,NULL,'Loan Recovery',2,@LoanDed,GETUTCDATE());

            DECLARE @TotalDed DECIMAL(18,2) = @Ded + @LoanDed;
            DECLARE @Net DECIMAL(18,2) = @Gross - @TotalDed;

            UPDATE dbo.Payslips SET GrossEarnings=@Gross, TotalDeductions=@TotalDed, LoanDeduction=@LoanDed, NetPay=@Net
            WHERE Id=@PsId;

            FETCH NEXT FROM cur INTO @Emp,@StructId,@Basic,@Curr;
        END
        CLOSE cur; DEALLOCATE cur;

        -- roll up run totals
        UPDATE R SET R.TotalGross=T.G, R.TotalDeductions=T.D, R.TotalNet=T.N, R.EmployeeCount=T.C,
                     R.StatusId=2, R.ProcessedBy=@CreatedBy, R.ProcessedDate=GETUTCDATE(),
                     R.UpdatedBy=@CreatedBy, R.UpdatedDate=GETUTCDATE()
        FROM dbo.PayrollRuns R
        CROSS APPLY (SELECT ISNULL(SUM(GrossEarnings),0) G, ISNULL(SUM(TotalDeductions),0) D,
                            ISNULL(SUM(NetPay),0) N, COUNT(1) C FROM dbo.Payslips WHERE PayrollRunId=@RunId) T
        WHERE R.Id=@RunId;

        COMMIT;
        DECLARE @cnt INT = (SELECT EmployeeCount FROM dbo.PayrollRuns WHERE Id=@RunId);
        SELECT 1 AS RowsAffected, CONCAT('Payroll processed for ',@cnt,' employee(s).') AS [Message], @RunId AS RunId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message];
    END CATCH
END
GO
-- =============================================================================
-- SP_SetPayrollRunStatus — approve / mark paid / cancel.
-- Cancel of a Draft/Processed run also reverses loan installments.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetPayrollRunStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetPayrollRunStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetPayrollRunStatus;
GO
CREATE PROCEDURE dbo.SP_SetPayrollRunStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @ActionBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @Cur INT = (SELECT StatusId FROM dbo.PayrollRuns WHERE Id=@Id);
        IF @Cur IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Run not found.' AS [Message]; RETURN; END
        IF @Cur=5 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Run is cancelled.' AS [Message]; RETURN; END
        IF @Cur=4 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Run is already paid.' AS [Message]; RETURN; END

        IF @StatusId=5   -- cancel: reverse loan installments tied to this run
        BEGIN
            UPDATE L SET L.PaidInstallments=L.PaidInstallments-1, L.OutstandingAmount=L.OutstandingAmount+LI.Amount,
                         L.StatusId=CASE WHEN L.StatusId=2 THEN 1 ELSE L.StatusId END,
                         L.UpdatedBy=@ActionBy, L.UpdatedDate=GETUTCDATE()
            FROM dbo.Loans L
            INNER JOIN dbo.LoanInstallments LI ON LI.LoanId=L.Id
            INNER JOIN dbo.Payslips P ON P.Id=LI.PayslipId
            WHERE P.PayrollRunId=@Id;

            DELETE LI FROM dbo.LoanInstallments LI INNER JOIN dbo.Payslips P ON P.Id=LI.PayslipId WHERE P.PayrollRunId=@Id;
        END

        UPDATE dbo.PayrollRuns
        SET StatusId=@StatusId,
            ApprovedBy = CASE WHEN @StatusId=3 THEN @ActionBy ELSE ApprovedBy END,
            ApprovedDate = CASE WHEN @StatusId=3 THEN GETUTCDATE() ELSE ApprovedDate END,
            UpdatedBy=@ActionBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        COMMIT;
        SELECT 1 AS RowsAffected,'Payroll run updated.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_GetPayslipsByRun — list payslips of a run (JSON array)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetPayslipsByRun','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetPayslipsByRun;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetPayslipsByRun;
GO
CREATE PROCEDURE dbo.SP_GetPayslipsByRun @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT P.Id,P.PayslipCode,P.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               P.PaidDays,P.LopDays,P.BasicEarned,P.GrossEarnings,P.TotalDeductions,P.LoanDeduction,P.NetPay,P.Currency
        FROM dbo.Payslips P
        INNER JOIN dbo.Employees E ON E.Id=P.EmployeeId
        WHERE P.PayrollRunId=@RunId
        ORDER BY E.FullName
        FOR JSON PATH
    ) AS result;
END
GO
-- =============================================================================
-- SP_GetPayslipDetail — one payslip header + lines (JSON object)
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetPayslipDetail','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetPayslipDetail;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetPayslipDetail;
GO
CREATE PROCEDURE dbo.SP_GetPayslipDetail @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT P.Id,P.PayslipCode,P.PayrollRunId,R.RunCode,R.PayMonth,R.PayYear,
               P.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               P.PaidDays,P.LopDays,P.BasicEarned,P.GrossEarnings,P.TotalDeductions,P.LoanDeduction,P.NetPay,P.Currency,
               (SELECT ComponentName,ComponentTypeId,Amount FROM dbo.PayslipLines WHERE PayslipId=P.Id ORDER BY ComponentTypeId, Amount DESC FOR JSON PATH) AS Lines
        FROM dbo.Payslips P
        INNER JOIN dbo.PayrollRuns R ON R.Id=P.PayrollRunId
        INNER JOIN dbo.Employees E ON E.Id=P.EmployeeId
        WHERE P.Id=@Id
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) AS result;
END
GO
-- =============================================================================
-- SP_GetAllPayrollRunsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllPayrollRunsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllPayrollRunsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllPayrollRunsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllPayrollRunsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.PayrollRuns R
    WHERE (@SearchTerm='' OR R.RunCode LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT R.Id,R.RunCode,R.PayMonth,R.PayYear,R.PeriodStart,R.PeriodEnd,R.WorkingDays,
               R.StatusId,RS.[Name] AS StatusName,R.TotalGross,R.TotalDeductions,R.TotalNet,R.EmployeeCount,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='' AND @SortColumnDirection='ASC'  THEN R.PayYear END ASC,
                 CASE WHEN @SortColumnName='' AND @SortColumnDirection='DESC' THEN R.PayYear END DESC,
                 CASE WHEN @SortColumnName='' AND @SortColumnDirection='ASC'  THEN R.PayMonth END ASC,
                 CASE WHEN @SortColumnName='' AND @SortColumnDirection='DESC' THEN R.PayMonth END DESC
               ) AS RowNum
        FROM dbo.PayrollRuns R
        INNER JOIN dbo.PayrollRunStatuses RS ON RS.Id=R.StatusId
        WHERE (@SearchTerm='' OR R.RunCode LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,RunCode,PayMonth,PayYear,PeriodStart,PeriodEnd,WorkingDays,StatusId,StatusName,TotalGross,TotalDeductions,TotalNet,EmployeeCount
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllLoansPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllLoansPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllLoansPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllLoansPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllLoansPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.Loans Ln INNER JOIN dbo.Employees E ON E.Id=Ln.EmployeeId
    WHERE (@SearchTerm='' OR Ln.LoanCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT Ln.Id,Ln.LoanCode,Ln.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               Ln.PrincipalAmount,Ln.InstallmentAmount,Ln.TotalInstallments,Ln.PaidInstallments,
               Ln.OutstandingAmount,Ln.StartMonth,Ln.StartYear,Ln.StatusId,LS.[Name] AS StatusName,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='LoanCode' AND @SortColumnDirection='ASC'  THEN Ln.LoanCode END ASC,
                 CASE WHEN @SortColumnName='LoanCode' AND @SortColumnDirection='DESC' THEN Ln.LoanCode END DESC,
                 CASE WHEN @SortColumnName=''         AND @SortColumnDirection='ASC'  THEN Ln.CreatedDate END ASC,
                 CASE WHEN @SortColumnName=''         AND @SortColumnDirection='DESC' THEN Ln.CreatedDate END DESC
               ) AS RowNum
        FROM dbo.Loans Ln
        INNER JOIN dbo.Employees E ON E.Id=Ln.EmployeeId
        INNER JOIN dbo.LoanStatuses LS ON LS.Id=Ln.StatusId
        WHERE (@SearchTerm='' OR Ln.LoanCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,LoanCode,EmployeeId,EmployeeName,EmployeeCode,PrincipalAmount,InstallmentAmount,
                          TotalInstallments,PaidInstallments,OutstandingAmount,StartMonth,StartYear,StatusId,StatusName
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
GO

-- #####################################################################################################
-- SECTION 15: Payroll FunctionsTriggers   (was 16_Payroll_FunctionsTriggers.sql)
-- #####################################################################################################
-- Payroll: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Payroll Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups + a starter set of pay components
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.PayComponentTypes)
INSERT INTO dbo.PayComponentTypes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'EARNING','Earning',1,1),
 (2,'DEDUCTION','Deduction',2,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.PayrollRunStatuses)
INSERT INTO dbo.PayrollRunStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),
 (2,'PROCESSED','Processed',2,1),
 (3,'APPROVED','Approved',3,1),
 (4,'PAID','Paid',4,1),
 (5,'CANCELLED','Cancelled',5,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.LoanStatuses)
INSERT INTO dbo.LoanStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'ACTIVE','Active',1,1),
 (2,'CLOSED','Closed',2,1),
 (3,'CANCELLED','Cancelled',3,1);
GO
-- starter pay components (CalcMethod 1 Fixed / 2 % of basic)
IF NOT EXISTS (SELECT 1 FROM dbo.PayComponents)
INSERT INTO dbo.PayComponents (Id, ComponentCode, [Name], ComponentTypeId, CalcMethod, DefaultValue, IsTaxable, IsProrated, SortOrder, IsActive, CreatedDate)
VALUES
 (NEWID(),'HRA','House Rent Allowance',1,2,40,1,1,2,1,GETUTCDATE()),
 (NEWID(),'CONVEY','Conveyance Allowance',1,1,500,1,1,3,1,GETUTCDATE()),
 (NEWID(),'MEDICAL','Medical Allowance',1,1,300,0,1,4,1,GETUTCDATE()),
 (NEWID(),'SPECIAL','Special Allowance',1,2,15,1,1,5,1,GETUTCDATE()),
 (NEWID(),'PF','Provident Fund',2,2,12,0,0,10,1,GETUTCDATE()),
 (NEWID(),'TAX','Income Tax',2,1,0,0,0,11,1,GETUTCDATE());
GO
-- =============================================================================
-- fn_GetNextPayrollCode — generic prefixed sequence (PRUN-####, PSL-####, ...)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextPayrollCode','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetNextPayrollCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextPayrollCode;
GO
CREATE FUNCTION dbo.fn_GetNextPayrollCode(@Prefix NVARCHAR(10), @CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- fn_GetPaidDays — paid days for an employee in a period.
--   WorkingDays - LOP (unpaid leave + unauthorised absence) read from
--   dbo.Attendances + dbo.LeaveApplications (approved unpaid leave).
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetPaidDays','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetPaidDays;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetPaidDays;
GO
CREATE FUNCTION dbo.fn_GetPaidDays(@EmployeeId UNIQUEIDENTIFIER, @PeriodStart DATETIME, @PeriodEnd DATETIME, @WorkingDays DECIMAL(5,1))
RETURNS DECIMAL(5,1)
AS
BEGIN
    DECLARE @Lop DECIMAL(5,1) = 0;

    -- approved unpaid-leave days that fall in the period
    SELECT @Lop = @Lop + ISNULL(SUM(A.TotalDays),0)
    FROM dbo.LeaveApplications A
    INNER JOIN dbo.LeaveTypes LT ON LT.Id = A.LeaveTypeId
    WHERE A.EmployeeId = @EmployeeId AND A.StatusId = 2 AND LT.IsPaid = 0
      AND A.FromDate <= @PeriodEnd AND A.ToDate >= @PeriodStart;

    DECLARE @Paid DECIMAL(5,1) = @WorkingDays - @Lop;
    IF @Paid < 0 SET @Paid = 0;
    RETURN @Paid;
END
GO
-- =============================================================================
-- TRIGGERS — PayComponents / SalaryStructures / PayrollRuns / Loans
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertPayComponents','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertPayComponents;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertPayComponents;
GO
CREATE TRIGGER dbo.TRG_InsertPayComponents ON dbo.PayComponents AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PayComponentsLog (Id,PayComponentId,ComponentCode,[Name],ComponentTypeId,CalcMethod,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ComponentCode,I.[Name],I.ComponentTypeId,I.CalcMethod,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdatePayComponents','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdatePayComponents;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdatePayComponents;
GO
CREATE TRIGGER dbo.TRG_UpdatePayComponents ON dbo.PayComponents AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PayComponentsLog (Id,PayComponentId,ComponentCode,[Name],ComponentTypeId,CalcMethod,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ComponentCode,I.[Name],I.ComponentTypeId,I.CalcMethod,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertSalaryStructures','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertSalaryStructures;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertSalaryStructures;
GO
CREATE TRIGGER dbo.TRG_InsertSalaryStructures ON dbo.SalaryStructures AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SalaryStructuresLog (Id,SalaryStructureId,StructureCode,EmployeeId,BasicSalary,EffectiveFrom,EffectiveTo,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.StructureCode,I.EmployeeId,I.BasicSalary,I.EffectiveFrom,I.EffectiveTo,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateSalaryStructures','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateSalaryStructures;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateSalaryStructures;
GO
CREATE TRIGGER dbo.TRG_UpdateSalaryStructures ON dbo.SalaryStructures AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SalaryStructuresLog (Id,SalaryStructureId,StructureCode,EmployeeId,BasicSalary,EffectiveFrom,EffectiveTo,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.StructureCode,I.EmployeeId,I.BasicSalary,I.EffectiveFrom,I.EffectiveTo,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertPayrollRuns','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertPayrollRuns;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertPayrollRuns;
GO
CREATE TRIGGER dbo.TRG_InsertPayrollRuns ON dbo.PayrollRuns AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PayrollRunsLog (Id,PayrollRunId,RunCode,PayMonth,PayYear,StatusId,TotalNet,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RunCode,I.PayMonth,I.PayYear,I.StatusId,I.TotalNet,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdatePayrollRuns','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdatePayrollRuns;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdatePayrollRuns;
GO
CREATE TRIGGER dbo.TRG_UpdatePayrollRuns ON dbo.PayrollRuns AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PayrollRunsLog (Id,PayrollRunId,RunCode,PayMonth,PayYear,StatusId,TotalNet,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RunCode,I.PayMonth,I.PayYear,I.StatusId,I.TotalNet,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertLoans','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertLoans;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertLoans;
GO
CREATE TRIGGER dbo.TRG_InsertLoans ON dbo.Loans AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LoansLog (Id,LoanId,LoanCode,EmployeeId,PrincipalAmount,OutstandingAmount,StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.LoanCode,I.EmployeeId,I.PrincipalAmount,I.OutstandingAmount,I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateLoans','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateLoans;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateLoans;
GO
CREATE TRIGGER dbo.TRG_UpdateLoans ON dbo.Loans AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LoansLog (Id,LoanId,LoanCode,EmployeeId,PrincipalAmount,OutstandingAmount,StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.LoanCode,I.EmployeeId,I.PrincipalAmount,I.OutstandingAmount,I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
GO

-- #####################################################################################################
-- SECTION 16: Performance StoredProcedures   (was 17b_Performance_StoredProcedures.sql)
-- #####################################################################################################
-- Performance: stored procedures

-- =============================================================================
-- HumanResourcesDB — Performance Module : Stored Procedures
-- Score-moving SPs are transactional and return RowsAffected / Message.
-- =============================================================================

-- =============================================================================
-- SP_GetAllAppraisalCyclesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllAppraisalCyclesPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllAppraisalCyclesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllAppraisalCyclesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllAppraisalCyclesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.AppraisalCycles C
    WHERE (@SearchTerm='' OR C.CycleCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT C.Id,C.CycleCode,C.[Name],C.PeriodStart,C.PeriodEnd,C.GoalDueDate,C.ReviewDueDate,
               C.RatingScaleMax,C.StatusId,S.[Name] AS StatusName,C.IsActive,
               (SELECT COUNT(1) FROM dbo.Reviews R WHERE R.AppraisalCycleId=C.Id AND R.IsActive=1) AS ReviewCount,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='CycleCode' AND @SortColumnDirection='ASC'  THEN C.CycleCode END ASC,
                 CASE WHEN @SortColumnName='CycleCode' AND @SortColumnDirection='DESC' THEN C.CycleCode END DESC,
                 CASE WHEN @SortColumnName=''          AND @SortColumnDirection='ASC'  THEN C.PeriodStart END ASC,
                 CASE WHEN @SortColumnName=''          AND @SortColumnDirection='DESC' THEN C.PeriodStart END DESC
               ) AS RowNum
        FROM dbo.AppraisalCycles C
        INNER JOIN dbo.AppraisalCycleStatuses S ON S.Id=C.StatusId
        WHERE (@SearchTerm='' OR C.CycleCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,CycleCode,[Name],PeriodStart,PeriodEnd,GoalDueDate,ReviewDueDate,RatingScaleMax,StatusId,StatusName,ReviewCount,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllGoalsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllGoalsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllGoalsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllGoalsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllGoalsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.Goals G INNER JOIN dbo.Employees E ON E.Id=G.EmployeeId
    WHERE (@SearchTerm='' OR G.GoalCode LIKE '%'+@SearchTerm+'%' OR G.Title LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT G.Id,G.GoalCode,G.AppraisalCycleId,C.[Name] AS CycleName,G.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               G.Title,G.Weight,G.ProgressPercent,G.StatusId,GS.[Name] AS StatusName,G.StartDate,G.DueDate,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='GoalCode' AND @SortColumnDirection='ASC'  THEN G.GoalCode END ASC,
                 CASE WHEN @SortColumnName='GoalCode' AND @SortColumnDirection='DESC' THEN G.GoalCode END DESC,
                 CASE WHEN @SortColumnName=''         AND @SortColumnDirection='ASC'  THEN G.CreatedDate END ASC,
                 CASE WHEN @SortColumnName=''         AND @SortColumnDirection='DESC' THEN G.CreatedDate END DESC
               ) AS RowNum
        FROM dbo.Goals G
        INNER JOIN dbo.Employees E ON E.Id=G.EmployeeId
        INNER JOIN dbo.AppraisalCycles C ON C.Id=G.AppraisalCycleId
        INNER JOIN dbo.GoalStatuses GS ON GS.Id=G.StatusId
        WHERE (@SearchTerm='' OR G.GoalCode LIKE '%'+@SearchTerm+'%' OR G.Title LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,GoalCode,AppraisalCycleId,CycleName,EmployeeId,EmployeeName,EmployeeCode,Title,Weight,ProgressPercent,StatusId,StatusName,StartDate,DueDate
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetAllReviewsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllReviewsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllReviewsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllReviewsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllReviewsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.Reviews R INNER JOIN dbo.Employees E ON E.Id=R.EmployeeId
    WHERE (@SearchTerm='' OR R.ReviewCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT R.Id,R.ReviewCode,R.AppraisalCycleId,C.[Name] AS CycleName,R.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               R.ReviewerId,Rev.FullName AS ReviewerName,R.StatusId,RS.[Name] AS StatusName,
               R.SelfRating,R.ManagerRating,R.OverallRating,C.RatingScaleMax,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='ReviewCode' AND @SortColumnDirection='ASC'  THEN R.ReviewCode END ASC,
                 CASE WHEN @SortColumnName='ReviewCode' AND @SortColumnDirection='DESC' THEN R.ReviewCode END DESC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='ASC'  THEN E.FullName END ASC,
                 CASE WHEN @SortColumnName=''           AND @SortColumnDirection='DESC' THEN E.FullName END DESC
               ) AS RowNum
        FROM dbo.Reviews R
        INNER JOIN dbo.Employees E ON E.Id=R.EmployeeId
        INNER JOIN dbo.AppraisalCycles C ON C.Id=R.AppraisalCycleId
        INNER JOIN dbo.ReviewStatuses RS ON RS.Id=R.StatusId
        LEFT JOIN dbo.Employees Rev ON Rev.Id=R.ReviewerId
        WHERE (@SearchTerm='' OR R.ReviewCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,ReviewCode,AppraisalCycleId,CycleName,EmployeeId,EmployeeName,EmployeeCode,ReviewerId,ReviewerName,StatusId,StatusName,SelfRating,ManagerRating,OverallRating,RatingScaleMax
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetReviewDetail — review header + rating lines as one JSON object.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetReviewDetail','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetReviewDetail;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetReviewDetail;
GO
CREATE PROCEDURE dbo.SP_GetReviewDetail @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT R.Id,R.ReviewCode,R.AppraisalCycleId,C.[Name] AS CycleName,C.RatingScaleMax,
               R.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               R.ReviewerId,Rev.FullName AS ReviewerName,
               R.StatusId,RS.[Name] AS StatusName,R.SelfRating,R.ManagerRating,R.OverallRating,
               R.SelfComments,R.ManagerComments,R.EmployeeAcknowledgement,
               (SELECT RR.Id,RR.ItemTypeId,RR.ItemId,RR.ItemName,RR.Weight,RR.SelfScore,RR.ManagerScore,RR.Comments
                FROM dbo.ReviewRatings RR WHERE RR.ReviewId=R.Id
                ORDER BY RR.ItemTypeId, RR.ItemName FOR JSON PATH) AS Ratings
        FROM dbo.Reviews R
        INNER JOIN dbo.AppraisalCycles C ON C.Id=R.AppraisalCycleId
        INNER JOIN dbo.Employees E ON E.Id=R.EmployeeId
        INNER JOIN dbo.ReviewStatuses RS ON RS.Id=R.StatusId
        LEFT JOIN dbo.Employees Rev ON Rev.Id=R.ReviewerId
        WHERE R.Id=@Id
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) AS result;
END
GO
-- =============================================================================
-- SP_InitiateReviewsForCycle — create a Review per active employee in the cycle,
-- seeding rating lines from each employee's active goals + the competency set.
-- Idempotent: skips employees who already have a review for the cycle.
-- Goal weights are used as-is; competencies share the remaining weight evenly.
-- =============================================================================
IF OBJECT_ID('dbo.SP_InitiateReviewsForCycle','P') IS NOT NULL DROP PROCEDURE dbo.SP_InitiateReviewsForCycle;
GO
DROP PROCEDURE IF EXISTS dbo.SP_InitiateReviewsForCycle;
GO
CREATE PROCEDURE dbo.SP_InitiateReviewsForCycle @CycleId UNIQUEIDENTIFIER, @CreatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.AppraisalCycles WHERE Id=@CycleId)
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Cycle not found.' AS [Message]; RETURN; END

        DECLARE @rvMax INT = (SELECT MAX(TRY_CAST(SUBSTRING(ReviewCode,4,50) AS INT)) FROM dbo.Reviews WHERE ReviewCode LIKE 'RV-%');
        SET @rvMax = ISNULL(@rvMax,0);
        DECLARE @count INT = 0;

        -- employees who have at least one goal in this cycle and no review yet
        DECLARE @Emp UNIQUEIDENTIFIER, @Mgr UNIQUEIDENTIFIER;
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT DISTINCT G.EmployeeId, E.ReportingManagerId
            FROM dbo.Goals G
            INNER JOIN dbo.Employees E ON E.Id=G.EmployeeId AND E.IsActive=1
            WHERE G.AppraisalCycleId=@CycleId AND G.IsActive=1
              AND NOT EXISTS (SELECT 1 FROM dbo.Reviews R WHERE R.AppraisalCycleId=@CycleId AND R.EmployeeId=G.EmployeeId AND R.IsActive=1);
        OPEN cur; FETCH NEXT FROM cur INTO @Emp,@Mgr;
        WHILE @@FETCH_STATUS=0
        BEGIN
            SET @rvMax = @rvMax + 1;
            DECLARE @Rid UNIQUEIDENTIFIER = NEWID();
            INSERT INTO dbo.Reviews (Id,ReviewCode,AppraisalCycleId,EmployeeId,ReviewerId,StatusId,CreatedBy,CreatedDate,IsActive)
            VALUES (@Rid, dbo.fn_GetNextPerformanceCode('RV',@rvMax-1), @CycleId,@Emp,@Mgr,1,@CreatedBy,GETUTCDATE(),1);

            -- goal rating lines (weight from the goal)
            INSERT INTO dbo.ReviewRatings (Id,ReviewId,ItemTypeId,ItemId,ItemName,Weight,CreatedBy,CreatedDate)
            SELECT NEWID(),@Rid,1,G.Id,G.Title,G.Weight,@CreatedBy,GETUTCDATE()
            FROM dbo.Goals G
            WHERE G.AppraisalCycleId=@CycleId AND G.EmployeeId=@Emp AND G.IsActive=1 AND G.StatusId <> 4;

            -- competency rating lines (share the leftover weight evenly)
            DECLARE @goalWeight DECIMAL(6,2) = (SELECT ISNULL(SUM(Weight),0) FROM dbo.ReviewRatings WHERE ReviewId=@Rid AND ItemTypeId=1);
            DECLARE @compCount INT = (SELECT COUNT(1) FROM dbo.Competencies WHERE IsActive=1);
            DECLARE @compWeight DECIMAL(6,2) = CASE WHEN @compCount>0 AND @goalWeight<100 THEN ROUND((100-@goalWeight)/@compCount,2) ELSE 0 END;

            INSERT INTO dbo.ReviewRatings (Id,ReviewId,ItemTypeId,ItemId,ItemName,Weight,CreatedBy,CreatedDate)
            SELECT NEWID(),@Rid,2,Cm.Id,Cm.[Name],@compWeight,@CreatedBy,GETUTCDATE()
            FROM dbo.Competencies Cm WHERE Cm.IsActive=1;

            SET @count = @count + 1;
            FETCH NEXT FROM cur INTO @Emp,@Mgr;
        END
        CLOSE cur; DEALLOCATE cur;

        COMMIT;
        SELECT 1 AS RowsAffected, CONCAT('Initiated ',@count,' review(s).') AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SaveReviewRatings — upsert scores/comments for a review's lines, then
-- recompute SelfRating / ManagerRating / OverallRating as weighted averages.
-- @Ratings is JSON: [{ "Id":"..","SelfScore":4,"ManagerScore":4.5,"Comments":".." }]
-- =============================================================================
IF OBJECT_ID('dbo.SP_SaveReviewRatings','P') IS NOT NULL DROP PROCEDURE dbo.SP_SaveReviewRatings;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SaveReviewRatings;
GO
CREATE PROCEDURE dbo.SP_SaveReviewRatings
    @ReviewId UNIQUEIDENTIFIER, @Ratings NVARCHAR(MAX),
    @SelfComments NVARCHAR(2000)=NULL, @ManagerComments NVARCHAR(2000)=NULL,
    @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.Reviews WHERE Id=@ReviewId)
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Review not found.' AS [Message]; RETURN; END

        ;WITH J AS (
            SELECT * FROM OPENJSON(@Ratings)
            WITH (Id UNIQUEIDENTIFIER '$.Id', SelfScore DECIMAL(4,2) '$.SelfScore',
                  ManagerScore DECIMAL(4,2) '$.ManagerScore', Comments NVARCHAR(1000) '$.Comments')
        )
        UPDATE RR SET RR.SelfScore=J.SelfScore, RR.ManagerScore=J.ManagerScore, RR.Comments=J.Comments,
                      RR.UpdatedBy=@UpdatedBy, RR.UpdatedDate=GETUTCDATE()
        FROM dbo.ReviewRatings RR INNER JOIN J ON J.Id=RR.Id
        WHERE RR.ReviewId=@ReviewId;

        -- weighted roll-up (falls back to simple average if weights sum to 0)
        DECLARE @wsum DECIMAL(9,2) = (SELECT ISNULL(SUM(Weight),0) FROM dbo.ReviewRatings WHERE ReviewId=@ReviewId);
        DECLARE @self DECIMAL(4,2), @mgr DECIMAL(4,2);

        IF @wsum > 0
        BEGIN
            SELECT @self = ROUND(SUM(ISNULL(SelfScore,0)*Weight)/@wsum,2),
                   @mgr  = ROUND(SUM(ISNULL(ManagerScore,0)*Weight)/@wsum,2)
            FROM dbo.ReviewRatings WHERE ReviewId=@ReviewId;
        END
        ELSE
        BEGIN
            SELECT @self = ROUND(AVG(ISNULL(SelfScore,0)),2),
                   @mgr  = ROUND(AVG(ISNULL(ManagerScore,0)),2)
            FROM dbo.ReviewRatings WHERE ReviewId=@ReviewId;
        END

        UPDATE dbo.Reviews
        SET SelfRating=@self, ManagerRating=@mgr, OverallRating=@mgr,
            SelfComments=ISNULL(@SelfComments,SelfComments),
            ManagerComments=ISNULL(@ManagerComments,ManagerComments),
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@ReviewId;

        COMMIT;
        SELECT 1 AS RowsAffected,'Ratings saved.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetReviewStatus — advance the review workflow.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetReviewStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetReviewStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetReviewStatus;
GO
CREATE PROCEDURE dbo.SP_SetReviewStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @Acknowledgement NVARCHAR(1000)=NULL, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Cur INT = (SELECT StatusId FROM dbo.Reviews WHERE Id=@Id);
        IF @Cur IS NULL BEGIN SELECT 0 AS RowsAffected,'Review not found.' AS [Message]; RETURN; END

        UPDATE dbo.Reviews
        SET StatusId=@StatusId,
            SubmittedDate    = CASE WHEN @StatusId=3 THEN GETUTCDATE() ELSE SubmittedDate END,
            CompletedDate    = CASE WHEN @StatusId=4 THEN GETUTCDATE() ELSE CompletedDate END,
            AcknowledgedDate = CASE WHEN @StatusId=5 THEN GETUTCDATE() ELSE AcknowledgedDate END,
            EmployeeAcknowledgement = ISNULL(@Acknowledgement,EmployeeAcknowledgement),
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        SELECT 1 AS RowsAffected,'Review updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetAppraisalCycleStatus — Draft/Active/Closed
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetAppraisalCycleStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetAppraisalCycleStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetAppraisalCycleStatus;
GO
CREATE PROCEDURE dbo.SP_SetAppraisalCycleStatus @Id UNIQUEIDENTIFIER, @StatusId INT, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.AppraisalCycles WHERE Id=@Id)
        BEGIN SELECT 0 AS RowsAffected,'Cycle not found.' AS [Message]; RETURN; END
        UPDATE dbo.AppraisalCycles SET StatusId=@StatusId, UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE() WHERE Id=@Id;
        SELECT 1 AS RowsAffected,'Cycle updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_GetAllCompetenciesPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllCompetenciesPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllCompetenciesPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllCompetenciesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllCompetenciesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.Competencies C
    WHERE (@SearchTerm='' OR C.CompetencyCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT C.Id,C.CompetencyCode,C.[Name],C.[Description],C.SortOrder,C.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='CompetencyCode' AND @SortColumnDirection='ASC'  THEN C.CompetencyCode END ASC,
                 CASE WHEN @SortColumnName='CompetencyCode' AND @SortColumnDirection='DESC' THEN C.CompetencyCode END DESC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='ASC'  THEN C.SortOrder END ASC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='DESC' THEN C.SortOrder END DESC
               ) AS RowNum
        FROM dbo.Competencies C
        WHERE (@SearchTerm='' OR C.CompetencyCode LIKE '%'+@SearchTerm+'%' OR C.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,CompetencyCode,[Name],[Description],SortOrder,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
GO

-- #####################################################################################################
-- SECTION 17: Performance FunctionsTriggers   (was 17_Performance_FunctionsTriggers.sql)
-- #####################################################################################################
-- Performance: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Performance Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups + a starter set of competencies
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.AppraisalCycleStatuses)
INSERT INTO dbo.AppraisalCycleStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),
 (2,'ACTIVE','Active',2,1),
 (3,'CLOSED','Closed',3,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.GoalStatuses)
INSERT INTO dbo.GoalStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),
 (2,'ACTIVE','Active',2,1),
 (3,'COMPLETED','Completed',3,1),
 (4,'CANCELLED','Cancelled',4,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.ReviewStatuses)
INSERT INTO dbo.ReviewStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'NOT_STARTED','Not Started',1,1),
 (2,'SELF_ASSESSMENT','Self Assessment',2,1),
 (3,'MANAGER_REVIEW','Manager Review',3,1),
 (4,'COMPLETED','Completed',4,1),
 (5,'ACKNOWLEDGED','Acknowledged',5,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.RatingItemTypes)
INSERT INTO dbo.RatingItemTypes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'GOAL','Goal',1,1),
 (2,'COMPETENCY','Competency',2,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Competencies)
INSERT INTO dbo.Competencies (Id, CompetencyCode, [Name], [Description], SortOrder, IsActive, CreatedDate) VALUES
 (NEWID(),'COMM','Communication','Clarity, listening, and stakeholder updates',1,1,GETUTCDATE()),
 (NEWID(),'OWN','Ownership','Accountability and follow-through',2,1,GETUTCDATE()),
 (NEWID(),'TEAM','Teamwork','Collaboration and supporting peers',3,1,GETUTCDATE()),
 (NEWID(),'QUAL','Quality','Accuracy and attention to detail',4,1,GETUTCDATE()),
 (NEWID(),'INNO','Innovation','Initiative and process improvement',5,1,GETUTCDATE());
GO
-- =============================================================================
-- fn_GetNextPerformanceCode — generic prefixed sequence (CYC-####, GL-####, RV-####)
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextPerformanceCode','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetNextPerformanceCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextPerformanceCode;
GO
CREATE FUNCTION dbo.fn_GetNextPerformanceCode(@Prefix NVARCHAR(10), @CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN @Prefix + '-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- TRIGGERS — AppraisalCycles / Competencies / Goals / Reviews (audit to *Log)
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertAppraisalCycles','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertAppraisalCycles;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertAppraisalCycles;
GO
CREATE TRIGGER dbo.TRG_InsertAppraisalCycles ON dbo.AppraisalCycles AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AppraisalCyclesLog (Id,AppraisalCycleId,CycleCode,[Name],StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CycleCode,I.[Name],I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateAppraisalCycles','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateAppraisalCycles;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateAppraisalCycles;
GO
CREATE TRIGGER dbo.TRG_UpdateAppraisalCycles ON dbo.AppraisalCycles AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AppraisalCyclesLog (Id,AppraisalCycleId,CycleCode,[Name],StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CycleCode,I.[Name],I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertCompetencies','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertCompetencies;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertCompetencies;
GO
CREATE TRIGGER dbo.TRG_InsertCompetencies ON dbo.Competencies AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CompetenciesLog (Id,CompetencyId,CompetencyCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CompetencyCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateCompetencies','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateCompetencies;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateCompetencies;
GO
CREATE TRIGGER dbo.TRG_UpdateCompetencies ON dbo.Competencies AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CompetenciesLog (Id,CompetencyId,CompetencyCode,[Name],IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.CompetencyCode,I.[Name],I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertGoals','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertGoals;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertGoals;
GO
CREATE TRIGGER dbo.TRG_InsertGoals ON dbo.Goals AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.GoalsLog (Id,GoalId,GoalCode,AppraisalCycleId,EmployeeId,Title,Weight,ProgressPercent,StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.GoalCode,I.AppraisalCycleId,I.EmployeeId,I.Title,I.Weight,I.ProgressPercent,I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateGoals','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateGoals;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateGoals;
GO
CREATE TRIGGER dbo.TRG_UpdateGoals ON dbo.Goals AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.GoalsLog (Id,GoalId,GoalCode,AppraisalCycleId,EmployeeId,Title,Weight,ProgressPercent,StatusId,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.GoalCode,I.AppraisalCycleId,I.EmployeeId,I.Title,I.Weight,I.ProgressPercent,I.StatusId,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertReviews','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertReviews;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertReviews;
GO
CREATE TRIGGER dbo.TRG_InsertReviews ON dbo.Reviews AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ReviewsLog (Id,ReviewId,ReviewCode,AppraisalCycleId,EmployeeId,StatusId,OverallRating,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ReviewCode,I.AppraisalCycleId,I.EmployeeId,I.StatusId,I.OverallRating,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateReviews','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateReviews;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateReviews;
GO
CREATE TRIGGER dbo.TRG_UpdateReviews ON dbo.Reviews AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ReviewsLog (Id,ReviewId,ReviewCode,AppraisalCycleId,EmployeeId,StatusId,OverallRating,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ReviewCode,I.AppraisalCycleId,I.EmployeeId,I.StatusId,I.OverallRating,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
GO

-- #####################################################################################################
-- SECTION 18: Separation StoredProcedures   (was 18b_Separation_StoredProcedures.sql)
-- #####################################################################################################
-- Separation: stored procedures

-- =============================================================================
-- HumanResourcesDB — Separation Module : Stored Procedures
-- Workflow SPs are transactional and return RowsAffected / Message.
-- SP_SettleSeparation enqueues EmployeeOffboardedEvent into dbo.IntegrationOutbox.
-- =============================================================================

-- =============================================================================
-- SP_GetAllSeparationsPagingWithSearch
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetAllSeparationsPagingWithSearch','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetAllSeparationsPagingWithSearch;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetAllSeparationsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllSeparationsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1)
    FROM dbo.Separations S INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
    WHERE (@SearchTerm='' OR S.SeparationCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%' OR E.EmployeeCode LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT S.Id,S.SeparationCode,S.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               S.SeparationTypeId,ST.[Name] AS SeparationTypeName,S.StatusId,SS.[Name] AS StatusName,
               S.NoticeDate,S.LastWorkingDate,S.ExitInterviewDone,S.SettlementDate,S.IsActive,
               (SELECT COUNT(1) FROM dbo.SeparationClearances C WHERE C.SeparationId=S.Id) AS ClearanceTotal,
               (SELECT COUNT(1) FROM dbo.SeparationClearances C WHERE C.SeparationId=S.Id AND C.ClearanceStatusId=2) AS ClearanceCleared,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='SeparationCode' AND @SortColumnDirection='ASC'  THEN S.SeparationCode END ASC,
                 CASE WHEN @SortColumnName='SeparationCode' AND @SortColumnDirection='DESC' THEN S.SeparationCode END DESC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='ASC'  THEN S.CreatedDate END ASC,
                 CASE WHEN @SortColumnName=''               AND @SortColumnDirection='DESC' THEN S.CreatedDate END DESC
               ) AS RowNum
        FROM dbo.Separations S
        INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
        INNER JOIN dbo.SeparationTypes ST ON ST.Id=S.SeparationTypeId
        INNER JOIN dbo.SeparationStatuses SS ON SS.Id=S.StatusId
        WHERE (@SearchTerm='' OR S.SeparationCode LIKE '%'+@SearchTerm+'%' OR E.FullName LIKE '%'+@SearchTerm+'%' OR E.EmployeeCode LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,SeparationCode,EmployeeId,EmployeeName,EmployeeCode,SeparationTypeId,SeparationTypeName,StatusId,StatusName,NoticeDate,LastWorkingDate,ExitInterviewDone,SettlementDate,ClearanceTotal,ClearanceCleared,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO
-- =============================================================================
-- SP_GetSeparationDetail — header + clearance lines + exit interview as one JSON.
-- =============================================================================
IF OBJECT_ID('dbo.SP_GetSeparationDetail','P') IS NOT NULL DROP PROCEDURE dbo.SP_GetSeparationDetail;
GO
DROP PROCEDURE IF EXISTS dbo.SP_GetSeparationDetail;
GO
CREATE PROCEDURE dbo.SP_GetSeparationDetail @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT (
        SELECT S.Id,S.SeparationCode,S.EmployeeId,E.FullName AS EmployeeName,E.EmployeeCode,
               E.DepartmentId,E.DesignationId,
               S.SeparationTypeId,ST.[Name] AS SeparationTypeName,S.StatusId,SS.[Name] AS StatusName,
               S.Reason,S.NoticeDate,S.RequestedLastWorkingDate,S.LastWorkingDate,S.NoticePeriodDays,
               S.IsNoticeWaived,S.ApprovedBy,S.ApprovedDate,S.ExitInterviewDone,S.SettlementDate,
               S.SettlementRemarks,S.IsRehireEligible,
               (SELECT C.Id,C.DepartmentId,CD.[Name] AS DepartmentName,C.ClearanceStatusId,CS.[Name] AS ClearanceStatusName,
                       C.Remarks,C.DuesAmount,C.ClearedBy,C.ClearedDate
                FROM dbo.SeparationClearances C
                INNER JOIN dbo.ClearanceDepartments CD ON CD.Id=C.DepartmentId
                INNER JOIN dbo.ClearanceStatuses CS ON CS.Id=C.ClearanceStatusId
                WHERE C.SeparationId=S.Id ORDER BY CD.SortOrder FOR JSON PATH) AS Clearances,
               (SELECT TOP 1 X.Id,X.InterviewDate,X.InterviewerId,X.PrimaryReason,X.OverallExperienceRating,
                       X.WouldRecommend,X.Feedback,X.Suggestions
                FROM dbo.ExitInterviews X WHERE X.SeparationId=S.Id AND X.IsActive=1
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS ExitInterview
        FROM dbo.Separations S
        INNER JOIN dbo.Employees E ON E.Id=S.EmployeeId
        INNER JOIN dbo.SeparationTypes ST ON ST.Id=S.SeparationTypeId
        INNER JOIN dbo.SeparationStatuses SS ON SS.Id=S.StatusId
        WHERE S.Id=@Id
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) AS result;
END
GO
-- =============================================================================
-- SP_ApproveSeparation — approve a Pending request, set the approved LWD,
-- seed one clearance line per active ClearanceDepartment, move to InClearance.
-- Idempotent on the clearance seed (won't duplicate a dept line).
-- =============================================================================
IF OBJECT_ID('dbo.SP_ApproveSeparation','P') IS NOT NULL DROP PROCEDURE dbo.SP_ApproveSeparation;
GO
DROP PROCEDURE IF EXISTS dbo.SP_ApproveSeparation;
GO
CREATE PROCEDURE dbo.SP_ApproveSeparation
    @Id UNIQUEIDENTIFIER, @LastWorkingDate DATETIME=NULL, @ApprovedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @Status INT = (SELECT StatusId FROM dbo.Separations WHERE Id=@Id);
        IF @Status IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Separation not found.' AS [Message]; RETURN; END
        IF @Status NOT IN (1,2) BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Only a Draft or Pending separation can be approved.' AS [Message]; RETURN; END

        UPDATE dbo.Separations
        SET StatusId=4,                                    -- InClearance
            LastWorkingDate = ISNULL(@LastWorkingDate, LastWorkingDate),
            ApprovedBy = TRY_CAST(@ApprovedBy AS UNIQUEIDENTIFIER), ApprovedDate=GETUTCDATE(),
            UpdatedBy=@ApprovedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        -- seed clearance lines for any active department that doesn't have one yet
        INSERT INTO dbo.SeparationClearances (Id,SeparationId,DepartmentId,ClearanceStatusId,CreatedBy,CreatedDate)
        SELECT NEWID(),@Id,CD.Id,1,@ApprovedBy,GETUTCDATE()
        FROM dbo.ClearanceDepartments CD
        WHERE CD.IsActive=1
          AND NOT EXISTS (SELECT 1 FROM dbo.SeparationClearances C WHERE C.SeparationId=@Id AND C.DepartmentId=CD.Id);

        COMMIT;
        SELECT 1 AS RowsAffected,'Separation approved; clearance checklist created.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetClearanceStatus — mark a clearance line Cleared/Blocked/Pending.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetClearanceStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetClearanceStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetClearanceStatus;
GO
CREATE PROCEDURE dbo.SP_SetClearanceStatus
    @Id UNIQUEIDENTIFIER, @ClearanceStatusId INT, @Remarks NVARCHAR(500)=NULL,
    @DuesAmount DECIMAL(18,2)=NULL, @ClearedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.SeparationClearances WHERE Id=@Id)
        BEGIN SELECT 0 AS RowsAffected,'Clearance line not found.' AS [Message]; RETURN; END

        UPDATE dbo.SeparationClearances
        SET ClearanceStatusId=@ClearanceStatusId,
            Remarks=ISNULL(@Remarks,Remarks), DuesAmount=@DuesAmount,
            ClearedBy = CASE WHEN @ClearanceStatusId=2 THEN TRY_CAST(@ClearedBy AS UNIQUEIDENTIFIER) ELSE ClearedBy END,
            ClearedDate = CASE WHEN @ClearanceStatusId=2 THEN GETUTCDATE() ELSE ClearedDate END,
            UpdatedBy=@ClearedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        SELECT 1 AS RowsAffected,'Clearance updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SettleSeparation — final settlement. Guards: all clearance lines Cleared,
-- no Blocked line. Stamps SettlementDate, moves to Settled, and enqueues
-- EmployeeOffboardedEvent into dbo.IntegrationOutbox (so Security can deactivate
-- the AppUser). One transaction; the OutboxPublisher relays it to RabbitMQ.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SettleSeparation','P') IS NOT NULL DROP PROCEDURE dbo.SP_SettleSeparation;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SettleSeparation;
GO
CREATE PROCEDURE dbo.SP_SettleSeparation
    @Id UNIQUEIDENTIFIER, @SettlementRemarks NVARCHAR(1000)=NULL,
    @IsRehireEligible BIT=NULL, @SettledBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @Status INT, @EmpId UNIQUEIDENTIFIER, @SepCode NVARCHAR(30), @TypeId INT, @LWD DATETIME;
        SELECT @Status=StatusId, @EmpId=EmployeeId, @SepCode=SeparationCode, @TypeId=SeparationTypeId, @LWD=LastWorkingDate
        FROM dbo.Separations WHERE Id=@Id;

        IF @Status IS NULL BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Separation not found.' AS [Message]; RETURN; END
        IF @Status <> 4 BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'Separation must be In Clearance to settle.' AS [Message]; RETURN; END

        IF EXISTS (SELECT 1 FROM dbo.SeparationClearances WHERE SeparationId=@Id AND ClearanceStatusId=3)
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'A clearance line is Blocked — resolve it before settling.' AS [Message]; RETURN; END
        IF EXISTS (SELECT 1 FROM dbo.SeparationClearances WHERE SeparationId=@Id AND ClearanceStatusId <> 2)
        BEGIN ROLLBACK; SELECT 0 AS RowsAffected,'All clearance lines must be Cleared before settling.' AS [Message]; RETURN; END

        UPDATE dbo.Separations
        SET StatusId=5,                                    -- Settled
            SettlementDate=GETUTCDATE(), SettlementRemarks=@SettlementRemarks,
            IsRehireEligible=@IsRehireEligible, UpdatedBy=@SettledBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        -- close the employee record: separation type -> employment status
        -- (Resignation/End of contract -> Resigned 4, Termination -> Terminated 5, Retirement -> Retired 6)
        DECLARE @PositionId UNIQUEIDENTIFIER = (SELECT PositionId FROM dbo.Employees WHERE Id=@EmpId);
        UPDATE dbo.Employees
        SET EmploymentStatus = CASE @TypeId WHEN 2 THEN 5 WHEN 3 THEN 6 ELSE 4 END,
            DateOfLeaving = ISNULL(@LWD, GETUTCDATE()),
            UpdatedBy = @SettledBy, UpdatedDate = GETUTCDATE()
        WHERE Id=@EmpId;

        -- release the seat the employee occupied (same rules as SP_SetPositionFill)
        IF @PositionId IS NOT NULL
            UPDATE dbo.Positions
            SET FilledHeadcount = CASE WHEN FilledHeadcount > 0 THEN FilledHeadcount - 1 ELSE 0 END,
                StatusId = CASE WHEN StatusId IN (4,5) THEN StatusId ELSE 2 END,
                UpdatedBy = @SettledBy, UpdatedDate = GETUTCDATE()
            WHERE Id=@PositionId;

        -- build the integration event payload
        DECLARE @AppUserId UNIQUEIDENTIFIER = (SELECT AppUserId FROM dbo.Employees WHERE Id=@EmpId);
        DECLARE @Email NVARCHAR(256) = (SELECT OfficialEmail FROM dbo.Employees WHERE Id=@EmpId);
        DECLARE @FullName NVARCHAR(300) = (SELECT FullName FROM dbo.Employees WHERE Id=@EmpId);
        DECLARE @EmpCode NVARCHAR(50) = (SELECT EmployeeCode FROM dbo.Employees WHERE Id=@EmpId);

        DECLARE @Payload NVARCHAR(MAX) = (
            SELECT @EmpId AS EmployeeId, @AppUserId AS AppUserId, @EmpCode AS EmployeeCode,
                   @FullName AS FullName, @Email AS OfficialEmail, @Id AS SeparationId,
                   @SepCode AS SeparationCode, @TypeId AS SeparationTypeId, @LWD AS LastWorkingDate
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        INSERT INTO dbo.IntegrationOutbox
            (Id, EventType, RoutingKey, AggregateType, AggregateId, Payload, OccurredOn)
        VALUES
            (NEWID(), 'EmployeeOffboardedEvent', 'hr.employee.offboarded', 'Employee', @EmpId, @Payload, GETUTCDATE());

        COMMIT;
        SELECT 1 AS RowsAffected,'Separation settled; offboarded event queued.' AS [Message];
    END TRY
    BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
-- =============================================================================
-- SP_SetSeparationStatus — generic transitions (submit, cancel, complete).
-- Cancel is blocked once Settled. Complete requires Settled.
-- =============================================================================
IF OBJECT_ID('dbo.SP_SetSeparationStatus','P') IS NOT NULL DROP PROCEDURE dbo.SP_SetSeparationStatus;
GO
DROP PROCEDURE IF EXISTS dbo.SP_SetSeparationStatus;
GO
CREATE PROCEDURE dbo.SP_SetSeparationStatus
    @Id UNIQUEIDENTIFIER, @StatusId INT, @UpdatedBy NVARCHAR(450)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Cur INT = (SELECT StatusId FROM dbo.Separations WHERE Id=@Id);
        IF @Cur IS NULL BEGIN SELECT 0 AS RowsAffected,'Separation not found.' AS [Message]; RETURN; END

        IF @StatusId=7 AND @Cur>=5
        BEGIN SELECT 0 AS RowsAffected,'A settled separation can no longer be cancelled.' AS [Message]; RETURN; END
        IF @StatusId=6 AND @Cur<>5
        BEGIN SELECT 0 AS RowsAffected,'Only a settled separation can be completed.' AS [Message]; RETURN; END

        UPDATE dbo.Separations SET StatusId=@StatusId,
            IsActive = CASE WHEN @StatusId=7 THEN 0 ELSE IsActive END,
            UpdatedBy=@UpdatedBy, UpdatedDate=GETUTCDATE()
        WHERE Id=@Id;

        SELECT 1 AS RowsAffected,'Separation updated.' AS [Message];
    END TRY
    BEGIN CATCH SELECT 0 AS RowsAffected, ERROR_MESSAGE() AS [Message]; END CATCH
END
GO
GO

-- #####################################################################################################
-- SECTION 19: Separation FunctionsTriggers   (was 18_Separation_FunctionsTriggers.sql)
-- #####################################################################################################
-- Separation: functions + audit triggers (lookup seeds are guarded and normally no-ops; EF HasData seeds them)

-- =============================================================================
-- HumanResourcesDB — Separation Module : Seed + Functions + Triggers
-- =============================================================================

-- =============================================================================
-- SEED — lookups
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.SeparationTypes)
INSERT INTO dbo.SeparationTypes (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'RESIGNATION','Resignation',1,1),
 (2,'TERMINATION','Termination',2,1),
 (3,'RETIREMENT','Retirement',3,1),
 (4,'END_OF_CONTRACT','End of Contract',4,1),
 (5,'DEATH','Death',5,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.SeparationStatuses)
INSERT INTO dbo.SeparationStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'DRAFT','Draft',1,1),
 (2,'PENDING','Pending Approval',2,1),
 (3,'APPROVED','Approved',3,1),
 (4,'IN_CLEARANCE','In Clearance',4,1),
 (5,'SETTLED','Settled',5,1),
 (6,'COMPLETED','Completed',6,1),
 (7,'CANCELLED','Cancelled',7,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.ClearanceStatuses)
INSERT INTO dbo.ClearanceStatuses (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'PENDING','Pending',1,1),
 (2,'CLEARED','Cleared',2,1),
 (3,'BLOCKED','Blocked',3,1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.ClearanceDepartments)
INSERT INTO dbo.ClearanceDepartments (Id, Code, [Name], SortOrder, IsActive) VALUES
 (1,'IT','IT / Systems',1,1),
 (2,'FINANCE','Finance / Accounts',2,1),
 (3,'HR','Human Resources',3,1),
 (4,'ADMIN','Admin / Facilities',4,1),
 (5,'MANAGER','Reporting Manager',5,1);
GO
-- =============================================================================
-- fn_GetNextSeparationCode — SEP-#### sequence
-- =============================================================================
IF OBJECT_ID('dbo.fn_GetNextSeparationCode','FN') IS NOT NULL DROP FUNCTION dbo.fn_GetNextSeparationCode;
GO
DROP FUNCTION IF EXISTS dbo.fn_GetNextSeparationCode;
GO
CREATE FUNCTION dbo.fn_GetNextSeparationCode(@CurrentMax INT)
RETURNS NVARCHAR(30)
AS
BEGIN
    RETURN 'SEP-' + RIGHT('0000' + CAST(ISNULL(@CurrentMax,0) + 1 AS NVARCHAR(10)), 4);
END
GO
-- =============================================================================
-- TRIGGERS — Separations / SeparationClearances / ExitInterviews
-- =============================================================================
IF OBJECT_ID('dbo.TRG_InsertSeparations','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertSeparations;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertSeparations;
GO
CREATE TRIGGER dbo.TRG_InsertSeparations ON dbo.Separations AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SeparationsLog (Id,SeparationId,SeparationCode,EmployeeId,SeparationTypeId,StatusId,LastWorkingDate,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationCode,I.EmployeeId,I.SeparationTypeId,I.StatusId,I.LastWorkingDate,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateSeparations','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateSeparations;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateSeparations;
GO
CREATE TRIGGER dbo.TRG_UpdateSeparations ON dbo.Separations AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SeparationsLog (Id,SeparationId,SeparationCode,EmployeeId,SeparationTypeId,StatusId,LastWorkingDate,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationCode,I.EmployeeId,I.SeparationTypeId,I.StatusId,I.LastWorkingDate,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertSeparationClearances','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertSeparationClearances;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertSeparationClearances;
GO
CREATE TRIGGER dbo.TRG_InsertSeparationClearances ON dbo.SeparationClearances AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SeparationClearancesLog (Id,SeparationClearanceId,SeparationId,DepartmentId,ClearanceStatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationId,I.DepartmentId,I.ClearanceStatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateSeparationClearances','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateSeparationClearances;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateSeparationClearances;
GO
CREATE TRIGGER dbo.TRG_UpdateSeparationClearances ON dbo.SeparationClearances AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.SeparationClearancesLog (Id,SeparationClearanceId,SeparationId,DepartmentId,ClearanceStatusId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationId,I.DepartmentId,I.ClearanceStatusId,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_InsertExitInterviews','TR') IS NOT NULL DROP TRIGGER dbo.TRG_InsertExitInterviews;
GO
DROP TRIGGER IF EXISTS dbo.TRG_InsertExitInterviews;
GO
CREATE TRIGGER dbo.TRG_InsertExitInterviews ON dbo.ExitInterviews AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExitInterviewsLog (Id,ExitInterviewId,SeparationId,OverallExperienceRating,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationId,I.OverallExperienceRating,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
IF OBJECT_ID('dbo.TRG_UpdateExitInterviews','TR') IS NOT NULL DROP TRIGGER dbo.TRG_UpdateExitInterviews;
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateExitInterviews;
GO
CREATE TRIGGER dbo.TRG_UpdateExitInterviews ON dbo.ExitInterviews AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExitInterviewsLog (Id,ExitInterviewId,SeparationId,OverallExperienceRating,IsActive,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.SeparationId,I.OverallExperienceRating,I.IsActive,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
GO

-- #####################################################################################################
-- SECTION 20: Biometric Attendance   (mobile app Identification API + door terminals)
-- #####################################################################################################
-- Tables (EF): AttendanceDevices(+Log), AttendanceZones(+Log), EmployeeBiometricRegistrations(+Log),
-- AttendancePunches (append-only raw punches), lookups BiometricTypes / BiometricRegistrationStatuses /
-- PunchSources / PunchStatuses, sequence SEQ_BiometricRegistrationNo.
-- The daily dbo.Attendances row (SourceType 2) is calculated by AttendanceProcessor in the service:
-- earliest accepted punch of the work day = check-in, latest = check-out.

-- =============================================================================
-- VW_AttendancePunchDetails — punches with employee, source, status and device names
-- =============================================================================
DROP VIEW IF EXISTS dbo.VW_AttendancePunchDetails;
GO
CREATE VIEW dbo.VW_AttendancePunchDetails
AS
SELECT P.Id, P.EmployeeId, E.EmployeeCode, E.FullName AS EmployeeName, E.DepartmentId,
       P.PunchTimeUtc, P.PunchLocalTime, P.AttendanceDate, P.PunchTypeId,
       CASE P.PunchTypeId WHEN 1 THEN 'Check-In' WHEN 2 THEN 'Check-Out' ELSE 'Auto' END AS PunchTypeName,
       P.SourceId, PS.[Name] AS SourceName, P.StatusId, PT.[Name] AS StatusName, P.RejectReason,
       P.BiometricTypeId, BT.[Name] AS BiometricTypeName, P.BiometricRegistrationId,
       P.AttendanceDeviceId, D.[Name] AS DeviceName, P.DeviceUserPin,
       P.ClientIp, P.IsWithinOffice, P.LocationCheck, P.Latitude, P.Longitude, P.WifiBssid, P.CreatedDate
FROM dbo.AttendancePunches P
LEFT JOIN dbo.Employees E          ON E.Id = P.EmployeeId
LEFT JOIN dbo.PunchSources PS      ON PS.Id = P.SourceId
LEFT JOIN dbo.PunchStatuses PT     ON PT.Id = P.StatusId
LEFT JOIN dbo.BiometricTypes BT    ON BT.Id = P.BiometricTypeId
LEFT JOIN dbo.AttendanceDevices D  ON D.Id = P.AttendanceDeviceId;
GO

-- =============================================================================
-- VW_DailyAttendanceFromPunches — check-in = earliest, check-out = latest accepted punch per
-- employee and work day. For reports and to cross-check dbo.Attendances (payroll officers).
-- =============================================================================
DROP VIEW IF EXISTS dbo.VW_DailyAttendanceFromPunches;
GO
CREATE VIEW dbo.VW_DailyAttendanceFromPunches
AS
SELECT P.EmployeeId, E.EmployeeCode, E.FullName AS EmployeeName, E.DepartmentId, P.AttendanceDate,
       MIN(P.PunchLocalTime) AS FirstPunch,
       MAX(P.PunchLocalTime) AS LastPunch,
       COUNT(1) AS AcceptedPunches,
       CAST(DATEDIFF(MINUTE, MIN(P.PunchLocalTime), MAX(P.PunchLocalTime)) / 60.0 AS DECIMAL(5, 2)) AS SpanHours
FROM dbo.AttendancePunches P
JOIN dbo.Employees E ON E.Id = P.EmployeeId
WHERE P.StatusId = 1
GROUP BY P.EmployeeId, E.EmployeeCode, E.FullName, E.DepartmentId, P.AttendanceDate;
GO

-- =============================================================================
-- SP_GetAllAttendanceDevicesPagingWithSearch
-- =============================================================================
DROP PROCEDURE IF EXISTS dbo.SP_GetAllAttendanceDevicesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllAttendanceDevicesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.AttendanceDevices D
    WHERE (@SearchTerm='' OR D.DeviceCode LIKE '%'+@SearchTerm+'%' OR D.[Name] LIKE '%'+@SearchTerm+'%'
           OR D.SerialNumber LIKE '%'+@SearchTerm+'%' OR D.[Location] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT D.Id,D.DeviceCode,D.[Name],D.Vendor,D.Model,D.SerialNumber,D.Protocol,
               CASE D.Protocol WHEN 1 THEN 'ZKTeco ADMS (Push)' WHEN 2 THEN 'Generic JSON API' ELSE '' END AS ProtocolName,
               D.BranchId,B.[Name] AS BranchName,D.[Location],D.IpAddress,
               CAST(CASE WHEN D.ApiKeyHash IS NULL THEN 0 ELSE 1 END AS BIT) AS HasApiKey,
               D.LastSeenDate,D.LastPunchDate,D.Remarks,D.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='Name' AND @SortColumnDirection='ASC'  THEN D.[Name] END ASC,
                 CASE WHEN @SortColumnName='Name' AND @SortColumnDirection='DESC' THEN D.[Name] END DESC,
                 CASE WHEN @SortColumnName IN ('','DeviceCode') AND @SortColumnDirection='ASC'  THEN D.DeviceCode END ASC,
                 CASE WHEN @SortColumnName IN ('','DeviceCode') AND @SortColumnDirection='DESC' THEN D.DeviceCode END DESC
               ) AS RowNum
        FROM dbo.AttendanceDevices D
        LEFT JOIN dbo.Branches B ON B.Id = D.BranchId
        WHERE (@SearchTerm='' OR D.DeviceCode LIKE '%'+@SearchTerm+'%' OR D.[Name] LIKE '%'+@SearchTerm+'%'
               OR D.SerialNumber LIKE '%'+@SearchTerm+'%' OR D.[Location] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,DeviceCode,[Name],Vendor,Model,SerialNumber,Protocol,ProtocolName,BranchId,BranchName,
                          [Location],IpAddress,HasApiKey,LastSeenDate,LastPunchDate,Remarks,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
                   ORDER BY RowNum FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO

-- =============================================================================
-- SP_GetAllAttendanceZonesPagingWithSearch
-- =============================================================================
DROP PROCEDURE IF EXISTS dbo.SP_GetAllAttendanceZonesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllAttendanceZonesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.AttendanceZones Z
    WHERE (@SearchTerm='' OR Z.ZoneCode LIKE '%'+@SearchTerm+'%' OR Z.[Name] LIKE '%'+@SearchTerm+'%');
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT Z.Id,Z.ZoneCode,Z.[Name],Z.BranchId,B.[Name] AS BranchName,Z.AllowedIpRanges,Z.AllowedWifiBssids,
               Z.Latitude,Z.Longitude,Z.RadiusMeters,Z.Remarks,Z.IsActive,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='Name' AND @SortColumnDirection='ASC'  THEN Z.[Name] END ASC,
                 CASE WHEN @SortColumnName='Name' AND @SortColumnDirection='DESC' THEN Z.[Name] END DESC,
                 CASE WHEN @SortColumnName IN ('','ZoneCode') AND @SortColumnDirection='ASC'  THEN Z.ZoneCode END ASC,
                 CASE WHEN @SortColumnName IN ('','ZoneCode') AND @SortColumnDirection='DESC' THEN Z.ZoneCode END DESC
               ) AS RowNum
        FROM dbo.AttendanceZones Z
        LEFT JOIN dbo.Branches B ON B.Id = Z.BranchId
        WHERE (@SearchTerm='' OR Z.ZoneCode LIKE '%'+@SearchTerm+'%' OR Z.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Items=(SELECT Id,ZoneCode,[Name],BranchId,BranchName,AllowedIpRanges,AllowedWifiBssids,
                          Latitude,Longitude,RadiusMeters,Remarks,IsActive
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
                   ORDER BY RowNum FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO

-- =============================================================================
-- SP_GetAllBiometricRegistrationsPagingWithSearch
-- =============================================================================
DROP PROCEDURE IF EXISTS dbo.SP_GetAllBiometricRegistrationsPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllBiometricRegistrationsPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='ASC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    ;WITH Src AS (
        SELECT R.Id,R.RegistrationNo,R.EmployeeId,E.EmployeeCode,E.FullName AS EmployeeName,
               R.BiometricTypeId,BT.[Name] AS BiometricTypeName,R.MethodId,
               CASE R.MethodId WHEN 1 THEN 'Mobile App' WHEN 2 THEN 'Door Terminal' ELSE '' END AS MethodName,
               R.StatusId,RS.[Name] AS StatusName,R.DevicePlatform,R.DeviceModel,R.DeviceUserPin,D.[Name] AS DeviceName,
               CAST(CASE WHEN R.TemplateFileName IS NULL THEN 0 ELSE 1 END AS BIT) AS HasTemplate,
               R.ConsentGiven,R.VerifiedDate,R.LastUsedDate,R.RevokedDate,R.RevokedReason,R.CreatedDate
        FROM dbo.EmployeeBiometricRegistrations R
        JOIN dbo.Employees E ON E.Id = R.EmployeeId
        LEFT JOIN dbo.BiometricTypes BT ON BT.Id = R.BiometricTypeId
        LEFT JOIN dbo.BiometricRegistrationStatuses RS ON RS.Id = R.StatusId
        LEFT JOIN dbo.AttendanceDevices D ON D.Id = R.AttendanceDeviceId
        WHERE (@SearchTerm='' OR R.RegistrationNo LIKE '%'+@SearchTerm+'%' OR E.EmployeeCode LIKE '%'+@SearchTerm+'%'
               OR E.FullName LIKE '%'+@SearchTerm+'%' OR R.DeviceUserPin LIKE '%'+@SearchTerm+'%' OR RS.[Name] LIKE '%'+@SearchTerm+'%')
    )
    SELECT @Total=COUNT(1) FROM Src;
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Src AS (
        SELECT R.Id,R.RegistrationNo,R.EmployeeId,E.EmployeeCode,E.FullName AS EmployeeName,
               R.BiometricTypeId,BT.[Name] AS BiometricTypeName,R.MethodId,
               CASE R.MethodId WHEN 1 THEN 'Mobile App' WHEN 2 THEN 'Door Terminal' ELSE '' END AS MethodName,
               R.StatusId,RS.[Name] AS StatusName,R.DevicePlatform,R.DeviceModel,R.DeviceUserPin,D.[Name] AS DeviceName,
               CAST(CASE WHEN R.TemplateFileName IS NULL THEN 0 ELSE 1 END AS BIT) AS HasTemplate,
               R.ConsentGiven,R.VerifiedDate,R.LastUsedDate,R.RevokedDate,R.RevokedReason,R.CreatedDate
        FROM dbo.EmployeeBiometricRegistrations R
        JOIN dbo.Employees E ON E.Id = R.EmployeeId
        LEFT JOIN dbo.BiometricTypes BT ON BT.Id = R.BiometricTypeId
        LEFT JOIN dbo.BiometricRegistrationStatuses RS ON RS.Id = R.StatusId
        LEFT JOIN dbo.AttendanceDevices D ON D.Id = R.AttendanceDeviceId
        WHERE (@SearchTerm='' OR R.RegistrationNo LIKE '%'+@SearchTerm+'%' OR E.EmployeeCode LIKE '%'+@SearchTerm+'%'
               OR E.FullName LIKE '%'+@SearchTerm+'%' OR R.DeviceUserPin LIKE '%'+@SearchTerm+'%' OR RS.[Name] LIKE '%'+@SearchTerm+'%')
    ), Sorted AS (
        SELECT *, ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='EmployeeName' AND @SortColumnDirection='ASC'  THEN EmployeeName END ASC,
                 CASE WHEN @SortColumnName='EmployeeName' AND @SortColumnDirection='DESC' THEN EmployeeName END DESC,
                 CASE WHEN @SortColumnName='RegistrationNo' AND @SortColumnDirection='ASC'  THEN RegistrationNo END ASC,
                 CASE WHEN @SortColumnName='RegistrationNo' AND @SortColumnDirection='DESC' THEN RegistrationNo END DESC,
                 CreatedDate DESC
               ) AS RowNum
        FROM Src
    )
    SELECT @Items=(SELECT Id,RegistrationNo,EmployeeId,EmployeeCode,EmployeeName,BiometricTypeId,BiometricTypeName,
                          MethodId,MethodName,StatusId,StatusName,DevicePlatform,DeviceModel,DeviceUserPin,DeviceName,
                          HasTemplate,ConsentGiven,VerifiedDate,LastUsedDate,RevokedDate,RevokedReason,CreatedDate
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
                   ORDER BY RowNum FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO

-- =============================================================================
-- SP_GetAllAttendancePunchesPagingWithSearch — newest first; search also matches a date (yyyy-MM-dd)
-- =============================================================================
DROP PROCEDURE IF EXISTS dbo.SP_GetAllAttendancePunchesPagingWithSearch;
GO
CREATE PROCEDURE dbo.SP_GetAllAttendancePunchesPagingWithSearch
    @SearchTerm NVARCHAR(200)='', @SortColumnName NVARCHAR(50)='', @SortColumnDirection NVARCHAR(5)='DESC',
    @PageNumber INT=1, @PageSize INT=10
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #T ([RowCount] INT,[CurrentPage] INT,[PageSize] INT,[PageCount] INT,[Items] NVARCHAR(MAX));
    DECLARE @Total INT,@Pages INT,@Items NVARCHAR(MAX),@Result NVARCHAR(MAX);
    SELECT @Total=COUNT(1) FROM dbo.VW_AttendancePunchDetails V
    WHERE (@SearchTerm='' OR V.EmployeeCode LIKE '%'+@SearchTerm+'%' OR V.EmployeeName LIKE '%'+@SearchTerm+'%'
           OR V.DeviceUserPin LIKE '%'+@SearchTerm+'%' OR V.StatusName LIKE '%'+@SearchTerm+'%'
           OR V.SourceName LIKE '%'+@SearchTerm+'%' OR CONVERT(VARCHAR(10), V.AttendanceDate, 23) = @SearchTerm);
    SET @Pages=CEILING(@Total*1.0/@PageSize);
    ;WITH Sorted AS (
        SELECT V.Id,V.EmployeeId,V.EmployeeCode,V.EmployeeName,V.PunchLocalTime,V.AttendanceDate,V.PunchTypeId,
               V.SourceId,V.SourceName,V.StatusId,V.StatusName,V.RejectReason,V.BiometricTypeName,V.DeviceName,
               V.DeviceUserPin,V.ClientIp,V.IsWithinOffice,V.LocationCheck,
               ROW_NUMBER() OVER (ORDER BY
                 CASE WHEN @SortColumnName='EmployeeName' AND @SortColumnDirection='ASC'  THEN V.EmployeeName END ASC,
                 CASE WHEN @SortColumnName='EmployeeName' AND @SortColumnDirection='DESC' THEN V.EmployeeName END DESC,
                 CASE WHEN @SortColumnName IN ('','PunchLocalTime') AND @SortColumnDirection='ASC'  THEN V.PunchTimeUtc END ASC,
                 CASE WHEN @SortColumnName IN ('','PunchLocalTime') AND @SortColumnDirection='DESC' THEN V.PunchTimeUtc END DESC,
                 V.PunchTimeUtc DESC
               ) AS RowNum
        FROM dbo.VW_AttendancePunchDetails V
        WHERE (@SearchTerm='' OR V.EmployeeCode LIKE '%'+@SearchTerm+'%' OR V.EmployeeName LIKE '%'+@SearchTerm+'%'
               OR V.DeviceUserPin LIKE '%'+@SearchTerm+'%' OR V.StatusName LIKE '%'+@SearchTerm+'%'
               OR V.SourceName LIKE '%'+@SearchTerm+'%' OR CONVERT(VARCHAR(10), V.AttendanceDate, 23) = @SearchTerm)
    )
    SELECT @Items=(SELECT Id,EmployeeId,EmployeeCode,EmployeeName,PunchLocalTime,AttendanceDate,PunchTypeId,SourceId,
                          SourceName,StatusId,StatusName,RejectReason,BiometricTypeName,DeviceName,DeviceUserPin,
                          ClientIp,IsWithinOffice,LocationCheck
                   FROM Sorted WHERE RowNum BETWEEN (@PageNumber-1)*@PageSize+1 AND @PageNumber*@PageSize
                   ORDER BY RowNum FOR JSON AUTO);
    INSERT INTO #T VALUES (@Total,@PageNumber,@PageSize,@Pages,@Items);
    SET @Result=(SELECT * FROM #T FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
    DROP TABLE #T;
    SELECT @Result AS result;
END
GO

-- =============================================================================
-- Log triggers (same shape as the other *Log tables; PerformedUser = employee who acted)
-- =============================================================================
-- ---- AttendanceDevices ----
DROP TRIGGER IF EXISTS dbo.TRG_InsertAttendanceDevices;
GO
CREATE TRIGGER dbo.TRG_InsertAttendanceDevices ON dbo.AttendanceDevices AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AttendanceDevicesLog (Id,AttendanceDeviceId,DeviceCode,[Name],SerialNumber,Protocol,IpAddress,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.DeviceCode,I.[Name],I.SerialNumber,I.Protocol,I.IpAddress,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateAttendanceDevices;
GO
CREATE TRIGGER dbo.TRG_UpdateAttendanceDevices ON dbo.AttendanceDevices AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    -- the terminal's own heartbeat (LastSeenDate / LastPunchDate) is not a change of the device
    IF NOT (UPDATE(DeviceCode) OR UPDATE([Name]) OR UPDATE(Vendor) OR UPDATE(Model) OR UPDATE(SerialNumber) OR UPDATE(Protocol)
            OR UPDATE(BranchId) OR UPDATE([Location]) OR UPDATE(IpAddress) OR UPDATE(ApiKeyHash) OR UPDATE(Remarks) OR UPDATE(IsActive))
        RETURN;
    INSERT INTO dbo.AttendanceDevicesLog (Id,AttendanceDeviceId,DeviceCode,[Name],SerialNumber,Protocol,IpAddress,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.DeviceCode,I.[Name],I.SerialNumber,I.Protocol,I.IpAddress,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- AttendanceZones ----
DROP TRIGGER IF EXISTS dbo.TRG_InsertAttendanceZones;
GO
CREATE TRIGGER dbo.TRG_InsertAttendanceZones ON dbo.AttendanceZones AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AttendanceZonesLog (Id,AttendanceZoneId,ZoneCode,[Name],AllowedIpRanges,AllowedWifiBssids,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ZoneCode,I.[Name],I.AllowedIpRanges,I.AllowedWifiBssids,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateAttendanceZones;
GO
CREATE TRIGGER dbo.TRG_UpdateAttendanceZones ON dbo.AttendanceZones AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AttendanceZonesLog (Id,AttendanceZoneId,ZoneCode,[Name],AllowedIpRanges,AllowedWifiBssids,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.ZoneCode,I.[Name],I.AllowedIpRanges,I.AllowedWifiBssids,I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),'Updated'
    FROM inserted I;
END
GO
-- ---- EmployeeBiometricRegistrations ----
DROP TRIGGER IF EXISTS dbo.TRG_InsertEmployeeBiometricRegistrations;
GO
CREATE TRIGGER dbo.TRG_InsertEmployeeBiometricRegistrations ON dbo.EmployeeBiometricRegistrations AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.EmployeeBiometricRegistrationsLog (Id,RegistrationId,RegistrationNo,EmployeeId,BiometricTypeId,MethodId,StatusId,DeviceInstallationId,AttendanceDeviceId,DeviceUserPin,RevokedReason,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RegistrationNo,I.EmployeeId,I.BiometricTypeId,I.MethodId,I.StatusId,I.DeviceInstallationId,I.AttendanceDeviceId,I.DeviceUserPin,I.RevokedReason,
           I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.CreatedBy AS UNIQUEIDENTIFIER)),'Created'
    FROM inserted I;
END
GO
DROP TRIGGER IF EXISTS dbo.TRG_UpdateEmployeeBiometricRegistrations;
GO
CREATE TRIGGER dbo.TRG_UpdateEmployeeBiometricRegistrations ON dbo.EmployeeBiometricRegistrations AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    -- each check-in only stamps LastUsedDate: not an audit event
    IF NOT (UPDATE(StatusId) OR UPDATE(EmployeeId) OR UPDATE(BiometricTypeId) OR UPDATE(PublicKey) OR UPDATE(DeviceUserPin)
            OR UPDATE(AttendanceDeviceId) OR UPDATE(TemplateFileName) OR UPDATE(RevokedReason) OR UPDATE(IsActive))
        RETURN;
    INSERT INTO dbo.EmployeeBiometricRegistrationsLog (Id,RegistrationId,RegistrationNo,EmployeeId,BiometricTypeId,MethodId,StatusId,DeviceInstallationId,AttendanceDeviceId,DeviceUserPin,RevokedReason,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive,PerformedUser,[Action])
    SELECT NEWID(),I.Id,I.RegistrationNo,I.EmployeeId,I.BiometricTypeId,I.MethodId,I.StatusId,I.DeviceInstallationId,I.AttendanceDeviceId,I.DeviceUserPin,I.RevokedReason,
           I.CreatedBy,I.CreatedDate,I.UpdatedBy,I.UpdatedDate,I.IsActive,
           (SELECT TOP 1 FullName FROM dbo.Employees WHERE Id = TRY_CAST(I.UpdatedBy AS UNIQUEIDENTIFIER)),
           CASE WHEN I.StatusId = 3 THEN 'Revoked' WHEN I.StatusId = 2 THEN 'Activated' ELSE 'Updated' END
    FROM inserted I;
END
GO
