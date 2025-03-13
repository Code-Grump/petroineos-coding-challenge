using Services;

namespace Petroineos.DayAheadPowerPositionReporting;

public class ReportGeneratorTests
{
    [Fact]
    public async Task ReportGenerationFetchesDataFromThePowerService()
    {
        var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London")!;
        var localReportGenerationTime = new LocalDateTime(2025, 03, 13, 11, 50);

        var powerService = Substitute.For<IPowerService>();

        powerService
            .GetTradesAsync(default)
            .ReturnsForAnyArgs(Task.FromResult<IEnumerable<PowerTrade>>([new PowerTrade { }]));

        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(localReportGenerationTime.InZoneLeniently(timeZone).ToInstant());

        var localTimeZoneProvider = Substitute.For<ILocalTimeZoneProvider>();
        localTimeZoneProvider.GetLocalTimeZone().Returns(timeZone);

        var generator = new ReportGenerator(
            powerService,
            Substitute.For<IReportEmitter>(),
            clock,
            localTimeZoneProvider);

        await generator.GenerateReportAsync();

        await powerService.Received().GetTradesAsync(new DateTime(2025, 03, 13));
    }
}
