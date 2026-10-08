namespace Shiki.FileSystem.Implementations.Assembly;

internal static class Constants
{
    public static readonly NotSupportedException CannotModifyException =
        new("Cannot modify assembly embedded filesystem during runtime");
}