using deneme2._0.Data;
using deneme2._0.Models;
using deneme2._0.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace deneme2._0.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AccountController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        // ========== LOGIN ==========

        [HttpGet]
        public IActionResult Login()
        {
            if (_authService.IsUserLoggedIn())
                return RedirectToAction("Index", "Journal");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model.Username, model.Password, model.RememberMe);

                if (result)
                    return RedirectToAction("Index", "Journal");

                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
            }

            return View(model);
        }

        // ========== REGISTER ==========

        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsUserLoggedIn())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model.Username, model.Password);

                if (result)
                {
                    TempData["SuccessMessage"] = "Kayıt başarıyla oluşturuldu. Lütfen giriş yapın.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Bu kullanıcı adı zaten alınmış");
            }

            return View(model);
        }

        // ========== LOGOUT ==========

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            
            return RedirectToAction("Index", "Home");
        }

        // ========== KULLANICI ADI GÜNCELLEME ==========

        [HttpPost]
        public async Task<IActionResult> UpdateUsername(string newUsername)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Json(new { success = false, message = "Oturum kapalı." });

            if (string.IsNullOrEmpty(newUsername) || newUsername.Trim().Length < 3)
                return Json(new { success = false, message = "Kullanıcı adı en az 3 karakter olmalıdır." });

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            var trimmedName = newUsername.Trim();

            if (user.Username == trimmedName)
                return Json(new { success = false, message = "Yeni kullanıcı adı eskisiyle aynı olamaz." });

            var isUsernameTaken = await _context.Users.AnyAsync(u => u.Username == trimmedName && u.Id != userId.Value);
            if (isUsernameTaken)
                return Json(new { success = false, message = "Bu kullanıcı adı zaten alınmış." });

            user.Username = trimmedName;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            return Json(new { success = true });
        }

        // ========== ŞİFRE GÜNCELLEME ==========

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Json(new { success = false, message = "Oturum kapalı." });

            var user = await _context.Users.FindAsync(userId.Value);

            if (user != null && user.PasswordHash == oldPassword)
            {
                if (oldPassword == newPassword)
                    return Json(new { success = false, message = "Yeni şifre eskisiyle aynı olamaz." });

                if (newPassword != confirmNewPassword)
                    return Json(new { success = false, message = "Yeni şifre ve tekrar şifre eşleşmiyor." });

                if (string.IsNullOrEmpty(newPassword) || newPassword.Trim().Length < 4)
                    return Json(new { success = false, message = "Sifre en az 4 karakter olmalıdır." });

                user.PasswordHash = newPassword;
                await _context.SaveChangesAsync();

                
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Mevcut şifreniz hatalı." });
        }

        // ========== YARDIMCI METOT ==========

        private int? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return string.IsNullOrEmpty(userIdString) ? null : int.Parse(userIdString);
        }
    }
}