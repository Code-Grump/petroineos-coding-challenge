using NSubstitute;

namespace Petroineos.DayAheadPowerPositionReporting;

public class CsvReportEmitterTests
{
    [Fact]
    public async Task ReportsAreWrittenInCsvFormat()
    {
        var report = new Report(
            SystemClock.Instance.GetCurrentInstant(),
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
            ]);

        var fileSystem = Substitute.For<IFileSystem>();
        var fileStream = new MemoryStream();

        fileSystem
            .OpenFile(default!, default, default)
            .ReturnsForAnyArgs(fileStream);

        var emitter = new CsvReportEmitter(fileSystem);

        await emitter.EmitReportAsync(report);

        using var reader = new StreamReader(new MemoryStream(fileStream.ToArray()));

        var result = reader.ReadToEnd();

        Assert.Equal(
            """
            Local Time,Volume
            23:00,150
            00:00,150
            01:00,150
            02:00,150
            03:00,150
            04:00,150
            05:00,150
            06:00,150
            07:00,150
            08:00,150
            09:00,150
            10:00,80
            11:00,80
            12:00,80
            13:00,80
            14:00,80
            15:00,80
            16:00,80
            17:00,80
            18:00,80
            19:00,80
            20:00,80
            21:00,80
            22:00,80

            """,
            result);
    }
}
