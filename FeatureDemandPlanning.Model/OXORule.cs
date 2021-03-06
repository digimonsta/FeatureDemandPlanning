using System;
using System.ComponentModel.DataAnnotations;

namespace FeatureDemandPlanning.Model
{
    public class OXORule : BusinessObject
    {

        [Required]
        [Range(1, Int16.MaxValue)]
        public int ProgrammeId { get; set; }
        [Required]
        public string RuleCategory { get; set; }
        [Required]
        public string RuleGroup { get; set; }
        public string RuleAssertLogic { get; set; }
        public string RuleReportLogic { get; set; }
        [Required]
        [StringLength(4000)]
        public string RuleResponse { get; set; }

        [StringLength(4000)]
        public string RuleReason { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Owner { get; set; }
           
        // A blank constructor
        public OXORule() {;}
    }
}