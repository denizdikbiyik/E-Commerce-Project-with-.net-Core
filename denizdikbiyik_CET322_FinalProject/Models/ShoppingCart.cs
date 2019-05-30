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
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public string KermesUserId { get; set; }
        public virtual KermesUser KermesUser { get; set; }
        public virtual Product Product { get; set; }
        public virtual int ProductId { get; set; }
        public int ProductCount { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
