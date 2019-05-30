using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace denizdikbiyik_CET322_FinalProject.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<KermesUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<KermesUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Şu kişiye erişilemiyor: '{_userManager.GetUserId(User)}'.");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"İki faktörlü doğrulama şu anda engellenemiyor: '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Şu kişiye erişilemiyor: '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Şu kişi için iki faktörlü doğrulama kaldırılırken bilinmeyen bir hata oluştu: '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("Şu kullanıcı: '{UserId}' iki faktörlü doğrulamayı kaldırdı.", _userManager.GetUserId(User));
            StatusMessage = "İki faktörlü doğrulama kaldırıldı. Kimlik uygulaması kurduğunuzda tekrar izin verebilirsiniz.";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}