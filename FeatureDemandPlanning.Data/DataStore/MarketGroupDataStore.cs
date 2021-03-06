using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FeatureDemandPlanning.Model;
using FeatureDemandPlanning.Model.Dapper;
using FeatureDemandPlanning.Model.Empty;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Helpers;

namespace FeatureDemandPlanning.DataStore
{
    public class MarketGroupDataStore: DataStoreBase
    {
    
        public MarketGroupDataStore(string cdsid)
        {
            CurrentCDSID = cdsid;
        }

        public IEnumerable<MarketGroup> FdpMarketGroupGetMany(TakeRateFilter filter)
        {
            IEnumerable<MarketGroup> retVal;

            using (var conn = DbHelper.GetDBConnection())
            {
                try
                {
                    var para = DynamicParameters.FromCDSId(CurrentCDSID);
                    para.Add("@DocumentId", filter.DocumentId, DbType.Int32);
                    para.Add("@FdpVolumeHeaderId", filter.TakeRateId, DbType.Int32);

                    using (var results = conn.QueryMultiple("dbo.Fdp_MarketGroup_GetMany", para, commandType: CommandType.StoredProcedure))
                    {
                        var marketGroups = results.Read<MarketGroup>().ToList();

                        var markets = results.Read<Market>().ToList();
                        foreach (var marketGroup in marketGroups)
                        {
                            marketGroup.Markets = markets.Where(c => c.ParentId == marketGroup.Id).ToList();
                        }

                        marketGroups = marketGroups.Where(mg => mg.Markets.Any()).ToList();

                        retVal = marketGroups;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }

            return retVal;
        }

        public IEnumerable<MarketGroup> MarketGroupGetMany(bool deepGet = false)
        {
            IEnumerable<MarketGroup> retVal = null;
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
                    List<MarketGroup> marketGroups;

                    var para = new DynamicParameters();
                    para.Add("@p_deep_get", deepGet, DbType.Boolean);

                    using (var multi = conn.QueryMultiple("dbo.OXO_Master_MarketGroup_GetMany", para, commandType: CommandType.StoredProcedure))
                    {
                        marketGroups = multi.Read<MarketGroup>().ToList();
                        if (deepGet)
                        {
                            var markets = multi.Read<Market>().ToList();
                            foreach (var marketGroup in marketGroups)
                            {
                                marketGroup.Markets = markets.Where(c => c.ParentId == marketGroup.Id).ToList();
                            }
                        }
                    }

                    retVal = marketGroups;
                    
				}
				catch (Exception ex)
				{
                    Log.Error(ex);
                    throw;
				}
			}

            return retVal;   
        }

