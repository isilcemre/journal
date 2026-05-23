using deneme2._0.Data;
using deneme2._0.Models;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace deneme2._0.Services
{
    public class JournalService : IJournalService
    {
        private readonly ApplicationDbContext _context;

        public JournalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JournalEntry>> GetAllEntriesAsync(int userId)
        {
            return await _context.JournalEntries
                .Include(j => j.Images)  // Resimleri de getir
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<JournalEntry?> GetEntryByIdAsync(int id, int userId)
        {
            return await _context.JournalEntries
                .Include(j => j.Images)  // Resimleri de getir
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
        }


        public async Task CreateEntryAsync(JournalEntry entry, List<IFormFile>? imageFiles)
        {

            // Metodun içine, entry.Content'i aldıktan hemen sonra:
            var sanitizer = new HtmlSanitizer();
            entry.Content = sanitizer.Sanitize(entry.Content);


            // FOTOĞRAF ZORUNLU - en az 1 resim gerekli
            if (imageFiles == null || imageFiles.Count == 0)
            {
                throw new ArgumentException("Lütfen en az bir fotoğraf yükleyin.");
            }

            // Eğer tarih belirtilmemişse bugünü kullan, aksi halde gelen tarihi koru
            if (entry.CreatedAt == default)
                entry.CreatedAt = DateTime.Today;

            entry.Images = new List<JournalImage>();

            foreach (var imageFile in imageFiles)
            {
                // Resim boyutu kontrolü - max 5MB
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException($"Fotoğraf boyutu 5MB'dan büyük olamaz: {imageFile.FileName}");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    entry.Images.Add(new JournalImage
                    {
                        ImageData = memoryStream.ToArray(),
                        ImageContentType = imageFile.ContentType,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEntryAsync(JournalEntry entry, List<IFormFile>? imageFiles)
        {

            // XSS koruması - HTML temizle
            var sanitizer = new HtmlSanitizer();
            entry.Content = sanitizer.Sanitize(entry.Content);


            // Önce veritabanındaki MEVCUT entry'i bul (resimleriyle birlikte)
            var existingEntry = await _context.JournalEntries
                .Include(j => j.Images)
                .FirstOrDefaultAsync(j => j.Id == entry.Id && j.UserId == entry.UserId);

            if (existingEntry == null)
                throw new Exception("Entry bulunamadı");

            // SADECE kullanıcının değiştirebileceği alanları güncelle
            existingEntry.Title = entry.Title;
            existingEntry.Content = entry.Content;
            existingEntry.UpdatedAt = DateTime.Now;

            // Yeni resimler eklendiyse
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile.Length > 5 * 1024 * 1024)
                        throw new ArgumentException($"Fotoğraf boyutu 5MB'dan büyük olamaz: {imageFile.FileName}");

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        existingEntry.Images.Add(new JournalImage
                        {
                            JournalEntryId = entry.Id,
                            ImageData = memoryStream.ToArray(),
                            ImageContentType = imageFile.ContentType,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            _context.JournalEntries.Update(existingEntry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEntryAsync(int id, int userId)
        {
            var entry = await GetEntryByIdAsync(id, userId);
            if (entry != null)
            {
                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(int imageId, int userId)
        {
            var image = await _context.JournalImages
                .Include(i => i.JournalEntry)
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                throw new Exception("Resim bulunamadı");

            // Kullanıcı yetkisi kontrolü
            if (image.JournalEntry?.UserId != userId)
                throw new Exception("Bu resmi silme yetkiniz yok");

            _context.JournalImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task<List<JournalEntry>> SearchEntriesAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllEntriesAsync(userId);

            return await _context.JournalEntries
                .Include(j => j.Images)
                .Where(j => j.UserId == userId &&
                            (j.Title.Contains(searchTerm) ||
                             j.Content.Contains(searchTerm)))
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, List<JournalEntry>>> GetEntriesGroupedByDateAsync(int userId, int year, int month)
        {
            var entries = await _context.JournalEntries
                .Include(j => j.Images)
                .Where(j => j.UserId == userId &&
                            j.CreatedAt.Year == year &&
                            j.CreatedAt.Month == month)
                .ToListAsync();

            // Tarihe göre grupla (gün bazında)
            return entries
                .GroupBy(j => j.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}