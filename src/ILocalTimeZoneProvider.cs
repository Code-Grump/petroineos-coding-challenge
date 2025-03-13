namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// Defines a mechanism for gaining access to timezone information.
/// </summary>
public interface ILocalTimeZoneProvider
{
    /// <summary>
    /// Gets the local timezone of the system.
    /// </summary>
    /// <returns>The current local timezone.</returns>
    DateTimeZone GetLocalTimeZone();
}
