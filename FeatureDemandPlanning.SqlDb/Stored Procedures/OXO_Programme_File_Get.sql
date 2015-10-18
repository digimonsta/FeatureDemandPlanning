﻿CREATE PROCEDURE [OXO_Programme_File_Get] 
  @p_Id int
AS
	
	SELECT 
      Id  AS Id,      
      File_Category AS FileCategory,
      File_Comment AS FileComment,        
      File_Name  AS FileName,  
      File_Ext  AS FileExt,
      File_Type  AS FileType,  
      File_Size  AS FileSize,  
      File_Content  AS FileContent,  
      Gateway AS Gateway,
      ISNULL(PACN, 'N/A') AS PACN,
      GUID AS GUID,
      Status AS Status,
      Created_By  AS CreatedBy,  
      Created_On  AS CreatedOn,  
      Updated_By  AS UpdatedBy,  
      Last_Updated  AS LastUpdated  
      	     
    FROM OXO_Programme_File_VW
	WHERE Id = @p_Id;

