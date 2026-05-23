using deneme2._0.Data;
using deneme2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace deneme2._0.Services
{
    public class FolderService : IFolderService
    {
        private readonly ApplicationDbContext _context;

        // Sistem tanımlı varsayılan klasörler
        private static readonly List<(string Name, string Emoji)> DefaultFolders = new()
        {
            ("Genel",        "📔"),
            ("İş",           "💼"),
            ("Kişisel",      "🏠"),
            ("Sağlık & Spor","💪"),
            ("Seyahat",      "✈️"),
        };

        public FolderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanıcının tüm klasörlerini getir (günlük sayısıyla birlikte)
        public async Task<List<Folder>> GetUserFoldersAsync(int userId)
        {
            return await _context.Folders
                .Where(f => f.UserId == userId)
                .Include(f => f.JournalEntryFolders)
                    .ThenInclude(jef => jef.JournalEntry)
                        .ThenInclude(e => e.Images)
                .OrderBy(f => f.IsDefault)
                .ThenBy(f => f.CreatedAt)
                .ToListAsync();
        }

        // Tek klasör getir (içindeki günlüklerle)
        public async Task<Folder?> GetFolderByIdAsync(int folderId, int userId)
        {
            return await _context.Folders
                .Where(f => f.Id == folderId && f.UserId == userId)
                .Include(f => f.JournalEntryFolders)
                    .ThenInclude(jef => jef.JournalEntry)
                        .ThenInclude(e => e.Images)
                .FirstOrDefaultAsync();
        }

        // Yeni klasör oluştur
        public async Task CreateFolderAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
        }

        // Klasör güncelle
        public async Task UpdateFolderAsync(Folder folder)
        {
            var existing = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folder.Id && f.UserId == folder.UserId);

            if (existing == null)
                throw new Exception("Klasör bulunamadı.");

            existing.Name = folder.Name;
            existing.Emoji = folder.Emoji;
            existing.Description = folder.Description;

            await _context.SaveChangesAsync();
        }

        // Klasör sil (varsayılan klasörler silinemez)
        public async Task DeleteFolderAsync(int folderId, int userId)
        {
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);

            if (folder == null)
                throw new Exception("Klasör bulunamadı.");

            if (folder.IsDefault)
                throw new Exception("Varsayılan klasörler silinemez.");

            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
        }

        // Günlüğü klasöre ekle
        public async Task AddEntryToFolderAsync(int journalEntryId, int folderId, int userId)
        {
            // Klasör bu kullanıcıya mı ait?
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);
            if (folder == null)
                throw new Exception("Klasör bulunamadı.");

            // Günlük bu kullanıcıya mı ait?
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.Id == journalEntryId && e.UserId == userId);
            if (entry == null)
                throw new Exception("Günlük bulunamadı.");

            // Zaten ekli mi?
            var alreadyExists = await _context.JournalEntryFolders
                .AnyAsync(jef => jef.JournalEntryId == journalEntryId && jef.FolderId == folderId);
            if (alreadyExists)
                return;

            _context.JournalEntryFolders.Add(new JournalEntryFolder
            {
                JournalEntryId = journalEntryId,
                FolderId = folderId
            });
            await _context.SaveChangesAsync();
        }

        // Günlüğü klasörden çıkar
        public async Task RemoveEntryFromFolderAsync(int journalEntryId, int folderId, int userId)
        {
            // Güvenlik: klasör bu kullanıcıya mı ait?
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);
            if (folder == null)
                throw new Exception("Klasör bulunamadı.");

            var relation = await _context.JournalEntryFolders
                .FirstOrDefaultAsync(jef => jef.JournalEntryId == journalEntryId
                                         && jef.FolderId == folderId);
            if (relation != null)
            {
                _context.JournalEntryFolders.Remove(relation);
                await _context.SaveChangesAsync();
            }
        }

        // Bir günlüğün hangi klasörlerde olduğunu getir
        public async Task<List<Folder>> GetFoldersForEntryAsync(int journalEntryId, int userId)
        {
            return await _context.JournalEntryFolders
                .Where(jef => jef.JournalEntryId == journalEntryId
                           && jef.Folder!.UserId == userId)
                .Select(jef => jef.Folder!)
                .ToListAsync();
        }

        // Kullanıcı için varsayılan klasörleri oluştur (kayıt sırasında çağrılır)
        public async Task EnsureDefaultFoldersAsync(int userId)
        {
            var existingCount = await _context.Folders
                .CountAsync(f => f.UserId == userId && f.IsDefault);

            if (existingCount > 0)
                return; // Zaten oluşturulmuş

            foreach (var (name, emoji) in DefaultFolders)
            {
                _context.Folders.Add(new Folder
                {
                    UserId = userId,
                    Name = name,
                    Emoji = emoji,
                    IsDefault = true,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}