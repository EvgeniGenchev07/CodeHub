using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using DataLayer;
using BusinessLayer;
namespace MVC
{
    // Microsoft.VisualStudio.Web.CodeGeneration.Design v. 5.0
    // Microsoft.AspNetCore.Identity.UI 5.0.17
    // If you want to use or scaffold them with Visual Studio:
    // dotnet tool install -g dotnet-aspnet-codegenerator --version 5.0.0
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddScoped<IdentityContext>();
            services.AddScoped<LessonsContext>();
            services.AddScoped<LectorsContext>();
            services.AddScoped<CoursesContext>();
            services.AddScoped<ExercisesContext>();
            services.AddScoped<BattlesContext>();
            services.AddHttpContextAccessor();

            // Default for controllers is scoped!
            //services.AddScoped(typeof(MVCDbContext));

            services.AddDbContext<CodeHubDbContext>(options =>
    options.UseSqlite(Configuration.GetConnectionString("CodeHubDbContext")).EnableSensitiveDataLogging());

            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<CodeHubDbContext>()
                .AddDefaultTokenProviders();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = true;

                // Log in required.
                options.SignIn.RequireConfirmedAccount = false; // default
                options.SignIn.RequireConfirmedEmail = false; // default
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

    }
}
