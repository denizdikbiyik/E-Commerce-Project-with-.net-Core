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
    public class Sales
    {
        public int SalesId { get; set; }
        public string KermesUserId { get; set; }
        public virtual KermesUser KermesUser { get; set; }
        public string CargoAddress { get; set; }
        [Phone]
        public string ContactTelNo { get; set; }
        public virtual IList<SaledProducts> SaledProducts { get; set; }
        
        public DateTime SaledDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public int ProductCountSales { get; set; }
    }
}
