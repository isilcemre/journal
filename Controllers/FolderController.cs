using deneme2._0.Models;
using deneme2._0.Services;
using Microsoft.AspNetCore.Mvc;

namespace deneme2._0.Controllers
{
    public class FolderController : Controller
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        private int? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return null;
            return int.Parse(userIdString);
        }

        // Klasör listesi
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var folders = await _folderService.GetUserFoldersAsync(userId.Value);
            return View(folders);
        }

        // Klasör detayı (içindeki günlükler)
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var folder = await _folderService.GetFolderByIdAsync(id, userId.Value);
            if (folder == null) return NotFound();

            return View(folder);
        }

        // Yeni klasör oluştur (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            return View();
        }

        // Yeni klasör oluştur (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string name, string emoji, string? description)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Klasör adı boş olamaz.");
                return View();
            }

            await _folderService.CreateFolderAsync(new Folder
            {
                UserId = userId.Value,
                Name = name.Trim(),
                Emoji = string.IsNullOrWhiteSpace(emoji) ? "📁" : emoji.Trim(),
                Description = description?.Trim(),
                IsDefault = false,
                CreatedAt = DateTime.Now
            });

            return RedirectToAction("Index");
        }

        // Klasör sil
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            try
            {
                await _folderService.DeleteFolderAsync(id, userId.Value);
                TempData["SuccessMessage"] = "Klasör silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // Günlüğü klasöre ekle
        [HttpPost]
        public async Task<IActionResult> AddEntry(int folderId, int journalEntryId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            try
            {
                await _folderService.AddEntryToFolderAsync(journalEntryId, folderId, userId.Value);
                TempData["SuccessMessage"] = "Günlük klasöre eklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> SeedFolders()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            await _folderService.EnsureDefaultFoldersAsync(userId.Value);
            TempData["SuccessMessage"] = "Varsayılan klasörler oluşturuldu!";
            return RedirectToAction("Index");
        }

        // Günlüğü klasörden çıkar
        [HttpPost]
        public async Task<IActionResult> RemoveEntry(int folderId, int journalEntryId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            await _folderService.RemoveEntryFromFolderAsync(journalEntryId, folderId, userId.Value);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}