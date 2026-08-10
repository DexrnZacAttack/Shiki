using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Result.Serialization;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Common.Result;

/// <summary>
/// A BooleanResult that can be serialized and deserialized
/// </summary>
[JsonConverter(typeof(BooleanResultJsonConverter))]
public readonly partial record struct TransportableBooleanResult : IBooleanResult<ResultExceptionDto>
{
    /// <inheritdoc/>
    [MemberNotNullWhen(false,  nameof(Error))]
    public bool Success { get; }
    
    /// <inheritdoc/>
    public ResultExceptionDto? Error { get; }

    /// <summary>
    /// Creates a new successful TransportableBooleanResult
    /// </summary>
    public TransportableBooleanResult() : this(null) {}
    
    /// <summary>
    /// Creates a new TransportableBooleanResult with the held error if present
    /// </summary>
    /// <param name="error">The error</param>
    [FactoryConstructable]
    public TransportableBooleanResult(ResultExceptionDto? error = null)
    {
        Error = error;
        Success = Error == null;
    }
    
    /// <summary>
    /// Creates a TransportableBooleanResult from a BooleanResult
    /// </summary>
    /// <param name="result">The BooleanResult</param>
    /// <typeparam name="TException">The exception type</typeparam>
    /// <returns>a TransportableBooleanResult</returns>
    public static TransportableBooleanResult FromBooleanResult<TException>(BooleanResult<TException> result)
    where TException : Exception
    => new(result.Error != null ? new ResultExceptionDto(result.Error) : null);
    
    /// <summary>
    /// Creates a TransportableBooleanResult from a Result
    /// </summary>
    /// <param name="result">The Result</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <typeparam name="TException">The exception type</typeparam>
    /// <returns>a TransportableBooleanResult</returns>
    public static TransportableBooleanResult FromResult<TValue, TException>(Result<TValue, TException> result)
        where TException : Exception
        => new(result.Error != null ? new ResultExceptionDto(result.Error) : null);
    
    /// <summary>
    /// Creates a TransportableBooleanResult from an IResult
    /// </summary>
    /// <param name="result">The Result</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>a TransportableBooleanResult</returns>
    public static TransportableBooleanResult FromIResult<TValue>(IResult<TValue, ResultExceptionDto> result)
        => new(result.Error);
    
    /// <summary>
    /// Creates a TransportableBooleanResult from a TransportableResult
    /// </summary>
    /// <param name="result">The BooleanResult</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>a TransportableBooleanResult</returns>
    public static TransportableBooleanResult FromTransportableResult<TValue>(TransportableResult<TValue> result)
        => new(result.Error);

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(nameof(TransportableBooleanResult))
          .Append(" { ");

        if (Success)
        {
            sb.Append("Success = ").Append(Success);
        }
        else
        {
            sb.Append("Error = ").Append(Error);
        }
        
        return sb.Append(" }").ToString();
    }
}