﻿using enums = FeatureDemandPlanning.Model.Enumerations;

namespace FeatureDemandPlanning.Model
{
    public class ImportExceptionType
    {
        public int FdpImportErrorTypeId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int WorkflowOrder { get; set; }

        public enums.ImportExceptionType ExceptionType
        {
            get { return (enums.ImportExceptionType)FdpImportErrorTypeId; }
        }
    }
}
