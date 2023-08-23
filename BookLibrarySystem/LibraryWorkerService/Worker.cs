using LibraryWorkerService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessReservationsService _reservationsCheckService;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)//, IReservationsCheckService reservationsCheckService
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            //_reservationsCheckService = reservationsCheckService;   
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IProcessLoansService loansProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessLoansService>();

                    await loansProcessingService.DoWorkAsync(stoppingToken);

                    IProcessReservationsService reservationsProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessReservationsService>();

                    await reservationsProcessingService.DoWorkAsync(stoppingToken);
                }
            }
        }
    }
}