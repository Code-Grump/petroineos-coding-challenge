namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// A service which generates reports.
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Generates the report using the data presently available.
    /// </summary>
    /// <param name="cancellationToken">A token used to signal when the operation should be canceled.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task GenerateReportAsync(CancellationToken cancellationToken = default);
}
