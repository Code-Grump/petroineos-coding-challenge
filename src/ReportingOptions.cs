using System.ComponentModel.DataAnnotations;

namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// Options to configure the reporting process.
/// </summary>
public class ReportingOptions
{
    /// <summary>
    /// Gets or sets the number of minutes between report generation.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int ReportIntervalMinutes { get; set; }

    /// <summary>
    /// Gets or sets the directory in the file system to write reports to.
    /// </summary>
    [Required]
    public string? ReportDirectory { get; set; }
}
