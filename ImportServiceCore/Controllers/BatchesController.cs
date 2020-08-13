using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImportServiceCore.Model;
using ImportServiceCore.Repo;
//using AutoMapper;

namespace ImportServiceCore.Controllers
{
    public class BatchesController : Controller
    {
        private readonly IBatchRep _repository;
      //  private readonly IMapper _mapper;

        public BatchesController(IBatchRep context)
        {
            _repository = context;
           // _mapper = mapper;
        }

        // GET: Batches
        public ActionResult Index()
        {


            var all = _repository.GetBatches().ToList();

            var vm = buildVM(all);
            //ViewBag.datasource = vm.ToList();


            ViewBag.dataSource = vm;

            return View();
        }

        public async Task<IActionResult> Testme()
        {

            var batches = _repository.GetBatches();
            var vm = buildVM(batches);
            // ViewBag.DataSource = vm.ToList();
            return View(vm);
        }

        /// <summary>
        /// Builds a VM
        /// </summary>
        /// <param name="batches"></param>
        /// <returns></returns>
        private static IEnumerable<ViewModels.BatchVM> buildVM(IEnumerable<DTO.BatchDTO> batches)
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
            }).AsEnumerable();
        }

        //// GET: Batches/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var batch = await _context.Batch
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (batch == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(batch);
        //}

        //// GET: Batches/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Batches/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Sku,Name,FormSize,Location,Quantity,WholesalePrice,ImageExists,Active,GrowingQuantity,AllocatedQuantity,DateStamp,BuyPrice,Comment")] Batch batch)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(batch);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(batch);
        //}

        //// GET: Batches/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var batch = await _context.Batch.SingleOrDefaultAsync(m => m.Id == id);
        //    if (batch == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(batch);
        //}

        //// POST: Batches/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Sku,Name,FormSize,Location,Quantity,WholesalePrice,ImageExists,Active,GrowingQuantity,AllocatedQuantity,DateStamp,BuyPrice,Comment")] Batch batch)
        //{
        //    if (id != batch.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(batch);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BatchExists(batch.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(batch);
        //}

        //// GET: Batches/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var batch = await _context.Batch
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (batch == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(batch);
        //}

        //// POST: Batches/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var batch = await _context.Batch.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Batch.Remove(batch);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool BatchExists(int id)
        //{
        //    return _context.Batch.Any(e => e.Id == id);
        //}
    }
}
