

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
                //ViewBag.Error = ex.InnerException.Message;
                return View("version");
            }


            try
            {
                // db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Pannebakker]");
                db.EmptyImport();

                // db.BulkInsert<Pannebakker>(newRecords);
                db.BulkInsert(recordsIn);
                db.RemoveDuplicateImport();
                //AddBatch(records);
                db.MergeImportToPB();
                db.cleanForms();
                db.RemoveDuplicatePB();
                db.RemoveDuplicateBatch();
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