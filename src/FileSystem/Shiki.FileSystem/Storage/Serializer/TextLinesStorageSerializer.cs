using System.Text;
using Shiki.Common.Result;

namespace Shiki.FileSystem.Storage.Serializer;

/// <summary>
/// (De)Serializes a list of strings as lines in a text file
/// </summary>
public class TextLinesStorageSerializer : IStorageFileSerializer<IEnumerable<string>>
{
    /// <inheritdoc/>
    public string FileExtension => "txt";
    
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>, Exception>> DeserializeAsync(Stream stream, CancellationToken ct = default)
    {
        List<string> lines = [];

        try
        {
            using StreamReader reader = new(stream, Encoding.UTF8);
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                lines.Add(line);
            }

            return lines;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc/>
    public async Task<BooleanResult<Exception>> SerializeAsync(Stream stream, IEnumerable<string> data, CancellationToken ct = default)
    {
        try
        {
            await using StreamWriter writer = new(stream);
            foreach (string line in data)
            {
                ct.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(line.AsMemory(), ct);
            }

            return BooleanResult<Exception>.Successful;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}