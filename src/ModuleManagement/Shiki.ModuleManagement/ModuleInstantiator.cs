using System.Diagnostics.CodeAnalysis;
using Shiki.ModuleManagement.Module;

namespace Shiki.ModuleManagement;

/// <summary>
/// Simple class to instantiate modules because I don't like copying the same shit everywhere
///
/// Only after writing some of the methods did I realize this is actually pretty useless, but whatever, I can't be bothered to remove it right now.
/// </summary>
internal static class ModuleInstantiator
{
    /// <summary>
    /// Instantiates a module of a given type
    /// </summary>
    /// <typeparam name="TModule">The module type</typeparam>
    /// <returns>The instantiated module</returns>
    /// <exception cref="NullReferenceException">If the module is null</exception>
    public static TModule Instantiate<TModule>()
        where TModule : IModule, new() => new();
    
    /// <summary>
    /// Instantiates a module of a given type
    /// </summary>
    /// <param name="t">The module type</param>
    /// <typeparam name="TModuleBase">The module base type</typeparam>
    /// <returns>The instantiated module</returns>
    /// <exception cref="NullReferenceException">If the module is null</exception>
    public static TModuleBase Instantiate<TModuleBase>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors)] Type t)
    where TModuleBase : IModule
    {
        TModuleBase? module = (TModuleBase?)Activator.CreateInstance(t);
        return module ?? throw new NullReferenceException($"Module {t.FullName} is null");
    }
    
    /// <summary>
    /// Instantiates a module of a given type
    /// </summary>
    /// <param name="t">The module type</param>
    /// <returns>The instantiated module</returns>
    /// <exception cref="NullReferenceException">If the module is null</exception>
    public static IModule Instantiate([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors)] Type t) => Instantiate<IModule>(t);
}