using AgriStreamIoT.Data;
using AgriStreamIoT.Service2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(
           builder.Configuration.GetConnectionString("AzureSqlDatabase")


       ));
            // Register HttpClient factory for dependency injection
            builder.Services.AddHttpClient();
            // Add Cookie Authentication Middleware
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // Redirects unauthenticated users here
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                });
        
            // Register the services
          
            // Register your custom table service for dependency injection
            builder.Services.AddScoped<AgriStreamIoT.Service2.AzureTableService>();
            // Register your custom table service for dependency injection
            builder.Services.AddScoped<AgriStreamIoT.Service2.IAzureBlobService, AgriStreamIoT.Service2.AzureBlobService>();

            var app = builder.Build();
        


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
