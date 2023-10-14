using BookLibrarySystem.Data;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using LibraryWorkerService;
using LibraryWorkerService.Interfaces;
using LibraryWorkerService.Services;
using Microsoft.EntityFrameworkCore;

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
        services.AddScoped<IProcessLoans, ProcessLoans>();
        services.AddScoped<ISendEmail, SendEmail>();
        services.AddScoped<ISetBkdEmail, SetBkdEmail>();    
        services.AddScoped<IProcessReservations, ProcessReservations>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
