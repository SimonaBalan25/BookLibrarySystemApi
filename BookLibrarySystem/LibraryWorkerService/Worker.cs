using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService
{
    public class Worker : BackgroundService
    {
        private int _executionCount;
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ++_executionCount;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                   "{ServiceName} working, running at: {time}",
                   nameof(Worker),
                   DateTimeOffset.Now);

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IProcessLoans loansProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessLoans>();

                    await loansProcessingService.DoWorkAsync(_executionCount);

                    IProcessReservations reservationsProcessingService =
                        scope.ServiceProvider.GetRequiredService<IProcessReservations>();

                    await reservationsProcessingService.DoWorkAsync(_executionCount);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(Worker)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}