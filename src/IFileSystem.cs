
namespace Petroineos.DayAheadPowerPositionReporting;

/// <summary>
/// Represents a file system.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Opens a stream to a file in the file system.
    /// </summary>
    /// <param name="path">The path in the file system to open.</param>
    /// <param name="mode">A <see cref="FileMode"/> value specifying whether a file should be created, or an
    /// existing file accessed.</param>
    /// <param name="access">A <see cref="FileAccess"/> value specifying the operations 
    /// that can be performed on the file.</param>
    /// <returns>A <see cref="Stream"/> to the file contents. If the file has been opened for reading, the stream
    /// is positioned at the start of the file. If the file has been opened for writing, the stream is positioned
    /// at the end of the file's existing contents.</returns>
    Stream OpenFile(string path, FileMode mode, FileAccess access);
}

public class LocalFileSystem : IFileSystem
{
    public Stream OpenFile(string path, FileMode mode, FileAccess access) => File.Open(path, mode, access);
}
