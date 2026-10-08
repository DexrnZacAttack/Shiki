using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Result;

/// <summary>
/// Simple holder for a bool and an optional exception if the boolean is false
///
/// Useful for responding with simple acknowledgements across the network
/// </summary>
/// <typeparam name="TException">The exception type</typeparam>
[JsonConverter(typeof(NotSerializableJsonConverterFactory))]
public readonly partial struct BooleanResult<TException> : IResultState<TException?>
where TException : Exception
{
    /// <summary>
    /// Constant, 'always successful' instance of BooleanResult
    /// </summary>
    public static readonly BooleanResult<TException> Successful = new(null);
    
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccessful => Error is null;
    
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
    }
    
    /// <summary>
    /// Creates a new BooleanResult with no exception stored
    /// </summary>
    public static BooleanResult<TException> FromTrue() => Successful;
    
    /// <summary>
    /// Creates a new BooleanResult with an exception stored
    /// </summary>
    public static BooleanResult<TException> FromException(TException exception) => new(exception); 

    /// <inheritdoc/>
    public override string ToString() => new StringBuilder().Append(GetType().Name)
                                                            .Append(" { ")
                                                            .Append("Error = ")
                                                            .Append(Error)
                                                            .Append(" } ").ToString();
    

    /// <summary>
    /// Converts the BooleanResult into a transportable boolean result DTO for sending across the network
    /// </summary>
    public TransportableBooleanResult ToTransportableBooleanResult() =>
        IsSuccessful ? TransportableBooleanResult.Successful : new TransportableBooleanResult(Error);
    
    /// <summary>
    /// Converts the BooleanResult into a transportable boolean result DTO for sending across the network
    /// </summary>
    /// <param name="res">The result</param>
    public static implicit operator TransportableBooleanResult(BooleanResult<TException> res) => res.ToTransportableBooleanResult();
    
    /// <summary>
    /// Allows returning an exception to be converted directly to a falsy BooleanResult
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>The new BooleanResult</returns>
    public static implicit operator BooleanResult<TException>(TException exception) => new(exception);
    
    /// <summary>
    /// Allows converting the BooleanResult directly to a bool for easy evaluation such as in if statements
    /// </summary>
    /// <param name="result">The BooleanResult</param>
    /// <returns>Whether the BooleanResult is successful</returns>
    public static implicit operator bool(BooleanResult<TException> result) => result.IsSuccessful;
}