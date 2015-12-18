﻿CREATE PROCEDURE [dbo].[Fdp_ChangesetDataItem_Recalculate]
	  @FdpChangesetDataItemId AS INT
AS
	SET NOCOUNT ON;
	
	DECLARE @IsVolumeUpdate BIT;
	DECLARE @IsModelUpdate BIT; -- Update performed at the model level as opposed to feature
	
	SELECT @IsVolumeUpdate = D.IsVolumeUpdate
		, @IsModelUpdate = 
			CASE
				WHEN D.FeatureId IS NULL AND D.FdpFeatureId IS NULL THEN 1
				ELSE 0
			END
	FROM Fdp_ChangesetDataItem AS D
	WHERE
	D.FdpChangesetDataItemId = @FdpChangesetDataItemId;
	
	-- Dependant on what has been updated, recalculate the other figure % or volume accordingly
	
	IF @IsVolumeUpdate = 1
	BEGIN
		EXEC Fdp_ChangesetDataItem_CalculatePercentageTakeRate @FdpChangesetDataItemId = @FdpChangesetDataItemId;
	END
	ELSE
	BEGIN
		EXEC Fdp_ChangesetDataItem_CalculateVolume @FdpChangesetDataItemId = @FdpChangesetDataItemId;
	END
	
	-- Calculate the feature mix (both percentage and volume)
	IF @IsModelUpdate = 1
	BEGIN
		EXEC Fdp_ChangesetDataItem_CalculateFeatureMixForAllFeatures @FdpChangesetDataItemId = @FdpChangesetDataItemId;
	END
	ELSE
	BEGIN
		EXEC Fdp_ChangesetDataItem_CalculateFeatureMix @FdpChangesetDataItemId = @FdpChangesetDataItemId;
	END
	
	EXEC Fdp_ChangesetDataItem_Get @FdpChangesetDataItemId = @FdpChangesetDataItemId;