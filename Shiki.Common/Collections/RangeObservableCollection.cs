using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Shiki.Common.Collections;

/// <summary>
/// ObservableCollection that supports AddRange
/// </summary>
/// <typeparam name="T">The type stored within</typeparam>
public class RangeObservableCollection<T> : ObservableCollection<T>
{
    /// <summary>
    /// Whether OnCollectionChanged should be ignored
    ///
    /// Used in AddRange to not spam listeners with OnCollectionChanged
    /// </summary>
    private bool _skipCollectionChanged;
    
    /// <inheritdoc/>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_skipCollectionChanged)
        {
            base.OnCollectionChanged(e);
        }
    }

    /// <summary>
    /// Adds all values from the given enumerable to this collection
    /// </summary>
    /// <param name="enumerable">The enumerable</param>
    /// <exception cref="ArgumentNullException">If enumerable is null</exception>
    public void AddRange(IEnumerable<T> enumerable)
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        var added = enumerable.ToList();
        if (added.Count == 0) return;

        _skipCollectionChanged = true;
        try
        {
            foreach (T t in added)
            {
                Add(t);
            }
        }
        finally
        {
            _skipCollectionChanged = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added));
        }
    }
}