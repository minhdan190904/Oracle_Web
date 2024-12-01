using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Services;
using Oracle_WEB_BTL.ViewModels;
using Oracle_WEB_BTL.Helpers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Oracle_WEB_BTL.Models;

namespace Oracle_WEB_BTL.Controllers
{
    public class AccessController : Controller
    {
        private readonly OracleContext _context;
        private readonly IEmailService _emailService;
        private static string OTPCodeTemp = "";

        public AccessController(OracleContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelLogin)
        {
            if (ModelState.IsValid)
            {
                var nhanvien = await _context.Nhanviens
                    .SingleOrDefaultAsync(nv => nv.Email == modelLogin.Email && nv.Matkhau == modelLogin.Password);

                if (nhanvien != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, nhanvien.Email),
                        new Claim("IsAdmin", nhanvien.Quyenadmin.ToString()),
                        new Claim("Manv", nhanvien.Manv.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    return RedirectToAction("Index", "Dashboard");
                }

                ViewData["ValidateMessage"] = AppDefaults.InvalidLoginMessage;
            }

            return View(modelLogin);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nhanvien = await _context.Nhanviens.SingleOrDefaultAsync(nv => nv.Email == model.Email);
                if (nhanvien != null)
                {
                    OTPCodeTemp = GenerateOtp();
                    await _emailService.SendEmailAsync(nhanvien.Email, AppDefaults.ResetPasswordOtpMessage, AppDefaults.GetOtpMessage(OTPCodeTemp));
                    return RedirectToAction("ResetPasswordConfirm", new { email = model.Email });
                }
                else
                {
                    ViewData["Message"] = AppDefaults.EmailNotFoundMessage;
                }
            }
            return View("ResetPassword", model);
        }

        public IActionResult ResetPasswordConfirm(string email)
        {
            var model = new ResetPasswordConfirmViewModel
            {
                Email = email,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nhanvien = await _context.Nhanviens.SingleOrDefaultAsync(nv => nv.Email == model.Email);
                if (nhanvien != null)
                {
                    if (model.OtpCode == OTPCodeTemp)
                    {
                        nhanvien.Matkhau = model.NewPassword;
                        _context.Nhanviens.Update(nhanvien);
                        await _context.SaveChangesAsync();

                        ViewData["Message"] = AppDefaults.PasswordUpdatedMessage;
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewData["Message"] = AppDefaults.InvalidOtpMessage;
                    }
                }
                else
                {
                    ViewData["Message"] = AppDefaults.EmailNotFoundMessage;
                }
            }
            return View(model);
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
