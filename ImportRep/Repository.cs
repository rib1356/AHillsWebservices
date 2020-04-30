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

        void BulkInsertGMBatch(IEnumerable<ImportModel.Batch> newRecords);

        void BulkInsert(IEnumerable<ImportModel.rawImport> newRecords);

        void BulkInsertPBintoBatch(IEnumerable<ImportModel.Batch> newRecords);

        void MergeImportToNames();

        void MergeImportToPB();

        void MergePbToBatch();

        void cleanForms();

        void cleanPBForms();

        void RemoveDuplicateNames();

        void RemoveDuplicatePB();

        void RemoveDuplicateImport();

        void RemoveDuplicateBatch();

        void RemovePBFromBatch();
    }
}
