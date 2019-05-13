

using ImportService.Models;
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
                //AddtoDb
            }

            ViewBag.Title = "Home Page";

            return View();
        }

        private static string GetFieldX(OfficeOpenXml.ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column].Value != null)
            {
                return workSheet.Cells[row, workSheet.Dimension.Start.Column].Value.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}