
/*===============================================================================
 *
 *      Code Comment Block Here.
 *      
 *      Generated by Code Generator on 28/07/2015 12:16  
 * 
 *===============================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FeatureDemandPlanning.Dapper;

namespace FeatureDemandPlanning.BusinessObjects
{
    public class ForecastComparisonView : BusinessObject
    {
        public int Id { get; set; } 
            public int ForecastId { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedBy { get; set; }
            public int? VehicleId { get; set; }
            public int? ProgrammeId { get; set; }
            public int? GatewayId { get; set; }
            public string Make { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string ModelYear { get; set; }
            public string Gateway { get; set; }
            public int SortOrder { get; set; }
           
        // A blank constructor
        public ForecastComparisonView() {;}
    }
}