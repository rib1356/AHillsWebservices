using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.BulkInsert.Extensions;
using ImportModel;

namespace ImportRep
{
    public class ImportRepository : IImportRepository, IDisposable
    {


        private ImportModel.ImportEntities context;

        public ImportRepository(ImportModel.ImportEntities context)
        {
            this.context = context;
        }

        public ImportRepository()
        {
        }

        public void EmptyImport()
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [rawImport]");
        }


        public void MergeImport()
        {
            context.sp_importmerge();
        }

        public void RemoveDuplicates()
        {
            context.sp_removeimportduplicates();
            //context.sp_removepannebakkerduplicates();
        }

        public IEnumerable<ImportModel.Pannebakker> GetPannebakkers()
        {
            return context.Pannebakkers; 
        }

        public void BulkInsert(IEnumerable<ImportModel.rawImport> newRecords)
        {
            context.BulkInsert<ImportModel.rawImport>(newRecords);
            
        }


        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
