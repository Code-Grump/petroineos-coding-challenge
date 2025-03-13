
namespace Petroineos.DayAheadPowerPositionReporting;

public class LocalFileSystem : IFileSystem
{
    public Stream OpenFile(string path, FileMode mode, FileAccess access) => File.Open(path, mode, access);
}
