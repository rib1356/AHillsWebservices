using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ImportServiceCore.Model;
using System.Collections.Generic;
//using AutoMapper;

namespace ImportServiceCore.Controllers
{

   public class TestController : Controller
   {

        private readonly Repo.IBatchRep _context;
      //  private readonly IMapper _mapper;

        public TestController(Repo.IBatchRep context)
        {
            this._context = context;
        }

  //      private HillsContext _context;

		//public TestController(HillsContext Context)
		//{
  //          this._context=Context;
		//}

        /// <summary>
        /// Builds a VM
        /// </summary>
        /// <param name="batches"></param>
        /// <returns></returns>
        private static List<ViewModels.BatchVM> buildVM(List<DTO.BatchDTO> batches)
        {
            return batches.Select(b => new ViewModels.BatchVM
            {
                BatchId = b.Id,
                Sku = b.Sku,
                Name = b.Name,
                FormSize = b.FormSize,
                Quantity = b.Quantity,
                Location = b.Location,
                WholesalePrice = b.WholesalePrice
            }).ToList();
        }

        public ActionResult Index()
        {


            var all = _context.GetBatches().ToList();

            var vm = buildVM(all);
            //ViewBag.datasource = vm.ToList();


            ViewBag.dataSource = vm;

            return View();
        }

    }
}