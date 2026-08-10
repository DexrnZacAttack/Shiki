using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Result;

/// <summary>
/// Interface abstraction for BooleanResult
/// </summary>
/// <typeparam name="TFailure">The error type</typeparam>
public interface IBooleanResult<out TFailure>
{
    /// <summary>
    /// The stored value
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }

    /// <summary>
    /// The stored error
    /// </summary>
    public TFailure? Error { get; }
}

/// <summary>
/// Simple holder for a bool and an optional exception if the boolean is false
///
/// Useful for responding with simple acknowledgements across the network
/// </summary>
/// <typeparam name="TException">The exception type</typeparam>
[JsonConverter(typeof(NotSerializableJsonConverterFactory))]
public readonly partial struct BooleanResult<TException> : IBooleanResult<TException>
where TException : Exception
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; }
    
    /// <summary>
    /// The stored exception
    /// </summary>
    public TException? Error { get; }

    /// <summary>
    /// Creates a new BooleanResult
    /// </summary>
    /// <param name="exception">The exception, if present</param>
    [FactoryConstructable]
    public BooleanResult(TException? exception)
    {
        Error = exception;
        Success = Error is null;
    }
    
    /// <summary>
    /// Creates a new BooleanResult with no exception stored
    /// </summary>
    public static BooleanResult<TException> FromTrue() => new(null);
    
    /// <summary>
    /// Creates a new BooleanResult with an exception stored
    /// </summary>
    public static BooleanResult<TException> FromException(TException exception) => new(exception); 
    
    /// <summary>
    /// Creates a new BooleanResult from a Result
    /// </summary>
    /// <param name="result">The result</param>
    /// <typeparam name="TValue">The result's held value type</typeparam>
    /// <returns>A new BooleanResult</returns>
    public static BooleanResult<TException> FromResult<TValue>(Result<TValue, TException> result) =>
        new(result.Error);
    
    /// <summary>
    /// Gets a transportable result from a BooleanResult
    /// </summary>
    public TransportableBooleanResult GetTransportableResult() => TransportableBooleanResult.FromBooleanResult(this);

    /// <inheritdoc/>
    public override string ToString() => new StringBuilder().Append(GetType().Name)
                                                            .Append(" { ")
                                                            .Append("Error = ")
                                                            .Append(Error)
                                                            .Append(" } ").ToString();

}