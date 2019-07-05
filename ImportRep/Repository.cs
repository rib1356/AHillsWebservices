using ImportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportRep
{
    public interface IImportRepository : IDisposable
    {
        void EmptyImport();

        IEnumerable<Pannebakker> GetPannebakkers();

        void BulkInsert(IEnumerable<ImportModel.rawImport> newRecords);

        void MergeImport();

        void RemoveDuplicates();
    }
}
