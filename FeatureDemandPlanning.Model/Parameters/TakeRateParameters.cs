﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureDemandPlanning.Model.Parameters
{
    public class TakeRateParameters : JQueryDataTableParameters
    {
        public int? TakeRateId { get; set; }
        public string FilterMessage { get; set; }
    }
}
