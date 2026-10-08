using System.Reflection;
using Shiki.FileSystem.Abstractions;
using Shiki.FileSystem.Implementations.Assembly;

namespace Shiki.Tests;

public class AssemblyEmbeededFileSystemTests
{
    private readonly EmbeddedDirectoryManager _directoryManager = new(typeof(AssemblyEmbeededFileSystemTests).Assembly);
    private readonly EmbeddedFileManager _fileManager = new(typeof(AssemblyEmbeededFileSystemTests).Assembly);
    
    [Test, Description($"Enumerate entries in current assembly")]
    public async Task Enumerate() {
        await foreach (string entry in _directoryManager.EnumerateAsync("/Embedded"))
        {
            Console.WriteLine(entry);
        }
        
        Assert.Pass();
    }
    
    [Test, Description($"Enumerate files in current assembly")]
    public async Task EnumerateFiles() {
        await foreach (string entry in _directoryManager.EnumerateAsync("/Embedded", type: IPlatformDirectoryManager.DirectoryEnumerationType.Files))
        {
            Console.WriteLine(entry);
            var s = await _fileManager.GetStreamAsync(Path.Combine("/Embedded", entry), FileMode.Open, FileAccess.Read, FileShare.Read);
            var d = new StreamReader(s);
            
            Console.WriteLine("data: " + await d.ReadToEndAsync());
        }
        
        Assert.Pass();
    }
}