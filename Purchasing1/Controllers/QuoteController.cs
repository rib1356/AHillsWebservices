using System.Linq;
using System.Web.Mvc;
using Purchasing1.Models;
namespace Purchasing1.Controllers
{
   public class QuoteController : Controller
   {
        private HillsStockPEntities db = new HillsStockPEntities();
        public ActionResult Index()
        {

            ViewBag.dataSource = db.Quotes.ToList();

            return View();
        }

    }
}