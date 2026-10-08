namespace Shiki.ModuleManagement.Module;

/// <summary>
/// A base module which can be initialized
/// </summary>
/// <typeparam name="TInitArgs">The args the module should expect to receive on initialization</typeparam>
public interface IInitializableModule<in TInitArgs> : IModule
    where TInitArgs : EventArgs
{
    /// <summary>
    /// Called when it's time to initialize a module
    /// </summary>
    /// <param name="args">The args</param>
    void OnInitialize(TInitArgs args);
}