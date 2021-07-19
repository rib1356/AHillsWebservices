using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // hello mother
            return View();
        }

        public ActionResult GetIn()
        {
            var un = Request.Form["un"];
            var pw = Request.Form["pw"];
            if (un == "503141" && pw == "827191")
            {
                Session["admin"] = true;
                return RedirectToAction("Index", "StepUpload");
            }
            else
            {
                Session["admin"] = false;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}