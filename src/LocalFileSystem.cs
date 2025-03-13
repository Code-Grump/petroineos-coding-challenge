namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// Provides access to the local file system.
/// </summary>
public class LocalFileSystem : IFileSystem
{
    /// <inheritdoc />
    public Stream OpenFile(string path, FileMode mode, FileAccess access) => File.Open(path, mode, access);
}
