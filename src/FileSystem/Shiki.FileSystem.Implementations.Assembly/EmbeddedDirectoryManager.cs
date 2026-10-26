using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using Shiki.FileSystem.Abstractions;

namespace Shiki.FileSystem.Implementations.Assembly;

/// <summary>
/// Assembly 'Embedded' read-only implementation for directory management
/// </summary>
public class EmbeddedDirectoryManager(System.Reflection.Assembly assembly) : EmbeddedFileSystemNodeManager(assembly), IPlatformDirectoryManager
{
    /// <summary>
    /// Gets the EmbeddedDirectoryManager for the currently executing assembly
    /// </summary>
    public static EmbeddedDirectoryManager OfCurrentlyExecuting { get; } = new(System.Reflection.Assembly.GetExecutingAssembly());
    
    /// <inheritdoc/>
    public Task CreateDirectoryAsync(string path) => Task.FromException(Constants.CannotModifyException);

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> EnumerateAsync(string path, string glob = "*",
                                                   IPlatformDirectoryManager.DirectoryEnumerationType type =
                                                       IPlatformDirectoryManager.DirectoryEnumerationType.All,
                                                   [EnumeratorCancellation] CancellationToken ct = default)
    {
        string p = await GetPath(path);
        string pp = p.EndsWith('.') ? p : p + ".";

        var entries = assembly.GetManifestResourceNames()
                              .Where(name => name.StartsWith(pp, StringComparison.Ordinal))
                              .Select(name => name[pp.Length..]);
        
        var nodes = type switch {
            IPlatformDirectoryManager.DirectoryEnumerationType.All         => entries.Select(S).Distinct(),
            IPlatformDirectoryManager.DirectoryEnumerationType.Files       => entries.Where(B),
            IPlatformDirectoryManager.DirectoryEnumerationType.Directories => entries.Where(name => !B(name)).Select(S).Distinct(),
            _                                                              => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        nodes = nodes.Where(name => FileSystemName.MatchesSimpleExpression(glob, name));

        foreach (string node in nodes)
        {
            ct.ThrowIfCancellationRequested();
            yield return await GetNativePath(p, node);
        }

        yield break;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string S(string name)
        {
            int f = name.IndexOf('.');
            return (f != -1 && f != name.LastIndexOf('.')) ? name[..f] : name;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool B(string name)
        {
            int f = name.IndexOf('.');
            return f == -1 || f == name.LastIndexOf('.');
        }
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string path, bool recursive = false) => Task.FromException(Constants.CannotModifyException);
}