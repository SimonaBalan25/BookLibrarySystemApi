using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web.Middleware;
using BookLibrarySystem.Web.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

internal class Program
{
    private static void Main(string[] args)
    {
        
        var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();        
        logger.Debug("init main");

        var builder = WebApplication.CreateBuilder(args);

        try 
        {            
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IBooksService, BooksService>();
            builder.Services.AddScoped<IAuthorsService, AuthorsService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });

            ApplicationInsightsServiceOptions telemetryOptions = new();
            telemetryOptions.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
            telemetryOptions.EnableRequestTrackingTelemetryModule = true;
            telemetryOptions.EnableDependencyTrackingTelemetryModule = true;
            telemetryOptions.EnableDiagnosticsTelemetryModule = true;
            telemetryOptions.EnableQuickPulseMetricStream = true;

            // Can enable/disable adaptive sampling here.
            // https://learn.microsoft.com/en-us/azure/azure-monitor/app/sampling
            telemetryOptions.EnableAdaptiveSampling = false;
            builder.Services.AddApplicationInsightsTelemetry(telemetryOptions);

            builder.Services.AddLogging(logBuilder =>
            {
                var instrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
                logBuilder.AddApplicationInsights(instrumentationKey);
            });

            // Create a TelemetryConfiguration instance.
            var apiKey = builder.Configuration["ApplicationInsights:APIKey"];
           
            if (!string.IsNullOrEmpty(apiKey))
            {
                var module = TelemetryModules.Instance.Modules
                                             .OfType<QuickPulseTelemetryModule>().FirstOrDefault();
                if (module != null)
                {
                    module.AuthenticationApiKey = apiKey;
                }
            }

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Host.UseNLog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            //app.UseClaimsTransformation();
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
        catch (Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }
}