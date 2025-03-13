namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// Defines a service which takes the data of a report and emits it in some format.
/// </summary>
public interface IReportEmitter
{
    /// <summary>
    /// Emits a report.
    /// </summary>
    /// <param name="report">The report to emit.</param>
    /// <param name="cancellationToken">A token used to signal when the emitting process should be canceled.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task EmitReportAsync(Report report, CancellationToken cancellationToken = default);
}
