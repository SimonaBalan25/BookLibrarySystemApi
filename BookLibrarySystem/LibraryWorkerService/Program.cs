using BookLibrarySystem.Data;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using LibraryWorkerService;
using LibraryWorkerService.Interfaces;
using LibraryWorkerService.Models;
using LibraryWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ILoansService, LoansService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthorsService, AuthorsService>();
        services.AddScoped<IBooksService, BooksService>();
        services.AddScoped<IReservationsService, ReservationsService>();    
        services.AddScoped<IProcessLoansService, ProcessLoansService>();
        services.AddScoped<IProcessReservationsService, ProcessReservationsService>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
