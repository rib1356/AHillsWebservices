using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportServiceCore.DTO
{
    public class ImportSet
    {
            public List<Model.RawImport> rawImport { get; set; }
            public List<Model.Batch> batchImport { get; set; }
    }
}