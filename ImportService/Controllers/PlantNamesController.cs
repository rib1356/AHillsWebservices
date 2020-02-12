using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImportModel;
using ImportRep;
using ImportService.DTO;
using ImportService.Models;
using OfficeOpenXml;
using PagedList;

namespace ImportService.Controllers
{
    public class PlantNamesController : Controller
    {
        private ImportEntities db = new ImportEntities();


        private IImportRepository RepDb;

        public PlantNamesController()
        {
            this.RepDb = new ImportRepository(new ImportModel.ImportEntities());
        }

        // GET: PlantNames
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "nameD" : "";
            ViewBag.SkuSortParm = sortOrder == "sku" ? "skuD" : "sku";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
           // List<PlantNameDTO> vm = new List<PlantNameDTO>();
            List<PlantName> vm = new List<PlantName>();
            var allPlantNames = db.PlantNames;
           // var allPlantNames = ServiceLayer.PlantNameService.GetNames();

            if (!String.IsNullOrEmpty(searchString))
            {
                vm = allPlantNames.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Sku.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            else
            {
                vm = allPlantNames.ToList();
            }

            switch (sortOrder)
            {
                case "nameD":
                    vm = vm.OrderByDescending(s => s.Name).ToList();
                    break;
                case "name":
                    vm = vm.OrderBy(s => s.Name).ToList();
                    break;
                case "sku":
                    vm = vm.OrderBy(s => s.Sku).ToList();
                    break;
                case "skuD":
                    vm = vm.OrderByDescending(s => s.Sku).ToList();
                    break;
                default:
                    vm = vm.OrderBy(s => s.Name).ToList();
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(vm.ToPagedList(pageNumber, pageSize));
        }

        // GET: PlantNames/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // GET: PlantNames/Create
        public ActionResult Create()
        {
            return View();
        }

        #region fileUpload

        public ActionResult GetStockFile(HttpPostedFileBase file)
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

            return RedirectToAction("UpLoad");
        }

        public ActionResult UpLoad()
        {

            ImportSet importSet = new ImportSet();
            List<ImportModel.rawImport> recordsIn = new List<ImportModel.rawImport>();
            List<ImportModel.Batch> batchIn = new List<ImportModel.Batch>();
            //List<ImportModel.Pannebakker> existingRecords = new List<ImportModel.Pannebakker>();
            //List<ImportModel.rawImport> newRecords = new List<ImportModel.rawImport>();

            try
            {
                importSet = ReadInputFile();
                recordsIn = importSet.rawImport;
                batchIn = importSet.batchImport;
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
                // "TRUNCATE TABLE [rawImport]"]");
                RepDb.EmptyImport();

                // db.BulkInsert<Pannebakker>(newRecords);
                RepDb.BulkInsert(recordsIn);
                RepDb.BulkInsertGMBatch(batchIn);
               // RepDb.RemoveDuplicateNames();
                //AddBatch(records);
                RepDb.MergeImportToNames();
                RepDb.RemoveDuplicateNames();
                ViewBag.Title = "done";
                Response.Write("<script>console.log('Data has been saved to db');</script>");
                //return View("Index");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException.Message;
                return View("shit");
            }

        }


        private ImportService.DTO.ImportSet ReadInputFile()
        {
            ImportService.DTO.ImportSet outputSet = new ImportSet();
            List<ImportModel.rawImport> recordsIn = new List<ImportModel.rawImport>();
            List<ImportModel.Batch> batchIn = new List<ImportModel.Batch>();
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
                            ImportModel.rawImport obj = new ImportModel.rawImport();
                            ImportModel.Batch batch = new Batch();
                            obj.Sku = GetPBSKU(workSheet, row);
                            batch.Sku = obj.Sku;
                            obj.FormSizeCode = "";
                            obj.Name = GetName(workSheet, row);
                            batch.Name = obj.Name;
                            obj.FormSize =  GetSize(workSheet,row) + " " + GetForm(workSheet,row);
                            batch.FormSize = obj.FormSize;
                            var price = GetPrice(workSheet, row);
                            obj.Price = Convert.ToDecimal(price);
                            batch.WholesalePrice = System.Convert.ToInt32(obj.Price * 100);
                            batch.Active = true;
                            batch.GrowingQuantity = System.Convert.ToInt32(GetStockLevel(workSheet, row));
                            batch.DateStamp = DateTime.Now;
                            batch.AllocatedQuantity = 0;
                            batch.ImageExists = false;
                            batch.Location = GetLocation(workSheet, row);
                            recordsIn.Add(obj);
                            batchIn.Add(batch);

                        }
                    }
            outputSet.rawImport = recordsIn;
            outputSet.batchImport = batchIn;
            return outputSet;
        }

     

        private static bool HasData(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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
        private static string GetPBSKU(OfficeOpenXml.ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 0].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 0].Value).ToString();
            }
            else
            {
                return null;
            }
        }



        // Abelia 'Edward Goucher'
        private static string GetName(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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

        // Abelia 'Edward Goucher'
        private static string GetSize(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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
        private static string GetForm(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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

        // Abelia 'Edward Goucher'
        private static string GetLocation(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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

        private static string GetStockLevel(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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

        private static string GetPrice(OfficeOpenXml.ExcelWorksheet workSheet, int row)
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



        #endregion

        // POST: PlantNames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Sku,Name")] PlantName plantName)
        {
            if (ModelState.IsValid)
            {
                db.PlantNames.Add(plantName);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plantName);
        }

        // GET: PlantNames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // POST: PlantNames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlantId,Sku,Name")] PlantName plantName)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plantName).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plantName);
        }

        // GET: PlantNames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // POST: PlantNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlantName plantName = db.PlantNames.Find(id);
            db.PlantNames.Remove(plantName);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
