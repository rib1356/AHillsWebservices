using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ImportServiceCore.Model;
namespace ImportServiceCore.Controllers
{

   public class crudController : Controller
   {
        private HillsContext _context;

		public crudController(HillsContext Context)
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