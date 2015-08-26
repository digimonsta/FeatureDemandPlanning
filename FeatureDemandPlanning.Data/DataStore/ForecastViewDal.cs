
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
    public class ForecastViewDataStore: DataStoreBase
    {
    
        public ForecastViewDataStore(string cdsid)
        {
            this.CurrentCDSID = cdsid;
        }

        public IEnumerable<ForecastView> ForecastViewGetMany
        (
            	     
        )
        {
            IEnumerable<ForecastView> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					    
					retVal = conn.Query<ForecastView>("dbo.ForecastView_GetMany", para, commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ForecastViewDataStore.ForecastViewGetMany", ex.Message, CurrentCDSID);
				}
			}

            return retVal;   
        }

        public ForecastView ForecastViewGet(int id)
        {
            ForecastView retVal = null;

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					retVal = conn.Query<ForecastView>("dbo.ForecastView_Get", para, commandType: CommandType.StoredProcedure).FirstOrDefault();
				}
				catch (Exception ex)
				{
				   AppHelper.LogError("ForecastViewDataStore.ForecastViewGet", ex.Message, CurrentCDSID);
				}
			}

            return retVal;
        }

        public bool ForecastViewSave(ForecastView obj)
        {
            bool retVal = true;
            string procName = (obj.Id == 0 ? "dbo.ForecastView_New" : "dbo.ForecastView_Edit");

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();

					 para.Add("@p_ForecastId", obj.ForecastId, dbType: DbType.Int32);
					 para.Add("@p_CreatedOn", obj.CreatedOn, dbType: DbType.DateTime);
					 para.Add("@p_CreatedBy", obj.CreatedBy, dbType: DbType.String, size: 16);
					 para.Add("@p_VehicleId", obj.VehicleId, dbType: DbType.Int32);
					 para.Add("@p_ProgrammeId", obj.ProgrammeId, dbType: DbType.Int32);
					 para.Add("@p_GatewayId", obj.GatewayId, dbType: DbType.Int32);
					 para.Add("@p_Make", obj.Make, dbType: DbType.String, size: 500);
					 para.Add("@p_Code", obj.Code, dbType: DbType.String, size: 500);
					 para.Add("@p_Description", obj.Description, dbType: DbType.String, size: 1003);
					 para.Add("@p_ModelYear", obj.ModelYear, dbType: DbType.String, size: 50);
					 para.Add("@p_Gateway", obj.Gateway, dbType: DbType.String, size: 50);
					    

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
					AppHelper.LogError("ForecastViewDataStore.ForecastViewSave", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
            
        }


        public bool ForecastViewDelete(int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					conn.Execute("dbo.ForecastView_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ForecastViewDataStore.ForecastViewDelete", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
        }
    }
}