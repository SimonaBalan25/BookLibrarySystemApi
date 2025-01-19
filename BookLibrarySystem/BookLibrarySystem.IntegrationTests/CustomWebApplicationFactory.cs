using BookLibrarySystem.Data;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web;
using BookLibrarySystem.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace BookLibrarySystem.IntegrationTests
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration { get; private set; }

        private ApplicationDbContext _context;

        private static object _contextLock = new object();

        public ApplicationDbContext GetContext() => _context;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

                Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

                config.AddConfiguration(Configuration);
            });

            builder.ConfigureServices(services =>
            {

                var descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)).ToList();

                var descriptors2 = services.Where(
                    d => d.ServiceType == typeof(ApplicationDbContext)).ToList();

                var descriptors3 = services.Where(
                    d => d.ServiceType == typeof(DbConnection)).ToList();

                if (descriptors != null)
                {
                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }
                }

                if (descriptors2 != null)
                {
                    foreach (var descriptor in descriptors2)
                    {
                        services.Remove(descriptor);
                    }
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)),
                    ServiceLifetime.Singleton);
                 

                // Seed the database
                services.AddScoped<IApplicationDbContextSeeder, ApplicationDbContextSeeder>();
            });

            builder.ConfigureServices(services =>
            {
                services.AddEndpointsApiExplorer();

                services.AddSwaggerGen();

                services.AddDatabaseDeveloperPageExceptionFilter();

                services.AddScoped<ITelemetryService, TelemetryService>();
                
                services.AddScoped<IAuthorsService, AuthorsService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ISendEmail, SendEmail>();
                services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();
                services.AddAutoMapper(typeof(Program));
                services.AddLogging();

                services.AddScoped<IBooksService, BooksService>();
                //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //.AddRoles<IdentityRole>()
                //.AddEntityFrameworkStores<ApplicationDbContext>();

                //services.AddIdentityServer()
                //.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

                // services.AddAuthentication()
                //     .AddIdentityServerJwt()
                //     .AddGoogle(options =>
                //     {
                //         options.ClientId = Configuration["Authentication:Google:ClientId"];
                //         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                //         options.SignInScheme = IdentityConstants.ExternalScheme;
                //     });

                // //configure ApplicationInsights
                //services.AddApplicationInsightsTelemetry(Configuration);

                // Add Rate Limiting
                //services.AddRateLimiting(Configuration);

                // Configure AutoMapper
                //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                /*services.AddCors(
                    options =>
                    {
                        options.AddPolicy("AllowAllHeadersPolicy",
                        builder =>
                        {
                            builder.AllowAnyOrigin()//WithOrigins("https://localhost:44490")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                    });

                services.AddControllersWithViews();
                services.AddRazorPages();*/
            });
        }

        public ApplicationDbContext GetDbContext()
        {
            var dbContext = Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>()!;
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}
