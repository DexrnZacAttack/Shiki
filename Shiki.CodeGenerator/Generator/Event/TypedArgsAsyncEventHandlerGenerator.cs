using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Shiki.CodeGenerator.Generator.Event;

//TODO should remove?

[Generator]
public class TypedArgsAsyncEventHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            StringBuilder sb = new();
            
            sb.AppendLine("//GENERATED"                 )
              .AppendLine(                              ) 
              .AppendLine("namespace Shiki.Common.Event;")
              .AppendLine(                              );

            for (int i = 0; i < 16 + 1; i++)
            {
                var p = Enumerable.Range(1, i).ToDictionary(n => n, n => $"T{n}");

                
                sb.AppendLine("/// <summary>"                                                                                                                                                                )
                  .AppendLine("/// A delegate type meant to be called with an Event"                                                                                                                  )
                  .AppendLine("/// </summary>"                                                                                                                                                                    )
                  .AppendLine("/// <remarks>"                                                                                                                                                                    )
                  .AppendLine($"/// Functionally equivalent to Action&lt;{string.Join(", ", p.Values)}&gt;, but with added async support, while enforcing a 'sender' argument."                                                 )
                  .AppendLine("/// </remarks>"                                                                                                                                                               )
                  .AppendLine("/// <typeparam name=\"TSender\">The type that sends the event</typeparam>"                                                                                                     );
                
               foreach (var tp in p)
               {
                   sb.AppendLine($"/// <typeparam name=\"{tp.Value}\">The type of the {tp.Key.ToOrdinalWords()} param of your event func</typeparam>");
               }

               string ps = string.Join(", ", p.Values.Select(s => $"in {s}").Prepend("in TSender"));
               string fp = string.Join(", ", p.Select(n => $"{n.Value} {n.Key.ToOrdinalWords().Replace(" ", "-")}").Prepend("TSender sender"));
               sb.AppendLine($"public delegate Task TypedArgsAsyncEventHandler<{ps}>({fp})");
                  
               foreach (var tp in p)
               {
                   sb.AppendLine($"    where {tp.Value} : allows ref struct");
               }
               
               sb.Append(";")
                 .AppendLine()
                 .AppendLine();
            }
            
            ctx.AddSource("TypedArgsAsyncEventHandler.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }
}