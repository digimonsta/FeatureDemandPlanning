﻿using FeatureDemandPlanning.Model.Context;
using FeatureDemandPlanning.Model.Empty;
using FeatureDemandPlanning.Model.Enumerations;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using FluentSecurity.Caching;
using log4net.Config;
using Cache = System.Web.Caching.Cache;

namespace FeatureDemandPlanning.Model.ViewModel
{
    public class TakeRateViewModel : SharedModelBase
    {
        #region "Public Properties"

        public TakeRateSummary TakeRate { get; set; }
        public TakeRateDataItem CurrentTakeRateDataItem { get; set; }
        public TakeRateDataItemAction CurrentAction { get; set; }
        public IEnumerable<TakeRateStatus> Statuses { get; set; }
        public PagedResults<TakeRateSummary> TakeRates { get; set; }
        public TakeRateDocument Document { get; set; }
        public FdpChangeset Changes { get; set; }
        public FdpChangesetHistory History { get; set; }
        public MarketReviewStatus MarketReviewStatus { get; set; }
        public string Filter { get; set; }
        public RawTakeRateData RawData { get; set; }

        // Can the take rate file be edited
        public bool AllowEdit
        {
            get
            {
                if (_allowEdit.HasValue)
                {
                    return _allowEdit.Value;
                }

                var programmeId = Document.UnderlyingOxoDocument.ProgrammeId;
                var marketId = Document.Market.Id;

                // User must be allowed to edit the programme / market itself and be in a role that allows for editing
                // In addition, the document cannot have been published
                _allowEdit = CurrentUser.HasEditRole() &&
                       CurrentUser.IsProgrammeEditable(programmeId) &&
                       CurrentUser.IsMarketEditable(marketId) &&
                       !TakeRate.IsPublished();

                return _allowEdit.Value;
            }
        }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="ForecastComparisonViewModel"/> class.
        /// </summary>
        public TakeRateViewModel()
        {
            InitialiseMembers();
        }
        public TakeRateViewModel(SharedModelBase baseModel) : base(baseModel)
        {
            InitialiseMembers();
        }

        #endregion

        #region "Public Methods"

