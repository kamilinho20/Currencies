using Currencies.Core.Interface;
using Polly;
namespace Currencies.Server.HostedService;
public class UpdateCurrenciesService(IServiceScopeFactory scopeFactory, ILogger<UpdateCurrenciesService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));
        do
        {
            try
            {
                await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                var dataService = scope.ServiceProvider.GetRequiredService<IExchangesDataService>();
                var repository = scope.ServiceProvider.GetRequiredService<IExchangeRateRepository>();
                
                var rates = await repository.AnyDataExists() ? await dataService.GetLastExchanges(stoppingToken)
                    : await dataService.GetHistoricalExchanges(DateTime.Now.AddMonths(-1).Date, DateTime.Now.Date, stoppingToken);
                // need retry mechanism
                foreach ( var rateByDate in rates.GroupBy(r => r.ExDate))
                {
                    var failed = await repository.AddBulkAsync([.. rateByDate]);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

        }
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
    }
}