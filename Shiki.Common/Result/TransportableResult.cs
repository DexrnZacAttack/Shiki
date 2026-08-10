using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Result.Serialization;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Common.Result;

/// <summary>
/// Base TransportableResult interface
/// </summary>
public interface ITransportableResult;

/// <summary>
/// Result type built to be serialized and deserialized
/// </summary>
/// <typeparam name="TValue">The value type</typeparam>
[JsonConverter(typeof(ResultJsonConverterFactory))]
public readonly partial struct TransportableResult<TValue> : IResult<TValue?, ResultExceptionDto?>, ITransportableResult
{
    /// <inheritdoc/>
    public TValue? Value { get; }

    /// <inheritdoc/>
    public ResultExceptionDto? Error { get; }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }
    
    /// <summary>
    /// Constructs a Result with only a Value stored within
    /// </summary>
    /// <param name="value">The value</param>
    public TransportableResult(TValue value)
    {
        this.Value = value;
        this.Error = null;
        
        this.HasValue = true;
    }

    /// <summary>
    /// Constructs a Result with only an Exception stored within
    /// </summary>
    /// <param name="error">The exception</param>
    public TransportableResult(ResultExceptionDto error)
    {
        this.Value = default;
        this.Error = error;
        
        this.HasValue = false;
    }
    
    /// <summary>
    /// Constructs a Result depending on which object was passed
    ///
    /// If value is null, exception is used, if value is not null, value is used (even if exception is nonnull)
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="exception">The exception</param>
    [FactoryConstructable]
    public TransportableResult(TValue? value, ResultExceptionDto? exception)
    {
        if (value == null)
        {
            this.Value = default;
            this.Error = exception ?? throw new ArgumentNullException(nameof(exception));
            
            this.HasValue = false;
        }
        else
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.Error = null;

            this.HasValue = true;
        }
    }
    
    /// <summary>
    /// Creates a new TransportableResult from a Result
    /// </summary>
    /// <param name="result">The Result</param>
    /// <typeparam name="TException">The Result's exception type</typeparam>
    /// <returns>new TransportableResult</returns>
    public static TransportableResult<TValue> FromResult<TException>(Result<TValue, TException> result)
    where TException : Exception
    => result.HasValue
           ? new TransportableResult<TValue>(result.Value)
           : new TransportableResult<TValue>(new ResultExceptionDto(result.Error));

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(nameof(TransportableBooleanResult))
          .Append(" { ");

        if (HasValue)
        {
            sb.Append("Value = ").Append(Value?.ToString() ?? "null");
        }
        else
        {
            sb.Append("Error = ").Append(Error);
        }
        
        return sb.Append(" }").ToString();
    }
}