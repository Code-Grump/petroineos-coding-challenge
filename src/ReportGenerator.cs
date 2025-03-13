using Polly;
using Services;

namespace Petroineos.DayAheadPowerPositionReporting;

public class ReportGenerator(
    ILogger<ReportGenerator> logger,
    IPowerService powerService,
    IReportEmitter emitter,
    IClock clock,
    ILocalTimeZoneProvider localTimeZoneProvider) : IReportGenerator
{
    public async Task GenerateReportAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.GetCurrentInstant();
        var date = now.InZone(localTimeZoneProvider.GetLocalTimeZone()).Date;

        // This API looks flaky. We'll attempt the fetch a number of times until we're provided with a result.
        var retryPolicy = Policy
            .Handle<PowerServiceException>()
            .WaitAndRetryAsync(
                3, // Maximum of 3 attempts.
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Use an exponential backoff delay.
                (exception, delay, retryCount, context) =>
                {
                    logger.LogWarning(
                        exception,
                        "Failed to fetch trades from PowerService (attempt {AttemptCount}",
                        retryCount);
                }); 
        
        var trades = await retryPolicy.ExecuteAsync(() => powerService.GetTradesAsync(date.ToDateTimeUnspecified()));

        var report = CreateReport(now, trades);

        await emitter.EmitReportAsync(report, cancellationToken);
    }

    private static Report CreateReport(Instant extractedAt, IEnumerable<PowerTrade> trades)
    {
        var volumeByPeriod = new double[24];
        for (var i = 0; i < volumeByPeriod.Length; i++)
        {
            volumeByPeriod[i] = trades.Select(trade => trade.Periods[i].Volume).Sum();
        }

        var lines = volumeByPeriod.Select((volume, index) => new ReportLine(GetPeriodTime(index + 1), volume));

        return new Report(extractedAt, [.. lines]);
    }

    private static LocalTime GetPeriodTime(int period)
    {
        if (period == 1)
        {
            return new LocalTime(23, 00);
        }

        return new LocalTime(period - 2, 00);
    }
}
