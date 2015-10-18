﻿CREATE PROCEDURE [dbo].[ImportError_Save]
	  @ImportQueueId INT
	, @Error NVARCHAR(MAX)
AS
	SET NOCOUNT ON;
	
	IF NOT EXISTS(SELECT TOP 1 1 FROM ImportQueue WHERE ImportQueueId = @ImportQueueId)
		RAISERROR(N'Import item does not exist', 16, 1);
		
	INSERT INTO ImportError
	(
		  ImportQueueId
		, Error
	)
	VALUES
	(
		  @ImportQueueId
		, @Error
	)
