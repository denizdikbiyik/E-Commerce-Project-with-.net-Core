using System;
using System.Collections.Generic;
using System.Text;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using denizdikbiyik_CET322_FinalProject.ViewModels;

namespace denizdikbiyik_CET322_FinalProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<KermesUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<denizdikbiyik_CET322_FinalProject.Models.Product> Product { get; set; }
        public DbSet<denizdikbiyik_CET322_FinalProject.Models.Category> Category { get; set; }
        public DbSet<denizdikbiyik_CET322_FinalProject.Models.ShoppingCart> ShoppingCart { get; set; }
        public DbSet<denizdikbiyik_CET322_FinalProject.Models.Sales> Sales { get; set; }
        public DbSet<denizdikbiyik_CET322_FinalProject.Models.SaledProducts> SaledProducts { get; set; }
    }
}
