using deneme2._0.Models;

namespace deneme2._0.Services
{
    public interface IJournalService
    {
        Task<List<JournalEntry>> GetAllEntriesAsync(int userId);
        Task<JournalEntry?> GetEntryByIdAsync(int id, int userId);
        Task CreateEntryAsync(JournalEntry entry, List<IFormFile>? imageFiles); 
        Task UpdateEntryAsync(JournalEntry entry, List<IFormFile>? imageFiles);  
        Task DeleteEntryAsync(int id, int userId);
        Task DeleteImageAsync(int imageId, int userId); 
        Task<List<JournalEntry>> SearchEntriesAsync(int userId, string searchTerm);
        Task<Dictionary<DateTime, List<JournalEntry>>> GetEntriesGroupedByDateAsync(int userId, int year, int month);
    }
}