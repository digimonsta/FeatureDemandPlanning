
/*===============================================================================
 *
 *      Code Comment Block Here.
 *      
 *      Generated by Code Generator on 28/07/2015 12:15  
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
    public class PermissionOperationDataStore: DataStoreBase
    {
    
        public PermissionOperationDataStore(string cdsid)
        {
            this.CurrentCDSID = cdsid;
        }

        public IEnumerable<PermissionOperation> PermissionOperationGetMany
        (
            	     
        )
        {
            IEnumerable<PermissionOperation> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					    
					retVal = conn.Query<PermissionOperation>("dbo.PermissionOperation_GetMany", para, commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					AppHelper.LogError("PermissionOperationDataStore.PermissionOperationGetMany", ex.Message, CurrentCDSID);
				}
			}

            return retVal;   
        }

        public PermissionOperation PermissionOperationGet(int id)
        {
            PermissionOperation retVal = null;

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					retVal = conn.Query<PermissionOperation>("dbo.PermissionOperation_Get", para, commandType: CommandType.StoredProcedure).FirstOrDefault();
				}
				catch (Exception ex)
				{
				   AppHelper.LogError("PermissionOperationDataStore.PermissionOperationGet", ex.Message, CurrentCDSID);
				}
			}

            return retVal;
        }

        public bool PermissionOperationSave(PermissionOperation obj)
        {
            bool retVal = true;
            string procName = (obj.Id == 0 ? "dbo.PermissionOperation_New" : "dbo.PermissionOperation_Edit");

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();

					 para.Add("@p_FdpPermissionOperationId", obj.FdpPermissionOperationId, dbType: DbType.Int32);
					 para.Add("@p_FdpPermissionObjectType", obj.FdpPermissionObjectType, dbType: DbType.String, size: 50);
					 para.Add("@p_Operation", obj.Operation, dbType: DbType.String, size: 25);
					 para.Add("@p_Description", obj.Description, dbType: DbType.String, size: -1);
					    

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
					AppHelper.LogError("PermissionOperationDataStore.PermissionOperationSave", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
            
        }


        public bool PermissionOperationDelete(int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					conn.Execute("dbo.PermissionOperation_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
					AppHelper.LogError("PermissionOperationDataStore.PermissionOperationDelete", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
        }
    }
}