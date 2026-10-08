namespace Shiki.Common.Util;

/// <summary>
/// Simple interface that enforces the implementation to have a singleton instance
/// </summary>
/// <typeparam name="TSelf">Self</typeparam>
public interface ISingleton<out TSelf>
where TSelf : ISingleton<TSelf>
{
    /// <summary>
    /// The instance of the class implementing ISingleton
    /// </summary>
    static abstract TSelf Instance { get; } 
}