using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Data;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AspNetCoreRateLimit;
using BookLibrarySystem.Web.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BookLibrarySystem.Web.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookLibrarySystem.IntegrationTests
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Load the appsettings.Test.json configuration file
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            //var configuration = builder.Build();

            // Configure services for testing
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Configure the test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllers();
            services.AddScoped<ITelemetryService, TelemetryService>();
            services.AddScoped<IBooksService, BooksService>();
            services.AddScoped<IAuthorsService, AuthorsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISendEmail, SendEmail>();
            services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            services
                    .AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    })
                    .AddCookie()
                    .AddScheme<AuthenticationSchemeOptions, DynamicAuthenticationHandler>("DynamicScheme", options => { })
                    .AddScheme<AuthenticationSchemeOptions, GuidKeyAuthenticationHandler>("GuidKeyScheme", null);
            // Authorization
            services.AddAuthorization();

            services.AddApplicationInsightsLogging(Configuration);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeadersPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            app.UseApplicationInsightsLogging();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseCors("AllowAllHeadersPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
        }
    }
}
