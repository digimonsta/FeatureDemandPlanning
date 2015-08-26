
/*===============================================================================
 *
 *      Code Comment Block Here.
 *      
 *      Generated by Code Generator on 28/07/2015 12:14  
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
    public class ForecastVehicleDataStore: DataStoreBase
    {
    
        public ForecastVehicleDataStore(string cdsid)
        {
            this.CurrentCDSID = cdsid;
        }

        public IEnumerable<ForecastVehicle> ForecastVehicleGetMany
        (
            	     
        )
        {
            IEnumerable<ForecastVehicle> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					    
					retVal = conn.Query<ForecastVehicle>("dbo.ForecastVehicle_GetMany", para, commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ForecastVehicleDataStore.ForecastVehicleGetMany", ex.Message, CurrentCDSID);
				}
			}

            return retVal;   
        }

        public ForecastVehicle ForecastVehicleGet(int id)
        {
            ForecastVehicle retVal = null;

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					retVal = conn.Query<ForecastVehicle>("dbo.ForecastVehicle_Get", para, commandType: CommandType.StoredProcedure).FirstOrDefault();
				}
				catch (Exception ex)
				{
				   AppHelper.LogError("ForecastVehicleDataStore.ForecastVehicleGet", ex.Message, CurrentCDSID);
				}
			}

            return retVal;
        }

        public bool ForecastVehicleSave(ForecastVehicle obj)
        {
            bool retVal = true;
            string procName = (obj.Id == 0 ? "dbo.ForecastVehicle_New" : "dbo.ForecastVehicle_Edit");

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();

					 para.Add("@p_ForecastVehicleId", obj.ForecastVehicleId, dbType: DbType.Int32);
					 para.Add("@p_ProgrammeId", obj.ProgrammeId, dbType: DbType.Int32);
					 para.Add("@p_VehicleId", obj.VehicleId, dbType: DbType.Int32);
					 para.Add("@p_GatewayId", obj.GatewayId, dbType: DbType.Int32);
					    

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
					AppHelper.LogError("ForecastVehicleDataStore.ForecastVehicleSave", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
            
        }


        public bool ForecastVehicleDelete(int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, dbType: DbType.Int32);
					conn.Execute("dbo.ForecastVehicle_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
					AppHelper.LogError("ForecastVehicleDataStore.ForecastVehicleDelete", ex.Message, CurrentCDSID);
					retVal = false;
				}
			}

            return retVal;
        }
    }
}