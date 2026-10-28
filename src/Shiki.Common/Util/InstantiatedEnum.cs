using System.Collections.Immutable;
using System.Reflection;

namespace Shiki.Common.Util;

public interface IInstantiatedEnum<TSelf>
where TSelf : IInstantiatedEnum<TSelf>
{
    static ImmutableArray<string> Keys => Cache.Keys;
    static ImmutableArray<TSelf> Values => Cache.Values;
    static ImmutableDictionary<string, TSelf> Entries => Cache.Entries;
    static ImmutableArray<KeyValuePair<string, TSelf>> OrderedEntries => Cache.OrderedEntries;

    int Ordinal => Cache.Values.IndexOf((TSelf)this);
    
    private static class Cache
    {
        public static ImmutableDictionary<string, TSelf> Entries;
        public static ImmutableArray<KeyValuePair<string, TSelf>> OrderedEntries;
        public static ImmutableArray<string> Keys;
        public static ImmutableArray<TSelf> Values;

        static Cache()
        {
            ImmutableDictionary<string, TSelf>.Builder entries = ImmutableDictionary.CreateBuilder<string, TSelf>();
            
            ImmutableArray<KeyValuePair<string, TSelf>>.Builder orderedEntries = ImmutableArray.CreateBuilder<KeyValuePair<string, TSelf>>();
            ImmutableArray<string>.Builder keys = ImmutableArray.CreateBuilder<string>();
            ImmutableArray<TSelf>.Builder values = ImmutableArray.CreateBuilder<TSelf>();
            
            Type t = typeof(TSelf);
            if (t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length != 0)
                throw new InvalidOperationException(t.FullName + " cannot have a public constructor.");
            
            MemberInfo[] fields =
                [.. t.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == t)];
            
            MemberInfo[] properties =
                [.. t.GetProperties(BindingFlags.Public | BindingFlags.Static).Where(f => f.PropertyType == t && f is { CanRead: true, CanWrite: false })];

            var info = fields.Concat(properties).OrderBy(m => m.MetadataToken);
            
            foreach (MemberInfo inf in info)
            {
                TSelf inst = inf switch
                {
                    FieldInfo f    => (TSelf)f.GetValue(null)!,
                    PropertyInfo p => (TSelf)p.GetValue(null)!,
                    _              => throw new InvalidOperationException()
                };

                if (inst != null && !entries.ContainsKey(inf.Name))
                {
                    entries.Add(inf.Name, inst);
                    orderedEntries.Add(new KeyValuePair<string, TSelf>(inf.Name, inst));
                    
                    keys.Add(inf.Name);
                    values.Add(inst);
                }
            }
            
            Entries = entries.ToImmutable();
            OrderedEntries = orderedEntries.ToImmutable();
            Keys = keys.ToImmutable();
            Values = values.ToImmutable();
        }
    } 
}

public abstract class InstantiatedEnum<TSelf> : IInstantiatedEnum<TSelf>
    where TSelf : InstantiatedEnum<TSelf>
{
    public static ImmutableArray<string> Keys => IInstantiatedEnum<TSelf>.Keys;
    public static ImmutableArray<TSelf> Values => IInstantiatedEnum<TSelf>.Values;
    public static ImmutableDictionary<string, TSelf> Entries => IInstantiatedEnum<TSelf>.Entries;
    public static ImmutableArray<KeyValuePair<string, TSelf>> OrderedEntries => IInstantiatedEnum<TSelf>.OrderedEntries;

    public int Ordinal => IInstantiatedEnum<TSelf>.Values.IndexOf((TSelf)this);
}