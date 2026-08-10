using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shiki.CodeGenerator.Core;

public record struct Symbol(
    string Namespace,
    string ClassName,
    string FullClassName,
    string ClassKind,
    ImmutableArray<(string TypeName, string Name)> Params,
    ImmutableArray<(string TypeName, string? Constraint)> TemplateParams,
    ImmutableArray<string> Imports,
    string? Documentation,
    ImmutableArray<(string TypeName, ImmutableArray<string> TemplateParams)> Parents
)
{
    public static Symbol FromMethodSymbol(IMethodSymbol sym)
    {
        INamedTypeSymbol clazz = sym.ContainingType;
        if (clazz is null)
        {
            throw new Exception("No parent class/object is associated with this constructor");
        }

        List<(string TypeName, string? Constraint)> templateParams = [];

        SyntaxNode? declSyntax = sym.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        if (declSyntax != null)
        {
            TypeDeclarationSyntax? clazzSyntax = declSyntax is ConstructorDeclarationSyntax ctorSyntax
                                                    ? ctorSyntax.FirstAncestorOrSelf<TypeDeclarationSyntax>()
                                                    : declSyntax as TypeDeclarationSyntax;
            if (clazzSyntax?.TypeParameterList != null)
            {
                foreach (TypeParameterSyntax tp in clazzSyntax.TypeParameterList.Parameters)
                {
                    string n = tp.Identifier.Text;
                    TypeParameterConstraintClauseSyntax? c =
                        clazzSyntax.ConstraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == n);

                    templateParams.Add((n, c?.ToString()));
                }
            }
        }

        List<string> namespaces = [];
        foreach (ITypeParameterSymbol tp in clazz.OriginalDefinition.TypeParameters)
        {
            foreach (ITypeSymbol ct in tp.ConstraintTypes)
            {
                string? nm = ct.ContainingNamespace?.ToDisplayString();
                if (nm != null && !string.IsNullOrEmpty(nm) && !namespaces.Contains(nm) &&
                    nm != clazz.ContainingNamespace.ToDisplayString())
                {
                    namespaces.Add(nm);
                }
            }
            
            string? nms = tp.ContainingNamespace?.ToDisplayString();
            if (nms != null && !string.IsNullOrEmpty(nms) && !namespaces.Contains(nms) &&
                nms != clazz.ContainingNamespace.ToDisplayString())
            {
                namespaces.Add(nms);
            }
        }

        foreach (IParameterSymbol g in sym.Parameters)
        {
            string? nm = g.Type.ContainingNamespace?.ToDisplayString();
            if (nm != null && !string.IsNullOrEmpty(nm) && !namespaces.Contains(nm) &&
                nm != clazz.ContainingNamespace.ToDisplayString())
            {
                namespaces.Add(nm);
            }
        }

        string cl = clazz.Name;
        if (templateParams.Count > 0)
            cl = $"{clazz.Name}<{string.Join(", ", templateParams.Select(t => t.TypeName))}>";

        string kind = clazz.TypeKind switch
        {
            TypeKind.Class     => clazz.IsRecord ? "partial record class" : "partial class",
            TypeKind.Interface => "partial interface",
            TypeKind.Delegate  => "partial delegate",
            TypeKind.Enum      => "enum",
            TypeKind.Struct => (clazz.IsRecord, clazz.IsReadOnly, clazz.IsRefLikeType) switch
            {
                (true, true, _)      => "readonly partial record struct",
                (true, false, _)     => "partial record struct",
                (false, true, true)  => "readonly partial ref struct",
                (false, false, true) => "partial ref struct",
                (false, true, false) => "readonly partial struct",
                _                    => "partial struct"
            },
            _ => "unknown"
        };
        
        var p = new List<INamedTypeSymbol>();
        if (clazz.BaseType != null && clazz.BaseType.SpecialType == SpecialType.System_Object)
        {
            p.Add(clazz.BaseType);
        }
        
        p.AddRange(clazz.Interfaces);

        ImmutableArray<(string TypeName, ImmutableArray<string> TemplateParams)> parents =
        [
            .. p.Select(par => (Name: par.Name, TemplateParams: par.TypeArguments.Select(ptp => ptp.ToDisplayString()).ToImmutableArray()))
        ];
        
        string? documentation = sym.GetDocumentationCommentXml();
        return new Symbol(
                          Namespace: clazz.ContainingNamespace.ToDisplayString(),
                          ClassName: clazz.Name,
                          FullClassName: cl,
                          ClassKind: kind,
                          Params: [.. sym.Parameters.Select(p => (p.Type.ToDisplayString(), p.Name))],
                          TemplateParams: [.. templateParams],
                          Imports: [.. namespaces],
                          Documentation: string.IsNullOrEmpty(documentation) ? null : documentation,
                          Parents: parents
                         );
    }
    
}