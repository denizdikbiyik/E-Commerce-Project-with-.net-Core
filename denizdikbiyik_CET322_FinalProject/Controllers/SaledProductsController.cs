using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using denizdikbiyik_CET322_FinalProject.Data;
using denizdikbiyik_CET322_FinalProject.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace denizdikbiyik_CET322_FinalProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class SaledProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaledProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToSaledProducts(int SalesId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            var sItem = _context.SaledProducts.FirstOrDefault(s => s.SalesId == SalesId);
            SaledProducts item = new SaledProducts();
            item.SalesId = SalesId;
            _context.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: SaledProducts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SaledProducts.Include(s => s.Product).Include(s => s.Sales);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SaledProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saledProducts = await _context.SaledProducts
                .Include(s => s.Product)
                .Include(s => s.Sales)
                .FirstOrDefaultAsync(m => m.SaledProductsId == id);
            if (saledProducts == null)
            {
                return NotFound();
            }

            return View(saledProducts);
        }

        // GET: SaledProducts/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName");
            ViewData["SalesId"] = new SelectList(_context.Sales, "SalesId", "SalesId");
            return View();
        }

        // POST: SaledProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaledProductsId,SalesId,ProductId,SaledProductCount,TotalPrice")] SaledProducts saledProducts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saledProducts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", saledProducts.ProductId);
            ViewData["SalesId"] = new SelectList(_context.Sales, "SalesId", "SalesId", saledProducts.SalesId);
            return View(saledProducts);
        }

        // GET: SaledProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saledProducts = await _context.SaledProducts.FindAsync(id);
            if (saledProducts == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", saledProducts.ProductId);
            ViewData["SalesId"] = new SelectList(_context.Sales, "SalesId", "SalesId", saledProducts.SalesId);
            return View(saledProducts);
        }

        // POST: SaledProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaledProductsId,SalesId,ProductId,SaledProductCount,TotalPrice")] SaledProducts saledProducts)
        {
            if (id != saledProducts.SaledProductsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saledProducts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaledProductsExists(saledProducts.SaledProductsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", saledProducts.ProductId);
            ViewData["SalesId"] = new SelectList(_context.Sales, "SalesId", "SalesId", saledProducts.SalesId);
            return View(saledProducts);
        }

        // GET: SaledProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saledProducts = await _context.SaledProducts
                .Include(s => s.Product)
                .Include(s => s.Sales)
                .FirstOrDefaultAsync(m => m.SaledProductsId == id);
            if (saledProducts == null)
            {
                return NotFound();
            }

            return View(saledProducts);
        }

        // POST: SaledProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saledProducts = await _context.SaledProducts.FindAsync(id);
            _context.SaledProducts.Remove(saledProducts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaledProductsExists(int id)
        {
            return _context.SaledProducts.Any(e => e.SaledProductsId == id);
        }
    }
}
