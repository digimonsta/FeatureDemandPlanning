﻿CREATE PROCEDURE [dbo].[OXO_Programme_MarketGroup_GetMany]
  @p_prog_id int,	
  @p_doc_id int = 0,
  @p_deep_get BIT = 0
AS
	
	DECLARE @p_archived BIT
	
	SELECT @p_archived = Archived 
	FROM OXO_Doc 
	WHERE Id = @p_doc_id	
	AND Programme_Id = @p_prog_id;
	
	IF ISNULL(@p_archived, 0) = 0		
	BEGIN
		WITH models AS
		(
			SELECT COUNT(Distinct OD.Model_Id) VariantCount,  
				   OD.Market_Group_Id
			FROM OXO_Item_Data_MBM OD WITH(NOLOCK)
			INNER JOIN OXO_Programme_Model V WITH(NOLOCK)
			ON V.Id = OD.Model_Id
			AND V.Active = 1 
			AND V.Programme_Id = @p_prog_id
			WHERE OD.OXO_Doc_Id = @p_doc_id
			AND OD.OXO_Code = 'Y'
			AND OD.Active = 1
			GROUP BY OD.Market_Group_Id
		), marketGroups AS
		(
			SELECT Distinct 'Programme' AS Type,
			Market_Group_Id  AS Id,
			Programme_Id AS ProgrammeId,
			Market_Group_Name AS GroupName,
			Display_Order
			FROM OXO_Programme_MarketGroupMarket_VW WITH(NOLOCK)
			WHERE Programme_Id = @p_prog_id	
		)		
		SELECT G.Type, G.Id, G.ProgrammeId, G.GroupName, G.Display_Order, ISNULL(M.VariantCount,0) AS VariantCount
		FROM marketGroups G
		LEFT OUTER JOIN models M
		ON G.Id = M.Market_Group_Id
		ORDER BY Display_Order;
	    		    
		IF @p_deep_get = 1
			WITH models AS
			(
				SELECT COUNT(Distinct OD.Model_Id) VariantCount,  
					   OD.Market_Id
				FROM OXO_Item_Data_MBM OD WITH(NOLOCK)
				INNER JOIN OXO_Programme_Model V WITH(NOLOCK)
				ON V.Id = OD.Model_Id
				AND V.Active = 1 
				AND V.Programme_Id = @p_prog_id
				WHERE OD.OXO_Doc_Id = @p_doc_id
				AND OD.OXO_Code = 'Y'
				AND OD.Active = 1
				GROUP BY OD.Market_Id
			), markets AS
			(
				SELECT Distinct 
					Market_Id AS Id,
					Market_Name AS Name,
					WHD AS WHD,
					PAR AS PAR_X,
					PAR AS PAR_L,
					Market_Group_Id AS ParentId,
					SubRegion AS SubRegion,
					SubRegionOrder
				FROM OXO_Programme_MarketGroupMarket_VW WITH(NOLOCK)
				WHERE Programme_Id = @p_prog_id	
			)
			SELECT MK.Id, MK.Name, MK.WHD AS WHD,
				   MK.PAR_X, MK.PAR_L, MK.ParentId,
				   MK.SubRegion, MK.SubRegionOrder,
				   ISNULL(M.VariantCount,0) AS VariantCount  
			FROM markets MK
			LEFT OUTER JOIN models M
			ON MK.Id = M.Market_Id
			ORDER BY ParentId, SubRegionOrder, SubRegion, Name;
	END
	ELSE
	BEGIN
		WITH models AS
		(
			SELECT COUNT(Distinct OD.Model_Id) VariantCount,  
				   OD.Market_Group_Id
			FROM OXO_Item_Data_MBM OD WITH(NOLOCK)
			INNER JOIN OXO_Archived_Programme_Model V WITH(NOLOCK)
			ON V.Id = OD.Model_Id
			AND V.Active = 1 
			AND V.Programme_Id = @p_prog_id
			AND V.Doc_Id = @p_doc_id
			WHERE OD.OXO_Doc_Id = @p_doc_id
			AND OD.OXO_Code = 'Y'
			AND OD.Active = 1
			GROUP BY OD.Market_Group_Id
		), marketGroups AS
		(
			SELECT Distinct 'Programme' AS Type,
			Market_Group_Id  AS Id,
			Programme_Id AS ProgrammeId,
			Market_Group_Name AS GroupName,
			Display_Order
			FROM OXO_Archived_Programme_MarketGroupMarket_VW
			WHERE Programme_Id = @p_prog_id	
			AND Doc_Id = @p_doc_id
		)		
		SELECT G.Type, G.Id, G.ProgrammeId, G.GroupName, G.Display_Order, ISNULL(M.VariantCount,0) AS VariantCount
		FROM marketGroups G
		LEFT OUTER JOIN models M
		ON G.Id = M.Market_Group_Id
		ORDER BY Display_Order;
	    		    
		IF @p_deep_get = 1
			WITH models AS
			(
				SELECT COUNT(Distinct OD.Model_Id) VariantCount,  
					   OD.Market_Id
				FROM OXO_Item_Data_MBM OD WITH(NOLOCK)
				INNER JOIN OXO_Archived_Programme_Model V WITH(NOLOCK)
				ON V.Id = OD.Model_Id
				AND V.Active = 1 
				AND V.Programme_Id = @p_prog_id
				AND V.Doc_Id = @p_doc_id
				WHERE OD.OXO_Doc_Id = @p_doc_id
				AND OD.OXO_Code = 'Y'
				AND OD.Active = 1
				GROUP BY OD.Market_Id
			), markets AS
			(
				SELECT Distinct 
					Market_Id AS Id,
					Market_Name AS Name,
					WHD AS WHD,
					PAR AS PAR_X,
					PAR AS PAR_L,
					Market_Group_Id AS ParentId,
					SubRegion AS SubRegion,
					SubRegionOrder
				FROM OXO_Archived_Programme_MarketGroupMarket_VW WITH(NOLOCK)
				WHERE Programme_Id = @p_prog_id	
				AND Doc_Id = @p_doc_id
			)
			SELECT MK.Id, MK.Name, MK.WHD AS WHD,
				   MK.PAR_X, MK.PAR_L, MK.ParentId,
				   MK.SubRegion, MK.SubRegionOrder,
				   ISNULL(M.VariantCount,0) AS VariantCount  
			FROM markets MK
			LEFT OUTER JOIN models M
			ON MK.Id = M.Market_Id
			ORDER BY ParentId, SubRegionOrder, SubRegion, Name;
	END

