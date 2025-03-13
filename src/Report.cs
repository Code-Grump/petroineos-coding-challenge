using System.Collections.Immutable;

namespace Petroineos.DayAheadPowerPositionReporting;

public record Report(Instant ExtractedAt, ImmutableList<ReportLine> Lines);

/// <summary>
/// A line in a report.
/// </summary>
/// <param name="LocalTime">The local time marking the start of the hour period the line applies to.</param>
/// <param name="Volume">The aggregated trade positions for the specified hour.</param>
public record ReportLine(LocalTime LocalTime, int Volume);
