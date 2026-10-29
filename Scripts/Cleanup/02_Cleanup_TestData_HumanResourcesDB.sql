/* =============================================================================
   HumanResourcesDB - remove end-to-end TEST data (Org Setup -> Separation)
   -----------------------------------------------------------------------------
   Run AFTER 01_Cleanup_TestData_SecurityDB.sql (that script reads the test
   employees from this database).

   What is "test data" (nothing else is touched):
     * Candidates whose e-mail matches @CandidateEmailPattern, and everything that
       hangs off them: applications, interviews, offers, onboardings (+tasks),
       and the EMPLOYEES created from those offers with all their records
       (attendance, regularization, overtime, rosters, leave, salary structures,
       loans, payslips, goals, reviews, separations, exit interviews, outbox events).
     * Org records named with @TagPrefix: positions "E2E Analyst %", cost centers
       "E2E Sales & Marketing %", job grades "E2E Grade %", holiday calendars
       "E2E Calendar %", leave policies "E2E-%", appraisal cycles "E2E-%", and the
       requisitions / job postings raised against those positions.
     * Leave entitlements the test policies granted to REAL employees (only where no
       non-test policy exists for that leave type + year and nothing was used).
     * Payroll runs made up only of test payslips (mixed runs keep the real payslips
       and get their totals recomputed).
     * The audit (*Log) rows of all of the above, including the "Deleted" rows the
       TRG_Delete* triggers write during this cleanup.

   Safety:
     * @Execute = 0 (default) is a DRY RUN: every delete runs inside a transaction,
       the per-table counts are printed, then everything is rolled back.
     * Seed rows (POS-0001..7, CC-0001..3, GR-0001..6, HC-0001, lookups, leave
       types, pay components, competencies, shifts) and real employees are never
       matched; the script aborts if a test employee looks like a real one.
   ============================================================================= */
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;   -- required for DML on tables with filtered indexes

DECLARE @Execute               BIT           = 0;                       -- 1 = commit
DECLARE @CandidateEmailPattern NVARCHAR(200) = N'aisha.%@example.com';
DECLARE @TagPrefix             NVARCHAR(20)  = N'E2E';

IF DB_NAME() <> N'HumanResourcesDB'
BEGIN RAISERROR(N'Run this script in HumanResourcesDB.', 16, 1); RETURN; END

CREATE TABLE #Report (Step INT IDENTITY, TableName SYSNAME, RowsDeleted INT);
DECLARE @n INT;

-- -----------------------------------------------------------------------------
-- 1. Collect the test key sets
-- -----------------------------------------------------------------------------
SELECT Id INTO #Cand FROM dbo.Candidates WHERE Email LIKE @CandidateEmailPattern;

