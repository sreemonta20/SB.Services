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
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] XML
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items XML, @Result XML;

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
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] XML
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items XML, @Result XML;

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
        [Items]       XML
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items XML, @Result XML;

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
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] XML
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items XML, @Result XML;

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

CREATE PROCEDURE dbo.SP_GetEmployeeInitialData
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #InitTBL
    (
        companies          XML,
        branches           XML,
        departments        XML,
        designations       XML,
        reportingManagers  XML,
        genders            XML,
        maritalStatuses    XML,
        bloodGroups        XML,
        employmentTypes    XML,
        employmentStatuses XML,
        nextEmployeeCode   NVARCHAR(30)
    );

    DECLARE @Company XML, @Branch XML, @Dept XML, @Desg XML, @Mgr XML, @Gender XML,
            @Marital XML, @Blood XML, @EmpType XML, @EmpStatus XML,
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
        [RowCount] INT, [CurrentPage] INT, [PageSize] INT, [PageCount] INT, [Items] XML
    );

    DECLARE @TotalRecords INT, @TotalPages INT, @Items XML, @Result XML;

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
               EmploymentStatus, EmploymentStatusName, IsActive
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
            E.PhotoUrl, E.SignatureUrl, E.IsActive,

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
