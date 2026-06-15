using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Shiki.CodeGenerator.Generator.Util;

[Generator]
public class AsyncFactoryConstructableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            StringBuilder sb = new();
            
            sb.AppendLine("//GENERATED"                 )
              .AppendLine(                              ) 
              .AppendLine("namespace Shiki.Common.Util;")
              .AppendLine(                              );

            for (int i = 0; i < 16 + 1; i++)
            {
                var p = Enumerable.Range(1, i).ToDictionary(n => n, n => $"T{n}");

                sb.AppendLine("/// <summary>"                                                                                                                                                                )
                  .AppendLine("/// Allows for asynchronously creating an object statically"                                                                                                                  )
                  .AppendLine("/// </summary>"                                                                                                                                                                    )
                  .AppendLine("/// <remarks>"                                                                                                                                                                    )
                  .AppendLine("/// Useful for defining a contract for creating a type that you only need to know the IAsyncFactoryConstructable params for."                                                 )
                  .AppendLine("/// <br />"                                                 )
                  .AppendLine("/// <br />"                                                 )
                  .AppendLine("/// Also useful for defining a type that you want to be specifically async constructable (ie. a function that does async work before passing the results into a constructor)." )
                  .AppendLine("/// </remarks>"                                                                                                                                                               )
                  .AppendLine("/// <typeparam name=\"TSelf\">The type to make constructable</typeparam>"                                                                                                     );
                   
               foreach (var tp in p)
               {
                   sb.AppendLine($"/// <typeparam name=\"{tp.Value}\">The {tp.Key.ToOrdinalWords()} param of your constructor</typeparam>");
               }

               string ps = string.Join(", ", p.Values.Select(s => $"in {s}").Prepend("TSelf"));
               string fp = string.Join(", ", p.Select(n => $"{n.Value} {n.Key.ToOrdinalWords().Replace(" ", "-")}"));
               sb.AppendLine($"public interface IAsyncFactoryConstructable<{ps}>" )
                 .AppendLine($"    where TSelf : IAsyncFactoryConstructable<{string.Join(", ", p.Values.Prepend("TSelf"))}>")
                 .AppendLine( "{"                                                                  )
                 .AppendLine( "    /// <summary>"                                                  )
                 .AppendLine( "    /// Creates an instance of TSelf"                               )            
                 .AppendLine( "    /// </summary>"                                                 )
                 .AppendLine( "    /// <returns>The created TSelf instance</returns>"              )
                 .AppendLine($"    public static abstract Task<TSelf> CreateInstanceAsync({fp});"  )
                 .AppendLine( "}"                                                                  )
                 .AppendLine()
                 .AppendLine();
            }
            
            ctx.AddSource("IAsyncFactoryConstructable.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }
}