SELECT DISTINCT e.Id, e.EmployeeCode INTO #Emp
FROM dbo.Employees e
WHERE e.Id IN (SELECT ConvertedEmployeeId FROM dbo.JobOffers WHERE CandidateId IN (SELECT Id FROM #Cand) AND ConvertedEmployeeId IS NOT NULL)
   OR e.Id IN (SELECT EmployeeId FROM dbo.Onboardings WHERE CandidateId IN (SELECT Id FROM #Cand) AND EmployeeId IS NOT NULL);

-- guard: a test employee must carry the candidate's e-mail and must not be anyone's manager/head
IF EXISTS (SELECT 1 FROM dbo.Employees e JOIN #Emp t ON t.Id = e.Id
           WHERE ISNULL(e.PersonalEmail, N'') NOT LIKE @CandidateEmailPattern
             AND ISNULL(e.OfficialEmail, N'') NOT LIKE @CandidateEmailPattern)
   OR EXISTS (SELECT 1 FROM dbo.Employees WHERE ReportingManagerId IN (SELECT Id FROM #Emp) AND Id NOT IN (SELECT Id FROM #Emp))
   OR EXISTS (SELECT 1 FROM dbo.Departments WHERE HeadEmployeeId IN (SELECT Id FROM #Emp))
BEGIN RAISERROR(N'A matched employee does not look like test data (or is referenced by real data). Nothing deleted.', 16, 1); RETURN; END

SELECT Id INTO #Pos   FROM dbo.Positions        WHERE Title LIKE @TagPrefix + N' Analyst %' AND PositionCode NOT IN (N'POS-0001',N'POS-0002',N'POS-0003',N'POS-0004',N'POS-0005',N'POS-0006',N'POS-0007');
SELECT Id INTO #CC    FROM dbo.CostCenters      WHERE Name LIKE @TagPrefix + N' Sales & Marketing %' AND CostCenterCode NOT IN (N'CC-0001',N'CC-0002',N'CC-0003');
SELECT Id INTO #Grade FROM dbo.JobGrades        WHERE Name LIKE @TagPrefix + N' Grade %' AND GradeCode NOT LIKE N'GR-000[1-6]';
SELECT Id INTO #Cal   FROM dbo.HolidayCalendars WHERE Name LIKE @TagPrefix + N' Calendar %' AND CalendarCode <> N'HC-0001';
SELECT Id, LeaveTypeId, EffectiveYear INTO #Pol FROM dbo.LeavePolicies WHERE PolicyCode LIKE @TagPrefix + N'-%';
SELECT Id INTO #Cycle FROM dbo.AppraisalCycles  WHERE CycleCode LIKE @TagPrefix + N'-%';

SELECT Id INTO #Req  FROM dbo.ManpowerRequisitions WHERE PositionId IN (SELECT Id FROM #Pos);
SELECT Id INTO #Post FROM dbo.JobPostings WHERE ManpowerRequisitionId IN (SELECT Id FROM #Req);
SELECT Id INTO #App  FROM dbo.JobApplications WHERE CandidateId IN (SELECT Id FROM #Cand) OR JobPostingId IN (SELECT Id FROM #Post);
SELECT Id INTO #Int  FROM dbo.Interviews WHERE JobApplicationId IN (SELECT Id FROM #App);
SELECT Id INTO #Off  FROM dbo.JobOffers WHERE CandidateId IN (SELECT Id FROM #Cand) OR JobApplicationId IN (SELECT Id FROM #App);
SELECT Id INTO #Onb  FROM dbo.Onboardings WHERE JobOfferId IN (SELECT Id FROM #Off) OR EmployeeId IN (SELECT Id FROM #Emp);

SELECT Id INTO #Sep  FROM dbo.Separations WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #Clr  FROM dbo.SeparationClearances WHERE SeparationId IN (SELECT Id FROM #Sep);
SELECT Id INTO #Exit FROM dbo.ExitInterviews WHERE SeparationId IN (SELECT Id FROM #Sep);
SELECT Id INTO #Rev  FROM dbo.Reviews WHERE EmployeeId IN (SELECT Id FROM #Emp) OR AppraisalCycleId IN (SELECT Id FROM #Cycle);
SELECT Id INTO #Goal FROM dbo.Goals   WHERE EmployeeId IN (SELECT Id FROM #Emp) OR AppraisalCycleId IN (SELECT Id FROM #Cycle);
SELECT Id INTO #Slip FROM dbo.Payslips WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #Loan FROM dbo.Loans    WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #Sal  FROM dbo.SalaryStructures WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #LApp FROM dbo.LeaveApplications WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #OT   FROM dbo.OvertimeRequests WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #Reg  FROM dbo.AttendanceRegularizationRequests WHERE EmployeeId IN (SELECT Id FROM #Emp);
SELECT Id INTO #Rost FROM dbo.ShiftRosters WHERE EmployeeId IN (SELECT Id FROM #Emp);
-- runs affected by test payslips; "pure" ones go entirely
SELECT DISTINCT PayrollRunId AS Id INTO #Run FROM dbo.Payslips WHERE Id IN (SELECT Id FROM #Slip);
SELECT r.Id INTO #RunPure FROM #Run r
WHERE NOT EXISTS (SELECT 1 FROM dbo.Payslips s WHERE s.PayrollRunId = r.Id AND s.Id NOT IN (SELECT Id FROM #Slip));
-- entitlements test policies granted to real employees (no real policy, nothing used)
SELECT DISTINCT p.LeaveTypeId, p.EffectiveYear AS [Year] INTO #PolScope FROM #Pol p
WHERE NOT EXISTS (SELECT 1 FROM dbo.LeavePolicies x WHERE x.LeaveTypeId = p.LeaveTypeId AND x.EffectiveYear = p.EffectiveYear
                  AND x.Id NOT IN (SELECT Id FROM #Pol) AND x.IsActive = 1);

DECLARE @summary NVARCHAR(MAX) = CONCAT(N'Test candidates: ', (SELECT COUNT(*) FROM #Cand), N' | test employees: ',
             ISNULL(STUFF((SELECT N', ' + EmployeeCode FROM #Emp ORDER BY EmployeeCode FOR XML PATH('')), 1, 2, N''), N'none'));
PRINT @summary;

BEGIN TRANSACTION;

-- -----------------------------------------------------------------------------
-- 2. Employee-owned records (children first)
-- -----------------------------------------------------------------------------
DELETE FROM dbo.ExitInterviews WHERE Id IN (SELECT Id FROM #Exit);                    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ExitInterviews', @n);
DELETE FROM dbo.SeparationClearances WHERE Id IN (SELECT Id FROM #Clr);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SeparationClearances', @n);
DELETE FROM dbo.Separations WHERE Id IN (SELECT Id FROM #Sep);                        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Separations', @n);

DELETE FROM dbo.ReviewRatings WHERE ReviewId IN (SELECT Id FROM #Rev);                SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ReviewRatings', @n);
DELETE FROM dbo.Reviews WHERE Id IN (SELECT Id FROM #Rev);                            SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Reviews', @n);
DELETE FROM dbo.Goals WHERE Id IN (SELECT Id FROM #Goal);                             SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Goals', @n);

DELETE FROM dbo.LoanInstallments WHERE LoanId IN (SELECT Id FROM #Loan) OR PayslipId IN (SELECT Id FROM #Slip);
                                                                                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LoanInstallments', @n);
DELETE FROM dbo.PayslipLines WHERE PayslipId IN (SELECT Id FROM #Slip);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'PayslipLines', @n);
DELETE FROM dbo.Payslips WHERE Id IN (SELECT Id FROM #Slip);                          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Payslips', @n);
DELETE FROM dbo.PayrollRuns WHERE Id IN (SELECT Id FROM #RunPure);                    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'PayrollRuns (test-only)', @n);
-- mixed runs: keep real payslips, recompute totals
UPDATE r SET TotalGross = s.G, TotalDeductions = s.D, TotalNet = s.N, EmployeeCount = s.C, UpdatedDate = GETUTCDATE()
FROM dbo.PayrollRuns r
JOIN (SELECT PayrollRunId, SUM(GrossEarnings) G, SUM(TotalDeductions) D, SUM(NetPay) N, COUNT(*) C
      FROM dbo.Payslips GROUP BY PayrollRunId) s ON s.PayrollRunId = r.Id
WHERE r.Id IN (SELECT Id FROM #Run) AND r.Id NOT IN (SELECT Id FROM #RunPure);
                                                                                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'PayrollRuns (mixed, totals recomputed)', @n);
DELETE FROM dbo.Loans WHERE Id IN (SELECT Id FROM #Loan);                             SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Loans', @n);
DELETE FROM dbo.SalaryStructureLines WHERE SalaryStructureId IN (SELECT Id FROM #Sal);SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SalaryStructureLines', @n);
DELETE FROM dbo.SalaryStructures WHERE Id IN (SELECT Id FROM #Sal);                   SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SalaryStructures', @n);

DELETE FROM dbo.LeaveTransactions WHERE EmployeeId IN (SELECT Id FROM #Emp);          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveTransactions (test employees)', @n);
DELETE FROM dbo.LeaveApplications WHERE Id IN (SELECT Id FROM #LApp);                 SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveApplications', @n);
DELETE FROM dbo.LeaveBalances WHERE EmployeeId IN (SELECT Id FROM #Emp);              SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveBalances (test employees)', @n);
-- grants the test policies made to real employees
DELETE t FROM dbo.LeaveTransactions t JOIN #PolScope s ON s.LeaveTypeId = t.LeaveTypeId AND s.[Year] = t.[Year]
WHERE t.TransactionTypeId = 1 AND t.LeaveApplicationId IS NULL;                       SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveTransactions (test grants to real employees)', @n);
DELETE b FROM dbo.LeaveBalances b JOIN #PolScope s ON s.LeaveTypeId = b.LeaveTypeId AND s.[Year] = b.[Year]
WHERE b.Used = 0 AND b.Pending = 0
  AND NOT EXISTS (SELECT 1 FROM dbo.LeaveTransactions t WHERE t.EmployeeId = b.EmployeeId AND t.LeaveTypeId = b.LeaveTypeId AND t.[Year] = b.[Year]);
                                                                                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveBalances (test grants to real employees)', @n);

DELETE FROM dbo.OvertimeRequests WHERE Id IN (SELECT Id FROM #OT);                    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'OvertimeRequests', @n);
DELETE FROM dbo.AttendanceRegularizationRequests WHERE Id IN (SELECT Id FROM #Reg);   SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AttendanceRegularizationRequests', @n);
DELETE FROM dbo.Attendances WHERE EmployeeId IN (SELECT Id FROM #Emp);                SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Attendances', @n);
DELETE FROM dbo.ShiftRosters WHERE Id IN (SELECT Id FROM #Rost);                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ShiftRosters', @n);

DELETE FROM dbo.IntegrationOutbox WHERE AggregateId IN (SELECT Id FROM #Emp);         SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'IntegrationOutbox', @n);

-- -----------------------------------------------------------------------------
-- 3. Recruitment / onboarding chain, then the employees themselves
-- -----------------------------------------------------------------------------
DELETE FROM dbo.OnboardingTasks WHERE OnboardingId IN (SELECT Id FROM #Onb);          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'OnboardingTasks', @n);
DELETE FROM dbo.Onboardings WHERE Id IN (SELECT Id FROM #Onb);                        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Onboardings', @n);

-- profile child tables cascade with the employee
DELETE FROM dbo.Employees WHERE Id IN (SELECT Id FROM #Emp);                          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Employees', @n);

DELETE FROM dbo.JobOffers WHERE Id IN (SELECT Id FROM #Off);                          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobOffers', @n);
DELETE FROM dbo.InterviewPanelMembers WHERE InterviewId IN (SELECT Id FROM #Int);     SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'InterviewPanelMembers', @n);
DELETE FROM dbo.Interviews WHERE Id IN (SELECT Id FROM #Int);                         SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Interviews', @n);
DELETE FROM dbo.JobApplications WHERE Id IN (SELECT Id FROM #App);                    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobApplications', @n);
DELETE FROM dbo.Candidates WHERE Id IN (SELECT Id FROM #Cand);                        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Candidates', @n);
DELETE FROM dbo.JobPostings WHERE Id IN (SELECT Id FROM #Post);                       SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobPostings', @n);
DELETE FROM dbo.ManpowerRequisitions WHERE Id IN (SELECT Id FROM #Req);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ManpowerRequisitions', @n);

-- -----------------------------------------------------------------------------
-- 4. Tagged org setup / configuration records
-- -----------------------------------------------------------------------------
DELETE FROM dbo.Positions WHERE Id IN (SELECT Id FROM #Pos);                          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'Positions', @n);
DELETE FROM dbo.CostCenters WHERE Id IN (SELECT Id FROM #CC);                         SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'CostCenters', @n);
DELETE FROM dbo.JobGrades WHERE Id IN (SELECT Id FROM #Grade);                        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobGrades', @n);
DELETE FROM dbo.HolidayDates WHERE HolidayCalendarId IN (SELECT Id FROM #Cal);        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'HolidayDates', @n);
DELETE FROM dbo.HolidayCalendars WHERE Id IN (SELECT Id FROM #Cal);                   SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'HolidayCalendars', @n);
DELETE FROM dbo.LeavePolicies WHERE Id IN (SELECT Id FROM #Pol);                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeavePolicies', @n);
DELETE FROM dbo.AppraisalCycles WHERE Id IN (SELECT Id FROM #Cycle);                  SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppraisalCycles', @n);

-- -----------------------------------------------------------------------------
-- 5. Audit rows (incl. the "Deleted" rows written by triggers just now)
-- -----------------------------------------------------------------------------
DELETE FROM dbo.EmployeesLog WHERE EmployeeId IN (SELECT Id FROM #Emp);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'EmployeesLog', @n);
DELETE FROM dbo.CandidatesLog WHERE CandidateId IN (SELECT Id FROM #Cand);            SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'CandidatesLog', @n);
DELETE FROM dbo.JobApplicationsLog WHERE JobApplicationId IN (SELECT Id FROM #App);   SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobApplicationsLog', @n);
DELETE FROM dbo.JobOffersLog WHERE JobOfferId IN (SELECT Id FROM #Off);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobOffersLog', @n);
DELETE FROM dbo.JobPostingsLog WHERE JobPostingId IN (SELECT Id FROM #Post);          SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobPostingsLog', @n);
DELETE FROM dbo.ManpowerRequisitionsLog WHERE ManpowerRequisitionId IN (SELECT Id FROM #Req); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ManpowerRequisitionsLog', @n);
DELETE FROM dbo.OnboardingsLog WHERE OnboardingId IN (SELECT Id FROM #Onb);           SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'OnboardingsLog', @n);
DELETE FROM dbo.PositionsLog WHERE PositionId IN (SELECT Id FROM #Pos);               SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'PositionsLog', @n);
DELETE FROM dbo.CostCentersLog WHERE CostCenterId IN (SELECT Id FROM #CC);            SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'CostCentersLog', @n);
DELETE FROM dbo.JobGradesLog WHERE JobGradeId IN (SELECT Id FROM #Grade);             SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'JobGradesLog', @n);
DELETE FROM dbo.HolidayCalendarsLog WHERE HolidayCalendarId IN (SELECT Id FROM #Cal); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'HolidayCalendarsLog', @n);
DELETE FROM dbo.LeavePoliciesLog WHERE LeavePolicyId IN (SELECT Id FROM #Pol);        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeavePoliciesLog', @n);
DELETE FROM dbo.LeaveApplicationsLog WHERE LeaveApplicationId IN (SELECT Id FROM #LApp); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LeaveApplicationsLog', @n);
DELETE FROM dbo.AppraisalCyclesLog WHERE AppraisalCycleId IN (SELECT Id FROM #Cycle); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AppraisalCyclesLog', @n);
DELETE FROM dbo.GoalsLog WHERE GoalId IN (SELECT Id FROM #Goal);                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'GoalsLog', @n);
DELETE FROM dbo.ReviewsLog WHERE ReviewId IN (SELECT Id FROM #Rev);                   SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ReviewsLog', @n);
DELETE FROM dbo.PayrollRunsLog WHERE PayrollRunId IN (SELECT Id FROM #RunPure);       SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'PayrollRunsLog', @n);
DELETE FROM dbo.LoansLog WHERE LoanId IN (SELECT Id FROM #Loan);                      SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'LoansLog', @n);
DELETE FROM dbo.SalaryStructuresLog WHERE SalaryStructureId IN (SELECT Id FROM #Sal); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SalaryStructuresLog', @n);
DELETE FROM dbo.OvertimeRequestsLog WHERE OvertimeRequestId IN (SELECT Id FROM #OT);  SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'OvertimeRequestsLog', @n);
DELETE FROM dbo.AttendanceRegularizationRequestsLog WHERE RegularizationRequestId IN (SELECT Id FROM #Reg); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'AttendanceRegularizationRequestsLog', @n);
DELETE FROM dbo.ShiftRostersLog WHERE ShiftRosterId IN (SELECT Id FROM #Rost);        SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ShiftRostersLog', @n);
DELETE FROM dbo.SeparationsLog WHERE SeparationId IN (SELECT Id FROM #Sep);           SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SeparationsLog', @n);
DELETE FROM dbo.SeparationClearancesLog WHERE SeparationClearanceId IN (SELECT Id FROM #Clr); SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'SeparationClearancesLog', @n);
DELETE FROM dbo.ExitInterviewsLog WHERE ExitInterviewId IN (SELECT Id FROM #Exit);    SET @n=@@ROWCOUNT; INSERT #Report VALUES (N'ExitInterviewsLog', @n);

-- -----------------------------------------------------------------------------
-- 6. Report, then commit or roll back
-- -----------------------------------------------------------------------------
SELECT TableName, RowsDeleted FROM #Report WHERE RowsDeleted > 0 ORDER BY Step;
SELECT SUM(RowsDeleted) AS TotalRows FROM #Report;

IF @Execute = 1
BEGIN
    COMMIT TRANSACTION;
    PRINT N'HumanResourcesDB test data DELETED (committed).';
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    PRINT N'DRY RUN - nothing was changed. Set @Execute = 1 to delete.';
END
