﻿CREATE PROCEDURE [dbo].[Fdp_DPCK_Update]
	  @DocumentId		AS INT
	, @TrimId			AS INT
	, @DPCK				AS NVARCHAR(20) = NULL
	, @CDSID			AS NVARCHAR(16)
AS

	SET NOCOUNT ON;

	DECLARE @FdpImportId AS INT;
	DECLARE @IsArchived AS BIT;
	SELECT TOP 1 @IsArchived = ISNULL(Archived, 0) FROM Oxo_Doc WHERE Id = @DocumentId;

	IF @IsArchived = 1
	BEGIN
		UPDATE OXO_Archived_Programme_Trim SET 
			DPCK = @DPCK,
			--Updated_By = @CDSID,
			Last_Updated = GETDATE()
		WHERE
		Doc_Id = @DocumentId
		AND
		Id = @TrimId;

		SELECT
			  D.Id AS DocumentId
			, T.DPCK
			, T.Name
			, T.[Level]
		FROM
		OXO_Doc AS D
		JOIN OXO_Archived_Programme_Trim AS T ON D.Id = T.Doc_Id
		WHERE
		D.Id = @DocumentId
		AND
		T.Id = @TrimId
	END
	ELSE
	BEGIN
		UPDATE T SET DPCK = @DPCK,
			--Updated_By = @CDSID,
			Last_Updated = GETDATE()
		FROM
		OXO_Doc AS D
		JOIN OXO_Programme_Trim AS T ON D.Programme_Id = T.Programme_Id
		WHERE
		D.Id = @DocumentId
		AND
		T.Id = @TrimId;

		SELECT
			  D.Id AS DocumentId
			, T.DPCK
			, T.Name
			, T.[Level]
		FROM
		OXO_Doc AS D
		JOIN OXO_Programme_Trim AS T ON D.Programme_Id = T.Programme_Id
		WHERE
		D.Id = @DocumentId
		AND
		T.Id = @TrimId
	END
	
	-- If we have a worktray item for this document that is currently unprocessed and in an error state then 
	-- reprocess the derivative errors
	
	SELECT TOP 1 @FdpImportId = Q.FdpImportId
	FROM
	Fdp_ImportQueue_VW AS Q
	WHERE
	Q.DocumentId = @DocumentId
	AND
	Q.FdpImportStatusId = 4;
	
	IF @FdpImportId IS NOT NULL
	BEGIN
		EXEC Fdp_ImportData_Process @FdpImportId = @FdpImportId
	END;