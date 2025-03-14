﻿using Microsoft.Extensions.Options;
using System.Globalization;

namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// A service which can emit a report to the file system in a CSV format.
/// </summary>
/// <param name="fileSystem">The file system CSVs will be written to.</param>
/// <param name="localTimeZoneProvider">The provider to use when determining the local time zone information.</param>
public class CsvReportEmitter(
    IOptions<ReportingOptions> options,
    IFileSystem fileSystem,
    ILocalTimeZoneProvider localTimeZoneProvider) : IReportEmitter
{
    public async Task EmitReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var timestamp = report.ExtractedAt
            .InZone(localTimeZoneProvider.GetLocalTimeZone())
            .ToString("yyyyMMdd_HHmm", culture);

        var filename = $"PowerPosition_{timestamp}.csv";

        using var fs = fileSystem.OpenFile(
            Path.Combine(options.Value.ReportDirectory!, filename),
            FileMode.CreateNew,
            FileAccess.Write);

        using var writer = new StreamWriter(fs);

        await WriteReportAsync(report, culture, writer, cancellationToken);
    }

    private static async Task WriteReportAsync(
        Report report,
        CultureInfo culture,
        StreamWriter writer,
        CancellationToken cancellationToken)
    {
        // Write the header.
        await writer.WriteLineAsync("Local Time,Volume");

        // Write the report lines.
        foreach (var line in report.Lines)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await writer.WriteAsync(line.LocalTime.ToString("HH:mm", culture));
            await writer.WriteAsync(',');
            await writer.WriteLineAsync(line.Volume.ToString());
        }

        await writer.FlushAsync(cancellationToken);
    }
}
