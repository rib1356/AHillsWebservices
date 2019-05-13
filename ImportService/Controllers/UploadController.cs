

using ImportService.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload

        private HillsStockImportEntities db = new HillsStockImportEntities();
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

            ViewBag.Title = "Home Page";

            return RedirectToAction("UpLoad");
        }


        public ActionResult UpLoad()
        {


            var fred = TempData["path"].ToString();
            //var filesData = Directory.GetFiles(@fred);
            string path = Server.MapPath("~/App_Data/" + fred);
            //string path = Server.MapPath(fred.ToString());
            var package = new OfficeOpenXml.ExcelPackage(new FileInfo(path));

            OfficeOpenXml.ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

           // var allrunners = db.runners.Where(r => r.Active == true).ToList();
            
            for (int row = workSheet.Dimension.Start.Row;
                     row <= workSheet.Dimension.End.Row;
                     row++)
            {
                Pannebakker obj = new Pannebakker();
                obj.Sku = GetPBSKU(workSheet, row);
                obj.FormSizeCode = GetPBFSCOde(workSheet, row);
                obj.Name = GetName(workSheet, row);
                obj.FormSize = GetFSDecription(workSheet, row);
                obj.Price = GetPrice(workSheet, row);
                db.Pannebakkers.Add(obj);
            }
            try
            {
                db.SaveChanges();
                ViewBag.Title = "done";
                return RedirectToAction("Index");

            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
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