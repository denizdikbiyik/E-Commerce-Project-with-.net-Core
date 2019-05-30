using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace denizdikbiyik_CET322_FinalProject.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CategoryCreatedDate { get; set; }
        public virtual IList<Product> Products { get; set; }
        public string KermesUserId { get; set; }
        public virtual KermesUser KermesUser { get; set; }
    }
}
