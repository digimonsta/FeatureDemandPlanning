﻿using FeatureDemandPlanning.BusinessObjects;
using FeatureDemandPlanning.BusinessObjects.Filters;
using FeatureDemandPlanning.Interfaces;
using FeatureDemandPlanning.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FeatureDemandPlanning.DataStore
{
    public class VolumeDataContext : BaseDataContext, IVolumeDataContext
    {
        public VolumeDataContext(string cdsId) : base(cdsId)
        {
            _vehicleDataStore = new VehicleDataStore(cdsId);
            _documentDataStore = new OXODocDataStore(cdsId);
            _volumeDataStore = new FdpVolumeDataStore(cdsId);
            _modelDataStore = new ModelDataStore(cdsId);
        }
        public IVolume GetVolume(VolumeFilter filter)
        {
            return Volume.FromFilter(filter);
        }
        public void ProcessMappedData(IVolume volumeToProcess)
        {

        }
        public void SaveVolume(IVolume volumeToSave)
        {
            foreach(var header in volumeToSave.FdpVolumeHeaders) {

                var fdpOxoDoc = new FdpOxoDoc() {
                    Header = header,
                    Document = volumeToSave.Document
                };
                _volumeDataStore.FdpOxoDocSave(fdpOxoDoc);
            }
        }
        public FdpVolumeHeader GetVolumeHeader(VolumeFilter filter)
        {
            if (filter.FdpVolumeHeaderId.GetValueOrDefault() == 0)
                return new EmptyVolumeHeader();

            return _volumeDataStore.FdpVolumeHeaderGet(filter.FdpVolumeHeaderId.Value);
        }
        public IEnumerable<FdpVolumeHeader> ListVolumeHeaders(VolumeFilter filter)
        {
            if (!filter.OxoDocId.HasValue)
                return Enumerable.Empty<FdpVolumeHeader>();

            return _volumeDataStore.FdpVolumeHeaderGetManyByOxoDocIdAndUsername(filter);
        }
        public void SaveVolumeHeader(FdpVolumeHeader header)
        {
            _volumeDataStore.FdpVolumeHeaderSave(header);
        }
        public OXODoc GetOxoDocument(VolumeFilter filter)
        {
            if (!filter.ProgrammeId.HasValue || filter.OxoDocId.GetValueOrDefault() == 0)
                return new EmptyOxoDocument();

            return _documentDataStore.OXODocGet(filter.OxoDocId.Value, filter.ProgrammeId.Value);
        }
        public IEnumerable<OXODoc> ListAvailableOxoDocuments(VehicleFilter filter)
        {
            return _documentDataStore
                        .OXODocGetManyByUser(this.CDSID)
                        .Where(d => IsDocumentForVehicle(d, VehicleFilter.ToVehicle(filter)))
                        .Distinct(new OXODocComparer());
        }
        public VolumeData ListVolumeData(VolumeFilter filter) 
        {
            if (!IsFilterValidForVolumeData(filter))
                return new VolumeData();

            return _volumeDataStore.FdpOxoVolumeDataItemList(filter);
        }
        public FdpOxoVolumeDataItem GetData(FdpOxoVolumeDataItem forData)
        {
            if (!forData.FdpOxoVolumeDataItemId.HasValue)
                return null;
            
            var dataItem = _volumeDataStore.FdpOxoVolumeDataItemGet(forData.FdpOxoVolumeDataItemId.Value);
            dataItem.History = _volumeDataStore.FdpOxoVolumeDataItemHistoryGetMany(forData.FdpOxoVolumeDataItemId.Value);
            dataItem.Notes = _volumeDataStore.FdpOxoVolumeDataItemNoteGetMany(forData.FdpOxoVolumeDataItemId.Value).ToList();

            return dataItem;
        }
        public void SaveData(FdpOxoVolumeDataItem dataItemToSave)
        {
            _volumeDataStore.FdpOxoVolumeDataItemSave(dataItemToSave);

            foreach (var note in dataItemToSave.Notes)
            {
                if (!note.FdpOxoVolumeDataItemNoteId.HasValue)
                {
                    _volumeDataStore.FdpOxoVolumeDataItemNoteSave(note, dataItemToSave);
                }
            }
        }
        public IEnumerable<FdpOxoVolumeDataItemHistory> ListHistory(FdpOxoVolumeDataItem forData)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<FdpOxoVolumeDataItemNote> ListNotes(FdpOxoVolumeDataItem forData)
        {
            throw new System.NotImplementedException();
        }
        private bool IsDocumentForVehicle(OXODoc documentToCheck, IVehicle vehicle)
        {
            return (!vehicle.ProgrammeId.HasValue || documentToCheck.ProgrammeId == vehicle.ProgrammeId.Value) &&
                (string.IsNullOrEmpty(vehicle.Gateway) || documentToCheck.Gateway == vehicle.Gateway);
        }
        private bool IsFilterValidForVolumeData(VolumeFilter filter)
        {
            //return filter.OxoDocId.HasValue && (filter.MarketId.HasValue || filter.MarketGroupId.HasValue);
            return filter.OxoDocId.HasValue;
        }

        private VehicleDataStore _vehicleDataStore = null;
        private ModelDataStore _modelDataStore = null;
        private OXODocDataStore _documentDataStore = null;
        private FdpVolumeDataStore _volumeDataStore = null;
    }
}