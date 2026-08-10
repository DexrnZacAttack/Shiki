namespace Shiki.Common.Util;

/// <summary>
/// Interface for implementing a cached HashCode
/// </summary>
/// <remarks>
/// Useful for fast hashing on immutable objects that are kept around for a long time
/// </remarks>
public interface ICachedHashCode
{
    /// <summary>
    /// Cached hash code, used for fast hashing if the object is immutable and kept around for a long time
    /// </summary>
    int HashCode { get; }

    /// <summary>
    /// Generates a HashCode
    /// </summary>
    /// <returns>The hash code</returns>
    int GenerateHashCode();
}