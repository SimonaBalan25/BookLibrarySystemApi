using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web.Middleware;
using BookLibrarySystem.Web.Providers;
using BookLibrarySystem.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Configuration;
using System.Security.Claims;

namespace BookLibrarySystem.Web
{
    public class Program
    {
        private static void Main(string[] args)
        {

            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Debug("init main");

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));

            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();
                // Add services to the container.
                // Read the AuthMode from configuration
                var authMode = builder.Configuration["Authentication:AuthMode"];

                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                builder.Services.AddEndpointsApiExplorer();

                builder.Services.AddSwaggerGen();

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                    options.EnableSensitiveDataLogging();
                });                    
                
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                builder.Services.AddScoped<ITelemetryService, TelemetryService>();
                builder.Services.AddScoped<IBooksService, BooksService>();
                builder.Services.AddScoped<IAuthorsService, AuthorsService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<ISendEmail, SendEmail>();
                builder.Services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();

                builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

                builder.Services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

                builder.Services
                    .AddAuthentication(options =>
                    {
                        //options.DefaultScheme = authMode == "Google" ? "Google" : "GuidKeyScheme";
                        //options.DefaultScheme = "DynamicScheme";
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    })
                    .AddCookie()
                    .AddScheme<AuthenticationSchemeOptions, DynamicAuthenticationHandler>("DynamicScheme", options => { })
                    .AddScheme<AuthenticationSchemeOptions, CustomGoogleAuthenticationHandler>("GoogleCustom", options => { }) // Custom Google handler
                    .AddIdentityServerJwt()
                    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                    {
                        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                        options.Events.OnCreatingTicket = ctx =>
                        {
                            // Retrieve user data from ctx
                            var userEmail = ctx.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                            // Assume you fetch roles from your database based on the userEmail
                            //var roles = GetUserRolesFromDB(userEmail); // Implement this method
                            var roles = new List<string> { "Administrator", "NormalUser" };
                            foreach (var role in roles)
                            {
                                ctx.Identity.AddClaim(new Claim(ClaimTypes.Role, role));
                            }

                            return Task.CompletedTask;
                        };
                    })
                    .AddScheme<AuthenticationSchemeOptions, GuidKeyAuthenticationHandler>("GuidKeyScheme", null);

                // Authorization
                builder.Services.AddAuthorization();

                //configure ApplicationInsights
                builder.Services.AddApplicationInsightsLogging(builder.Configuration);

                // Add Rate Limiting
                builder.Services.AddRateLimiting(builder.Configuration);

                // Configure AutoMapper
                builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                builder.Services.AddCors(
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
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseRateLimiting();
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                // Add this after static files but before MVC in order to provide ETags to MVC Views and Razor Pages.
                app.UseRouting();
                app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

                //app.UseClaimsTransformation();
                app.UseApplicationInsightsLogging();
                
                app.UseAuthentication();
                app.UseIdentityServer();

                app.UseCors("AllowAllHeadersPolicy");
                app.UseAuthorization();
                // Add this inside the Configure method of Startup.cs
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });


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
}