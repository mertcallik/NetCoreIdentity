using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetIdentityApp.Models;
using NetIdentityApp.ViewModels;

namespace NetIdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IEmailSender emailSender, ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                ModelState.AddModelError("", "Hatalı email ya da parola.");
                return View(model);
            }

            await _signInManager.SignOutAsync();

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Email adresinizi onaylayın");
                return View(model);
            }

            //if (!await _userManager.IsPhoneNumberConfirmedAsync(user))
            //{
            //    ModelState.AddModelError("", "Telefon numaranızı doğrulamalısınız");
            //    return View(model);
            //}

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password!, model.RememberMe, true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.RequiresTwoFactor)
                {
                    ModelState.AddModelError("", "Öncelikle e-Mail hesabınıza gönderilen bağlantıya tıklayın");
                    return View(model);
                }
                if (signInResult.IsLockedOut)
                {
                    var lockedOutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockedOutEndDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Ard Arda çok fazla deneme yaptınız,Lütfen {timeLeft.Minutes} dakika sonra tekrar deneyiniz");
                    return View(model);
                }
                ModelState.AddModelError("", "Hatalı email yada parola");
                return View(model);
            }


            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                await _userManager.AddClaimsAsync(user,new List<Claim>()
                    { new Claim(ClaimTypes.UserData,"admin.png"),new Claim(ClaimTypes.Name, user.UserName ?? ""), new Claim(ClaimTypes.Email, user.Email ?? ""), new Claim(ClaimTypes.GivenName, user.FullName ?? "") }
                );

            }

            if (await _userManager.IsInRoleAsync(user, "user"))
            {
                await _userManager.AddClaimsAsync(user, new List<Claim>() 
                    { new Claim(ClaimTypes.UserData,"user.png"),new Claim(ClaimTypes.Name, user.UserName ?? ""), new Claim(ClaimTypes.Email, user.Email ?? ""), new Claim(ClaimTypes.GivenName, user.FullName ?? "") }
                );

            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser()
            {
                Email = model.Email,
                FullName = model.FullName,
                UserName = model.UserName
            };
            var uservalidation = new UserValidator<AppUser>();
            var userValidationResult = await uservalidation.ValidateAsync(_userManager, user);
            if (!userValidationResult.Succeeded)
            {
                userValidationResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err.Description));
                return View(model);
            }

            var passwordValidation = new PasswordValidator<AppUser>();
            var passwordValidationResult = await passwordValidation.ValidateAsync(_userManager, user, model.Password);
            if (!passwordValidationResult.Succeeded)
            {
                passwordValidationResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err.Description));
                return View(model);
            }
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err.Description));
                return View(model);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();


            var url = Url.Action("ConfirmEmail", new { token, user.Id });
            _emailSender.SendEmailAsync(user.Email, "Hesabınızı onaylayın", $"Lütfen <a href='https://localhost:7150{url}'>tıklayınız</a>");
            TempData["message"] = $"Hesabınız oluşturuldu, mail adresinize giderek hesabınızı onaylayınız.";
            var smsmodel = new ConfirmPhoneNumberViewModel()
            {
                UserId = user.Id,
                Token = code
            };
            //_smsSender.SendSmsAsync("+905414625256", code);
            return RedirectToAction("ConfirmPhoneNumber", smsmodel);
        }

        public IActionResult ConfirmPhoneNumber(ConfirmPhoneNumberViewModel smsModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Doğrulama kodu alanı boş geçilemez");
                return View();
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CheckPhoneNumber(ConfirmPhoneNumberViewModel smsModel)
        {
            var user = await _userManager.FindByIdAsync(smsModel.UserId);
            if (smsModel.TokenFromUser == smsModel.Token)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ConfirmEmail(string? token, string? id)
        {
            if (token == null || id == null)
            {
                TempData["message"] = "Geçersiz token";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["message"] = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Home");
            }
            var confirmToken = await _userManager.ConfirmEmailAsync(user!, token!);
            if (!confirmToken.Succeeded)
            {
                TempData["message"] = "Hesap onaylanamadı";
            }
            TempData["message"] = "Hesabınız onaylandı";
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(PasswordForgotViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = "Email adresinizi giriniz.";

                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email ?? "");
            if (user == null)
            {
                TempData["message"] = "Email adresinizle ilişkili Kullanıcı bulunamadı.";

                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", new { token = token, userId = user.Id });
            await _emailSender.SendEmailAsync(user.Email, "Şifrenizi Sıfırlayabilirsiniz", $"<a href='https://localhost:7150{url}'>Şifrenizi Sıfırlamak için Tıklayınız</a>");
            TempData["message"] = "Email adresinize gönderilen bağlantı ile şifrenizi yenileyebilirsiniz.";

            return RedirectToAction("Login");
        }

        public IActionResult ResetPassword(string token, string userId)
        {
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(userId))
            {
                TempData["message"] = "Geçersiz token";
                return RedirectToAction("Login");
            }

            var model = new PasswordResetViewModel()
            {
                Code = token,
                Email = _userManager.FindByIdAsync(userId).Result.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["message"] = "Bu mail adresiyle eşleşen bir kullanıcı yok";
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err => TempData["message"] = err.Description);
            }

            TempData["message"] = "Şifreniz Başarıyla Güncellendi";
            return RedirectToAction("Login");
        }


    }

}

