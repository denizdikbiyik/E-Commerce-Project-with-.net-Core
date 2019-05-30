using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace denizdikbiyik_CET322_FinalProject.ViewModels
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public bool UserIsAdmin { get; set; }
    }
}
