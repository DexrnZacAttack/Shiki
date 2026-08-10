namespace Shiki.Tests.Util.Extensions;

public static class ObjectExtensions
{
    extension<T>(T? obj)
    {
        public string ToObjectString() => $"[{typeof(T).Name}]: {obj?.ToString() ?? "null"}";
    }
}