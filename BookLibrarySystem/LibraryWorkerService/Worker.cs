using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IProcessLoans loansProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessLoans>();

                    await loansProcessingService.DoWorkAsync(stoppingToken);

                    IProcessReservations reservationsProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessReservations>();

                    await reservationsProcessingService.DoWorkAsync(stoppingToken);
                }
            }
        }
    }
}