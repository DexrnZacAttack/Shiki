using System.Runtime.CompilerServices;

namespace Shiki.Common.Util;

//I was literally sick of implementing the same garbage multiple times

/// <summary>
/// Simple interface to allow for a list of providers sorted by priority, chosen by whether CanProvide returns true
///
/// Unlike the other IProvider implementations, this one is intended for providers that work off of the state of other objects.
/// </summary>
/// <typeparam name="TOutput">The type that is being provided</typeparam>
public interface IProvider<out TOutput>
{
    /// <summary>
    /// The priority of the provider
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Whether the provider can successfully provide with the current state
    /// </summary>
    /// <returns>Whether this provider can successfully provide</returns>
    bool CanProvide();

    /// <summary>
    /// Provides the object
    /// </summary>
    /// <returns>The provided object</returns>
    TOutput Provide();
}


/// <summary>
/// Simple interface to allow for a list of providers sorted by priority, chosen by whether CanProvide returns true
/// </summary>
/// <typeparam name="TOutput">The type that is being provided</typeparam>
/// <typeparam name="TArgs">The args required to provide the type</typeparam>
public interface IProvider<out TOutput, in TArgs>
{
    /// <summary>
    /// The priority of the provider
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Whether the provider can successfully provide with the given args
    /// </summary>
    /// <param name="args">The args</param>
    /// <returns>Whether this provider can successfully provide</returns>
    bool CanProvide(TArgs args);

    /// <summary>
    /// Provides the object
    /// </summary>
    /// <param name="args">The args</param>
    /// <returns>The provided object</returns>
    TOutput Provide(TArgs args);
}

/// <summary>
/// Simple interface to allow for a list of providers sorted by priority, chosen by whether CanProvide returns true
///
/// This variation extends an existing IProvider while also providing convenient typing and a check for whether args is TArgs in CanProvide
/// </summary>
/// <typeparam name="TOutput">The type that is being provided</typeparam>
/// <typeparam name="TBaseArgs">The base provider arguments</typeparam>
/// <typeparam name="TArgs">The typed arguments required to provide the type</typeparam>
public interface IProvider<out TOutput, in TBaseArgs, in TArgs> : IProvider<TOutput, TBaseArgs>
where TArgs : TBaseArgs
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IProvider<TOutput, TBaseArgs>.CanProvide(TBaseArgs args) => args is TArgs a && CanProvide(a);

    /// <summary>
    /// Whether the provider can successfully provide with the given args
    /// </summary>
    /// <param name="args">The args</param>
    /// <returns>Whether this provider can successfully provide</returns>
    bool CanProvide(TArgs args) => true;//default impl assuming most only need to check the type

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TOutput IProvider<TOutput, TBaseArgs>.Provide(TBaseArgs args) => Provide((TArgs)args);

    /// <summary>
    /// Provides the object
    /// </summary>
    /// <param name="args">The args</param>
    /// <returns>The provided object</returns>
    TOutput Provide(TArgs args);
}

/// <summary>
/// Simple interface to allow for a list of providers sorted by priority, chosen by whether CanProvide returns true
/// 
/// In addition to the changes brought by the typed IProvider, this also types the output type.
/// The reasons to use this interface over the typed IProvider are slim, however.
/// </summary>
/// <typeparam name="TBaseOutput">The base type that is being provided</typeparam>
/// <typeparam name="TOutput">The type that is being provided</typeparam>
/// <typeparam name="TBaseArgs">The base provider arguments</typeparam>
/// <typeparam name="TArgs">The typed arguments required to provide the type</typeparam>
public interface IProvider<out TBaseOutput, out TOutput, in TBaseArgs, in TArgs> : IProvider<TBaseOutput, TBaseArgs, TArgs>
    where TArgs : TBaseArgs
    where TOutput : TBaseOutput
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TBaseOutput IProvider<TBaseOutput, TBaseArgs, TArgs>.Provide(TArgs args) => Provide(args);
    
    /// <summary>
    /// Provides the object
    /// </summary>
    /// <param name="args">The args</param>
    /// <returns>The provided object</returns>
    new TOutput Provide(TArgs args);
}