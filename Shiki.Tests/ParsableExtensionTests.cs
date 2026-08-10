using Shiki.Common.Extensions;
using Shiki.Common.Identity;
using Shiki.Tests.Util.Extensions;

namespace Shiki.Tests;

public class ParsableExtensionTests
{
    [Test, Description($"ParseAs {nameof(Identifier)}")]
    public void ParseAsIdentifier() => Assert.Pass("Shiki.Tests:parsable_extension/parse_identifier".ParseAs<Identifier>().ToObjectString());

    [Test, Description($"TryParseAs {nameof(Identifier)}")]
    public void TryParseAsIdentifier()
    {
        if (!"Shiki.Tests:parsable_extension/::ass".TryParseAs<Identifier>(null, out Identifier? result))
        { 
            throw new ArgumentNullException(nameof(result));
        }

        Assert.Pass(result.ToObjectString());
    }
}