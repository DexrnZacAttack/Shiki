using MessagePack;
using Shiki.Common.Identity.Slug.Formatting.Formatters;
using Shiki.Extensions.MessagePack.Formatter.Identity;
using Shiki.Extensions.MessagePack.Formatter.Identity.Slug;
using Shiki.Extensions.MessagePack.Formatter.Internationalization;

[assembly: MessagePackKnownFormatter(typeof(SlugMessagePackFormatter<SnakeSlugFormatter>))]
[assembly: MessagePackKnownFormatter(typeof(SlugMessagePackFormatter<DashSlugFormatter>))]
[assembly: MessagePackKnownFormatter(typeof(IdentifierMessagePackFormatter))]
[assembly: MessagePackKnownFormatter(typeof(TranslatableTextComponentMessagePackFormatter))]