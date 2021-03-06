﻿using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Enumerations;
using FeatureDemandPlanning.Model.Interfaces;
using System.Collections.Generic;
using FeatureDemandPlanning.Model.Empty;

namespace FeatureDemandPlanning.Model
{
    public class TakeRateDocument : ITakeRateDocument
    {
        public int? TakeRateId { get; set; }
        public OXODoc UnderlyingOxoDocument { get; set; }
        public Market Market { get; set; }
        public MarketGroup MarketGroup { get; set; }
        public Vehicle Vehicle { get; set; }
        public TakeRateData TakeRateData { get; set; }
        public TakeRateResultMode Mode { get; set; }
        public int TotalDerivatives { get; set; }
        public IEnumerable<TakeRateSummary> TakeRateSummary { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int TotalDisplayRecords { get; set; }
        public string BMC { get; set; }
        public string DPCK { get; set; }

        public TakeRateDocument()
        {
            UnderlyingOxoDocument = new EmptyOxoDocument();
            MarketGroup = new EmptyMarketGroup();
            Market = new EmptyMarket();
            Vehicle = new EmptyVehicle();
            TakeRateData = new EmptyTakeRateData();
            Mode = TakeRateResultMode.PercentageTakeRate;
            TakeRateSummary = new List<TakeRateSummary>();
        }
        public static ITakeRateDocument FromFilter(TakeRateFilter filter)
        {
            var volume = new TakeRateDocument();

            if (filter.TakeRateId.HasValue)
            {
                volume.TakeRateId = filter.TakeRateId;
            }

            volume.UnderlyingOxoDocument = filter.DocumentId.HasValue ? new OXODoc() {Id = filter.DocumentId.Value} : new EmptyOxoDocument();

            if (filter.ProgrammeId.HasValue)
            {
                volume.Vehicle = new Vehicle() { ProgrammeId = filter.ProgrammeId.Value, Gateway = filter.Gateway };
            }

            if (filter.MarketGroupId.HasValue)
            {
                volume.MarketGroup = new MarketGroup() { Id = filter.MarketGroupId.Value };
            }

            if (filter.MarketId.HasValue)
            {
                volume.Market = new Market() { Id = filter.MarketId.Value };
            }

            if (!string.IsNullOrEmpty(filter.BMC))
            {
                volume.BMC = filter.BMC;
            }

            if (!string.IsNullOrEmpty(filter.DPCK))
            {
                volume.DPCK = filter.DPCK;
            }

            if (!string.IsNullOrEmpty(filter.FeatureCode))
            {
                volume.FeatureCode = filter.FeatureCode;
            }

            volume.Mode = filter.Mode;

            return volume;
        }

        public string FeatureCode { get; set; }

        public bool ShowCombinedPackOptions { get; set; }
        public bool ExcludeOptionalPackItems { get; set; }
    }
}