        public static async Task<TakeRateViewModel> GetModel(IDataContext context)
        {
            return await GetModel(context, new TakeRateFilter());
        }
        public static async Task<TakeRateViewModel> GetModel(IDataContext context,
                                                             TakeRateFilter filter,
                                                             TakeRateDataItemAction action)
        {
            var model = await GetModel(context, filter);
            model.CurrentAction = action;
            if (action != TakeRateDataItemAction.NotSet)
            {
                model.IdentifierPrefix = Enum.GetName(action.GetType(), action);
            }

            return model;
        }
        public static async Task<TakeRateViewModel> GetModel(IDataContext context, TakeRateFilter filter)
        {
            TakeRateViewModel model = null;
            
            switch (filter.Action)
            {
                case TakeRateDataItemAction.TakeRates:
                    model = await GetFullAndPartialViewModelForTakeRates(context, filter);
                    break;
                case TakeRateDataItemAction.TakeRateDataPage:
                    model = await GetFullAndPartialViewModelForTakeRateDataPage(context, filter);
                    break;
                case TakeRateDataItemAction.Validate:
                    model = await GetFullAndPartialViewModelForValidation(context, filter);
                    break;
                case TakeRateDataItemAction.TakeRateDataItemDetails:
                case TakeRateDataItemAction.UndoChange:
                case TakeRateDataItemAction.AddNote:
                    model = await GetFullAndPartialViewModelForTakeRateDataItem(context, filter);
                    break;
                case TakeRateDataItemAction.NotSet:
                    break;
                case TakeRateDataItemAction.SaveChanges:
                case TakeRateDataItemAction.History:
                case TakeRateDataItemAction.Filter:
                case TakeRateDataItemAction.MarketReview:
                    model = await GetFullAndPartialViewModelForTakeRateDataPageExcludingData(context, filter);
                    break;
                case TakeRateDataItemAction.Changeset:
                    model = await GetFullAndPartialViewModelForChangeset(context, filter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return model;
        }

        #endregion

        #region "Private Methods"

        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForTakeRateDataPage(IDataContext context, TakeRateFilter filter)
        {
            var watch = Stopwatch.StartNew();

            var modelBase = GetBaseModel(context);
            var takeRateModel = new TakeRateViewModel(modelBase)
            {
                Document = (TakeRateDocument) TakeRateDocument.FromFilter(filter), Configuration = context.ConfigurationSettings
            };

            await HydrateOxoDocument(context, takeRateModel);
            await HydrateFdpVolumeHeader(context, takeRateModel);
            //await HydrateFdpVolumeHeadersFromOxoDocument(context, takeRateModel);
            await HydrateVehicle(context, takeRateModel);
            await HydrateMarket(context, takeRateModel);
            await HydrateMarketGroup(context, takeRateModel);
            await HydrateModelsByMarket(context, takeRateModel);
            await HydrateDerivativesByMarket(context, takeRateModel);
            await HydrateData(context, takeRateModel);
            await HydrateMarketReview(context, takeRateModel);

            watch.Stop();
            Log.Debug("GetFullAndPartialViewModelForTakeRateDataPage : " + watch.ElapsedMilliseconds);
          
            return takeRateModel;
        }
        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForValidation(IDataContext context, TakeRateFilter filter)
        {
            var watch = Stopwatch.StartNew();

            var modelBase = GetBaseModel(context);
            var takeRateModel = new TakeRateViewModel(modelBase)
            {
                Document = (TakeRateDocument)TakeRateDocument.FromFilter(filter),
                Configuration = context.ConfigurationSettings
            };

            await HydrateFdpVolumeHeader(context, takeRateModel);
            HydrateRawData(context, takeRateModel);

            watch.Stop();
            Log.Debug("GetFullAndPartialViewModelForTakeRateDataPage : " + watch.ElapsedMilliseconds);

            return takeRateModel;
        }
        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForTakeRateDataPageExcludingData(IDataContext context, TakeRateFilter filter)
        {
            var modelBase = GetBaseModel(context);
            var takeRateModel = new TakeRateViewModel(modelBase)
            {
                Document = (TakeRateDocument)TakeRateDocument.FromFilter(filter),
                Configuration = context.ConfigurationSettings,
                MarketReviewStatus = filter.MarketReviewStatus,
                Filter = filter.Filter
            };

            await HydrateOxoDocument(context, takeRateModel);
            await HydrateFdpVolumeHeader(context, takeRateModel);
            //await HydrateFdpVolumeHeadersFromOxoDocument(context, takeRateModel);
            await HydrateVehicle(context, takeRateModel);
            await HydrateMarket(context, takeRateModel);
            await HydrateMarketGroup(context, takeRateModel);
            await HydrateModelsByMarket(context, takeRateModel);
            await HydrateDerivativesByMarket(context, takeRateModel);
            await HydrateMarketReview(context, takeRateModel);

            return takeRateModel;
        }
        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForChangeset(IDataContext context, TakeRateFilter filter)
        {
            var modelBase = GetBaseModel(context);
            var takeRateModel = new TakeRateViewModel(modelBase)
            {
                Document = (TakeRateDocument)TakeRateDocument.FromFilter(filter),
                Configuration = context.ConfigurationSettings
            };
            await HydrateOxoDocument(context, takeRateModel);
            await HydrateFdpVolumeHeaders(context, takeRateModel);
            //await HydrateFdpVolumeHeadersFromOxoDocument(context, takeRateModel);
            
            return takeRateModel;
        }
        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForTakeRateDataItem(IDataContext context, TakeRateFilter filter)
        {
            var takeRateModel = new TakeRateViewModel(GetBaseModel(context))
            {
                Document = (TakeRateDocument) TakeRateDocument.FromFilter(filter), 
                Configuration = context.ConfigurationSettings, 
                CurrentTakeRateDataItem = await context.TakeRate.GetDataItem(filter)
            };

            await HydrateOxoDocument(context, takeRateModel);
            await HydrateVehicle(context, takeRateModel);
            await HydrateMarket(context, takeRateModel);
            await HydrateMarketGroup(context, takeRateModel);
            await HydrateDerivativesByMarket(context, takeRateModel);
            await HydrateFeatures(context, takeRateModel);
            await HydrateCurrentModel(context, takeRateModel);
            await HydrateCurrentFeature(context, takeRateModel);

            return takeRateModel;
        }
        private static async Task<TakeRateViewModel> GetFullAndPartialViewModelForTakeRates(IDataContext context, TakeRateFilter filter)
        {
            var model = new TakeRateViewModel(GetBaseModel(context))
            {
                Configuration = context.ConfigurationSettings, TakeRates = await context.TakeRate.ListTakeRateDocuments(filter), Statuses = await context.TakeRate.ListTakeRateStatuses()
            };

            return model;
        }
        private static async Task<IVehicle> GetVehicle(IDataContext context, Vehicle forVehicle, OXODoc forDocument)
        {
            // Do not deep get all vehicle details such as markets, derivatives, etc, as these are populated elsewhere
            return await context.Vehicle.GetVehicle(new VehicleFilter()
            {
                ProgrammeId = forVehicle.ProgrammeId, Gateway = forVehicle.Gateway, DocumentId = forDocument.Id, Deep = false
            });
        }
        private static async Task<Market> GetMarket(IDataContext context, TakeRateDocument forTakeRateDocument)
        {
            Market market;
            var cacheKey = string.Format("Market_{0}_{1}",
                forTakeRateDocument.TakeRateId,
                forTakeRateDocument.Market.Id);

            var cachedLookup = GetCache(cacheKey);
            if (cachedLookup != null)
            {
                market = (Market) cachedLookup;
            }
            else
            {
                market = await Task.FromResult(context.Market.GetMarket(new TakeRateFilter()
                {
                    TakeRateId = forTakeRateDocument.TakeRateId,
                    MarketId = forTakeRateDocument.Market.Id
                }));

                if (!(market is EmptyMarket) && market.Id != 0)
                    AddCache(cacheKey, market);
            }
            return market;
        }

        private static async Task<MarketReview> GetMarketReview(IDataContext context,TakeRateDocument forTakeRateDocument)
        {
            return await context.TakeRate.GetMarketReview(new TakeRateFilter
            {
                TakeRateId = forTakeRateDocument.TakeRateId,
                MarketId = forTakeRateDocument.Market.Id
            });
        }
        private static async Task<MarketGroup> GetMarketGroup(IDataContext context, TakeRateDocument forTakeRateDocument)
        {
            MarketGroup marketGroup;
            var cacheKey = string.Format("MarketGroup_{0}_{1}", 
                forTakeRateDocument.TakeRateId, 
                forTakeRateDocument.MarketGroup.Id);

            var cachedLookup = GetCache(cacheKey);
            if (cachedLookup != null)
            {
                marketGroup = (MarketGroup) cachedLookup;
            }
            else
            {
                marketGroup = await Task.FromResult(context.Market.GetMarketGroup(new TakeRateFilter() 
                { 
                    TakeRateId = forTakeRateDocument.TakeRateId, 
                    MarketGroupId = forTakeRateDocument.MarketGroup.Id
                }));

                if (!(marketGroup is EmptyMarketGroup) && marketGroup.Id != 0)
                    AddCache(cacheKey, marketGroup);
            }
            return marketGroup;
        }

        private static async Task<OXODoc> GetOxoDocument(IDataContext context, OXODoc forOxoDocument)
        {
            OXODoc oxoDocument;
            var cacheKey = string.Format("OxoDocument_{0}", forOxoDocument.Id);
            var cachedLookup = GetCache(cacheKey);
            if (cachedLookup != null)
            {
                oxoDocument = (OXODoc) cachedLookup;
            }
            else
            {
                oxoDocument = await context.TakeRate.GetUnderlyingOxoDocument(new TakeRateFilter() {DocumentId = forOxoDocument.Id, ProgrammeId = forOxoDocument.ProgrammeId});
                AddCache(cacheKey, oxoDocument);
            }
            return oxoDocument;
        }

        private static async Task<OXODoc> GetOxoDocumentFromTakeRateFile(IDataContext context, TakeRateDocument takeRateFile)
        {
            OXODoc oxoDocument;
            var cacheKey = string.Format("TakeRateFile_{0}", takeRateFile.TakeRateId.GetValueOrDefault());
            var cachedLookup = GetCache(cacheKey);
            if (cachedLookup != null)
            {
                oxoDocument = (OXODoc)cachedLookup;
            }
            else
            {
                oxoDocument = await context.TakeRate.GetUnderlyingOxoDocument(new TakeRateFilter() { TakeRateId = takeRateFile.TakeRateId });
                AddCache(cacheKey, oxoDocument);
            }
            return oxoDocument;
        }

        private static async Task<TakeRateSummary> GetTakeRateDocumentHeader(IDataContext context, TakeRateSummary forHeader)
        {
            var cacheKey = string.Format("FdpVolumeHeader_{0}", forHeader.TakeRateId);
            var cachedLookup = GetCache(cacheKey);
            if (cachedLookup != null)
            {
                forHeader = (TakeRateSummary) cachedLookup;
            }
            else
            {
                forHeader = await context.TakeRate.GetTakeRateDocumentHeader(new TakeRateFilter() {TakeRateId = forHeader.TakeRateId});
                AddCache(cacheKey, forHeader);
            }
            return forHeader;
        }
        private static async Task<RawTakeRateData> GetRawData(IDataContext context, TakeRateViewModel takeRateViewModel)
        {
            return
                    await context.TakeRate.GetRawData(new TakeRateFilter()
                    {
                        TakeRateId = takeRateViewModel.Document.TakeRateId,
                        MarketId = takeRateViewModel.Document.Market.Id
                    });
        }
        private static async Task<Market> HydrateMarket(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            if (volumeModel.Document.Market is EmptyMarket)
                return volumeModel.Document.Market;

            volumeModel.Document.Market = await GetMarket(context, volumeModel.Document);
            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return volumeModel.Document.Market;
        }
        private static async Task<MarketGroup> HydrateMarketGroup(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            if (volumeModel.Document.MarketGroup is EmptyMarketGroup)
                return volumeModel.Document.MarketGroup;

            volumeModel.Document.MarketGroup = await GetMarketGroup(context, volumeModel.Document);
            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return volumeModel.Document.MarketGroup;
        }
        private static async Task<IEnumerable<Model>> HydrateDerivativesByMarket(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            volumeModel.Document.Vehicle.AvailableModels = await ListAvailableModelsFilteredByMarket(context, volumeModel.Document);
            watch.Stop();
           
            Log.Debug(watch.ElapsedMilliseconds);
            return volumeModel.Document.Vehicle.AvailableModels;
        }
        private static async Task<IEnumerable<MarketGroup>> HydrateModelsByMarket(IDataContext context, TakeRateViewModel takeRateModel)
        {
            var watch = Stopwatch.StartNew();
            takeRateModel.Document.Vehicle.AvailableMarketGroups = await ListAvailableMarketGroups(context, takeRateModel.Document);
            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return takeRateModel.Document.Vehicle.AvailableMarketGroups;
        }
        private static async Task<RawTakeRateData> HydrateRawData(IDataContext context,
            TakeRateViewModel takeRateViewModel)
        {
            var watch = Stopwatch.StartNew();
            takeRateViewModel.RawData = await GetRawData(context, takeRateViewModel);
            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return takeRateViewModel.RawData;
        }
        private static async Task<IEnumerable<MarketGroup>> ListAvailableMarketGroups(IDataContext context, ITakeRateDocument document)
        {
            return await context.TakeRate.ListAvailableMarketGroups(new TakeRateFilter()
            {
                DocumentId = document.UnderlyingOxoDocument.Id
            });
        }

        private static void AddCache(string cacheKey, object data)
        {
            if (HttpContext.Current == null)
                return;
            
            HttpContext.Current.Cache.Add(cacheKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }
        private static object GetCache(string cacheKey)
        {
            return HttpContext.Current == null ? null : HttpContext.Current.Cache.Get(cacheKey);
        }

        private static async Task<FdpModel> HydrateCurrentModel(IDataContext context, TakeRateViewModel takeRateModel)
        {
            FdpModel model = new EmptyFdpModel();

            if (takeRateModel.CurrentTakeRateDataItem.ModelId.HasValue)
            {
                model = takeRateModel.Document.Vehicle.AvailableModels.First(m => m.Id == takeRateModel.CurrentTakeRateDataItem.ModelId.Value);
            }
            else if (takeRateModel.CurrentTakeRateDataItem.FdpModelId.HasValue)
            {
                model = takeRateModel.Document.Vehicle.AvailableModels.First(m => m.FdpModelId == takeRateModel.CurrentTakeRateDataItem.FdpModelId.Value);
            }
            takeRateModel.CurrentTakeRateDataItem.Model = model;

            return await Task.FromResult(model);
        }
        private static async Task<FdpFeature> HydrateCurrentFeature(IDataContext context, TakeRateViewModel takeRateModel)
        {
            FdpFeature feature = new EmptyFdpFeature();

            if (takeRateModel.CurrentTakeRateDataItem.FeatureId.HasValue)
            {
                feature = takeRateModel.Document.Vehicle.AvailableFeatures.First(f => f.Id == takeRateModel.CurrentTakeRateDataItem.FeatureId.Value);
            }
            else if (takeRateModel.CurrentTakeRateDataItem.FdpFeatureId.HasValue)
            {
                feature = takeRateModel.Document.Vehicle.AvailableFeatures.First(f => f.FdpFeatureId == takeRateModel.CurrentTakeRateDataItem.FdpFeatureId.Value);
            }
            takeRateModel.CurrentTakeRateDataItem.Feature = feature;

            return await Task.FromResult(feature);
        }

        private static async Task<TakeRateSummary> HydrateFdpVolumeHeader(IDataContext context,
            TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            var volumeSummary = new TakeRateSummary {TakeRateId = volumeModel.Document.TakeRateId.GetValueOrDefault()};
            volumeSummary = await GetTakeRateDocumentHeader(context, volumeSummary);

            volumeModel.Document.TakeRateSummary = new List<TakeRateSummary> { volumeSummary };

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);

            return volumeSummary;
        }
        private static async Task<IEnumerable<TakeRateSummary>> HydrateFdpVolumeHeaders(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            var volumeSummary = new List<TakeRateSummary>();
            foreach (var header in volumeModel.Document.TakeRateSummary)
            {
                await GetTakeRateDocumentHeader(context, header);
            }

            volumeModel.Document.TakeRateSummary = volumeSummary;

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);

            return volumeSummary;
        }
        private static async Task HydrateFdpVolumeHeadersFromOxoDocument(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            if (volumeModel.Document.TakeRateSummary.Any())
                return;

            var volumeHeaders = await ListVolumeSummary(context, volumeModel.Document);
            volumeModel.Document.TakeRateSummary = volumeHeaders.CurrentPage;

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
        }
        private static async Task HydrateMarketReview(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            volumeModel.TakeRate.MarketReview = await GetMarketReview(context, volumeModel.Document);

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
        }
        private static async Task<OXODoc> HydrateOxoDocument(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            OXODoc retVal = new EmptyOxoDocument();

            if (!(volumeModel.Document.UnderlyingOxoDocument is EmptyOxoDocument))
            {
                volumeModel.Document.UnderlyingOxoDocument =
                    await GetOxoDocument(context, volumeModel.Document.UnderlyingOxoDocument);
            }
            else
            {
                volumeModel.Document.UnderlyingOxoDocument =
                    await GetOxoDocumentFromTakeRateFile(context, volumeModel.Document);
            }

            volumeModel.Document.Vehicle.ProgrammeId = volumeModel.Document.UnderlyingOxoDocument.ProgrammeId;
            volumeModel.Document.Vehicle.Gateway = volumeModel.Document.UnderlyingOxoDocument.Gateway;

            retVal = volumeModel.Document.UnderlyingOxoDocument;

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);

            return retVal;
        }
        private static async Task<TakeRateData> HydrateData(IDataContext context, TakeRateViewModel takeRateModel)
        {
            var watch = Stopwatch.StartNew();
            takeRateModel.Document.TakeRateData = await ListTakeRateData(context, takeRateModel.Document);

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return takeRateModel.Document.TakeRateData;
        }
        private static async Task<IVehicle> HydrateVehicle(IDataContext context, TakeRateViewModel volumeModel)
        {
            var watch = Stopwatch.StartNew();
            if (!(volumeModel.Document.Vehicle is EmptyVehicle))
                return volumeModel.Document.Vehicle;

            volumeModel.Document.Vehicle = (Vehicle) (await GetVehicle(context, volumeModel.Document.Vehicle, volumeModel.Document.UnderlyingOxoDocument));
            // Set this prior to filtering by market
            volumeModel.Document.TotalDerivatives = volumeModel.Document.Vehicle.AvailableModels.Count();

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);

