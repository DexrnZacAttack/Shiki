using Shiki.Common.Identity;

namespace Shiki.ModuleManagement.Module;

/// <summary>
/// A base module
/// </summary>
public interface IModule
{
    /// <summary>
    /// The identifier of the module
    /// </summary>
    Identifier Id { get; }
}