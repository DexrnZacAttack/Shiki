using Shiki.Common.Identity;

namespace Shiki.ModuleManagement;

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

/// <summary>
/// A base module
/// </summary>
/// <typeparam name="TInitArgs">The args the module should expect to receive on initialization</typeparam>
public interface IModule<in TInitArgs> : IModule
    where TInitArgs : EventArgs
{
    /// <summary>
    /// Called when it's time to initialize a module
    /// </summary>
    /// <param name="args">The args</param>
    public void OnInitialize(TInitArgs args);
}