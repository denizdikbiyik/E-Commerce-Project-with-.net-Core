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
    public class Product
    {
        public int ProductId { get; set; }   
        [Required(ErrorMessage = "Ürün adı girilmesi zorunludur!")]
        public string ProductName { get; set; }
     
        public string ProductImageUrl { get; set; }
        public string ProductContent { get; set; }
        [DisplayFormat(DataFormatString ="{0:N}", ApplyFormatInEditMode =true)]
        [Required(ErrorMessage = "Ürün fiyatı girilmesi zorunludur!")]
        [Range(0, 1000000)]
        public decimal ProductPrice { get; set; }
        public DateTime ProductCreatedDate { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [NotMapped]
        public virtual IEnumerable<SelectListItem> categories { get; set; }
        public string KermesUserId { get; set; }
        public virtual KermesUser KermesUser { get; set; }
    }
}
