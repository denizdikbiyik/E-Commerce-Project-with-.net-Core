using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace denizdikbiyik_CET322_FinalProject.Models
{
    public class SaledProducts
    {
        public int SaledProductsId { get; set; }
        public int SalesId { get; set; }
        public virtual Sales Sales { get; set; }
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }
        public int SaledProductCount { get; set; }
        public decimal TotalPrice { get; set; }  
    }
}
