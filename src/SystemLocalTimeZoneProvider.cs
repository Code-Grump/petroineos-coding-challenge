namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// A timezone provider that uses the system's default timezone.
/// </summary>
public class SystemLocalTimeZoneProvider : ILocalTimeZoneProvider
{
    /// <inheritdoc />
    public DateTimeZone GetLocalTimeZone() => DateTimeZoneProviders.Tzdb.GetSystemDefault();
}
