using System.Diagnostics;
using System.Reflection;

namespace Shiki.Common.Extensions;

/// <summary>
/// Extensions to aid with interacting with Assembly
/// </summary>
public static class AssemblyExtensions
{
    extension(Assembly assembly)
    {
        /// <summary>
        /// Version string of the assembly
        /// </summary>
        public string VersionString =>
            FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion ?? string.Empty;
    }
}