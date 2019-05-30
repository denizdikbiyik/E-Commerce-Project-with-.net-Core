using denizdikbiyik_CET322_FinalProject.Data;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace denizdikbiyik_CET322_FinalProject.Views.Shared.Components.CategoryMenu
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryMenuViewComponent(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await dbContext.Category.ToListAsync();
            return View(categories);
        }
    }
}
