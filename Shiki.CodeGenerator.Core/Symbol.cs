/*
   This place is a message... and part of a system of messages... pay attention to it!
   Sending this message was important to us. We considered ourselves to be a powerful culture.
   This place is not a place of honor... no highly esteemed deed is commemorated here... nothing valued is here.
   What is here was dangerous and repulsive to us. This message is a warning about danger.
   The danger is in a particular location... it increases towards a center... the center of danger is here... of a particular size and shape, and below us.
   The danger is still present, in your time, as it was in ours.
   The danger is to the body, and it can kill.
   The form of the danger is an emanation of energy.
   The danger is unleashed only if you substantially disturb this place physically. This place is best shunned and left uninhabited.
 */
//TODO send to the grave PLEASE THIS IS AN ABOMINATION
//most of this garbage can just be done with extension methods I'm pretty sure...
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
    string Accessibility, //todo shouldn't I make these use enums
    ImmutableArray<Symbol> ContainingClassTypes,
    ImmutableArray<(string TypeName, ImmutableArray<string> TemplateParams)> Parents
)
{
    public static Symbol FromNamedSymbol(INamedTypeSymbol sym)
    {
        List<(string TypeName, string? Constraint)> templateParams = [];

        TypeDeclarationSyntax? clazzSyntax =
            sym.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax;
        foreach (ITypeParameterSymbol tp in sym.TypeParameters)
        {
            string n = tp.Name;
            TypeParameterConstraintClauseSyntax? c =
                clazzSyntax?.ConstraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == n);

            templateParams.Add((tp.Name, c?.ToString()));
        }

        List<string> namespaces = [];
        foreach (ITypeParameterSymbol tp in sym.OriginalDefinition.TypeParameters)
        {
            foreach (ITypeSymbol ct in tp.ConstraintTypes)
            {
                string? nm = ct.ContainingNamespace?.ToDisplayString();
                if (nm != null && !string.IsNullOrEmpty(nm) && !namespaces.Contains(nm) &&
                    nm != sym.ContainingNamespace.ToDisplayString())
                {
                    namespaces.Add(nm);
                }
            }

            string? nms = tp.ContainingNamespace?.ToDisplayString();
            if (nms != null && !string.IsNullOrEmpty(nms) && !namespaces.Contains(nms) &&
                nms != sym.ContainingNamespace.ToDisplayString())
            {
                namespaces.Add(nms);
            }
        }

        string cl = sym.Name;
        if (templateParams.Count > 0)
            cl = $"{sym.Name}<{string.Join(", ", templateParams.Select(t => t.TypeName))}>";

        string kind = sym.TypeKind switch
        {
            TypeKind.Class => sym.IsRecord
                                  ? (sym.IsStatic ? "static partial record class" : "partial record class")
                                  : (sym.IsStatic ? "static partial class" : "partial class"),
            TypeKind.Interface => "partial interface",
            TypeKind.Delegate  => "partial delegate",
            TypeKind.Enum      => "enum",
            TypeKind.Struct => (sym.IsRecord, sym.IsReadOnly, sym.IsRefLikeType) switch
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

        string? documentation = sym.GetDocumentationCommentXml();

        List<Symbol> containingTypes = [];
        INamedTypeSymbol? containing = sym.ContainingType;

        while (containing != null)
        {
            containingTypes.Add(FromNamedSymbol(containing));
            containing = containing.ContainingType;
        }

        return new Symbol(
                          Namespace: sym.ContainingNamespace.ToDisplayString(),
                          ClassName: sym.Name,
                          FullClassName: cl,
                          ClassKind: kind,
                          Params: [],
                          TemplateParams: [.. templateParams],
                          Imports: [.. namespaces],
                          Documentation: string.IsNullOrEmpty(documentation) ? null : documentation,
                          Accessibility: sym.DeclaredAccessibility switch
                          {
                              Microsoft.CodeAnalysis.Accessibility.Public               => "public",
                              Microsoft.CodeAnalysis.Accessibility.Internal             => "internal",
                              Microsoft.CodeAnalysis.Accessibility.Protected            => "protected",
                              Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => "protected internal",
                              Microsoft.CodeAnalysis.Accessibility.Private              => "private",
                              _                                                         => "internal"
                          },
                          ContainingClassTypes: [.. containingTypes],
                          Parents: []
                         );
    }

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
            TypeKind.Class => clazz.IsRecord
                                  ? (clazz.IsStatic ? "static partial record class" : "partial record class")
                                  : (clazz.IsStatic ? "static partial class" : "partial class"),
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
            .. p.Select(par => (Name: par.Name,
                                TemplateParams: par.TypeArguments.Select(ptp => ptp.ToDisplayString())
                                                   .ToImmutableArray()))
        ];

        List<Symbol> containingTypes = [];
        INamedTypeSymbol? containing = clazz.ContainingType;

        while (containing != null)
        {
            containingTypes.Add(FromNamedSymbol(containing));
            containing = containing.ContainingType;
        }

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
                          Accessibility: sym.DeclaredAccessibility switch
                          {
                              Microsoft.CodeAnalysis.Accessibility.Public               => "public",
                              Microsoft.CodeAnalysis.Accessibility.Internal             => "internal",
                              Microsoft.CodeAnalysis.Accessibility.Protected            => "protected",
                              Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => "protected internal",
                              Microsoft.CodeAnalysis.Accessibility.Private              => "private",
                              _                                                         => "internal"
                          },
                          ContainingClassTypes: [.. containingTypes],
                          Parents: parents
                         );
    }
}