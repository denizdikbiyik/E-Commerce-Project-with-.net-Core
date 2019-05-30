using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace denizdikbiyik_CET322_FinalProject.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<KermesUser> _signInManager;
        private readonly UserManager<KermesUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;

        public RegisterModel(
            UserManager<KermesUser> userManager,
            SignInManager<KermesUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "İsim girilmesi zorunludur.")]
            [StringLength(100)]
            public string UserFirstName { get; set; }

            [Required(ErrorMessage = "Soyisim girilmesi zorunludur.")]
            [StringLength(100)]
            public string UserLastName { get; set; }

            [Required(ErrorMessage = "E-Posta girilmesi zorunludur.")]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            public string UserTelNo { get; set; }

            [DataType(DataType.ImageUrl)]
            public string UserImageUrl { get; set; }

            public string UserContent { get; set; }

            public DateTime UserCreatedDate { get; set; }

            [Required(ErrorMessage = "Şifre girilmesi zorunludur.")]
            [StringLength(100, ErrorMessage = "{0} en az {2} ve maksimum {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Doğrulama şifresi girilmesi zorunludur.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new KermesUser { UserName = Input.Email, Email = Input.Email };
                user.UserCreatedDate = DateTime.Now;
                user.UserFirstName = Input.UserFirstName;
                user.UserLastName = Input.UserLastName;
                user.UserTelNo = Input.UserTelNo;
                user.UserContent = Input.UserContent;

                if(Request.Form.Files?.Count>0) {

                IFormFile FileUrl = Request.Form.Files[0];
                string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
                    //var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + FileUrl.FileName;
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + Path.GetFileName(FileUrl.FileName);
                    using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create))
                {
                    await FileUrl.CopyToAsync(fileStream);
                }
                    user.UserImageUrl = fileName;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı şifreyle yeni bir hesap oluşturdu.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "E-Postanızı doğrulayın.",
                        $"Hesabınızı şu şekilde doğrulayın: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
