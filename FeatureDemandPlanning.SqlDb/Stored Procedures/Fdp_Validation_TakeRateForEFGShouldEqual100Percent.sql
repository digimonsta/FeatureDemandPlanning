﻿CREATE PROCEDURE [dbo].[Fdp_Validation_TakeRateForEFGShouldEqual100Percent]
	  @FdpVolumeHeaderId	INT
	, @MarketId				INT			 = NULL
	, @CDSId				NVARCHAR(16) = NULL
AS

	SET NOCOUNT ON;

	-- Get our market volumes taking into account any uncommitted changes to that market
	;WITH EfgTakeRates AS
	(
		SELECT
			  S.MarketId
			, S.FdpTakeRateSummaryId
			, S.ModelId
			, CAST(NULL AS INT) AS FdpModelId
			, F.EFGName
			, CAST(SUM(ISNULL(C.PercentageTakeRate, D.PercentageTakeRate)) AS DECIMAL(5,2)) AS PercentageTakeRate
		FROM
		Fdp_VolumeHeader				AS H 
		JOIN OXO_Doc					AS O	ON	H.DocumentId		= O.Id
		JOIN OXO_Programme_Feature_VW	AS F	ON	O.Programme_Id		= F.ProgrammeId
		LEFT JOIN Fdp_VolumeDataItem_VW		AS D	ON	H.FdpVolumeHeaderId = D.FdpVolumeHeaderId
												AND D.IsFeatureData		= 1
												AND F.ID				= D.FeatureId
		JOIN Fdp_TakeRateSummary		AS S	ON	H.FdpVolumeHeaderId	= S.FdpVolumeHeaderId
												AND S.ModelId			IS NOT NULL
		LEFT JOIN Fdp_ChangesetDataItem_VW AS C	ON	F.Id				= C.FeatureId
												AND S.ModelId			= C.ModelId
												AND C.IsFeatureUpdate		= 1
												AND C.CDSId					= @CDSId
		WHERE
		H.FdpVolumeHeaderId = @FdpVolumeHeaderId
		AND
		S.ModelId IS NOT NULL
		AND
		(@MarketId IS NULL OR S.MarketId = @MarketId)
		GROUP BY
		  S.MarketId
		, S.ModelId
		, S.FdpTakeRateSummaryId
		, F.EFGName

		UNION

		SELECT
			  S.MarketId
			, S.FdpTakeRateSummaryId
			--, F.ID AS FeatureId
			, CAST(NULL AS INT) AS ModelId
			, S.FdpModelId
			, F.EFGName
			, CAST(SUM(ISNULL(C.PercentageTakeRate, D.PercentageTakeRate)) AS DECIMAL(5,2)) AS PercentageTakeRate
		FROM
		Fdp_VolumeHeader				AS H 
		JOIN OXO_Doc					AS O	ON	H.DocumentId			= O.Id
		JOIN OXO_Programme_Feature_VW	AS F	ON	O.Programme_Id			= F.ProgrammeId
		LEFT JOIN Fdp_VolumeDataItem_VW		AS D	ON	H.FdpVolumeHeaderId		= D.FdpVolumeHeaderId
												AND D.IsFeatureData			= 1
												AND F.ID					= D.FeatureId
		JOIN Fdp_TakeRateSummary		AS S	ON	H.FdpVolumeHeaderId		= S.FdpVolumeHeaderId
												AND S.FdpModelId			IS NOT NULL
		LEFT JOIN Fdp_ChangesetDataItem_VW AS C	ON	F.Id					= C.FeatureId
												AND	S.FdpModelId			= C.FdpModelId
												AND C.IsFeatureUpdate		= 1
												AND C.CDSId					= @CDSId
		WHERE
		H.FdpVolumeHeaderId = @FdpVolumeHeaderId
		AND
		S.FdpModelId IS NOT NULL
		AND
		(@MarketId IS NULL OR S.MarketId = @MarketId)
		GROUP BY
		  S.MarketId
		, S.FdpModelId
		, S.FdpTakeRateSummaryId
		, F.EFGName
	)
	--INSERT INTO Fdp_Validation
	--(
	--	  FdpVolumeHeaderId
	--	, MarketId
	--	, FdpValidationRuleId
	--	, [Message]
	--	, FdpTakeRateSummaryId
	--)
	SELECT
		  @FdpVolumeHeaderId AS FdpVolumeHeaderId
		, E.MarketId
		, 7 AS FdpValidationRuleId -- TakeRateForEFGShouldEqual100Percent
		, 'Take rate of ''' + CAST(E.PercentageTakeRate AS NVARCHAR(10)) + '%'' for features in exclusive feature group ''' + E.EFGName + ''' must equal 100%' AS [Message]
		, E.FdpTakeRateSummaryId
		, E.ModelId
		, E.PercentageTakeRate
	FROM
	EfgTakeRates				AS E
	--LEFT JOIN Fdp_Validation	AS V	ON	E.MarketId				= V.MarketId
	--									AND E.FdpTakeRateSummaryId	= V.FdpTakeRateSummaryId
	--									AND V.IsActive				= 1
	WHERE
	E.PercentageTakeRate <> 1
	--AND
	--V.FdpValidationId IS NULL
	
	PRINT 'Total take rate for features as part of EFG must equal 100% validation errors added: ' + CAST(@@ROWCOUNT AS NVARCHAR(10))