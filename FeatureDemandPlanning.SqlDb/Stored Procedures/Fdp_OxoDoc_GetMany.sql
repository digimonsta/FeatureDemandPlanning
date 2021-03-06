﻿CREATE PROCEDURE [dbo].[Fdp_OxoDoc_GetMany]
	 @ProgrammeId INT		= NULL
   , @Gateway NVARCHAR(100) = NULL
AS
	SET NOCOUNT ON;

	DECLARE @ShowAll AS BIT;
	SELECT @ShowAll = CAST(Value AS BIT) 
	FROM Fdp_Configuration
	WHERE
	ConfigurationKey = 'ShowAllOXODocuments'
	
	SELECT 
		O.Id  AS Id,
		O.Programme_Id AS ProgrammeId,
		O.Version_Id AS VersionId,
		O.Gateway AS Gateway,  
		dbo.OXO_GetNextGateway(O.Gateway) AS NextGateway, 
		O.Status AS Status,
		O.Owner AS Owner,  
		V.VehicleName,
		V.VehicleAKA,
		V.ModelYear,
		V.VehicleMake,
		O.Created_By  AS CreatedBy,  
		O.Created_On  AS CreatedOn,  
		O.Updated_By  AS UpdatedBy,  
		O.Last_Updated  AS LastUpdated ,
		O.Archived,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'MBM', 0) AS MBMCreated,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'MBM', 1) AS MBMUpdated,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'FBM', 0) AS FBMCreated,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'FBM', 1) AS FBMUpdated,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'GSF', 0) AS GSFCreated,
		[dbo].[OXO_GetChangeTimeStamp] (O.Id, 'GSF', 1) AS GSFUpdated 		 
	FROM dbo.OXO_Doc O   
	JOIN dbo.OXO_Programme_VW AS V ON O.Programme_Id = V.Id
	LEFT JOIN OXO_Gateway AS G ON O.Gateway = G.Gateway
	WHERE 
	(@ProgrammeId IS NULL OR O.Programme_Id = @ProgrammeId)
	AND
	(@Gateway IS NULL OR O.Gateway = @Gateway)
	AND
	(
		@ShowAll = 1
		OR
		(@ShowAll = 0 AND [Status] = 'PUBLISHED')
	)
	ORDER BY 
	ISNULL(G.Display_Order, 1000);