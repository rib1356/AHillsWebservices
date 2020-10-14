using ImportRep;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class GrowMasterController : Controller
    {
        // GET: GrowMaster
        private IImportRepository db;

        public GrowMasterController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }


        public ActionResult Index()
        {
            //try
            //{
            //    var valid = (bool)Session["admin"];
            //    if (!valid)
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "Home");
            //}


            return View();
        }


        /// <summary>
        /// Get the file from the form and save it to the appdata folder
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Call to UpLoad Method using tempdata path as a pointer to the file</returns>
        public ActionResult GetFile(HttpPostedFileBase file)
        {
            //try
            //{
            //    var valid = (bool)Session["admin"];
            //    if (!valid)
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "Home");
            //}


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
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }

            ViewBag.Title = "Home Page";

            return RedirectToAction("GMUpLoad");
        }

        // GET: GrowMaster/Details/5
        public ActionResult GMUpLoad()
        {
            List<Models.GMItem> recordsIn = new List<Models.GMItem>();

            try
            {
                recordsIn = ReadInputFile();
               
                // existingRecords = db.GetPannebakkers().ToList();
                // newRecords = recordsIn.Union(existingRecords).ToList();
                //newRecords = existingRecords.Union(recordsIn, new DTO.PbComparer()).ToList();

            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.InnerException.Message;
                return View("version");
            }


            try
            {
                #region import
                ///// empty the import tables
                // db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");

                IEnumerable<ImportModel.Batch> GMImport = recordsIn.Select(b => new ImportModel.Batch
                {
                    Location = b.Location,
                    Name = b.Name,
                    BuyPrice = 0,
                    WholesalePrice = (int)b.WholeSalePrice,
                    Sku = b.Sku,
                    FormSize = b.Size + " " + b.Form,
                    FormSizeCode = b.PBFSCode,
                     Active = true,
                      AllocatedQuantity = b.AllocatedQuantity,
                       Comment = "",
                        GrowingQuantity = b.GrowingQuantity,
                         ImageExists = false,
                          Quantity = b.StockQuantity,
                           DateStamp = DateTime.Now
                }).AsEnumerable();

                IEnumerable<ImportModel.Batch> AllBatch = db.GetLocalBatches();
                /// insert into raw import and remove any duplicates just in case
                // db.BulkInsert<Pannebakker>(newRecords);
                var inImportButNotInBatches= GMImport.Except(AllBatch).ToList();
                var inList2ButNotInList = AllBatch.Except(GMImport).ToList();
  //              var InBoth = inListButNotInList2.Intersect(inList2ButNotInList).ToList();
            //    var InsertList = GMImport.Where(x => AllBatch.Any(z => x.FormSizeCode != z.FormSizeCode && x.Sku != z.Sku)).ToList();
            //   var UpdateList = AllBatch.Where(x => GMImport.Any(z => x.FormSizeCode == z.FormSizeCode && x.Sku == z.Sku)).ToList();
            // var InsertList = GMImport.Where(x => AllBatch.Any(z => x.FormSizeCode != z.FormSizeCode && x.Sku != z.Sku)).ToList();
            // db.BulkInsertGMBatch(InsertList);
            //db.RemoveDuplicateImport();
                #endregion import


                // db.MergePbToBatch();
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
        //private static decimal CalCapPrice(ImportModel.Pannebakker batch)
        //{
        //    // pb buy price
        //    var y = batch.Price;
        //    // base sales price
        //    var x = (batch.Price / 0.55m);

        //    PriceItemDTO price = PriceService.GetUnitPrice(batch.FormSize, batch.FormSizeCode);
        //    if (price != null)
        //    {
        //        var max = Convert.ToDecimal(price.MaxUnitValue * 100);
        //        var min = Convert.ToDecimal(price.MinUnitValue * 100);
        //        if (x < min)
        //        {
        //            return min + y;
        //        }
        //        if (x > max)
        //        {
        //            return max + y;
        //        }
        //    }
        //    return y;
        //}

        private List<Models.GMItem> ReadInputFile()
        {
            List<Models.GMItem> recordsIn = new List<Models.GMItem>();
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
                    Models.GMItem obj = new Models.GMItem();
                    obj.PBFSCode = GetPBFSC(workSheet, row);
                    obj.Name = GetName(workSheet, row);
                    obj.GrowingQuantity = GetGrowingQuantity(workSheet, row) ?? 0;
                    obj.Form = GetFormDecription(workSheet, row);
                    obj.Size = GetSizeDecription(workSheet, row);
                    obj.AllocatedQuantity = GetAllocatedQuantity(workSheet, row)?? 0;
                    obj.WholeSalePrice = GetPrice(workSheet, row);
                    obj.Location = GetLocation(workSheet,row);
                    obj.Sku = GetPBSKU(workSheet, row);
                    obj.StockQuantity = GetStockQuantity(workSheet, row)?? 0;
                    recordsIn.Add(obj);

                }
            }
            return recordsIn;
        }

        private static bool HasData(ExcelWorksheet workSheet, int row)
        {
            if ((workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value != null) && (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // 2040060C5
        private static string GetPBFSC(ExcelWorksheet workSheet, int row)
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

        // ABEGOUCH
        private static string GetPBSKU(ExcelWorksheet workSheet, int row)
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

        // 10-20
        private static string GetSizeDecription(ExcelWorksheet workSheet, int row)
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

        // c20
        private static string GetFormDecription(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value).ToString();
            }
            else
            {
                return null;
            }
        }
        
        // E23
        private static string GetLocation(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 6].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 6].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 6
        private static int? GetStockQuantity(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 7].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + 7].Value);
            }
            else
            {
                return null;
            }
        }

        // 6
        private static int? GetAllocatedQuantity(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 8].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + 8].Value);
            }
            else
            {
                return null;
            }
        }

        private static int? GetGrowingQuantity(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 9].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + 9].Value);
            }
            else
            {
                return null;
            }
        }

        // 3.23
        private static decimal GetPrice(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 12].Value != null)
            {
                var data = workSheet.Cells[row, workSheet.Dimension.Start.Column + 12].Value;
                return Convert.ToDecimal(data);
            }
            else
            {
                return 0.0m;
            }
        }
    }
}
