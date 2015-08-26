
/*===============================================================================
 *
 *      Code Comment Block Here.
 *      
 *      Generated by Code Generator on 03/02/2015 12:10  
 * 
 *===============================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FeatureDemandPlanning.Dapper;
using FeatureDemandPlanning.BusinessObjects;
using FeatureDemandPlanning.Helpers;

namespace FeatureDemandPlanning.DataStore
{
    public class ChangeDiaryDataStore: DataStoreBase
    {
    
        public ChangeDiaryDataStore(string cdsid)
        {
            this.CurrentCDSID = cdsid;
        }

        public IEnumerable<ChangeDiary> ChangeDiaryGetMany (int docId)
        {
            IEnumerable<ChangeDiary> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
                    para.Add("@p_OXO_Doc_Id",docId, dbType: DbType.Int32);    
					retVal = conn.Query<ChangeDiary>("dbo.OXO_ChangeDiary_GetMany", para, commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ChangeDiaryDataStore.ChangeDiaryGetMany", ex.Message, CurrentCDSID);
				}
			}

            return retVal;   
        }

        public ChangeDiary ChangeDiaryGet(int id)
        {
            ChangeDiary retVal = null;

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
                    retVal = conn.Query<ChangeDiary>("dbo.OXO_ChangeDiary_Get", para, commandType: CommandType.StoredProcedure).FirstOrDefault();
				}
				catch (Exception ex)
				{
				   AppHelper.LogError("ChangeDiaryDataStore.ChangeDiaryGet", ex.Message, CurrentCDSID);
				}
			}

            return retVal;
        }

        public bool ChangeDiarySave(ChangeDiary obj)
        {
            bool retVal = true;
            string procName = (obj.Id == 0 ? "dbo.OXO_ChangeDiary_New" : "dbo.OXO_ChangeDiary_Edit");

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();

					 para.Add("@p_OXO_Doc_Id", obj.OXODocId, dbType: DbType.Int32);
					 para.Add("@p_Programme_Id", obj.ProgrammeId, dbType: DbType.Int32);
                     para.Add("@p_Version_Info", obj.VersionInfo, dbType: DbType.String, size: 500);					
					 para.Add("@p_Entry_Header", obj.EntryHeader, dbType: DbType.String, size: -1);
					 para.Add("@p_Entry_Date", obj.EntryDate, dbType: DbType.DateTime);
					 para.Add("@p_Markets", obj.Markets, dbType: DbType.String, size: -1);
					 para.Add("@p_Models", obj.Models, dbType: DbType.String, size: -1);
					 para.Add("@p_Features", obj.Features, dbType: DbType.String, size: -1);
					 para.Add("@p_Current_Fitment", obj.CurrentFitment, dbType: DbType.String, size: 50);
					 para.Add("@p_Proposed_Fitment", obj.ProposedFitment, dbType: DbType.String, size: 50);
					 para.Add("@p_Comment", obj.Comment, dbType: DbType.String, size: -1);
                     para.Add("@p_Pricing_Status", obj.PricingStatus, dbType: DbType.String, size: 500);
                     para.Add("@p_Digital_Status", obj.DigitalStatus, dbType: DbType.String, size: 500);
                     para.Add("@p_Requester", obj.Requester, dbType: DbType.String, size: 100);
					 para.Add("@p_PACN", obj.PACN, dbType: DbType.String, size: 100);
					 para.Add("@p_ETracker", obj.ETracker, dbType: DbType.String, size: 100);
					 para.Add("@p_Order_Call", obj.OrderCall, dbType: DbType.String, size: 100);
					 para.Add("@p_Build_Effective_Date", obj.BuildEffectiveDate, dbType: DbType.DateTime);
					    

					if (obj.Id == 0)
					{
						para.Add("@p_Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
					}
					else
					{
						para.Add("@p_Id", obj.Id, dbType: DbType.Int32);
					}

					conn.Execute(procName, para, commandType: CommandType.StoredProcedure);

					if (obj.Id == 0)
					{
						obj.Id = para.Get<int>("@p_Id");
					}

				}
				catch (Exception ex)
				{
					AppHelper.LogError("ChangeDiaryDataStore.ChangeDiarySave", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
            
        }


        public bool ChangeDiaryDelete(int progid, int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
                    para.Add("@p_prog_Id", progid, dbType: DbType.Int32);
                    conn.Execute("dbo.OXO_ChangeDiary_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ChangeDiaryDataStore.ChangeDiaryDelete", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
        }
    }
}