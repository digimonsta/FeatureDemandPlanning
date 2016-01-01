﻿using FeatureDemandPlanning.Model;
using FeatureDemandPlanning.Model.Context;
using FeatureDemandPlanning.Model.Empty;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Interfaces;
using FeatureDemandPlanning.Model.Parameters;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureDemandPlanning.DataStore
{
    public class TakeRateDataContext : BaseDataContext, ITakeRateDataContext
    {
        #region "Constructors"

        public TakeRateDataContext(string cdsId) : base(cdsId)
        {
            _documentDataStore = new OXODocDataStore(cdsId);
            _takeRateDataStore = new TakeRateDataStore(cdsId);
            _marketGroupDataStore = new MarketGroupDataStore(cdsId);
        }

        #endregion

        #region "Public Methods"

        public async Task<IEnumerable<MarketGroup>> ListAvailableMarketGroups(TakeRateFilter filter)
        {
            return await Task.FromResult(_marketGroupDataStore.FdpMarketGroupGetMany(filter));
        }
        public async Task<PagedResults<TakeRateSummary>> ListTakeRateDocuments(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpTakeRateHeaderGetManyByUsername(filter));
        }
        public async Task<PagedResults<TakeRateSummary>> ListLatestTakeRateDocuments()
        {
            var documents = await ListTakeRateDocuments(new TakeRateFilter());
            documents.CurrentPage = documents.CurrentPage.Take(2);
            return documents;
        }
        public async Task<IEnumerable<TakeRateStatus>> ListTakeRateStatuses()
        {
            return await Task.FromResult<IEnumerable<TakeRateStatus>>(_takeRateDataStore.FdpTakeRateStatusGetMany());
        }
        public async Task<TakeRateSummary> GetTakeRateDocumentHeader(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.TakeRateDocumentHeaderGet(filter));
        }
        public async Task<ITakeRateDocument> GetTakeRateDocument(TakeRateFilter filter)
        {
            return await Task.FromResult(TakeRateDocument.FromFilter(filter));
        }
        public async Task<TakeRateData> GetTakeRateDocumentData(TakeRateFilter filter)
        {
            if (!IsFilterValidForTakeRateData(filter))
                return new TakeRateData();

            return await Task.FromResult(_takeRateDataStore.TakeRateDataItemList(filter));
        }
        public async Task<TakeRateDataItem> GetDataItem(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.TakeRateDataItemGet(filter));
        }
        public async Task<IEnumerable<TakeRateDataItemNote>> ListDataItemNotes(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.TakeRateDataItemNoteGetMany(filter));
        }
        public async Task<TakeRateDocumentHeader> SaveTakeRateDocumentHeader(TakeRateDocumentHeader headerToSave)
        {
            return await Task.FromResult(_takeRateDataStore.FdpVolumeHeaderSave(headerToSave));
        }
        public async Task<ITakeRateDocument> SaveTakeRateDocument(ITakeRateDocument documentToSave)
        {
            throw new System.NotImplementedException();
        }
        public async Task<TakeRateDataItem> SaveDataItem(TakeRateDataItem dataItemToSave)
        {
            return await Task.FromResult(_takeRateDataStore.TakeRateDataItemSave(dataItemToSave));
        }
        public async Task<ITakeRateDocument> ProcessMappedData(ITakeRateDocument documentToProcess)
        {
            throw new System.NotImplementedException();
        }
        public async Task<OXODoc> GetUnderlyingOxoDocument(TakeRateFilter filter)
        {
            OXODoc document = new EmptyOxoDocument();
            if (!filter.OxoDocId.HasValue)
                return document;

            return await Task.FromResult(_documentDataStore.OXODocGet(filter.OxoDocId.Value, filter.ProgrammeId.GetValueOrDefault()));
        }
        public async Task<IEnumerable<SpecialFeature>> ListSpecialFeatures(ProgrammeFilter programmeFilter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpSpecialFeatureTypeGetMany());
        }
        public async Task<IEnumerable<TakeRateDataItem>> CalculateTakeRateAndVolumeByMarket(TakeRateFilter filter, DataChange forChange)
        {
            IEnumerable<TakeRateDataItem> retVal = Enumerable.Empty<TakeRateDataItem>();
            if (filter.Mode == Model.Enumerations.TakeRateResultMode.PercentageTakeRate)
            {
                retVal = await Task.FromResult(_takeRateDataStore.FdpTakeRateByMarketGetMany(filter, forChange.PercentageTakeRate));
            }
            else
            {
                retVal = await Task.FromResult(_takeRateDataStore.FdpVolumeByMarketGetMany(filter, forChange.Volume));
            }
            return retVal;
        }
        public async Task<FdpChangeset> GetUnsavedChangesForUser(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpLatestUnsavedChangesetByUserGetMany(filter));
        }
        public async Task<FdpChangeset> SaveChangeset(TakeRateFilter filter, FdpChangeset changesetToSave)
        {
            var savedChangeset = await Task.FromResult(_takeRateDataStore.FdpChangesetSave(filter, changesetToSave));
            var savedDataChanges = new List<DataChange>();

            foreach (var dataChange in changesetToSave.Changes)
            {
                dataChange.FdpChangesetId = savedChangeset.FdpChangesetId;
                var savedDataChange = await SaveChangesetDataChange(filter, dataChange);
                if (!(savedDataChange is EmptyDataChange))
                {
                    savedDataChanges.Add(savedDataChange);
                }
                await RecalculateChangesetDataChange(savedDataChange);
            }
            savedChangeset.Changes = savedDataChanges;

            return savedChangeset;
        }
        public async Task<DataChange> SaveChangesetDataChange(TakeRateFilter filter, DataChange changeToSave)
        {
            return await Task.FromResult(_takeRateDataStore.FdpChangesetDataItemSave(filter, changeToSave));
        }
        public async Task<DataChange> RecalculateChangesetDataChange(DataChange changeToRecalculate) 
        {
            return await Task.FromResult(_takeRateDataStore.FdpChangesetDataItemRecalculate(changeToRecalculate));
        }
        public async Task<FdpChangeset> PersistChangeset(TakeRateFilter filter)
        {
            var changeset = await GetUnsavedChangesForUser(filter);
            return await Task.FromResult(_takeRateDataStore.FdpChangesetPersist(filter, changeset));
        }
        public async Task<FdpChangeset> RevertUnsavedChangesForUser(TakeRateFilter takeRateFilter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpChangesetRevert(takeRateFilter));
        }
        public async Task<int> GetVolumeForModel(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpVolumeByMarketAndModelGet(filter));
        }
        public async Task<int> GetVolumeForMarket(TakeRateFilter filter)
        {
            return await Task.FromResult(_takeRateDataStore.FdpVolumeByMarketGet(filter));
        }

        #endregion

        #region "Private Methods"

        private bool IsDocumentForVehicle(OXODoc documentToCheck, IVehicle vehicle)
        {
            return (!vehicle.ProgrammeId.HasValue || documentToCheck.ProgrammeId == vehicle.ProgrammeId.Value) &&
                (string.IsNullOrEmpty(vehicle.Gateway) || documentToCheck.Gateway == vehicle.Gateway);
        }
        private bool IsFilterValidForTakeRateData(TakeRateFilter filter)
        {
            //return filter.OxoDocId.HasValue && (filter.MarketId.HasValue || filter.MarketGroupId.HasValue);
            return filter.OxoDocId.HasValue;
        }

        #endregion

        #region "Private Members"

        private readonly OXODocDataStore _documentDataStore;
        private readonly TakeRateDataStore _takeRateDataStore;
        private readonly MarketGroupDataStore _marketGroupDataStore;

        #endregion
    }
}
