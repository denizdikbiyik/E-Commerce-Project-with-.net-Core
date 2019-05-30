using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using denizdikbiyik_CET322_FinalProject.Data;
using denizdikbiyik_CET322_FinalProject.Models;
using denizdikbiyik_CET322_FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace denizdikbiyik_CET322_FinalProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<KermesUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(ApplicationDbContext context, UserManager<KermesUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> Index()
        {
            var userList = _context.Users.ToList();

            List<UserModel> userModelList = new List<UserModel>();

            foreach (var item in userList)
            {
                bool isadmin = await _userManager.IsInRoleAsync(item, "admin");
                var user = new UserModel
                {
                    UserId = item.Id,
                    UserName = item.UserName,
                    UserFullName = item.UserFirstName + " " + item.UserLastName,
                    UserIsAdmin = isadmin,
                };
                userModelList.Add(user);
            }
            return View(userModelList);
        }

        public async Task<ActionResult> MakeAdmin(string id)
        {
            if (!(await _roleManager.RoleExistsAsync("admin")))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });
            }
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.AddToRoleAsync(user, "admin");
            return RedirectToAction("index");
        }

        public async Task<ActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "admin");
            return RedirectToAction("index");
        }
    }
}