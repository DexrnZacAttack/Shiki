using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shiki.Common.Collections;

// TODO make this comply with C# guidelines properly
/// <summary>
/// Dictionary-like type which stores and manages two separate dictionaries with opposite type params
///
/// Used for reverse lookups
/// </summary>
/// <param name="capacity"></param>
/// <typeparam name="TKey">The primary key</typeparam>
/// <typeparam name="TValue">The primary value</typeparam>
public class BiDictionary<TKey, TValue>(int capacity)
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// The primary dictionary, key | value
    /// </summary>
    private readonly Dictionary<TKey, TValue> _primary = new(capacity);
    /// <summary>
    /// The reverse dictionary, value | key
    /// </summary>
    private readonly Dictionary<TValue, TKey> _reverse = new(capacity);

    /// <summary>
    /// Total amount of items in the BiDictionary
    ///
    /// NOTE: This returns the count of the Primary dictionary, this is fine as both dictionaries are synced.
    /// </summary>
    public int Count => _primary.Count;
    /// <summary>
    /// Capacity of the BiDictionary
    ///
    /// NOTE: This returns the capacity of the Primary dictionary, this is fine as both dictionaries are synced.
    /// </summary>
    public int Capacity => _primary.Capacity;
    
    /// <summary>
    /// Key collection of the Primary dictionary
    /// </summary>
    public Dictionary<TKey, TValue>.KeyCollection PrimaryKeys => this._primary.Keys;
    /// <summary>
    /// Key collection of the reverse dictionary
    /// </summary>
    public Dictionary<TValue, TKey>.KeyCollection ReverseKeys => this._reverse.Keys;
    
    /// <summary>
    /// Value collection of the primary dictionary
    /// </summary>
    public ICollection<TKey> PrimaryValues => this._primary.Keys;
    /// <summary>
    /// Value collection of the reverse dictionary
    /// </summary>
    public ICollection<TValue> ReverseValues => this._reverse.Keys;
    
    /// <summary>
    /// Creates a new BiDictionary with a default capacity of 0
    /// </summary>
    public BiDictionary() : this(0)
    {
    }

    /// <summary>
    /// Get & Set indexer for the BiDictionary
    ///
    /// Getter gets from the Primary dictionary, while setter adds to both.
    /// </summary>
    /// <param name="key">The key</param>
    public TValue this[TKey key]
    {
        get => this._primary[key];
        set => this.Add(key, value);
    }
    
    /// <summary>
    /// GetValueOrDefault for the primary dictionary
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>The value, or default if not present</returns>
    public TValue? GetValueOrDefaultFromPrimary(TKey key) => this._primary.GetValueOrDefault(key);
    /// <summary>
    /// GetValueOrDefault for the reverse dictionary
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>The key, or default if not present</returns>
    public TKey? GetValueOrDefaultFromReverse(TValue value) => this._reverse.GetValueOrDefault(value);

    /// <summary>
    /// Adds a key and value to both dictionaries
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    public void Add(TKey key, TValue value)
    {
        this._primary.Add(key, value);
        this._reverse.Add(value, key);
    }
    
    /// <summary>
    /// Adds a key and value to both dictionaries
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="key">The key</param>
    public void Add(TValue value, TKey key)
    {
        this._reverse.Add(value, key);
        this._primary.Add(key, value);
    }
    
    /// <summary>
    /// Adds a key and value to both dictionaries
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    public void Add(KeyValuePair<TKey, TValue> kvp)
    {
        this._primary.Add(kvp.Key, kvp.Value);
        this._reverse.Add(kvp.Value, kvp.Key);
    }

    /// <summary>
    /// Adds a key and value to both dictionaries
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    public void Add(KeyValuePair<TValue, TKey> kvp)
    {
        this._reverse.Add(kvp.Key, kvp.Value);
        this._primary.Add(kvp.Value, kvp.Key);
    }
    
    /// <summary>
    /// Removes a key and value from both dictionaries
    /// </summary>
    /// <param name="key">The key</param>
    public bool Remove(TKey key)
    {
        TValue? v = this.GetValueOrDefaultFromPrimary(key);
        if (v != null)
        {
            this._reverse.Remove(v);
        }
        
        return this._primary.Remove(key);
    }

    /// <summary>
    /// Removes a key and value from both dictionaries
    /// </summary>
    /// <param name="value">The value</param>
    public bool Remove(TValue value)
    {
        TKey? v = this.GetValueOrDefaultFromReverse(value);
        if (v != null)
        {
            this._primary.Remove(v);
        }
        
        return this._reverse.Remove(value);
    }

    /// <summary>
    /// Removes a key and value from both dictionaries
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    public bool Remove(KeyValuePair<TKey, TValue> kvp) => this._primary.Remove(kvp.Key) && this._reverse.Remove(kvp.Value);
    
    /// <summary>
    /// Removes a key and value from both dictionaries
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    public bool Remove(KeyValuePair<TValue, TKey> kvp) => this._reverse.Remove(kvp.Key) && this._primary.Remove(kvp.Value);

    /// <summary>
    /// Removes all the items from both dictionaries
    /// </summary>
    public void Clear()
    {
        this._primary.Clear();
        this._reverse.Clear();
    }

    public void CopyTo(KeyValuePair<TValue, TKey>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Checks if the Primary dictionary contains a given key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>true, if the key is present</returns>
    public bool PrimaryContainsKey(TKey key) => this._primary.ContainsKey(key);
    /// <summary>
    /// Checks if the Reverse dictionary contains a given key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>true, if the key is present</returns>
    public bool ReverseContainsKey(TValue key) => this._reverse.ContainsKey(key);
    
    /// <summary>
    /// Checks if the Primary dictionary contains the given key and value
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    /// <returns>true, if the dictionary contains the given KeyValuePair</returns>
    public bool PrimaryContains(KeyValuePair<TKey, TValue> kvp) => this._primary.Contains(kvp);
    
    /// <summary>
    /// Checks if the Reverse dictionary contains the given key and value
    /// </summary>
    /// <param name="kvp">The KeyValuePair</param>
    /// <returns>true, if the dictionary contains the given KeyValuePair</returns>
    public bool ReverseContains(KeyValuePair<TValue, TKey> kvp) => this._reverse.Contains(kvp);
    
    /// <summary>
    /// Tries to retrieve a value by key in the Primary dictionary, and sets the out value with the gotten value.
    ///
    /// If retrieving the value fails, this function returns null and value is null.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The output value</param>
    /// <returns>true, if value is not null</returns>
    public bool TryGetValueFromPrimary(TKey key, [MaybeNullWhen(false)] out TValue value) => this._primary.TryGetValue(key, out value);
    
    /// <summary>
    /// Tries to retrieve a value by key in the Reverse dictionary, and sets the out value with the gotten value.
    ///
    /// If retrieving the value fails, this function returns null and value is null.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The output value</param>
    /// <returns>true, if value is not null</returns>
    public bool TryGetValueFromReverse(TValue key, [MaybeNullWhen(false)] out TKey value) => this._reverse.TryGetValue(key, out value);

    /// <summary>
    /// Returns an enumerator that iterates through the Primary dictionary
    /// </summary>
    /// <returns>The enumerator</returns>
    IEnumerator<KeyValuePair<TKey, TValue>> GetEnumeratorForPrimary() => this._primary.GetEnumerator();
    
    /// <summary>
    /// Returns an enumerator that iterates through the Reverse dictionary
    /// </summary>
    /// <returns>The enumerator</returns>
    IEnumerator<KeyValuePair<TValue, TKey>> GetEnumeratorForReverse() => this._reverse.GetEnumerator();
    
    /// <summary>
    /// Returns an enumerator that iterates through the BiDictionary
    /// </summary>
    /// <returns>The enumerator</returns>
    IEnumerator GetEnumerator() => throw new NotImplementedException();
}