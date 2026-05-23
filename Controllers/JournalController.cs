using deneme2._0.Models;
using deneme2._0.Services;
using Microsoft.AspNetCore.Mvc;

namespace deneme2._0.Controllers
{
    public class JournalController : Controller
    {
        private readonly IJournalService _journalService;
        private readonly IFolderService _folderService; 
        public JournalController(IJournalService journalService, IFolderService folderService)
        {
            _journalService = journalService;
            _folderService = folderService; 
        }

        

        // ========== LİSTELEME (INDEX) ==========

        public async Task<IActionResult> Index(int? year, int? month, int? day)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var today = DateTime.Today;
            var selectedYear = year ?? today.Year;
            var selectedMonth = month ?? today.Month;
            var selectedDay = day ?? today.Day;

            // Geçerli ay aralığı
            if (selectedMonth < 1) { selectedMonth = 12; selectedYear--; }
            if (selectedMonth > 12) { selectedMonth = 1; selectedYear++; }

            // Gün, o ayın gün sayısını aşmasın
            var daysInSelectedMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
            if (selectedDay > daysInSelectedMonth) selectedDay = daysInSelectedMonth;

            var selectedDate = new DateTime(selectedYear, selectedMonth, selectedDay);

            // Gelecek tarihe izin verme, bugüne sabitle
            if (selectedDate.Date > today)
            {
                selectedDate = today;
                selectedYear = today.Year;
                selectedMonth = today.Month;
            }

            // Tüm entry'leri al
            var allEntries = await _journalService.SearchEntriesAsync(userId.Value, "");

            // Seçili tarihe göre filtrele
            var filteredEntries = allEntries.Where(e => e.CreatedAt.Date == selectedDate.Date).ToList();

            // Takvim için günlük entry sayılarını al
            var entriesByDate = allEntries
                .GroupBy(e => e.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.SelectedDate = selectedDate;
            ViewBag.Year = selectedYear;
            ViewBag.Month = selectedMonth;
            ViewBag.EntriesByDate = entriesByDate;

            return View(filteredEntries);
        }

        // ========== DETAY GÖRÜNTÜLEME ==========

        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var entry = await _journalService.GetEntryByIdAsync(id, userId.Value);
            if (entry == null)
                return NotFound();

            // Kullanıcının tüm klasörleri ve bu günlüğün hangi klasörlerde olduğu
            var allFolders = await _folderService.GetUserFoldersAsync(userId.Value);
            var entryFolders = await _folderService.GetFoldersForEntryAsync(id, userId.Value);

            ViewBag.AllFolders = allFolders;
            ViewBag.EntryFolderIds = entryFolders.Select(f => f.Id).ToList();

            return View(entry);
        }

        // ========== YENİ KAYIT EKLEME (CREATE) ==========

        [HttpGet]
        public async Task<IActionResult> Create(DateTime? date)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var entryDate = date.HasValue && date.Value.Date <= DateTime.Today
                ? date.Value.Date
                : DateTime.Today;

            var entry = new JournalEntry { CreatedAt = entryDate };

