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

        IEnumerable<ImportModel.Batch> GetLocalBatches();

        IEnumerable<ImportModel.Batch> GetPBBatches();

        void BulkInsertGMBatch(IEnumerable<ImportModel.Batch> newRecords);

        void BulkInsertIntoImport(IEnumerable<ImportModel.rawImport> newRecords);

        void BulkInsertIntoPB(IEnumerable<ImportModel.Pannebakker> newRecords);

        void BulkInsertPBintoBatch(IEnumerable<ImportModel.Batch> newRecords);

       // void MergeImportToNames();

        void MergeBatchToNames();

        void MergeImportToPB();

        void MergePbToBatch();

        void ActivateBatch();

        void cleanForms();

        void cleanPBForms();

        void RemoveDuplicateNames();

        void RemoveDuplicatePB();

        void RemoveDuplicateImport();

        void RemoveDuplicateBatch();

        void RemovePBFromBatch();

        void EmptyPB();

        void BatchUpdate(List<Batch> updateList);
    }
}
