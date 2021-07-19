using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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


        public void BatchUpdate(List<Batch> updateList)
        {
            List<UpdatePrice> updatePrices = updateList.Select(b => new UpdatePrice
            {
                 Id = b.Id,
                  WholesalePrice = (int)b.WholesalePrice
            }).ToList();
            UpdateData(updatePrices);

        }

        public DataTable ConvertToDatatable(List<UpdatePrice> list)
        {
            var dt = new DataTable();

            try
            {
                dt.Columns.Add("Id");
                dt.Columns.Add("WholesalePrice");

                foreach (var item in list)
                {
                    var row = dt.NewRow();

                    row["Id"] = item.Id;
                    row["WholesalePrice"] = item.WholesalePrice;

                    dt.Rows.Add(row);
                }
            }
            catch (Exception e)
            {
                // Handle
                throw;
            }
            return dt;
        }

        public void UpdateData(List<UpdatePrice> list)
        {
            string cs = "Server=tcp:hills-server.database.windows.net,1433;Initial Catalog=HillsStock1;Persist Security Info=False;User ID=rib1356;Password=A-Hills-Stock;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            DataTable dt = new DataTable("MyTable");
            dt = ConvertToDatatable(list);
            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand command = new SqlCommand("CREATE TABLE #TmpTable([Id] int,[WholesalePrice] int)", conn))
                {
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 6600;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }
                        command.CommandTimeout = 3000;
                        command.CommandText = "UPDATE P SET P.[WholesalePrice]= T.[WholesalePrice] FROM [Batch] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id];DROP TABLE #TmpTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { // Handle exception properly }  finally
                        {
                            var exp = ex.InnerException;
                            conn.Close();
                        }
                    }
                }
            }
        }



        public void EmptyImport()
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [rawImport]");
        }


        public void MergeBatchToNames()
        {
            context.sp_mergenames();

        }



        /// <summary>
        /// /****** Object:  StoredProcedure [dbo].[sp_importmerge]    Script Date: 16/07/2019 18:48:30 ******/
        /****** Object:  StoredProcedure [dbo].[sp_MergePBintoBatch]    Script Date: 12/10/2020 16:37:40 ******/
                        //MERGE[dbo].[Batch] AS TARGET
                        //USING[dbo].[Pannebakker] AS SOURCE
                        //ON (TARGET.Sku = SOURCE.Sku AND TARGET.FormSizeCode = SOURCE.FormSizeCode)
                        //--When records are matched, update the records if there is any change
                        //WHEN MATCHED --  TARGET.BuyPrice<> SOURCE.Price
                        //THEN
                        //UPDATE SET TARGET.WholesalePrice = SOURCE.WholesalePrice, TARGET.BuyPrice = Source.Price, TARGET.Comment = SOURCE.Comment, Target.DateStamp = GETDATE()
                        //--When no records are matched, insert the incoming records from source table to target table
                        //WHEN NOT MATCHED BY TARGET
                        //THEN INSERT
                        //           ([Sku]
                        //           ,[Name]
                        //           ,[FormSize]
                        //           ,[FormSizeCode]
                        //           ,[Location]
                        //           ,[Quantity]
                        //           ,[WholesalePrice]
                        //           ,[ImageExists]
                        //           ,[Active]
                        //           ,[GrowingQuantity]
                        //           ,[AllocatedQuantity]
                        //           ,[DateStamp]
                        //           ,[BuyPrice]
                        //           ,[Comment])
                        //     VALUES
                        //           (SOURCE.Sku
                        //           , SOURCE.Name
                        //           , SOURCE.FormSize
                        //           , SOURCE.FormSizeCode
                        //           ,'PB'
                        //           ,5000
                        //           ,SOURCE.WholesalePrice
                        //           ,0
                        //           ,1
                        //           ,0
                        //           ,0
                        //           , GETDATE()
                        //           , Source.Price
                        //           , Source.Comment)
                        // WHEN NOT MATCHED BY SOURCE AND TARGET.Location = 'PB' AND Target.AllocatedQuantity = 0
                        //--THEN DELETE;
                        //        THEN UPDATE SET TARGET.Active = 0;
                        //        END
        /// </summary>
        public void MergeImportToPB()
        {
            context.sp_importmerge();
        }

        public void MergePbToBatch()
        {
            context.sp_MergePBintoBatch();
        }

        public void ActivateBatch()
        {
            context.UpdateActiveBatch();
        }

        public void cleanForms()
        {
            //try
            //    {
            //    context.sp_cleanFormSize();
            //}
            //catch(Exception ex)
            //{
            //    var result = ex.InnerException.InnerException;
            //}
        }

        public void cleanPBForms()
        {
            context.sp_cleanPB_FormSize();
        }

        public void RemoveDuplicateImport()
        {
            context.sp_removeimportduplicates();
        }

        public void RemoveDuplicateNames()
        {
            context.sp_removedupnames();
        }





        /// <summary>
        //        /// BEGIN
        //    -- SET NOCOUNT ON added to prevent extra result sets from
        //    -- interfering with SELECT statements.
        //    SET NOCOUNT ON

        //    -- Insert statements for procedure here
        //    DELETE FROM dbo.Pannebakker
        //WHERE  ID NOT IN (SELECT MAX(ID)
        //                  FROM   dbo.Pannebakker
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
        public void RemoveDuplicatePB()
        {
            
            context.sp_removepannebakkerduplicates();
        }

        public void RemovePBFromBatch()
        {
            context.sp_DeletePBFromBatch();
        }

        public void RemoveDuplicateBatch()
        {
            context.removeBatchDuplicates();
        }

        public void EmptyPB()
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");
        }


        public IEnumerable<ImportModel.Pannebakker> GetPannebakkers()
        {
            return context.Pannebakkers; 
        }

        public IEnumerable<ImportModel.Batch> GetLocalBatches()
        {
            return context.Batches.Where(b => b.Location != "PB");
        }

        public IEnumerable<ImportModel.Batch> GetPBBatches()
        {
            return context.Batches.Where(b => b.Location == "PB");
        }

        public void BulkInsertIntoImport(IEnumerable<ImportModel.rawImport> newRecords)
        {
            context.BulkInsert<ImportModel.rawImport>(newRecords);
            
        }

        public void BulkInsertIntoPB(IEnumerable<ImportModel.Pannebakker> newRecords)
        {
            context.BulkInsert<ImportModel.Pannebakker>(newRecords);

        }

        public void BulkInsertPBintoBatch(IEnumerable<ImportModel.Batch> newRecords)
        {
            context.BulkInsert<ImportModel.Batch>(newRecords);

        }

        public void BulkInsertGMBatch(IEnumerable<ImportModel.Batch> newRecords)
        {
            //context.BulkInsert<IEnumerable<ImportModel.Batch>(newRecords);

            context.BulkInsert<ImportModel.Batch>(newRecords);

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
