using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ImportServiceCore.Model;
namespace AzureTestApp.Controllers
{

   public class BatchController : Controller
   {
        private HillsContext _context;

		public BatchController(HillsContext Context)
		{
            this._context=Context;
		}

        public ActionResult Index()
        {

            ViewBag.dataSource = _context.Batch.ToList();

            return View();
        }

    }
}