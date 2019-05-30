using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using denizdikbiyik_CET322_FinalProject.Data;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace denizdikbiyik_CET322_FinalProject.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToSales([Bind("SalesId,KermesUserId,CargoAddress,ContactTelNo,SaledDate,TotalPrice,Status, ProductCountSales")] Sales sales)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            var sItem = _context.ShoppingCart.FirstOrDefault(s => s.KermesUserId == userId);
            var aItem = _context.ShoppingCart.FirstOrDefault(s => s.ProductCount == sales.ProductCountSales);
            var bItem = _context.ShoppingCart.FirstOrDefault(s => s.ProductId == sales.SalesId);
            var cItem = _context.ShoppingCart.FirstOrDefault(s => s.ProductCount == sales.ProductCountSales);
            Sales item = new Sales();
                item.SaledDate = DateTime.Now;
                item.KermesUserId = userId;
                item.CargoAddress = sales.CargoAddress;
                item.ContactTelNo = sales.ContactTelNo;
            item.SaledProducts = sales.SaledProducts;
            item.ProductCountSales = sales.ProductCountSales;
            item.SalesId = sales.SalesId;
                _context.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales.Include(s => s.KermesUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.KermesUser)
                .FirstOrDefaultAsync(m => m.SalesId == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesId,KermesUserId,CargoAddress,ContactTelNo,SaledDate,TotalPrice,Status")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", sales.KermesUserId);
            return View(sales);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", sales.KermesUserId);
            return View(sales);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalesId,KermesUserId,CargoAddress,ContactTelNo,SaledDate,TotalPrice,Status")] Sales sales)
        {
            if (id != sales.SalesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingsales = await _context.Sales.Include(p => p.KermesUser).FirstOrDefaultAsync(p => p.SalesId == sales.SalesId);
                    existingsales.SaledDate = DateTime.Now;
                    existingsales.CargoAddress = sales.CargoAddress;
                    existingsales.ContactTelNo = sales.ContactTelNo;
                    _context.Sales.Update(existingsales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesExists(sales.SalesId))
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
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", sales.KermesUserId);
            return View(sales);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.KermesUser)
                .FirstOrDefaultAsync(m => m.SalesId == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            _context.Sales.Remove(sales);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesExists(int id)
        {
            return _context.Sales.Any(e => e.SalesId == id);
        }
    }
}
