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
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            var applicationDbContext = _context.ShoppingCart.Include(s => s.KermesUser).Include(s => s.Product).Where(e=>e.KermesUserId==userId);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "admin")]
        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCart
                .Include(s => s.KermesUser)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.ShoppingCartId == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create
        public IActionResult AddToBasket(int productId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            var eItem = _context.ShoppingCart.FirstOrDefault(s => s.ProductId == productId && s.KermesUserId == userId);
            if(eItem==null) {
            ShoppingCart item = new ShoppingCart();
            item.AddedDate = DateTime.Now;
            item.KermesUserId = userId;
            item.ProductId = productId;
            item.ProductCount = 1;
                _context.Add(item);
            } else
            {
                eItem.ProductCount = eItem.ProductCount + 1;
                _context.Update(eItem);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteFromBasket(int productId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            var eItem = _context.ShoppingCart.FirstOrDefault(s => s.ProductId == productId && s.KermesUserId == userId);
            if (eItem.ProductCount == 1)
            {
                _context.ShoppingCart.Remove(eItem);
            }
            else
            {
                eItem.ProductCount = eItem.ProductCount - 1;
                _context.Update(eItem);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoppingCartId,KermesUserId,ProductId,ProductCount,AddedDate")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", shoppingCart.KermesUserId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        [Authorize(Roles = "admin")]
        // GET: ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", shoppingCart.KermesUserId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        [Authorize(Roles = "admin")]
        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoppingCartId,KermesUserId,ProductId,ProductCount,AddedDate")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.ShoppingCartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.ShoppingCartId))
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
            ViewData["KermesUserId"] = new SelectList(_context.Users, "Id", "Id", shoppingCart.KermesUserId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        [Authorize(Roles = "admin")]
        // GET: ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            _context.ShoppingCart.Remove(shoppingCart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCart.Any(e => e.ShoppingCartId == id);
        }
    }
}
