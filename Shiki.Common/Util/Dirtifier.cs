namespace Shiki.Common.Util;

/// <summary>
/// Setter that will set a given bool if the value has changed
/// </summary>
public static class Dirtifier
{
    /// <summary>
    /// Compares the given value against the current value, if the value is different, dirty is set to true and the value is set.
    /// </summary>
    /// <param name="input">The input value ref to change</param>
    /// <param name="value">The new value to set</param>
    /// <param name="dirty">Whether the value has been set</param>
    /// <typeparam name="T">The type to set</typeparam>
    public static void Set<T>(ref T input, T value, ref bool dirty)
    where T : IComparable<T>
    {
        if (value.CompareTo(input) <= 0)
            return;
        
        dirty = true;
        input = value;
    }
    
    /// <summary>
    /// Compares the given value against the current value, if the value is different, dirty is set to true and the value is set.
    /// </summary>
    /// <param name="getter">The getter for the input</param>
    /// <param name="setter">The setter for the input</param>
    /// <param name="value">The new value</param>
    /// <param name="dirty">Whether the value has been set</param>
    /// <typeparam name="T">The type to set</typeparam>
    public static void Set<T>(Func<T> getter, Action<T> setter, T value, ref bool dirty)
        where T : IComparable<T>
    {
        if (value.CompareTo(getter()) <= 0)
            return;
        
        dirty = true;
        setter(value);
    }
    
    /// <summary>
    /// Compares the given value against the current value, if the value is different, dirty is set to true and the value is set.
    /// </summary>
    /// <param name="getter">The getter for the input</param>
    /// <param name="setter">The setter for the input</param>
    /// <param name="comparable">The comparator for the input</param>
    /// <param name="value">The new value</param>
    /// <param name="dirty">Whether the value has been set</param>
    /// <typeparam name="T">The type to set</typeparam>
    public static void Set<T>(Func<T> getter, Action<T> setter, Func<T, T, bool> comparable, T value, ref bool dirty)
    {
        if (comparable(getter(), value))
            return;
        
        dirty = true;
        setter(value);
    }
}