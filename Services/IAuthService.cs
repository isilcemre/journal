using deneme2._0.Models;

namespace deneme2._0.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password, bool rememberMe);
        Task LogoutAsync();
        bool IsUserLoggedIn();
        Task<bool> RegisterAsync(string username, string password); 
    }
}