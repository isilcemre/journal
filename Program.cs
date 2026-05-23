using deneme2._0.Data;
using deneme2._0.Services;
using Microsoft.EntityFrameworkCore;

namespace deneme2._0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DbContext'i Dependency Injection'a kaydet
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Service'leri DI'a kaydet
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJournalService, JournalService>(); 

            builder.Services.AddScoped<IFolderService, FolderService>();

            // HttpContextAccessor'u ekle (AuthService'de IHttpContextAccessor kullanýyoruz)
            builder.Services.AddHttpContextAccessor();

            // Session servisini ekle
            builder.Services.AddDistributedMemoryCache(); // Session verilerini bellekte tutar
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika oturum süresi
                options.Cookie.HttpOnly = true; // JavaScript ile cookie'ye eriþilemesin (güvenlik)
                options.Cookie.IsEssential = true; // GDPR için zorunlu
            });

            // Cookie Authentication ekle
            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/Account/Login"; // Giriþ sayfasý yönlendirmesi
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Home/AccessDenied";
                    options.Cookie.Name = "SmartPhotoJournalAuth";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Beni hatýrla için
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // SIRA ÖNEMLÝ! Önce Authentication, sonra Authorization, sonra Session
            app.UseAuthentication(); // Kimlik doðrulama
            app.UseAuthorization();  // Yetkilendirme
            app.UseSession();        // Session

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}