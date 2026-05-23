using deneme2._0.Models;

namespace deneme2._0.Services
{
    public interface IFolderService
    {
        Task<List<Folder>> GetUserFoldersAsync(int userId);
        Task<Folder?> GetFolderByIdAsync(int folderId, int userId);
        Task CreateFolderAsync(Folder folder);
        Task UpdateFolderAsync(Folder folder);
        Task DeleteFolderAsync(int folderId, int userId);
        Task AddEntryToFolderAsync(int journalEntryId, int folderId, int userId);
        Task RemoveEntryFromFolderAsync(int journalEntryId, int folderId, int userId);
        Task<List<Folder>> GetFoldersForEntryAsync(int journalEntryId, int userId);
        Task EnsureDefaultFoldersAsync(int userId);
    }
}