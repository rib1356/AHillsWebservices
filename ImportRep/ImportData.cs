﻿using System;
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



        /// <summary>
        /// /****** Object:  StoredProcedure [dbo].[sp_importmerge]    Script Date: 16/07/2019 18:48:30 ******/
        //SET ANSI_NULLS ON
                //GO
                //SET QUOTED_IDENTIFIER ON
                //GO
                //BEGIN
                //    -- SET NOCOUNT ON added to prevent extra result sets from
                //    -- interfering with SELECT statements.
                //    SET NOCOUNT ON
                //MERGE[dbo].[Pannebakker] AS TARGET
                //USING[dbo].[rawImport] AS SOURCE
                //ON (TARGET.Sku = SOURCE.Sku AND TARGET.FormSizeCode = SOURCE.FormSizeCode AND TARGET.FormSize = SOURCE.FormSize AND TARGET.Name = SOURCE.Name)
                //--When records are matched, update the records if there is any change
                //WHEN MATCHED AND TARGET.Price<> SOURCE.Price
                //THEN UPDATE SET TARGET.Price = SOURCE.Price
                //--When no records are matched, insert the incoming records from source table to target table
                //WHEN NOT MATCHED BY TARGET
                //THEN INSERT ([Sku]
                //           ,[FormSizeCode]
                //           ,[Name]
                //           ,[FormSize]
                //           ,[Price]
                //           ,[RootBall]
                //           ,[BatchId]) VALUES (SOURCE.Sku, SOURCE.FormSizeCode, SOURCE.Name, SOURCE.FormSize, SOURCE.Price, SOURCE.RootBall, SOURCE.BatchId)
                // WHEN NOT MATCHED BY SOURCE
                //THEN DELETE;
                //        END
        /// </summary>
        public void MergeImport()
        {
            // context.sp_importmerge();
            context.sp_mergenames();

        }


        /// <summary>
        /// /****** Object:  StoredProcedure [dbo].[sp_removeimportduplicates]    Script Date: 16/07/2019 18:50:44 ******/
        //        SET ANSI_NULLS ON
        //        GO
        //SET QUOTED_IDENTIFIER ON
        //GO
        //BEGIN
        //    -- SET NOCOUNT ON added to prevent extra result sets from
        //    -- interfering with SELECT statements.
        //    SET NOCOUNT ON

        //    -- Insert statements for procedure here
        //    DELETE FROM dbo.rawImport
        //    WHERE  ID NOT IN (SELECT MAX(ID)
        //                  FROM   dbo.rawImport
        //                  GROUP  BY Sku,
        //                            [Name],
        //                            FormSizeCode,
        //                            FormSize
        //                  /*Even if ID is not null-able SQL Server treats MAX(ID) as potentially
        //                    nullable. Because of semantics of NOT IN (NULL) including the clause
        //                    below can simplify the plan*/
        //                  HAVING MAX(ID) IS NOT NULL)
        //END
        /// </summary>
        public void RemoveDuplicates()
        {
            context.sp_removedupnames();
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
