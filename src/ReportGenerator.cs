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
        var date = clock
            .GetCurrentInstant()
            .InZone(localTimeZoneProvider.GetLocalTimeZone())
            .Date;

        var trades = await powerService.GetTradesAsync(date.ToDateTimeUnspecified());
    }
}
