﻿using FeatureDemandPlanning.Model;
using FeatureDemandPlanning.Model.Context;
using FeatureDemandPlanning.Model.Empty;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureDemandPlanning.DataStore
{
    public class VehicleDataContext : BaseDataContext, IVehicleDataContext
    {
        public VehicleDataContext(string cdsId) : base(cdsId)
        {
            _vehicleDataStore = new VehicleDataStore(cdsId);
            _documentDataStore = new OXODocDataStore(cdsId);
            _programmeDataStore = new ProgrammeDataStore(cdsId);
            _volumeDataStore = new TakeRateDataStore(cdsId);
            _modelDataStore = new ModelDataStore(cdsId);
            _marketDataStore = new MarketDataStore(cdsId);
            _marketGroupDataStore = new MarketGroupDataStore(cdsId);
            _bodyDataStore = new ModelBodyDataStore(cdsId);
            _trimDataStore = new ModelTrimDataStore(cdsId);
            _engineDataStore = new ModelEngineDataStore(cdsId);
            _transmissionDataStore = new ModelTransmissionDataStore(cdsId);
            _vehicleDataStore = new VehicleDataStore(cdsId);
            _featureDataStore = new FeatureDataStore(cdsId);
            _derivativeDataStore = new DerivativeDataStore(cdsId);
        }
        public async Task<IVehicle> GetVehicle(VehicleFilter filter)
        {
            IVehicle vehicle = new EmptyVehicle();

            if (string.IsNullOrEmpty(filter.Code) && !filter.ProgrammeId.HasValue)
            {
                return vehicle;
            }
                
            var programme = _programmeDataStore.ProgrammeGetConfiguration(filter.ProgrammeId.Value);
            if (programme == null)
                return vehicle;

            var availableDocuments = await ListAvailableOxoDocuments(filter);
            var availableImports = await ListAvailableImports(filter, programme);
            var availableModels = await ListAvailableModels(filter);
            var availableMarketGroups = await ListAvailableMarketGroups(filter, programme);
            //var availableMarkets = ListAvailableMarkets(filter, programme);

            vehicle = HydrateVehicleFromProgramme(programme);
            vehicle.AvailableDocuments = availableDocuments;
            vehicle.AvailableImports = availableImports;
            vehicle.AvailableModels = availableModels;
            vehicle.AvailableMarketGroups = availableMarketGroups;
            //vehicle.AvailableMarkets = availableMarkets;
            vehicle.Gateway = !string.IsNullOrEmpty(filter.Gateway) ? filter.Gateway : vehicle.Gateway;
            vehicle.ModelYear = !string.IsNullOrEmpty(filter.ModelYear) ? filter.ModelYear : vehicle.ModelYear;

            return vehicle;
        }
        public async Task<IVehicle> GetVehicle(ProgrammeFilter filter)
        {
            var vehicleFilter = new VehicleFilter()
            {
                ProgrammeId = filter.ProgrammeId,
                Code = filter.Code
            };
            return await GetVehicle(vehicleFilter);
        }
        public async Task<IEnumerable<OXODoc>> ListAvailableOxoDocuments(VehicleFilter filter)
        {
            return await Task.FromResult(_documentDataStore
                        .OXODocGetManyByUser(this.CDSID)
                        .Where(d => IsDocumentForVehicle(d, VehicleFilter.ToVehicle(filter)))
                        .Distinct(new OXODocComparer()));
        }
        public async Task<IEnumerable<TakeRateSummary>> ListAvailableImports(VehicleFilter filter, Programme forProgramme)
        {
            var imports = await Task.FromResult(_volumeDataStore
                            .FdpTakeRateHeaderGetManyByUsername(new TakeRateFilter()
                            {
                                ProgrammeId = filter.ProgrammeId,
                                Gateway = filter.Gateway
                            })
                            .CurrentPage
                            .ToList());

            foreach (var import in imports) {
                //import.Vehicle = (Vehicle)HydrateVehicleFromProgramme(forProgramme);
                //import.Vehicle.Gateway = import.Gateway;
            }

            return imports;
        }

        public async Task<IEnumerable<FdpModel>> ListAvailableModels(ProgrammeFilter filter)
        {
            var models = await Task.FromResult(_modelDataStore
                            .ModelGetMany(filter)
                            .ToList());

            return models;
        }
        public async Task<IEnumerable<MarketGroup>> ListAvailableMarketGroups(VehicleFilter filter, Programme forProgramme)
        {
            var marketGroups = Enumerable.Empty<MarketGroup>();
            if (filter.DocumentId.HasValue) {
                marketGroups =  await Task.FromResult(_marketGroupDataStore
                                   .MarketGroupGetMany(forProgramme.Id, filter.DocumentId.Value, true));
            } else {
                marketGroups = await Task.FromResult(_marketGroupDataStore
                                   .MarketGroupGetMany(true));
            }
            return marketGroups;
        }
        public async Task<IEnumerable<string>> ListAvailableMakes()
        {
            var programmes = await Task.FromResult(_programmeDataStore.ProgrammeGetMany());
            if (programmes == null || !programmes.Any())
            {
                return Enumerable.Empty<string>();
            }

            return programmes.Select(p => p.VehicleMake).Distinct().OrderBy(p => p);
        }

        //public PagedResults<EngineCodeMapping> ListEngineCodeMappings(EngineCodeFilter filter)
        //{
        //    var results = new PagedResults<EngineCodeMapping>();

        //    var engineCodeMappings = _programmeDataStore.EngineCodeMappingGetMany();
        //    if (engineCodeMappings == null || !engineCodeMappings.Any())
        //        return results;

        //    // Filter the results 
        //    // TODO, get this in the database as parameters

        //    engineCodeMappings = engineCodeMappings
        //        .Where(e => !filter.ProgrammeId.HasValue || e.Id == filter.ProgrammeId.Value)
        //        .Where(e => !filter.VehicleId.HasValue || e.VehicleId == filter.VehicleId.Value)
        //        .Where(e => string.IsNullOrEmpty(filter.Code) || e.VehicleName.Equals(filter.Code, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Make) || e.VehicleMake.Equals(filter.Make, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.ModelYear) || e.ModelYear.Equals(filter.ModelYear, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Gateway) || e.Gateway.Equals(filter.Gateway, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.DerivativeCode) || (string.IsNullOrEmpty(e.ExternalEngineCode) ? string.Empty : e.ExternalEngineCode.ToUpper()).Contains(filter.DerivativeCode.ToUpper()))
        //        .Where(e => !filter.EngineId.HasValue || e.EngineId == filter.EngineId.Value)
        //        .Where(e => string.IsNullOrEmpty(filter.EngineSize) || e.EngineSize.Equals(filter.EngineSize, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Cylinder) || e.Cylinder.Equals(filter.Cylinder, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Fuel) || e.Fuel.Equals(filter.Fuel, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Power) || e.Power.Equals(filter.Power, StringComparison.InvariantCultureIgnoreCase))
        //        .Where(e => string.IsNullOrEmpty(filter.Electrification) || e.Electrification.Equals(filter.Electrification, StringComparison.InvariantCultureIgnoreCase));

        //    results.TotalRecords = engineCodeMappings.Count();

        //    if (filter.PageIndex.HasValue && filter.PageSize.HasValue)
        //    {
        //        results.CurrentPage = engineCodeMappings.Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value).Take(filter.PageSize.Value);
        //        results.PageIndex = filter.PageIndex.Value;
        //        results.PageSize = filter.PageSize.Value;
        //    }
        //    else
        //    {
        //        results.CurrentPage = engineCodeMappings;
        //    }

        //    return results;
        //}

        public IEnumerable<ModelBody> ListBodies(ProgrammeFilter filter)
        {
            return _bodyDataStore.ModelBodyGetMany(filter.ProgrammeId.GetValueOrDefault());
        }
        public IEnumerable<Derivative> ListDerivatives(DerivativeFilter filter)
        {
            var derivatives =_derivativeDataStore.DerivativeGetMany(filter);
            foreach (var derivative in derivatives)
            {
                if (derivative.BodyId.HasValue)
                    derivative.Body = _bodyDataStore.ModelBodyGet(derivative.BodyId.Value);

                if (derivative.EngineId.HasValue)
                    derivative.Engine = _engineDataStore.ModelEngineGet(derivative.EngineId.Value);

                if (derivative.TransmissionId.HasValue)
                    derivative.Transmission = _transmissionDataStore.ModelTransmissionGet(derivative.TransmissionId.Value);
            }
            return derivatives;
        }
        public IEnumerable<Gateway> ListGateways(ProgrammeFilter filter)
        {
            return _documentDataStore.GatewayGetMany().Where(g => string.IsNullOrEmpty(filter.Code) || g.VehicleName == filter.Code);
        }
        public IEnumerable<OXODoc> ListPublishedDocuments(ProgrammeFilter filter)
        {
            return _documentDataStore.FdpOxoDocumentsGetMany(filter);
        }
        public IEnumerable<ModelTransmission> ListTransmissions(ProgrammeFilter filter)
        {
            return _transmissionDataStore.ModelTransmissionGetMany(filter.ProgrammeId.GetValueOrDefault());   
        }
        public IEnumerable<ModelEngine> ListEngines(ProgrammeFilter filter)
        {
            return _engineDataStore.ModelEngineGetMany(filter.ProgrammeId.GetValueOrDefault());
        }
        public IEnumerable<FdpTrimMapping> ListTrim(TrimFilter filter)
        {
            return _trimDataStore.ModelTrimGetMany(filter);
        }
        public async Task<IEnumerable<Feature>> ListFeatures(ProgrammeFilter filter)
        {
            return await Task.FromResult(_featureDataStore.FeatureGetMany("fdp", paramId: filter.VehicleId.Value));
        }
        public async Task<IEnumerable<FdpFeature>> ListFeatures(FeatureFilter filter)
        {
            return await Task.FromResult(_featureDataStore.FeatureGetManyByDocumentId(filter));
        }
        public IEnumerable<FeatureGroup> ListFeatureGroups(ProgrammeFilter filter)
        {
            return _featureDataStore.FeatureGroupGetMany();
        }
        public EngineCodeMapping UpdateEngineCodeMapping(EngineCodeMapping mapping)
        {
            return _programmeDataStore.EngineCodeMappingSave(mapping);
        }
        public Programme GetProgramme(ProgrammeFilter filter)
        {
            var programmes = ListProgrammes(filter);
            if (!programmes.Any())
                return new EmptyProgramme();

            return programmes.First();
        }
        public IEnumerable<Programme> ListProgrammes(ProgrammeFilter filter)
        {
            var programmes = _programmeDataStore.ProgrammeByGatewayGetMany();
            if (programmes == null || !programmes.Any())
                return Enumerable.Empty<Programme>();

            programmes = programmes
                .Where(p => !filter.ProgrammeId.HasValue || p.Id == filter.ProgrammeId.Value)
                .Where(p => !filter.VehicleId.HasValue || p.VehicleId == filter.VehicleId.Value)
                .Where(p => string.IsNullOrEmpty(filter.Code) || p.VehicleName.Equals(filter.Code, StringComparison.InvariantCultureIgnoreCase))
                .Where(p => string.IsNullOrEmpty(filter.Make) || p.VehicleMake.Equals(filter.Make, StringComparison.InvariantCultureIgnoreCase))
                .Where(p => string.IsNullOrEmpty(filter.ModelYear) || p.ModelYear.Equals(filter.ModelYear, StringComparison.InvariantCultureIgnoreCase))
                .Where(p => string.IsNullOrEmpty(filter.Gateway) || p.Gateway.Equals(filter.Gateway, StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p);

            return programmes;
        }

        // Derivatives and mappings
        
        public async Task<FdpDerivative> DeleteFdpDerivative(FdpDerivative derivativeToDelete)
        {
            return await Task.FromResult<FdpDerivative>(_derivativeDataStore.FdpDerivativeDelete(derivativeToDelete));
        }
        public async Task<FdpDerivative> GetFdpDerivative(DerivativeFilter filter)
        {
            return await Task.FromResult<FdpDerivative>(_derivativeDataStore.FdpDerivativeGet(filter));
        }
        public async Task<PagedResults<FdpDerivative>> ListFdpDerivatives(DerivativeFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpDerivative>>(_derivativeDataStore.FdpDerivativeGetMany(filter));
        }
        public async Task<FdpDerivativeMapping> DeleteFdpDerivativeMapping(FdpDerivativeMapping derivativeMappingToDelete)
        {
            return await Task.FromResult<FdpDerivativeMapping>(_derivativeDataStore.FdpDerivativeMappingDelete(derivativeMappingToDelete));
        }
        public async Task<FdpDerivativeMapping> GetFdpDerivativeMapping(DerivativeMappingFilter filter)
        {
            return await Task.FromResult<FdpDerivativeMapping>(_derivativeDataStore.FdpDerivativeMappingGet(filter));
        }
        public async Task<PagedResults<FdpDerivativeMapping>> ListFdpDerivativeMappings(DerivativeMappingFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpDerivativeMapping>>(_derivativeDataStore.FdpDerivativeMappingGetMany(filter));
        }
        public async Task<FdpDerivativeMapping> CopyFdpDerivativeMappingToGateway(FdpDerivativeMapping fdpDerivativeMapping, IEnumerable<string> gateways)
        {
            return await Task.FromResult<FdpDerivativeMapping>(_derivativeDataStore.FdpDerivativeMappingCopy(fdpDerivativeMapping, gateways));
        }
        public Task<FdpDerivativeMapping> CopyFdpDerivativeMappingsToGateway(FdpDerivativeMapping fdpDerivativeMapping, IEnumerable<string> gateways)
        {
            throw new NotImplementedException();
        }

        // Features and mappings

        public async Task<FdpFeature> DeleteFdpFeature(FdpFeature featureToDelete)
        {
            return await Task.FromResult<FdpFeature>(_featureDataStore.FdpFeatureDelete(featureToDelete));
        }
        public async Task<FdpFeature> GetFdpFeature(FeatureFilter filter)
        {
            return await Task.FromResult<FdpFeature>(_featureDataStore.FdpFeatureGet(filter));
        }
        public async Task<PagedResults<FdpFeature>> ListFdpFeatures(FeatureFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpFeature>>(_featureDataStore.FdpFeatureGetMany(filter));
        }
        public async Task<FdpFeatureMapping> DeleteFdpFeatureMapping(FdpFeatureMapping featureMappingToDelete)
        {
            return await Task.FromResult<FdpFeatureMapping>(_featureDataStore.FdpFeatureMappingDelete(featureMappingToDelete));
        }
        public async Task<FdpFeatureMapping> GetFdpFeatureMapping(FeatureMappingFilter filter)
        {
            return await Task.FromResult<FdpFeatureMapping>(_featureDataStore.FdpFeatureMappingGet(filter));
        }
        public async Task<PagedResults<FdpFeatureMapping>> ListFdpFeatureMappings(FeatureMappingFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpFeatureMapping>>(_featureDataStore.FdpFeatureMappingGetMany(filter));
        }
        public async Task<FdpFeatureMapping> CopyFdpFeatureMappingToGateway(FdpFeatureMapping fdpFeatureMapping, IEnumerable<string> gateways)
        {
            return await Task.FromResult<FdpFeatureMapping>(_featureDataStore.FdpFeatureMappingCopy(fdpFeatureMapping, gateways));
        }
        public Task<FdpFeatureMapping> CopyFdpFeatureMappingsToGateway(FdpFeatureMapping fdpFeatureMapping, IEnumerable<string> gateways)
        {
            throw new NotImplementedException();
        }
        public async Task<FdpSpecialFeatureMapping> DeleteFdpSpecialFeatureMapping(FdpSpecialFeatureMapping featureMappingToDelete)
        {
            return await Task.FromResult<FdpSpecialFeatureMapping>(_featureDataStore.FdpSpecialFeatureMappingDelete(featureMappingToDelete));
        }
        public async Task<FdpSpecialFeatureMapping> GetFdpSpecialFeatureMapping(SpecialFeatureMappingFilter filter)
        {
            return await Task.FromResult<FdpSpecialFeatureMapping>(_featureDataStore.FdpSpecialFeatureMappingGet(filter));
        }
        public async Task<PagedResults<FdpSpecialFeatureMapping>> ListFdpSpecialFeatureMappings(SpecialFeatureMappingFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpSpecialFeatureMapping>>(_featureDataStore.FdpSpecialFeatureMappingGetMany(filter));
        }
        public async Task<FdpSpecialFeatureMapping> CopyFdpSpecialFeatureMappingToGateway(FdpSpecialFeatureMapping fdpSpecialFeatureMapping, IEnumerable<string> gateways)
        {
            return await Task.FromResult<FdpSpecialFeatureMapping>(_featureDataStore.FdpSpecialFeatureMappingCopy(fdpSpecialFeatureMapping, gateways));
        }
        public Task<FdpSpecialFeatureMapping> CopyFdpSpecialFeatureMappingsToGateway(FdpSpecialFeatureMapping fdpSpecialFeatureMapping, IEnumerable<string> gateways)
        {
            throw new NotImplementedException();
        }

        // Trim and mappings

        public async Task<FdpTrim> DeleteFdpTrim(FdpTrim trimToDelete)
        {
            return await Task.FromResult<FdpTrim>(_trimDataStore.FdpTrimDelete(trimToDelete));
        }
        public async Task<FdpTrim> GetFdpTrim(TrimFilter filter)
        {
            return await Task.FromResult<FdpTrim>(_trimDataStore.FdpTrimGet(filter));
        }
        public async Task<PagedResults<FdpTrim>> ListFdpTrims(TrimFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpTrim>>(_trimDataStore.FdpTrimGetMany(filter));
        }
        public async Task<FdpTrimMapping> DeleteFdpTrimMapping(FdpTrimMapping trimMappingToDelete)
        {
            return await Task.FromResult<FdpTrimMapping>(_trimDataStore.FdpTrimMappingDelete(trimMappingToDelete));
        }
        public async Task<FdpTrimMapping> GetFdpTrimMapping(TrimMappingFilter filter)
        {
            return await Task.FromResult<FdpTrimMapping>(_trimDataStore.FdpTrimMappingGet(filter));
        }
        public async Task<PagedResults<FdpTrimMapping>> ListFdpTrimMappings(TrimMappingFilter filter)
        {
            return await Task.FromResult<PagedResults<FdpTrimMapping>>(_trimDataStore.FdpTrimMappingGetMany(filter));
        }
        public Task<FdpTrimMapping> CopyFdpTrimMappingToGateway(FdpTrimMapping fdpTrimMapping, IEnumerable<string> gateways)
        {
            throw new NotImplementedException();
        }
        public Task<FdpTrimMapping> CopyFdpTrimMappingsToGateway(FdpTrimMapping fdpTrimMapping, IEnumerable<string> gateways)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<IVehicle> ListAvailableVehicles(VehicleFilter filter)
        {
            var programmes = ListProgrammes(filter);
            if (programmes == null || !programmes.Any())
                return Enumerable.Empty<IVehicle>();

            return programmes.Select(p => HydrateVehicleFromProgramme(p, filter.VehicleIndex));
        }
        public IEnumerable<TrimLevel> ListTrimLevels(ProgrammeFilter programmeFilter)
        {
            for (var i = 1; i <= 10; i++)
            {
                yield return new TrimLevel() {
                    Level = i,
                    DisplayOrder = i,
                    Description = string.Format("TL{0}", i)
                };
            }
        }

        #region "Private Methods"

        private IVehicle HydrateVehicleFromProgramme(Programme programme)
        {
            return HydrateVehicleFromProgramme(programme, null);
        }

        private IVehicle HydrateVehicleFromProgramme(Programme programme, int? vehicleIndex)
        {
            if (programme == null)
                return new EmptyVehicle();

            return new Vehicle()
            {
                Make = programme.VehicleMake,
                Code = programme.VehicleName,
                ProgrammeId = programme.Id,
                ModelYear = programme.ModelYear,
                Gateway = vehicleIndex.GetValueOrDefault() == 0 ? programme.Gateway : string.Empty,
                Description = string.Format("{0} - {1}", programme.VehicleName, programme.VehicleAKA),
                Programmes = new List<Programme>() { programme }
            };
        }

        private bool IsDocumentForVehicle(OXODoc documentToCheck, IVehicle vehicle)
        {
            return (!vehicle.ProgrammeId.HasValue || documentToCheck.ProgrammeId == vehicle.ProgrammeId.Value) &&
                (string.IsNullOrEmpty(vehicle.Gateway) || documentToCheck.Gateway == vehicle.Gateway);
        }

        #endregion

        #region "Private Members"

        private VehicleDataStore _vehicleDataStore = null;
        private ProgrammeDataStore _programmeDataStore = null;
        private OXODocDataStore _documentDataStore = null;
        private TakeRateDataStore _volumeDataStore = null;
        private ModelDataStore _modelDataStore = null;
        private MarketDataStore _marketDataStore = null;
        private MarketGroupDataStore _marketGroupDataStore = null;
        private ModelBodyDataStore _bodyDataStore = null;
        private ModelTransmissionDataStore _transmissionDataStore = null;
        private ModelTrimDataStore _trimDataStore = null;
        private ModelEngineDataStore _engineDataStore = null;
        private FeatureDataStore _featureDataStore = null;
        private DerivativeDataStore _derivativeDataStore = null;

        #endregion
    }
}
