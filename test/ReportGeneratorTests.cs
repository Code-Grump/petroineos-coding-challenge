using FluentAssertions;
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

    [Fact]
    public async Task ReportGenerationEmitsReportContainingAggregatedPositions()
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        var powerService = Substitute.For<IPowerService>();

        PowerTrade[] trades =
        [
            PowerTrade.Create(new DateTime(2015, 04, 01), 24),
            PowerTrade.Create(new DateTime(2015, 04, 01), 24),
        ];

        trades[0].Periods[0].Volume = 100;
        trades[0].Periods[1].Volume = 100;
        trades[0].Periods[2].Volume = 100;
        trades[0].Periods[3].Volume = 100;
        trades[0].Periods[4].Volume = 100;
        trades[0].Periods[5].Volume = 100;
        trades[0].Periods[6].Volume = 100;
        trades[0].Periods[7].Volume = 100;
        trades[0].Periods[8].Volume = 100;
        trades[0].Periods[9].Volume = 100;
        trades[0].Periods[10].Volume = 100;
        trades[0].Periods[11].Volume = 100;
        trades[0].Periods[12].Volume = 100;
        trades[0].Periods[13].Volume = 100;
        trades[0].Periods[14].Volume = 100;
        trades[0].Periods[15].Volume = 100;
        trades[0].Periods[16].Volume = 100;
        trades[0].Periods[17].Volume = 100;
        trades[0].Periods[18].Volume = 100;
        trades[0].Periods[19].Volume = 100;
        trades[0].Periods[20].Volume = 100;
        trades[0].Periods[21].Volume = 100;
        trades[0].Periods[22].Volume = 100;
        trades[0].Periods[23].Volume = 100;

        trades[1].Periods[0].Volume = 50;
        trades[1].Periods[1].Volume = 50;
        trades[1].Periods[2].Volume = 50;
        trades[1].Periods[3].Volume = 50;
        trades[1].Periods[4].Volume = 50;
        trades[1].Periods[5].Volume = 50;
        trades[1].Periods[6].Volume = 50;
        trades[1].Periods[7].Volume = 50;
        trades[1].Periods[8].Volume = 50;
        trades[1].Periods[9].Volume = 50;
        trades[1].Periods[10].Volume = 50;
        trades[1].Periods[11].Volume = -20;
        trades[1].Periods[12].Volume = -20;
        trades[1].Periods[13].Volume = -20;
        trades[1].Periods[14].Volume = -20;
        trades[1].Periods[15].Volume = -20;
        trades[1].Periods[16].Volume = -20;
        trades[1].Periods[17].Volume = -20;
        trades[1].Periods[18].Volume = -20;
        trades[1].Periods[19].Volume = -20;
        trades[1].Periods[20].Volume = -20;
        trades[1].Periods[21].Volume = -20;
        trades[1].Periods[22].Volume = -20;
        trades[1].Periods[23].Volume = -20;

        powerService
            .GetTradesAsync(default)
            .ReturnsForAnyArgs(Task.FromResult<IEnumerable<PowerTrade>>(trades));

        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(now);

        var localTimeZoneProvider = Substitute.For<ILocalTimeZoneProvider>();
        localTimeZoneProvider.GetLocalTimeZone().Returns(DateTimeZoneProviders.Tzdb.GetSystemDefault());

        var emitter = Substitute.For<IReportEmitter>();

        var generator = new ReportGenerator(
            powerService,
            emitter,
            clock,
            localTimeZoneProvider);

        await generator.GenerateReportAsync();

        await emitter.Received().EmitReportAsync(Arg.Is<Report>(report => report != null));

        var call = emitter.ReceivedCalls().Single();

        var report = (Report?)call.GetArguments()[0];

        report.Should().BeEquivalentTo(
            new Report(
                now,
                [
                    new ReportLine(new LocalTime(23, 00), 150),
                    new ReportLine(new LocalTime(00, 00), 150),
                    new ReportLine(new LocalTime(01, 00), 150),
                    new ReportLine(new LocalTime(02, 00), 150),
                    new ReportLine(new LocalTime(03, 00), 150),
                    new ReportLine(new LocalTime(04, 00), 150),
                    new ReportLine(new LocalTime(05, 00), 150),
                    new ReportLine(new LocalTime(06, 00), 150),
                    new ReportLine(new LocalTime(07, 00), 150),
                    new ReportLine(new LocalTime(08, 00), 150),
                    new ReportLine(new LocalTime(09, 00), 150),
                    new ReportLine(new LocalTime(10, 00), 80),
                    new ReportLine(new LocalTime(11, 00), 80),
                    new ReportLine(new LocalTime(12, 00), 80),
                    new ReportLine(new LocalTime(13, 00), 80),
                    new ReportLine(new LocalTime(14, 00), 80),
                    new ReportLine(new LocalTime(15, 00), 80),
                    new ReportLine(new LocalTime(16, 00), 80),
                    new ReportLine(new LocalTime(17, 00), 80),
                    new ReportLine(new LocalTime(18, 00), 80),
                    new ReportLine(new LocalTime(19, 00), 80),
                    new ReportLine(new LocalTime(20, 00), 80),
                    new ReportLine(new LocalTime(21, 00), 80),
                    new ReportLine(new LocalTime(22, 00), 80)
                ]));
    }
}
