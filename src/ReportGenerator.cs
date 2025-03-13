using Services;

namespace Petroineos.DayAheadPowerPositionReporting;

public class ReportGenerator(
    IPowerService powerService,
    IReportEmitter emitter,
    IClock clock,
    ILocalTimeZoneProvider localTimeZoneProvider) : IReportGenerator
{
    public async Task GenerateReportAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.GetCurrentInstant();
        var date = now.InZone(localTimeZoneProvider.GetLocalTimeZone()).Date;

        var trades = await powerService.GetTradesAsync(date.ToDateTimeUnspecified());

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