            return volumeModel.Document.Vehicle;
        }
        private static async Task<IEnumerable<FdpFeature>> HydrateFeatures(IDataContext context, TakeRateViewModel takeRateModel)
        {
            var watch = Stopwatch.StartNew();
            takeRateModel.Document.Vehicle.AvailableFeatures = await ListFeatures(context, takeRateModel.Document);

            watch.Stop();
            Log.Debug(watch.ElapsedMilliseconds);
            return takeRateModel.Document.Vehicle.AvailableFeatures;
        }
        private void InitialiseMembers()
        {
            TakeRate = new EmptyTakeRateSummary();
            TakeRates = new PagedResults<TakeRateSummary>();
            Statuses = Enumerable.Empty<TakeRateStatus>();
            CurrentTakeRateDataItem = new EmptyTakeRateDataItem();
            CurrentAction = TakeRateDataItemAction.NotSet;
            IdentifierPrefix = "Page";
            Document = new EmptyTakeRateDocument();
            MarketReviewStatus = MarketReviewStatus.NotSet;
        }
        private static async Task<IEnumerable<FdpModel>> ListAvailableModelsFilteredByMarket(IDataContext context, TakeRateDocument forVolume)
        {
            var filteredModels = Enumerable.Empty<FdpModel>();

            if (forVolume.UnderlyingOxoDocument is EmptyOxoDocument || forVolume.Vehicle is EmptyVehicle)
                return filteredModels;

            var filter = new TakeRateFilter()
            {
                TakeRateId = forVolume.TakeRateId, 
                ProgrammeId = forVolume.UnderlyingOxoDocument.ProgrammeId,
                Gateway = forVolume.UnderlyingOxoDocument.Gateway, 
                DocumentId = forVolume.UnderlyingOxoDocument.Id
            };

            if (!(forVolume.Market is EmptyMarket))
            {
                filter.MarketId = forVolume.Market.Id;
                filteredModels = (await context.Market.ListAvailableModelsByMarket(filter)).Where(m => m.Available);
            }
            else if (!(forVolume.MarketGroup is EmptyMarketGroup))
            {
                filter.MarketGroupId = forVolume.MarketGroup.Id;
                filteredModels = (await context.Market.ListAvailableModelsByMarketGroup(filter)).Where(m => m.Available);
            }
            else
            {
                filteredModels = forVolume.Vehicle.AvailableModels;
            }

            return filteredModels;
        }
        private static async Task<IEnumerable<FdpFeature>> ListFeatures(IDataContext context, TakeRateDocument forDocument)
        {
            if (forDocument.UnderlyingOxoDocument is EmptyOxoDocument)
                return Enumerable.Empty<FdpFeature>();

            return await context.Vehicle.ListFeatures(FeatureFilter.FromOxoDocumentId(forDocument.UnderlyingOxoDocument.Id));
        }
        private static async Task<TakeRateData> ListTakeRateData(IDataContext context, TakeRateDocument forDocument)
        {
            if (forDocument.UnderlyingOxoDocument is EmptyOxoDocument)
                return new TakeRateData();

            return await context.TakeRate.GetTakeRateDocumentData(TakeRateFilter.FromTakeRateDocument(forDocument));
        }

        //private static async Task<TakeRateSummary> GetVolumeSummary(IDataContext context, TakeRateDocument forVolume)
        //{
        //    if (forVolume.UnderlyingOxoDocument is EmptyOxoDocument)
        //        return new TakeRateSummary();

        //    return await context.TakeRate.GetTakeRateDocument(TakeRateFilter.FromTakeRateDocument(forVolume));
        //}
        private static async Task<PagedResults<TakeRateSummary>> ListVolumeSummary(IDataContext context, TakeRateDocument forVolume)
        {
            if (forVolume.UnderlyingOxoDocument is EmptyOxoDocument)
                return new PagedResults<TakeRateSummary>();

            return await context.TakeRate.ListTakeRateDocuments(TakeRateFilter.FromTakeRateDocument(forVolume));
        }
        #endregion

        #region "Private Members"

        private bool? _allowEdit;

        #endregion
    }
}
