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
