﻿using System.ComponentModel;
using FeatureDemandPlanning.Model.Enumerations;

namespace FeatureDemandPlanning.Model
{
    public class ValidationResult
    {
        public int? FdpValidationId { get; set; }
        public int TakeRateId { get; set; }
        public int MarketId { get; set; }
        public int MarketGroupId { get; set; }
        public int? ModelId { get; set; }
        public int? FdpModelId { get; set; }
        public int? FeatureId { get; set; }
        public int? FdpFeatureId { get; set; }
        public int? FeaturePackId { get; set; }
        public int? BodyId { get; set; }
        public int? EngineId { get; set; }
        public int? TransmissionId { get; set; }
        public string ExclusiveFeatureGroup { get; set; }

        public string ModelIdentifier { get; set; }
        public string FeatureIdentifier { get; set; }

        public string Message { get; set; }

        public int? FdpVolumeDataItemId { get; set; }
        public int? FdpTakeRateSummaryId { get; set; }
        public int? FdpTakeRateFeatureMixId { get; set; }
        public int? FdpPowertrainDataItemId { get; set; }
        public int? FdpChangesetDataItemId { get; set; }

        public ValidationRule ValidationRule { get; set; }

        public bool IsFeatureValidation
        {
            get { return !string.IsNullOrEmpty(FeatureIdentifier) && !string.IsNullOrEmpty(ModelIdentifier); }
        }
        public bool IsModelValidation
        {
            get { return !string.IsNullOrEmpty(ModelIdentifier) && string.IsNullOrEmpty(FeatureIdentifier); }
        }
        public bool IsWholeMarketValidation
        {
            get
            {
                return string.IsNullOrEmpty(ModelIdentifier) && string.IsNullOrEmpty(FeatureIdentifier) &&
                       !IsPowertrainValidation;
            }   
        }
        public bool IsFeatureMixValidation
        {
            get { return string.IsNullOrEmpty(ModelIdentifier) && !string.IsNullOrEmpty(FeatureIdentifier); }   
        }

        public bool IsPowertrainValidation
        {
            get { return BodyId.HasValue && EngineId.HasValue && TransmissionId.HasValue; }
        }

        public string DataTarget
        {
            get
            {
                string dataTarget;
                if (IsWholeMarketValidation)
                {
                    dataTarget = MarketId.ToString();
                }
                else if (IsPowertrainValidation)
                {
                    dataTarget = "PT|" + MarketId;
                }
                else if (IsModelValidation)
                {
                    dataTarget = string.Format("{0}|{1}", MarketId, ModelIdentifier);
                }
                else if (IsFeatureMixValidation)
                {
                    dataTarget = string.Format("{0}|{1}", MarketId, FeatureIdentifier);
                }
                else
                {
                    dataTarget = string.Format("{0}|{1}|{2}", MarketId, ModelIdentifier, FeatureIdentifier);
                }
                return dataTarget;
            }
        }

        public string RuleDescription
        {
            get
            {
                var retVal = string.Empty;
                var fi = ValidationRule.GetType().GetField(ValidationRule.ToString());

                if (fi == null) return retVal;
                var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs.Length > 0)
                    retVal = ((DescriptionAttribute)attrs[0]).Description;

                return retVal;
            }

        }
    }
}
