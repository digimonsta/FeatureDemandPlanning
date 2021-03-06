﻿using System;

namespace FeatureDemandPlanning.Model
{
    public class ForecastSummary
    {
        public int ForecastId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int ProgrammeId { get; set; }
        public string Gateway { get; set; }
        public string Code { get; set; }
        public string CarLine { get; set; }
        public string ModelYear { get; set; }

        public string[] ToJQueryDataTableResult()
        {
            return new[] 
            { 
                ForecastId.ToString(),
                CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                CreatedBy,
                CarLine,
                ModelYear,
                Gateway
            };
        }
    }
}
