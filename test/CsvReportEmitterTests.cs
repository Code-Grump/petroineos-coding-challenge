﻿using Microsoft.Extensions.Options;

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

        var timeZoneProvider = Substitute.For<ILocalTimeZoneProvider>();
        timeZoneProvider.GetLocalTimeZone().Returns(DateTimeZoneProviders.Tzdb.GetSystemDefault());

        var options = new ReportingOptions
        {
            ReportDirectory = "/tmp/"
        };

        var emitter = new CsvReportEmitter(Options.Create(options), fileSystem, timeZoneProvider);

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

    [Fact]
    public async Task ReportsAreEmittedToConfiguredFolderWithExpectedNamingConvention()
    {
        var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London")!;
        var localReportGenerationTime = new LocalDateTime(2025, 03, 13, 11, 50);

        var report = new Report(
            localReportGenerationTime.InZoneLeniently(timeZone).ToInstant(),
            [
                new ReportLine(new LocalTime(23, 00), 150),
                new ReportLine(new LocalTime(00, 00), 150),
            ]);

        var fileSystem = Substitute.For<IFileSystem>();

        fileSystem
            .OpenFile(default!, default, default)
            .ReturnsForAnyArgs(_ => new MemoryStream());

        var timeZoneProvider = Substitute.For<ILocalTimeZoneProvider>();
        timeZoneProvider.GetLocalTimeZone().Returns(timeZone);

        var options = new ReportingOptions
        {
            ReportDirectory = "/tmp/"
        };

        var emitter = new CsvReportEmitter(Options.Create(options), fileSystem, timeZoneProvider);

        await emitter.EmitReportAsync(report);

        fileSystem.Received().OpenFile("/tmp/PowerPosition_20250313_1150.csv", FileMode.CreateNew, FileAccess.Write);
    }
}
