using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    public class ImportSet
    {
            public List<ImportModel.rawImport> rawImport { get; set; }
            public List<ImportModel.Batch> batchImport { get; set; }
    }
}