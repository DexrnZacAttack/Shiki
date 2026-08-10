using Shiki.Common.Identity;
using Shiki.Common.Identity.Slug;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Tests.Util.Extensions;

namespace Shiki.Tests;

public class SlugTests
{
    #region Dash
        [Test, Description($"New {nameof(Slug<DashSlugFormatter>)}")]
        public void CreateDashSlug() => Assert.Pass(new Slug<DashSlugFormatter>("hello-world").ToObjectString());
    
        [Test, Description($"Create {nameof(Slug<DashSlugFormatter>)} from very malformed string")]
        public void CreateDashSlugFromInvalidString() => Assert.Pass(new Slug<DashSlugFormatter>("Loooook at my v3ry invalid Slug!!!1!...    DO you Like it???????    "));
    #endregion
    
    #region Snake
        [Test, Description($"New {nameof(Slug<SnakeSlugFormatter>)}")]
        public void CreateSnakeSlug() => Assert.Pass(new Slug<SnakeSlugFormatter>("hello_world").ToObjectString());
        
        [Test, Description($"Create {nameof(Slug<SnakeSlugFormatter>)} from very malformed string")]
        public void CreateSnakeSlugFromInvalidString() => Assert.Pass(new Slug<SnakeSlugFormatter>("Loooook at my v3ry invalid Slug!!!1!...    DO you Like it???????    "));
    #endregion
}