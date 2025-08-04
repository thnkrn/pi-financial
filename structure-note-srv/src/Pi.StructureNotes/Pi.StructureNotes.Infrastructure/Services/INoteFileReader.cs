using Pi.StructureNotes.Infrastructure.Models;

namespace Pi.StructureNotes.Infrastructure.Services;

public interface INoteFileReader
{
    IAsyncEnumerable<(FileType Type, string File)> GetFilesToRead(DateTime utc, CancellationToken ct = default);
    Task<Stream> ReadFileAsync(string file, CancellationToken ct = default);
}
