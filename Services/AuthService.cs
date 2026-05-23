using deneme2._0.Data;
using deneme2._0.Models;
using deneme2._0.Services;
using Microsoft.EntityFrameworkCore;

namespace deneme2._0.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFolderService _folderService;

        public AuthService(ApplicationDbContext context,
                           IHttpContextAccessor httpContextAccessor,
                           IFolderService folderService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _folderService = folderService;
        }

        public async Task<bool> LoginAsync(string username, string password, bool rememberMe)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
                return false;

            _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString());
            _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);

            return true;
        }

        public async Task LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        public bool IsUserLoggedIn()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserId") != null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = password,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Yeni kullanıcıya varsayılan klasörler oluştur
            await _folderService.EnsureDefaultFoldersAsync(user.Id);

            return true;
        }
    }
}