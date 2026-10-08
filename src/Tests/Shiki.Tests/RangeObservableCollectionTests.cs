using Shiki.Common.Collections;

namespace Shiki.Tests;

public class RangeObservableCollectionTests
{
    [Test, Description($"Add many items to a {nameof(RangeObservableCollection<string>)}")]
    public void AddRange()
    {
        RangeObservableCollection<string> collection = [];
        collection.CollectionChanged += (s, e) =>
        {
            Console.WriteLine($"New items: {string.Join(", ", e.NewItems?.Cast<string>() ?? [])}");
        };
        
        List<string> l = [
            "Hello",
            "world!",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine",
            "Ten",
        ];
        
        collection.AddRange(l);
    }

}