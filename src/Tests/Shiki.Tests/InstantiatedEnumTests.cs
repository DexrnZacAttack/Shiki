using Serilog;
using Shiki.Common.Identity;
using Shiki.Common.Util;
using Shiki.Tests.Util.Extensions;

namespace Shiki.Tests;

public class InstantiatedEnumTests
{
    public class MyEnum : InstantiatedEnum<MyEnum>
    {
        public static readonly MyEnum Value1 = new("Hello");
        public static readonly MyEnum Value2 = new(",");
        public static readonly MyEnum Value3 = new("World");
        public static MyEnum Value4 { get; } = new("!");
        
        public readonly string S;
        public MyEnum(string ss)
        {
            S = ss;
        }

        public override string ToString() => S;
    }

    [Test, Description($"Enumerate instantiated enum '{nameof(MyEnum)}'")]
    public void EnumerateMyEnum()
    {
        foreach (var kvp in MyEnum.OrderedEntries)
        {
            Log.Information("{Class}.{Name} ({Ordinal}) -> {Value}", kvp.Value.GetType().FullName, kvp.Key, kvp.Value.Ordinal, kvp.Value);
        }
    }

}