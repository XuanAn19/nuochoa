using CK_ASP_NET_CORE.Models;
using CK_ASP_NET_CORE.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CK_ASP_NET_CORE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<DataContext>(options =>
            {
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            //Dang kis session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                //thoi gia ton tai 30phut
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });
			//Xác thực Identity
			builder.Services.AddIdentity<AppUseModel, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;

				options.User.RequireUniqueEmail = true;
			});
			// Add services to the container.
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

            //app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
            //Session Usse
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "category",
                pattern: "/category/{Slug?}",
                defaults: new {controller="Category",action="Index"});

            app.MapControllerRoute(
              name: "brand",
              pattern: "/brand/{Slug?}",
              defaults: new { controller = "Brand", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            
            // SeedData
            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
           // SeedData.SeedingData(context);

            app.Run();
        }
    }
}