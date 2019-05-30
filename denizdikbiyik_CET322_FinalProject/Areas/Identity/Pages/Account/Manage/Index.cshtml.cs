using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using denizdikbiyik_CET322_FinalProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace denizdikbiyik_CET322_FinalProject.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<KermesUser> _userManager;
        private readonly SignInManager<KermesUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;

        public IndexModel(
            UserManager<KermesUser> userManager,
            SignInManager<KermesUser> signInManager,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

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

            public IFormFile FileUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Şu kullanıcıya erişilemiyor: '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var kermesuser = await _userManager.FindByNameAsync(userName);         
            var email = kermesuser.Email;
            var phoneNumber = kermesuser.UserTelNo;
            var firstName = kermesuser.UserFirstName;
            var lastName = kermesuser.UserLastName;
            var imageurl = kermesuser.UserImageUrl;
            var content = kermesuser.UserContent;

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                UserTelNo = phoneNumber,
                UserFirstName = firstName,
                UserLastName = lastName,
                UserImageUrl = imageurl,
                UserContent = content
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Şu kullanıcıya erişilemiyor: '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Şu kullanıcı için e-posta eklenirken bilinmeyen bir hata oluştu: '{userId}'.");
                }
            }

            user.UserFirstName = Input.UserFirstName;
            user.UserLastName = Input.UserLastName;
            user.UserContent = Input.UserContent;
            user.UserTelNo = Input.UserTelNo;

            if (Request.Form.Files?.Count>0) {
                Input.FileUrl = Request.Form.Files[0];
            string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + Input.FileUrl.FileName;
            using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create))
            {
                await Input.FileUrl.CopyToAsync(fileStream);
            }
            user.UserImageUrl = fileName;
            }
            await _userManager.UpdateAsync(user);          

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.UserTelNo != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.UserTelNo);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Şu kullanıcı için telefon numarası eklenirken bilinmeyen bir hata oluştu: '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profiliniz güncellendi";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Şu kullanıcıya erişilemiyor: '{_userManager.GetUserId(User)}'.");
            }
            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "E-Postanızı doğrulayın",
                $"Lütfen hesabınızı doğrulamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayınız</a>.");

            StatusMessage = "Doğrulama E-postası gönderildi. Lütfen e-posta adresinizi kontrol edin.";
            return RedirectToPage();
        }
    }
}
