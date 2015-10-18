﻿CREATE PROCEDURE [dbo].[OXO_Doc_Clone_Gateway_GSFs] 
   @p_doc_id  int, 
   @p_prog_id  int, 
   @p_new_doc_id  int,    
   @p_clone_by nvarchar(50)
AS
BEGIN

	-- Get Feature Link
	INSERT INTO OXO_Archived_Programme_GSF_Link (
	      Doc_Id, Programme_Id, Feature_Id, CDSID, Comment)
	SELECT Distinct @p_new_doc_id, @p_prog_id, Id, @p_clone_by, FeatureComment 
	FROM dbo.FN_Programme_GSF_Get (@p_prog_id, @p_doc_id);
	
	
	

END
