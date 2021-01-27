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
                ViewBag.Error = ex.InnerException.Message;
                return View();
            }


            try
            {
                #region import
           

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

                var result = GMImport.ToList();
                db.BulkInsertGMBatch(GMImport);
            
                #endregion import


                // db.MergePbToBatch();
                ViewBag.Title = "done";
                Response.Write("<script>console.log('Data has been saved to db');</script>");
                return RedirectToAction("index", "readBatch");
                //return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }

        }

 

        private List<Models.GMItem> ReadInputFile()
        {
            const int PBFSC = 1;
            const int Name = 2;
            const int GrowingQuantity = 8;
            const int Form = 4;
            const int Size = 3;
            const int AllocatedQuantity = 9;
            const int WholeSalePrice = 11;
            const int Location = 5;
            const int Sku = 0;
            const int StockQuantity = 6;

            List<Models.GMItem> recordsIn = new List<Models.GMItem>();
            var fred = TempData["path"].ToString();
            //var filesData = Directory.GetFiles(@fred);
            string path = Server.MapPath("~/App_Data/" + fred);
            //string path = Server.MapPath(fred.ToString());
            var package = new OfficeOpenXml.ExcelPackage(new FileInfo(path));

            OfficeOpenXml.ExcelWorksheet workSheet = package.Workbook.Worksheets[1];


            for (int row = (workSheet.Dimension.Start.Row + 1);
                 row <= workSheet.Dimension.End.Row;
                 row++)
            {
                if (HasData(workSheet, row))
                {
                    Models.GMItem obj = new Models.GMItem();
                    obj.PBFSCode = GetPBFSC(workSheet, row, PBFSC);
                    obj.Name = GetName(workSheet, row, Name);
                    obj.GrowingQuantity = GetGrowingQuantity(workSheet, row, GrowingQuantity) ?? 0;
                    obj.Form = GetFormDecription(workSheet, row, Form);
                    obj.Size = GetSizeDecription(workSheet, row,Size);
                    obj.AllocatedQuantity = GetAllocatedQuantity(workSheet, row, AllocatedQuantity)?? 0;
                    obj.WholeSalePrice = GetPrice(workSheet, row, WholeSalePrice);
                    obj.Location = GetLocation(workSheet,row, Location);
                    obj.Sku = GetPBSKU(workSheet, row, Sku);
                    obj.StockQuantity = GetStockQuantity(workSheet, row, StockQuantity)?? 0;
                    recordsIn.Add(obj);

                }
            }
            return recordsIn;
        }

        private static bool HasData(ExcelWorksheet workSheet, int row)
        {
            if ((workSheet.Cells[row, workSheet.Dimension.Start.Column + 0].Value != null) && (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // 2040060C5
        private static string GetPBFSC(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // ABEGOUCH
        private static string GetPBSKU(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // Abelia 'Edward Goucher'
        private static string GetName(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 10-20
        private static string GetSizeDecription(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // c20
        private static string GetFormDecription(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }
        
        // E23
        private static string GetLocation(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 6
        private static int? GetStockQuantity(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value);
            }
            else
            {
                return null;
            }
        }

        // 6
        private static int? GetAllocatedQuantity(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value);
            }
            else
            {
                return null;
            }
        }

        private static int? GetGrowingQuantity(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                return Convert.ToInt32(workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value);
            }
            else
            {
                return null;
            }
        }

        // 3.23
        private static decimal GetPrice(ExcelWorksheet workSheet, int row, int position)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value != null)
            {
                var data = workSheet.Cells[row, workSheet.Dimension.Start.Column + position].Value;
                return Convert.ToDecimal(data);
            }
            else
            {
                return 0.0m;
            }
        }
    }
}
