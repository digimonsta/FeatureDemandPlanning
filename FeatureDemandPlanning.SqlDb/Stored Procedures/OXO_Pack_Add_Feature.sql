﻿CREATE PROCEDURE [dbo].[OXO_Pack_Add_Feature] 
  @p_prog_id int,
  @p_doc_id int,
  @p_pack_id int,
  @p_feat_id int,
  @p_cdsid nvarchar(10),
  @p_changeset_id int
AS

  DECLARE @p_archived BIT;
  DECLARE @_count AS INT

  SELECT @p_archived = Archived 
  FROM OXO_Doc 
  WHERE Id = @p_doc_id	
  AND Programme_Id = @p_prog_id;
  
  IF ISNULL(@p_archived, 0) = 0
  BEGIN
	  SELECT @_count = COUNT(*)	
	  FROM dbo.OXO_Pack_Feature_Link
	  WHERE Pack_Id = @p_pack_id
	  AND Feature_Id = @p_feat_id;

	  IF (@_count = 0)
	  BEGIN 	
		
		INSERT INTO dbo.OXO_Pack_Feature_Link (Programme_Id, Pack_Id, Feature_Id, CDSID, ChangeSet_Id)
		VALUES (@p_prog_id, @p_pack_id, @p_feat_id, @p_cdsid, @p_changeset_id);
	 
		UPDATE T1
		SET T1.Active = 1
		FROM dbo.OXO_Item_Data_FPS AS T1
		INNER JOIN dbo.OXO_Doc AS T2
		ON T1.OXO_Doc_Id = T2.Id
		WHERE T1.OXO_Doc_Id = @p_doc_id
		AND T2.Programme_Id = @p_prog_id
		AND T1.Pack_Id = @p_pack_id
		AND T1.Feature_Id = @p_feat_id;
				
	  END
   END 
 ELSE
   BEGIN  
	  SELECT @_count = COUNT(*)	
	  FROM dbo.OXO_Archived_Pack_Feature_Link
	  WHERE Pack_Id = @p_pack_id
	  AND Feature_Id = @p_feat_id
	  AND Doc_Id = @p_doc_id;

	  IF (@_count = 0)
	  BEGIN 	
		
		INSERT INTO dbo.OXO_Archived_Pack_Feature_Link (Programme_Id, Doc_Id, Pack_Id, Feature_Id, CDSID, ChangeSet_Id)
		VALUES (@p_prog_id, @p_doc_id, @p_pack_id, @p_feat_id, @p_cdsid, @p_changeSet_id);
	 
		UPDATE T1
		SET T1.Active = 1
		FROM dbo.OXO_Item_Data_FPS AS T1
		INNER JOIN dbo.OXO_Doc AS T2
		ON T1.OXO_Doc_Id = T2.Id
		WHERE T1.OXO_Doc_Id = @p_doc_id
		AND T2.Programme_Id = @p_prog_id
		AND T1.Pack_Id = @p_pack_id
		AND T1.Feature_Id = @p_feat_id
				
	  END
   END
