using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using denizdikbiyik_CET322_FinalProject.Data;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace denizdikbiyik_CET322_FinalProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var productsContext = _context.Product.Include(p => p.Category).Include(p => p.KermesUser);
            return View(await productsContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.Include(p => p.Category).Include(p=>p.KermesUser).FirstOrDefaultAsync(m => m.ProductId == id);
            var kaydeden = _context.Users.FirstOrDefaultAsync(u => u.Id == product.KermesUserId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category select Category).ToList();
            Product product = new Product();
            product.categories = GetCategories(categorylist);
            return View(product);
        }
        private IEnumerable<SelectListItem> GetCategories(IEnumerable<Category> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.CategoryId.ToString(),
                    Text = element.CategoryName
                });
            }

            return selectList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, Product productmodel, IFormFile FileUrl)
        {            
            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            productmodel.KermesUserId = loginUser?.Id;

         if (ModelState.IsValid)
            {
               
                Product product = new Product();
                product.ProductName = productmodel.ProductName;
                 product.ProductContent = productmodel.ProductContent;
                product.ProductPrice = productmodel.ProductPrice;
                product.CategoryId = productmodel.CategoryId;
                product.KermesUserId = productmodel.KermesUserId;
                product.ProductCreatedDate = DateTime.Now;


                if (FileUrl != null)
                {
                    string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + Path.GetFileName(FileUrl.FileName);
                    using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create))
                    {
                        await FileUrl.CopyToAsync(fileStream);
                    }
                    product.ProductImageUrl = fileName;
                }

              

                _context.Product.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category select Category).ToList();
            
            productmodel.categories = GetCategories(categorylist);
            return View(productmodel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            List<Category> categorylist = new List<Category>();
            categorylist = _context.Category.ToList();
            product.categories = GetCategories(categorylist);

            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (product.KermesUserId != loginUser.Id)
            {
                return Unauthorized();
            }

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductImageUrl,ProductContent,ProductPrice,ProductCreatedDate,CategoryId,KermesUserId")] Product product, IFormFile FileUrl)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingproduct = await _context.Product.Include(p => p.KermesUser).FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                    if (!(existingproduct.KermesUser?.UserName == User.Identity.Name || User.IsInRole("admin")))
                    {
                        return Unauthorized();

                    }

                    if (FileUrl != null)
                    {
                        string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + Path.GetFileName(FileUrl.FileName);
                        using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create))
                        {
                            await FileUrl.CopyToAsync(fileStream);
                        }
                        existingproduct.ProductImageUrl = fileName;
                    }

                    existingproduct.ProductCreatedDate = DateTime.Now;
                    existingproduct.ProductPrice = product.ProductPrice;
                    existingproduct.ProductName = product.ProductName;
                    existingproduct.ProductContent = product.ProductContent;
                    _context.Product.Update(existingproduct);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.Include(p => p.Category).FirstOrDefaultAsync(m => m.ProductId == id);
            var kaydeden = _context.Users.FirstOrDefaultAsync(u => u.Id == product.KermesUserId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
