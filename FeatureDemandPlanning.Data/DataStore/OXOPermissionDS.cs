
/*===============================================================================
 *
 *      Code Comment Block Here.
 *      
 *      Generated by Code Generator on 28/07/2014 15:00  
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
    public class OXOPermissionDataStore: DataStoreBase
    {
    
        public OXOPermissionDataStore(string cdsid)
        {
            this.CurrentCDSID = cdsid;
        }

        public IEnumerable<Permission> PermissionGetMany(string cdsid, string objectType)
        {
            IEnumerable<Permission> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
                    para.Add("@p_CDSID", cdsid, dbType: DbType.String, size: 50);
                    para.Add("@p_Object_Type", objectType, dbType: DbType.String, size: 500);
					    
					retVal = conn.Query<Permission>("dbo.OXO_Permission_GetMany", para, commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					AppHelper.LogError("OXOPermissionDataStore.PermissionGetMany", ex.Message, CurrentCDSID);
				}
			}

            return retVal;   
        }

        public Permission PermissionGet(int id)
        {
            Permission retVal = null;

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					retVal = conn.Query<Permission>("dbo.OXO_Permission_Get", para, commandType: CommandType.StoredProcedure).FirstOrDefault();
				}
				catch (Exception ex)
				{
				   AppHelper.LogError("OXOPermissionDataStore.PermissionGet", ex.Message, CurrentCDSID);
				}
			}

            return retVal;
        }

        public bool PermissionSave(Permission obj)
        {
            bool retVal = true;
            string procName = (obj.Id == 0 ? "dbo.OXO_Permission_New" : "dbo.OXO_Permission_Edit");

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();

					 para.Add("@p_CDSID", obj.CDSID, dbType: DbType.String, size: 50);
					 para.Add("@p_Object_Type", obj.ObjectType, dbType: DbType.String, size: 500);
					 para.Add("@p_Object_Id", obj.ObjectId, dbType: DbType.Int32);
					 para.Add("@p_Object_Val", obj.ObjectVal, dbType: DbType.String, size: 500);
					 para.Add("@p_Operation", obj.Operation, dbType: DbType.String, size: 500);
					 para.Add("@p_Created_By", obj.CreatedBy, dbType: DbType.String, size: 50);
					 para.Add("@p_Created_On", obj.CreatedOn, dbType: DbType.DateTime);
					 para.Add("@p_Updated_By", obj.UpdatedBy, dbType: DbType.String, size: 50);
					 para.Add("@p_Last_Updated", obj.LastUpdated, dbType: DbType.DateTime);
					    

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
					AppHelper.LogError("OXOPermissionDataStore.PermissionSave", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
            
        }


        public bool PermissionDelete(int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					conn.Execute("dbo.OXO_Permission_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
					AppHelper.LogError("OXOPermissionDataStore.PermissionDelete", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
        }
    }
}