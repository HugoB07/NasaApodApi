namespace NasaApodApi.Services
{
    public class ApodDataFetcherService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApodDataFetcherService> _logger;

        public ApodDataFetcherService(IServiceProvider serviceProvider, ILogger<ApodDataFetcherService> logger)
        {
            _serviceProvider = serviceProvider; // Inject IServiceProvider
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calculate the initial delay until 10 AM UTC
            var now = DateTime.UtcNow;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, DateTimeKind.Utc);

            if(now > nextRunTime)
            {
                // If it's already past 10 AM UTC, schedule for the next day
                nextRunTime = nextRunTime.AddDays(1);
            }

            var initialDelay = nextRunTime - now;

            // Set the timer to fetch data at the specified interval
            _timer = new Timer(FetchData, null, initialDelay, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void FetchData(object state)
        {
            try
            {
                // Create a scope to resolve IApodService
                using (var scope = _serviceProvider.CreateScope())
                {
                    var apodService = scope.ServiceProvider.GetRequiredService<IApodService>();
                    await apodService.GetApodDataAsync();
                    _logger.LogInformation("APOD data fetched successfully at {time}.", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching APOD data.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
