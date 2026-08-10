using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Result;

/// <summary>
/// A throwable exception that contains the error code
/// </summary>
/// <param name="errno">The error code</param>
/// <typeparam name="TError">The error enum type</typeparam>
public class ErrorCodeException<TError>(TError errno) : Exception($"Error code {errno} is not {default(TError)}")
    where TError : struct, Enum
{
    /// <summary>
    /// The error code
    /// </summary>
    public TError ErrorCode { get; } = errno;
}

/// <summary>
/// A small impl similar to Rust's Result type, holding an error code instead of an Exception
/// Note that TError should have an Uninitialized value to handle the TValue instance being `default`.
///
/// Mostly used for serializing a value, making a request, etc.
/// </summary>
/// <typeparam name="TValue">The expected value type</typeparam>
/// <typeparam name="TError">The expected enum error code type</typeparam>
[JsonConverter(typeof(NotSerializableJsonConverterFactory))]
public readonly partial struct ErrorCodeResult<TValue, TError> : IResult<TValue?, TError>
    where TError : struct, Enum
{
    /// <summary>
    /// A default TError which should always be the last value in the enum.
    ///
    /// Used if Value is uninitialized
    /// </summary>
    public static readonly TError UninitializedFallbackError = Enum.GetValues<TError>().DefaultIfEmpty().Max(); //TODO this november replace with Union
    
    /// <summary>
    /// The stored value
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// The stored error code
    ///
    /// Returns an UninitializedFallbackError cast to TError when uninitialized.
    /// </summary>
    public TError Error {
        get
        {
            if (!HasValue && EqualityComparer<TError>.Default.Equals(field, default))
            {
                return UninitializedFallbackError;
            }

            return field;
        }
    }
    
    /// True when a value is stored
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }
    
    /// <summary>
    /// Constructs an ErrorCodeResult with only a Value stored within
    /// </summary>
    /// <param name="value">The value</param>
    public ErrorCodeResult(TValue value)
    {
        this.Value = value;
        this.Error = default;
        
        this.HasValue = true;
    }

    /// <summary>
    /// Constructs an ErrorCodeResult with only an error code stored within
    /// </summary>
    /// <param name="error">The error code</param>
    public ErrorCodeResult(TError error)
    {
        if (EqualityComparer<TError>.Default.Equals(error, default))
            throw new ArgumentException("An ErrorCodeResult containing error code of type OK must instead contain a value.");
            
        this.Value = default;
        this.Error = error;
        
        this.HasValue = false;
    }
    
    /// <summary>
    /// Constructs an ErrorCodeResult depending on which object was passed
    ///
    /// If value is null, the error code is used, if value is not null, value is used (even if the error code is not OK)
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="error">The error code</param>
    [FactoryConstructable]
    public ErrorCodeResult(TValue? value, TError? error)
    {
        if (value == null)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));
                
            if (EqualityComparer<TError>.Default.Equals(error.Value, default))
                throw new ArgumentException("An ErrorCodeResult containing error code of type OK must instead contain a value.");
            
            this.Value = default;
            this.Error = error.Value;
            
            this.HasValue = false;
        }
        else
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.Error = default;

            this.HasValue = true;
        }
    }

    /// <summary>
    /// Asserts that the value is fully expected to nonnull, and will throw the given exception otherwise.
    /// </summary>
    /// <param name="exception">The exception to throw</param>
    /// <returns>The value</returns>
    public TValue Expect(Exception exception) => HasValue ? Value : throw exception;

    /// <summary>
    /// Asserts that the value is fully expected to nonnull, and will throw the stored exception otherwise.
    /// </summary>
    /// <returns>The value</returns>
    public TValue ExpectNotNull()
    {
        if (HasValue)
        {
            return Value;
        }

        throw new ErrorCodeException<TError>(Error);
    }
    
    /// <summary>
    /// "Converts" a TError to an ErrorCodeResult.
    ///
    /// Useful for returning the TError directly without wrapping in <c>new ErrorCodeResult&lt;Value, Error&gt;(error);</c> 
    /// </summary>
    /// <param name="error">The error value</param>
    /// <returns>A new ErrorCodeResult</returns>
    public static implicit operator ErrorCodeResult<TValue, TError>(TError error) => new(error);
    /// <summary>
    /// "Converts" a TValue to an ErrorCodeResult.
    ///
    /// Useful for returning the TValue directly without wrapping in <c>new ErrorCodeResult&lt;Value, Error&gt;(value);</c> 
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A new ErrorCodeResult</returns>
    public static implicit operator ErrorCodeResult<TValue, TError>(TValue value) => new(value);
    
    /// <summary>
    /// "Converts" an ErrorCodeResult to a bool, with its value being whether a value is present in the ErrorCodeResult 
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>Whether the result contains a value</returns>
    public static implicit operator bool(ErrorCodeResult<TValue, TError> result) => result.HasValue;

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(GetType().Name)
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