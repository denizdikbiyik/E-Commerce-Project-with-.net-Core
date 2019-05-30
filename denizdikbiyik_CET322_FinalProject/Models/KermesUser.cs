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
    public class KermesUser : IdentityUser
    {
        [Required(ErrorMessage = "İsim girilmesi zorunludur!")]
        [StringLength(100)]
        public string UserFirstName { get; set; }
        [Required(ErrorMessage = "Soyisim girilmesi zorunludur!")]
        [StringLength(100)]
        public string UserLastName { get; set; }
        public string UserTelNo { get; set; }
        public string UserImageUrl { get; set; }
        public string UserContent { get; set; }
        public DateTime UserCreatedDate { get; set; }
    }
}
