using ImportRep;
using ImportService.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class StepUploadController : Controller
    {

        private IImportRepository db;

        public StepUploadController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }
        // GET: StepUpload
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
            //    return RedirectToAction("Index", "readBatchController");
            //}


            return View();
        }


        /// <summary>
        /// Step 1 Get the file from the form and save it to the appdata folder
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

            return View();
        }

        /// <summary>
        /// Step 2 Read impoted file into Import Table
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            List<ImportModel.rawImport> recordsIn = new List<ImportModel.rawImport>();
            List<ImportModel.rawImport> newRecords = new List<ImportModel.rawImport>();
            try
            {
                recordsIn = ReadInputFile();
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
                return View();
                #endregion import
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }


        }

        /// <summary>
        /// Step 3 Insert into Pannabacker
        /// </summary>
        /// <returns></returns>
        public ActionResult PannabackerInsert()
        {

            try
            {
                #region import
                db.RemoveDuplicatePB();
                db.MergeImportToPB();
                db.cleanPBForms();
                db.cleanForms();
                db.RemoveDuplicatePB();
                // so PB table sould now be solid and ready
                return View();
                #endregion import
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }


        }

        public ActionResult CalculateBasePrice(decimal margin)
        {
            decimal test = 0;
            test = margin;
            try
            {
                var allPB = db.GetPannebakkers().ToList();
                // db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");


                List<ImportModel.Pannebakker> newBatches = new List<ImportModel.Pannebakker>();
                foreach (var b in allPB)
                {
                    if (b.BatchId == 5313)
                    {
                        var fred = 1;
                    }
                    ImportModel.Pannebakker newB = new ImportModel.Pannebakker();
                    CalcData.Result result = CalcData.CalMarginPrice(b,margin);
                    decimal price = result.price;
                    if (price != 0)
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
                    newB.FromDate = b.FromDate;
                    if (price != b.Price)
                    {
                        newB.Comment = "Using Rule " + result.rule + "Price Modified from " + b.Price + " to " + newB.WholesalePrice;
                    }
                    if (price == b.Price)
                    {
                        newB.Comment = "Using Rule " + result.rule +  " Price Not Modified";
                    }
                    if (price == 0)
                    {
                        newB.Comment = null;
                    }
                    newBatches.Add(newB);
                }

                db.EmptyPB();
                db.BulkInsertIntoPB(newBatches);

                return View();
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }
        }


        public ActionResult BatchMerge()
        {
            try { 
   
            db.MergePbToBatch();
            ViewBag.Title = "done";
            Response.Write("<script>console.log('Data has been saved to db');</script>");
            return View();
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }
        }

        public ActionResult BatchActiveCheck()
        {
            try
            {

                db.ActivateBatch();
                ViewBag.Title = "done";
                Response.Write("<script>console.log('Data has been saved to db');</script>");
                return RedirectToAction("Index", "Home");
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }
        }

        /// <summary>
        /// Step 1 Read file and insert into Import Table
        /// </summary>
        /// <returns></returns>
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
                foreach (var b in allPB)
                {
                    ImportModel.Pannebakker newB = new ImportModel.Pannebakker();
                    CalcData.Result result = CalcData.CalCapPrice(b);
                    decimal price = result.price;
                    if (price != 0)
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
                        newB.Comment = "Using Rule " + result.rule + " Price Modified from " + b.Price + " to " + newB.WholesalePrice;
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


        private List<ImportModel.rawImport> ReadInputFile()
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
                if (ExcelData.HasData(workSheet, row))
                {
                    decimal price = 0.0m;
                    ImportModel.rawImport obj = new ImportModel.rawImport();
                    obj.Sku = ExcelData.GetPBSKU(workSheet, row);
                    obj.FormSizeCode = ExcelData.GetPBFSCOde(workSheet, row);
                    obj.Name = ExcelData.GetName(workSheet, row);
                    obj.FormSize = ExcelData.GetFSDecription(workSheet, row);
                    price = ExcelData.GetPrice(workSheet, row);
                    obj.FromDate = null;
                   // obj.FromDate = ExcelData.GetDate(workSheet, row);
                    obj.Price = price * 100;
                    obj.Location = "PB";
                    recordsIn.Add(obj);

                }
            }
            return recordsIn;
        }




    }
}