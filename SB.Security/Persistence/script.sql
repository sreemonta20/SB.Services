-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 24.04.2023
-- Description:	Get user details by supplying ID
-- =============================================
--EXEC SP_GetUserById 'D670A7BA-F10D-4241-8230-6CD8E0A2B7C0'
ALTER PROCEDURE SP_GetUserById 
	-- Add the parameters for the stored procedure here
	@Id UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.UserRole,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfos UI 
	WHERE UI.Id = @Id
END
GO
