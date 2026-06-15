using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Shiki.CodeGenerator.Generator.Util;

[Generator]
public class FactoryConstructableGenerator : IIncrementalGenerator
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

                sb.AppendLine("/// <summary>"                                                                                                                )
                  .AppendLine("/// Allows for creating an object statically"                                                                  )
                  .AppendLine("/// </summary>"                                                                                                                          )
                  .AppendLine("/// <remarks>"                                                                                                                          )
                  .AppendLine("/// Useful for defining a contract for creating a type that you only need to know the IFactoryConstructable params for." )
                  .AppendLine("/// </remarks>"                                                                                                               )
                  .AppendLine("/// <typeparam name=\"TSelf\">The type to make constructable</typeparam>"                                                     );
                   
               foreach (var tp in p)
               {
                   sb.AppendLine($"/// <typeparam name=\"{tp.Value}\">The {tp.Key.ToOrdinalWords()} param of your constructor</typeparam>");
               }

               string ps = string.Join(", ", p.Values.Select(s => $"in {s}").Prepend("out TSelf"));
               string fp = string.Join(", ", p.Select(n => $"{n.Value} {n.Key.ToOrdinalWords().Replace(" ", "-")}"));
               sb.AppendLine($"public interface IFactoryConstructable<{ps}>" )
                 .AppendLine($"    where TSelf : IFactoryConstructable<{string.Join(", ", p.Values.Prepend("TSelf"))}>")
                 .AppendLine( "{"                                                       )
                 .AppendLine( "    /// <summary>"                                       )
                 .AppendLine( "    /// Creates an instance of TSelf"                    )            
                 .AppendLine( "    /// </summary>"                                      )
                 .AppendLine( "    /// <returns>The created TSelf instance</returns>"   )
                 .AppendLine($"    public static abstract TSelf CreateInstance({fp});"  )
                 .AppendLine( "}"                                                       )
                 .AppendLine()
                 .AppendLine();
            }
            
            ctx.AddSource("IFactoryConstructable.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }
}