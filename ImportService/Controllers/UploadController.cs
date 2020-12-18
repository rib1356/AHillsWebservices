

using EntityFramework.BulkInsert.Extensions;
using ImportRep;
using ImportService.Models;
using OfficeOpenXml;
using SqlBulkTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using NLog;
using ImportService.ServiceLayer;

namespace ImportService.Controllers
{
    public class UploadController : Controller
    {
        private IImportRepository db;

        public UploadController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }

        // private HillsStockImportEntities db = new HillsStockImportEntities();
        public ActionResult Index()
        {
            try
            {
                var valid = (bool)Session["admin"];
                if (!valid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


            return View();
        }


        /// <summary>
        /// Get the file from the form and save it to the appdata folder
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Call to UpLoad Method using tempdata path as a pointer to the file</returns>
        public ActionResult GetFile(HttpPostedFileBase file)
        {
            try
            {
                var valid = (bool)Session["admin"];
                if (!valid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    string name = System.IO.Path.GetFileName(fileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/"), name);
                    file.SaveAs(path);
                    TempData["path"] = name;
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }

            ViewBag.Title = "Home Page";

            return RedirectToAction("UpLoad");
        }


        public ActionResult UpLoad()
        {
            List<ImportModel.rawImport> recordsIn = new List<ImportModel.rawImport>();
            List<ImportModel.Pannebakker> existingRecords = new List<ImportModel.Pannebakker>();
            List<ImportModel.rawImport> newRecords = new List<ImportModel.rawImport>();

            try
            {
                recordsIn = ReadInputFile();
               // existingRecords = db.GetPannebakkers().ToList();
               // newRecords = recordsIn.Union(existingRecords).ToList();
               // newRecords = existingRecords.Union(recordsIn, new DTO.PbComparer()).ToList();

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("version");
            }


            try
            {
                #region import
                ///// empty the import tables
                // db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");
                db.EmptyImport();
                /// insert into raw import and remove any duplicates just in case
                // db.BulkInsert<Pannebakker>(newRecords);
                db.BulkInsertIntoImport(recordsIn);
                db.RemoveDuplicateImport();
                #endregion import

                // merge import into PB and clean form sizes
                //AddBatch(records);
                db.MergeImportToPB();
                db.cleanPBForms();
                db.cleanForms();
                db.RemoveDuplicatePB();
                // so PB table sould now be solid and ready
                
                
                // remove any duplicates from PB and Batch
                // may not be needed but just in case

                // db.RemoveDuplicateBatch();

                // clean all old PB's from batch as we are going to provide a new lot.
                // worried about this moving frowards if quotes use batch ids from PB's i am removing
                //db.RemovePBFromBatch();


                var allPB = db.GetPannebakkers().ToList();
               // db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");


                List<ImportModel.Pannebakker> newBatches = new List<ImportModel.Pannebakker>();
                foreach(var b in allPB)
                {
                    ImportModel.Pannebakker newB = new ImportModel.Pannebakker();
                    decimal price = CalCapPrice(b);
                    if (price != 0 )
                    {
                        newB.WholesalePrice = Convert.ToInt32(price);
                    }
                    else
                    {
                        newB.WholesalePrice = Convert.ToInt32(b.Price);
                    }
                    newB.Price = b.Price;
                    newB.FormSize = b.FormSize;
                    newB.Location = "PB";
                    newB.Name = b.Name;
                    newB.Sku = b.Sku;
                    newB.WholesalePrice = Convert.ToInt32(price);
                    newB.FormSizeCode = b.FormSizeCode;
                    if (price != b.Price)
                    {
                        newB.Comment = "Price Modified from " + b.Price + " to " + newB.WholesalePrice;
                    }
                    if (price == b.Price)
                    {
                        newB.Comment = "Price Not Modified";
                    }
                    if (price == 0)
                    {
                        newB.Comment = null;
                    }
                    newBatches.Add(newB);
                }


                //IEnumerable<ImportModel.Batch> newBatches = allPB.Select(batch => new ImportModel.Batch
                //{
                //    Active = true,
                //    AllocatedQuantity = 0,
                //    BuyPrice = CalCapPrice(batch),
                //    Comment = "",
                //    FormSize = batch.FormSize,
                //    ImageExists = false,
                //    GrowingQuantity = 0,
                //    Location = "PB",
                //    Name = batch.Name,
                //    Sku = batch.Sku,
                //    Quantity = 5000,
                //    WholesalePrice = 0,
                //    DateStamp = DateTime.Now,

                //});
                db.EmptyPB();
                db.BulkInsertIntoPB(newBatches);
                db.MergePbToBatch();
                ViewBag.Title = "done";
                Response.Write("<script>console.log('Data has been saved to db');</script>");
                return View("uploadDone");
                //return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }

        }

        /// <summary>
        /// Calculates the proposed sale unit sale price dependant upon form - size
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>Proposed unit Sale Price</returns>
        private static decimal CalCapPrice(ImportModel.Pannebakker batch)
        {
            // pb buy price
            var y = batch.Price;
            // base sales price
            var x = (batch.Price / 0.55m);

            PriceItemDTO price = PriceService.GetUnitPrice(batch.FormSize,batch.FormSizeCode);
                if (price != null)
                { 
                    var max = Convert.ToDecimal(price.MaxUnitValue * 100);
                    var min = Convert.ToDecimal(price.MinUnitValue * 100);
                    if (x < min)
                    {
                       return min + y;
                    }
                    if (x > max)
                    {
                       return max + y;
                    }
                }
                else
                {
                return 0;
                }
            return y;
        }

        private List<ImportModel.rawImport>  ReadInputFile()
        {
            List<ImportModel.rawImport> recordsIn = new List<ImportModel.rawImport>();
            var fred = TempData["path"].ToString();
            //var filesData = Directory.GetFiles(@fred);
            string path = Server.MapPath("~/App_Data/" + fred);
            //string path = Server.MapPath(fred.ToString());
            var package = new OfficeOpenXml.ExcelPackage(new FileInfo(path));

            OfficeOpenXml.ExcelWorksheet workSheet = package.Workbook.Worksheets[1];


            for (int row = workSheet.Dimension.Start.Row;
                 row <= workSheet.Dimension.End.Row;
                 row++)
            {
                if (HasData(workSheet, row))
                {
                    decimal price = 0.0m;
                    ImportModel.rawImport obj = new ImportModel.rawImport();
                    obj.Sku = GetPBSKU(workSheet, row);
                    obj.FormSizeCode = GetPBFSCOde(workSheet, row);
                    obj.Name = GetName(workSheet, row);
                    obj.FormSize = GetFSDecription(workSheet, row);
                    price = GetPrice(workSheet, row);
                    obj.Price = price * 100;
                    obj.Location = "PB";
                    recordsIn.Add(obj);

                }
            }
            return recordsIn;
        }

        //private static string AddBatch(List<Pannebakker> records)
        //{
        //    var bulk = new BulkOperations();
        //    // books = GetBooks();
        //    try
        //    {
        //        using (TransactionScope trans = new TransactionScope())
        //        {
        //            var constr = "data source = hills-server.database.windows.net; initial catalog = HillsStock1; persist security info = False; user id = rib1356; password = A - Hills - Stock; MultipleActiveResultSets = False; TrustServerCertificate = False";
        //            using (SqlConnection conn = new SqlConnection(constr))
        //            {
        //                bulk.Setup<Pannebakker>()
        //                    .ForCollection(records)
        //                    .WithTable("Pannebakker")
        //                    .AddAllColumns()
        //                    .BulkInsert()
        //                    .SetIdentityColumn(x => x.Id, SqlBulkTools.Enumeration.ColumnDirectionType.InputOutput)
        //                    .Commit(conn);
        //            }

        //            trans.Complete();
        //            return "Well Done";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw; 
        //    }
        //}


        //public ActionResult OldUpLoad()
        //{
        //    List<Pannebakker> records = new List<Pannebakker>();
        //    try { 
        //    var fred = TempData["path"].ToString();
        //    //var filesData = Directory.GetFiles(@fred);
        //    string path = Server.MapPath("~/App_Data/" + fred);
        //    //string path = Server.MapPath(fred.ToString());
        //    var package = new OfficeOpenXml.ExcelPackage(new FileInfo(path));

        //    OfficeOpenXml.ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

        //        // var allrunners = db.runners.Where(r => r.Active == true).ToList();
        //        //db.Configuration.AutoDetectChangesEnabled = false;
        //        for (int row = workSheet.Dimension.Start.Row;
        //             row <= workSheet.Dimension.End.Row;
        //             row++)
        //    {
        //            if (HasData(workSheet, row))
        //            {
        //                Pannebakker obj = new Pannebakker();
        //                obj.Sku = GetPBSKU(workSheet, row);
        //                obj.FormSizeCode = GetPBFSCOde(workSheet, row);
        //                obj.Name = GetName(workSheet, row);
        //                obj.FormSize = GetFSDecription(workSheet, row);
        //                obj.Price = GetPrice(workSheet, row);
        //                records.Add(obj);

        //            }
        //    }

        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.Error = ex.InnerException.Message;
        //        return View("shit");
        //    }


        //    try
        //    {
        //        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");
        //        db.BulkInsert<Pannebakker>(records);

        //        ViewBag.Title = "done";
        //        Response.Write("<script>console.log('Data has been saved to db');</script>");
        //        return View("uploadDone");
        //        //return RedirectToAction("Index");

        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.Error = ex.InnerException.Message;
        //        return View("shit");
        //    }

        //}

        private static bool HasData(ExcelWorksheet workSheet, int row)
        {
            if ((workSheet.Cells[row, workSheet.Dimension.Start.Column + 0].Value != null) && (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ABEGOUCH
        private static string GetPBSKU(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 2C2
        private static string GetPBFSCOde(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // Abelia 'Edward Goucher'
        private static string GetName(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 3].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 3].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 2 Ltr pot
        private static string GetFSDecription(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 4].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 4].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 3.23
        private static decimal GetPrice(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value != null)
            {
                var data = workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value;
                return Convert.ToDecimal(data);
            }
            else
            {
                return 0.0m;
            }
        }



    }
}