        public IEnumerable<MarketGroup> MarketGroupGetMany(int progId, int docId, bool deepGet = false)
        {
            IEnumerable<MarketGroup> retVal = null;
            using (IDbConnection conn = DbHelper.GetDBConnection())
            {
                try
                {
                    List<MarketGroup> marketGroups;

                    var para = DynamicParameters.FromCDSId(CurrentCDSID);
                    para.Add("@DocumentId", docId, DbType.Int32);

                    using (var multi = conn.QueryMultiple("Fdp_MarketGroup_GetMany", para, commandType: CommandType.StoredProcedure))
                    {
                        marketGroups = multi.Read<MarketGroup>().ToList();
                        
                            var markets = multi.Read<Market>().ToList();
                            foreach (var marketGroup in marketGroups)
                            {
                                marketGroup.Markets = markets.Where(c => c.ParentId == marketGroup.Id).ToList();
                            }
                            marketGroups = marketGroups.Where(mg => mg.Markets.Any()).ToList();
                        
                    }

                    retVal = marketGroups;

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }

            return retVal;
        }

        public MarketGroup FdpMarketGroupGet(TakeRateFilter filter)
        {
            MarketGroup retVal = new EmptyMarketGroup();
            
            using (var conn = DbHelper.GetDBConnection())
            {
                try
                {
                    var para = new DynamicParameters();
                    para.Add("@MarketGroupId", filter.MarketGroupId, DbType.Int32);
                    para.Add("@FdpVolumeHeaderId", filter.TakeRateId, DbType.Int32);

                    using (var multi = conn.QueryMultiple("dbo.Fdp_MarketGroup_Get", para, commandType: CommandType.StoredProcedure))
                    {
                        retVal = multi.Read<MarketGroup>().FirstOrDefault();
                        
                        var markets = multi.Read<Market>().ToList();
                        if (retVal != null)
                            retVal.Markets = markets.ToList();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }

            return retVal;
        }

        public MarketGroup MarketGroupGet(string type, int id, int progid = 0, int docid = 0, bool deepGet = false)
        {
            MarketGroup retVal = null;
            string procName = (type == "Master" ? "dbo.OXO_Master_MarketGroup_Get" : "dbo.OXO_Programme_MarketGroup_Get");

			using (IDbConnection conn = DbHelper.GetDBConnection())
			{
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, DbType.Int32);
                    if (type != "Master")
                    {
                        para.Add("@p_prog_id", progid, DbType.Int32);
                        para.Add("@p_doc_id", docid, DbType.Int32);
                    }
                    para.Add("@p_deep_get", deepGet, DbType.Boolean);

                    using (var multi = conn.QueryMultiple(procName, para, commandType: CommandType.StoredProcedure))
                    {
                        retVal = multi.Read<MarketGroup>().FirstOrDefault();
                        if (deepGet)
                        {
                            var markets = multi.Read<Market>().ToList();
                            if (retVal != null)
                                retVal.Markets = markets.ToList();
                        }
                    }

				}
				catch (Exception ex)
				{
                    Log.Error(ex);
                    throw;
				}
			}

            return retVal;
        }

        public bool MarketGroupSave(MarketGroup obj)
        {
            bool retVal = true;
            string procName = "";
            if (obj.Type == "Master")
            {
                procName = (obj.Id == 0 ? "dbo.OXO_Master_MarketGroup_New" : "dbo.OXO_Master_MarketGroup_Edit");
            }
            else
            {
                procName = (obj.Id == 0 ? "dbo.OXO_Programme_MarketGroup_New" : "dbo.OXO_Programme_MarketGroup_Edit");
            }

			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
                    obj.Save(CurrentCDSID);

					var para = new DynamicParameters();
                    para.Add("@p_Group_Name", obj.GroupName, DbType.String, size: 500);
                    if (obj.Type != "Master")
                    {
                        para.Add("@p_prog_id", obj.ProgrammeId, DbType.Int32);
                    }
                    para.Add("@p_Active", obj.Active, DbType.Boolean);
                    para.Add("@p_Display_Order", obj.DisplayOrder, DbType.Int32);
                    para.Add("@p_Created_By", obj.CreatedBy, DbType.String, size: 8);
                    para.Add("@p_Created_On", obj.CreatedOn, DbType.DateTime);
                    para.Add("@p_Updated_By", obj.UpdatedBy, DbType.String, size: 8);
                    para.Add("@p_Last_Updated", obj.LastUpdated, DbType.DateTime);
                    para.Add("@p_Id", obj.Id, DbType.Int32, ParameterDirection.InputOutput);    					
					conn.Execute(procName, para, commandType: CommandType.StoredProcedure);

					if (obj.Id == 0)
					{
						obj.Id = para.Get<int>("@p_Id");
					}

				}
				catch (Exception ex)
				{
                    Log.Error(ex);
                    throw;
				}
			}

            return retVal;
            
        }


        public bool MarketGroupDelete(int id)
        {
            bool retVal = true;
            
			using (IDbConnection conn = DbHelper.GetDBConnection())
            {
				try
				{
					var para = new DynamicParameters();
					para.Add("@p_Id", id, DbType.Int32);
                    conn.Execute("dbo.OXO_Master_MarketGroup_Delete", para, commandType: CommandType.StoredProcedure);                   
				}
				catch (Exception ex)
				{
                    Log.Error(ex);
                    throw;
				}
			}

            return retVal;
        }

   
    }
}