            // Klasör listesini gönder
            ViewBag.AllFolders = await _folderService.GetUserFoldersAsync(userId.Value);

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JournalEntry entry, List<IFormFile>? imageFiles, string? createdAtStr)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!string.IsNullOrEmpty(createdAtStr) &&
                DateTime.TryParseExact(createdAtStr, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                entry.CreatedAt = parsedDate;
            }
            else if (entry.CreatedAt == default)
            {
                entry.CreatedAt = DateTime.Today;
            }

            // Gelecek tarihli giriş engelle
            if (entry.CreatedAt.Date > DateTime.Today)
            {
                ModelState.AddModelError("", "Gelecek tarihli günlük oluşturulamaz.");
                return View(entry);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    entry.UserId = userId.Value;
                    await _journalService.CreateEntryAsync(entry, imageFiles);
                    var selectedFolderIds = Request.Form["folderIds"].Select(id => int.Parse(id)).ToList();
                    foreach (var folderId in selectedFolderIds)
                        await _folderService.AddEntryToFolderAsync(entry.Id, folderId, userId.Value);

                    return RedirectToAction("Index", new
                    {
                        year = entry.CreatedAt.Year,
                        month = entry.CreatedAt.Month,
                        day = entry.CreatedAt.Day
                    });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(entry);
        }

        // ========== KAYIT GÜNCELLEME (EDIT) ==========

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var entry = await _journalService.GetEntryByIdAsync(id, userId.Value);
            if (entry == null)
                return NotFound();

            // Klasör listesini ve bu günlüğün mevcut klasörlerini gönder
            ViewBag.AllFolders = await _folderService.GetUserFoldersAsync(userId.Value);
            ViewBag.EntryFolderIds = (await _folderService.GetFoldersForEntryAsync(id, userId.Value))
                .Select(f => f.Id).ToList();

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, JournalEntry entry, List<IFormFile>? imageFiles)  // imageFiles OLARAK DÜZELTİLDİ
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (id != entry.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    entry.UserId = userId.Value;
                    await _journalService.UpdateEntryAsync(entry, imageFiles);  // imageFiles OLARAK DÜZELTİLDİ
                    var existingFolders = await _folderService.GetFoldersForEntryAsync(entry.Id, userId.Value);
                    foreach (var f in existingFolders)
                        await _folderService.RemoveEntryFromFolderAsync(entry.Id, f.Id, userId.Value);

                    var selectedFolderIds = Request.Form["folderIds"]
                        .Select(id => int.Parse(id)).ToList();
                    foreach (var folderId in selectedFolderIds)
                        await _folderService.AddEntryToFolderAsync(entry.Id, folderId, userId.Value);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(entry);
        }

        // ========== KAYIT SİLME (DELETE) ==========

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var entry = await _journalService.GetEntryByIdAsync(id, userId.Value);
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _journalService.DeleteEntryAsync(id, userId.Value);
            return RedirectToAction("Index");
        }

        // ========== RESİM SİLME ==========

        public async Task<IActionResult> DeleteImage(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                await _journalService.DeleteImageAsync(id, userId.Value);
                TempData["SuccessMessage"] = "Fotoğraf silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ========== TAKVİM ==========

        public async Task<IActionResult> Calendar(int? year, int? month)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Varsayılan olarak şu anki ay ve yıl
            var today = DateTime.Today;
            var selectedYear = year ?? today.Year;
            var selectedMonth = month ?? today.Month;

            // Geçerli ay/yıl kontrolü
            if (selectedMonth < 1) selectedMonth = 1;
            if (selectedMonth > 12) selectedMonth = 12;
            if (selectedYear < 2000) selectedYear = 2000;
            if (selectedYear > 2100) selectedYear = 2100;

            var entriesByDate = await _journalService.GetEntriesGroupedByDateAsync(userId.Value, selectedYear, selectedMonth);

            var viewModel = new CalendarViewModel
            {
                Year = selectedYear,
                Month = selectedMonth,
                EntriesByDate = entriesByDate
            };

            return View(viewModel);
        }

        public async Task<IActionResult> EntriesByDate(DateTime date)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var entries = await _journalService.SearchEntriesAsync(userId.Value, "");
            var filteredEntries = entries.Where(e => e.CreatedAt.Date == date.Date).ToList();

            ViewBag.SelectedDate = date.ToString("dd MMMM yyyy");
            return View(filteredEntries);
        }

        // ========== YARDIMCI METOT ==========

        private int? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return null;

            return int.Parse(userIdString);
        }

        // ========== TÜM GÜNLÜKLERİ LİSTELEME ==========
        public async Task<IActionResult> AllEntries()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var entries = await _journalService.SearchEntriesAsync(userId.Value, "");
            var sortedEntries = entries.OrderByDescending(e => e.CreatedAt).ToList();
            return View(sortedEntries);
        }



        // ========== ARAMA ==========
        public async Task<IActionResult> Search(string q)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.SearchTerm = q;

            var results = await _journalService.SearchEntriesAsync(userId.Value, q ?? "");
            return View(results);
        }
    }
}