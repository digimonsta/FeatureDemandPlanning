﻿using FeatureDemandPlanning.Model.Context;
using FeatureDemandPlanning.Model.Empty;
using FeatureDemandPlanning.Model.Filters;
using FeatureDemandPlanning.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureDemandPlanning.Model.ViewModel
{
    public class TakeRateViewModel : SharedModelBase
    {
        public TakeRateSummary TakeRate { get; set; }

        public PagedResults<TakeRateSummary> TakeRates 
        {
            get
            {
                return _takeRates;
            }
            set
            {
                _takeRates = value;
            } 
        }
        public dynamic Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForecastComparisonViewModel"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public TakeRateViewModel(IDataContext dataContext)
            : base(dataContext)
        {
            Configuration = dataContext.ConfigurationSettings;
            TakeRate = new EmptyTakeRateSummary();
        }

        public static async Task<TakeRateViewModel> GetModel(IDataContext context, TakeRateFilter filter)
        {
            var model = new TakeRateViewModel(context)
            {
                PageIndex = filter.PageIndex.HasValue ? filter.PageIndex.Value : 1,
                PageSize = filter.PageSize.HasValue ? filter.PageSize.Value : Int32.MaxValue
            };
            model.TakeRates = await context.Volume.ListTakeRateData(filter);

            return model;
        }

        private PagedResults<TakeRateSummary> _takeRates = new PagedResults<TakeRateSummary>();
    }
}
