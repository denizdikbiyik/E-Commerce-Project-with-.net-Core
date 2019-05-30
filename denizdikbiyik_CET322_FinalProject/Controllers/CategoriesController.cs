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
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Category.Include(c=>c.KermesUser).ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.Include(c => c.Products).FirstOrDefaultAsync(m => m.CategoryId == id);     

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryCreatedDate,KermesUserId")] Category category)
        {
            category.CategoryCreatedDate = DateTime.Now;
            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            category.KermesUserId = loginUser?.Id;
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }         
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (category.KermesUserId != loginUser.Id)
            {
                return Unauthorized();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,CategoryCreatedDate,KermesUserId")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentCategory = await _context.Category.Include(p => p.KermesUser).FirstOrDefaultAsync(p => p.CategoryId == category.CategoryId);
                    if (!(currentCategory.KermesUser?.UserName == User.Identity.Name || User.IsInRole("admin")))
                    {
                        return Unauthorized();

                    }
                    currentCategory.CategoryCreatedDate = DateTime.Now;
                    currentCategory.CategoryName = category.CategoryName;
                    currentCategory.KermesUserId = category.KermesUserId;
                    _context.Category.Update(currentCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FirstOrDefaultAsync(m => m.CategoryId == id);
            var kaydeden = _context.Users.FirstOrDefaultAsync(u => u.Id == category.KermesUserId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.CategoryId == id);
        }
    }
}
