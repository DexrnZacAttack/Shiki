using System.Runtime.CompilerServices;
using Shiki.FileSystem.Abstractions;

namespace Shiki.FileSystem.Implementations.Assembly;

/// <summary>
/// Assembly 'Embedded' read-only implementation for generic filesystem node management and helpers
/// </summary>
public class EmbeddedFileSystemNodeManager(System.Reflection.Assembly assembly) : IPlatformFileSystemNodeManager
{
    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string path) => ExistsAsync(assembly, path);
    
    public async Task<bool> ExistsAsync(System.Reflection.Assembly ass, string path)
    {
        string p = await GetPath(ass, path);
        return assembly.GetManifestResourceNames().Any(name => name.StartsWith(p));
    }

    public Task<string> GetPath(System.Reflection.Assembly ass, string path) => GetPath(ass.GetName().Name ?? string.Empty, path);
    
    public Task<string> GetPath(string assemblyName, string path)
    {
        string p = path.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')
                       .Trim('.');

        return Task.FromResult($"{assemblyName}.{p}");
    }

    public Task<string> GetPath(string path) => GetPath(assembly, path);
    
    public Task<string> GetNativePath(System.Reflection.Assembly ass, string directory, string filename) => GetNativePath(ass.GetName().Name ?? string.Empty, directory, filename);
    
    public Task<string> GetNativePath(string assemblyName, string directory, string filename)
    {
        if (directory.StartsWith(assemblyName, StringComparison.Ordinal))
        {
            directory = directory[assemblyName.Length..];
            if (directory.StartsWith('.'))
            {
                directory = directory[1..];
            }
            
            directory = Path.DirectorySeparatorChar + directory;
        }

        directory = directory.Replace('.', Path.DirectorySeparatorChar);
        
        return Task.FromResult(Path.Combine(directory, filename));
    }

    public Task<string> GetNativePath(string directory, string filename) => GetNativePath(assembly, directory, filename);
}