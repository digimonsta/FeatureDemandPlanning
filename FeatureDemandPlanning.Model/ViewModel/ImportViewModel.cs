﻿using FeatureDemandPlanning.Model;
using FeatureDemandPlanning.Model.Context;
using FeatureDemandPlanning.Model.Filters;
using enums = FeatureDemandPlanning.Model.Enumerations;
using FeatureDemandPlanning.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FeatureDemandPlanning.Model.ViewModel
{
    public class ImportViewModel : SharedModelBase
    {
        #region "Public Properties"

        public ImportQueue CurrentImport { get; set; }
        public ImportError CurrentException { get; set; }
        public DerivativeMapping CurrentDerivativeMapping { get; set; }
        public TrimMapping CurrentTrimMapping { get; set; }
        public enums.ImportAction CurrentAction { get; set; }
        public enums.ImportAction CurrentImportAction { get; set; }
        public string CurrentFeatureGroup { get; set; }
        public FeatureGroup CurrentFeatureSubGroup { get; set; }
        public Feature CurrentFeature { get; set; }
        public Market CurrentMarket { get; set; }
        public Programme CurrentProgramme { get; set; }

        public PagedResults<ImportError> Exceptions { get; set; }
        public PagedResults<ImportQueue> ImportQueue { get; set; }

        public Programme Programme { get; set; }
        public string Gateway { get; set; }

        public IEnumerable<Programme> AvailableProgrammes { get; set; }
        public IEnumerable<ModelEngine> AvailableEngines { get; set; }
        public IEnumerable<ModelTransmission> AvailableTransmissions { get; set; }
        public IEnumerable<ModelBody> AvailableBodies { get; set; }
        public IEnumerable<ModelTrim> AvailableTrim { get; set; }
        public IEnumerable<SpecialFeature> AvailableSpecialFeatures { get; set; }
        public IEnumerable<Market> AvailableMarkets { get; set; }
        public IEnumerable<Feature> AvailableFeatures { get; set; }
        public IEnumerable<FeatureGroup> AvailableFeatureGroups { get; set; }
        public IEnumerable<Derivative> AvailableDerivatives { get; set; }
        public IEnumerable<ImportExceptionType> AvailableExceptionTypes { get; set; }
        
        public dynamic Configuration { get; set; }

        #endregion

        #region "Constructors"

        public ImportViewModel(IDataContext dataContext) : base(dataContext)
        {
            Configuration = dataContext.ConfigurationSettings;
            InitialiseMembers();
        }

        #endregion

        #region "Public Members"

        public IEnumerable<CarLine> CarLines
        {
            get
            {
                return AvailableProgrammes.Select(p => new CarLine()
                {
                    VehicleId = p.VehicleId,
                    VehicleName = p.VehicleName,
                    VehicleAKA = p.VehicleAKA
                })
                .Distinct(new CarLineComparer())
                .OrderBy(c => c.VehicleName);
            }
        }

        public IEnumerable<Gateway> Gateways
        {
            get
            {
                return AvailableProgrammes.Select(p => new Gateway()
                    {
                        VehicleName = p.VehicleName,
                        Name = p.Gateway
                    })
                    .Distinct(new GatewayComparer())
                    .OrderBy(p => p.Name);
            }
        }

        public IEnumerable<ModelYear> ModelYears
        {
            get
            {
                return AvailableProgrammes.Select(p => new ModelYear()
                    {
                        VehicleName = p.VehicleName,
                        Name = p.ModelYear
                    })
                    .Distinct(new ModelYearComparer());
            }
        }

        public IEnumerable<string> FeatureGroups
        {
            get
            {
                return AvailableFeatureGroups.Select(g => g.FeatureGroupName).Distinct().OrderBy(g => g);
            }
        }
        public IEnumerable<FeatureGroup> FeatureSubGroups
        {
            get 
            {
                return AvailableFeatureGroups
                    .Where(g => string.IsNullOrEmpty(CurrentFeatureGroup) ||
                        g.FeatureGroupName.Equals(CurrentFeatureGroup, StringComparison.InvariantCultureIgnoreCase))
                    .OrderBy(g => g.FeatureSubGroup);
            }
        }
        public IEnumerable<Feature> Features
        {
            get
            {
                return AvailableFeatures
                    .Where(f => CurrentFeatureSubGroup is EmptyFeatureGroup ||
                        (f.FeatureGroup.Equals(CurrentFeatureSubGroup.FeatureGroupName, StringComparison.InvariantCultureIgnoreCase) &&
                         f.FeatureSubGroup.Equals(CurrentFeatureSubGroup.FeatureSubGroup, StringComparison.InvariantCultureIgnoreCase)))
                    .OrderBy(f => f.BrandDescription);
            }
        }
        public bool HasExceptions()
        {
            return Exceptions != null && Exceptions.CurrentPage.Any();
        }
        public bool HasExceptions(enums.ImportExceptionType ofType)
        {
            return HasExceptions() && Exceptions.CurrentPage.Where(e => e.ErrorType == ofType).Any();
        }
        public static async Task<ImportViewModel> GetModel(IDataContext context)
        {
            return await GetModel(context, new ImportQueueFilter());
        }
        public static async Task<ImportViewModel> GetModel(IDataContext context, 
                                                           ImportQueueFilter filter,
                                                           enums.ImportAction action)
        {
            var model = await GetModel(context, filter);
            model.CurrentAction = action;
            if (action != enums.ImportAction.NotSet)
            {
                model.IdentifierPrefix = Enum.GetName(action.GetType(), action);
            }

            return model;
        }
        public static async Task<ImportViewModel> GetModel(IDataContext context, 
                                                           ImportQueueFilter filter)
        {
            if (filter.ImportQueueId.HasValue)
            {
                return await GetFullAndPartialViewModelForImportQueueItem(context, filter);
            }
            if (filter.ExceptionId.HasValue)
            {
                return await GetFullAndPartialViewModelForException(context, filter);
            }
            return await GetFullAndPartialViewModelForImportQueue(context, filter);
        }

        #endregion

        #region "Private Members"

        private static async Task<ImportViewModel> GetFullAndPartialViewModelForImportQueue(IDataContext context,
                                                                                            ImportQueueFilter filter)
        {
            var model = new ImportViewModel(context)
            {
                PageIndex = filter.PageIndex.HasValue ? filter.PageIndex.Value : 1,
                PageSize = filter.PageSize.HasValue ? filter.PageSize.Value : Int32.MaxValue
            };
            model.ImportQueue = await context.Import.ListImportQueue(filter);
            model.TotalPages = model.ImportQueue.TotalPages;
            model.TotalRecords = model.ImportQueue.TotalRecords;
            model.TotalDisplayRecords = model.ImportQueue.TotalDisplayRecords;

            model.AvailableProgrammes = context.Vehicle.ListProgrammes(new ProgrammeFilter());

            return model;
        }
        private static async Task<ImportViewModel> GetFullAndPartialViewModelForException(IDataContext context,
                                                                                          ImportQueueFilter filter)
        {
            var model = new ImportViewModel(context)
            {
                PageIndex = filter.PageIndex.HasValue ? filter.PageIndex.Value : 1,
                PageSize = filter.PageSize.HasValue ? filter.PageSize.Value : Int32.MaxValue
            };
            model.CurrentException = await context.Import.GetException(filter);

            var programmeFilter = new ProgrammeFilter(model.CurrentException.ProgrammeId);

            model.Programme = context.Vehicle.GetProgramme(programmeFilter);
            programmeFilter.VehicleId = model.Programme.VehicleId;

            model.Gateway = model.CurrentException.Gateway;
            model.AvailableEngines = context.Vehicle.ListEngines(programmeFilter);
            model.AvailableTransmissions = context.Vehicle.ListTransmissions(programmeFilter);
            model.AvailableBodies = context.Vehicle.ListBodies(programmeFilter);
            model.AvailableTrim = context.Vehicle.ListTrim(programmeFilter);
            model.AvailableSpecialFeatures = context.Volume.ListSpecialFeatures(programmeFilter);
            model.AvailableMarkets = context.Market.ListAvailableMarkets(programmeFilter);
            model.AvailableFeatures = context.Vehicle.ListFeatures(programmeFilter);
            model.AvailableFeatureGroups = context.Vehicle.ListFeatureGroups(programmeFilter);
            model.AvailableDerivatives = context.Vehicle.ListDerivatives(programmeFilter);

            return model;
        }
        private static async Task<ImportViewModel> GetFullAndPartialViewModelForImportQueueItem(IDataContext context,
                                                                                                ImportQueueFilter filter)
        {
            var model = new ImportViewModel(context)
            {
                PageIndex = filter.PageIndex.HasValue ? filter.PageIndex.Value : 1,
                PageSize = filter.PageSize.HasValue ? filter.PageSize.Value : Int32.MaxValue
            };
            model.CurrentImport = await context.Import.GetImportQueue(filter);
            model.AvailableExceptionTypes = await context.Import.ListExceptionTypes(filter);

            var programmeFilter = new ProgrammeFilter(model.CurrentImport.ProgrammeId);
            model.Exceptions = await context.Import.ListExceptions(filter);
            model.TotalPages = model.Exceptions.TotalPages;
            model.TotalRecords = model.Exceptions.TotalRecords;
            model.TotalDisplayRecords = model.Exceptions.TotalDisplayRecords;
            model.Programme = context.Vehicle.GetProgramme(programmeFilter);
            model.Gateway = model.CurrentImport.Gateway;

            return model;
        }

        private void InitialiseMembers()
        {
 	        CurrentImport = new EmptyImportQueue();
            CurrentException = new EmptyImportError();
            CurrentDerivativeMapping = new EmptyDerivativeMapping();
            CurrentTrimMapping = new EmptyTrimMapping();
            CurrentAction = enums.ImportAction.NotSet;
            CurrentFeatureGroup = string.Empty;
            CurrentFeatureSubGroup = new EmptyFeatureGroup();
            CurrentFeature = new EmptyFeature();
            CurrentMarket = new EmptyMarket();

            AvailableEngines = Enumerable.Empty<ModelEngine>();
            AvailableTransmissions = Enumerable.Empty<ModelTransmission>();
            AvailableBodies = Enumerable.Empty<ModelBody>();
            AvailableTrim = Enumerable.Empty<ModelTrim>();
            AvailableSpecialFeatures  = Enumerable.Empty<SpecialFeature>();
            AvailableMarkets = Enumerable.Empty<Market>();
            AvailableFeatures = Enumerable.Empty<Feature>();
            AvailableFeatureGroups = Enumerable.Empty<FeatureGroup>();
            AvailableDerivatives = Enumerable.Empty<Derivative>();
            AvailableExceptionTypes = Enumerable.Empty<ImportExceptionType>();

            IdentifierPrefix = "Page";
        }

        #endregion
    }
}