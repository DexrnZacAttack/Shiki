using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Shiki.Common.Factory;
using Shiki.Common.Serialization.Converters;

namespace Shiki.Common.Result;

/// <summary>
/// Defines an interface with a Value and an Error, with a bool about whether Value is not null.
/// </summary>
/// <typeparam name="TSuccess">The value</typeparam>
/// <typeparam name="TFailure">The error</typeparam>
public interface IResult<out TSuccess, out TFailure>
    where TSuccess : allows ref struct
    where TFailure : allows ref struct
{
    /// <summary>
    /// The stored value
    /// </summary>
    public TSuccess Value { get; }

    /// <summary>
    /// The stored error
    /// </summary>
    public TFailure Error { get; }
    
    /// <summary>
    /// Returns true if a value is stored
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    bool HasValue { get; }
}

/// <summary>
/// A small impl similar to Rust's Result type
///
/// Mostly used for serializing a value, making a request, etc.
/// </summary>
/// <typeparam name="TValue">The expected value type</typeparam>
/// <typeparam name="TException">The expected Exception type</typeparam>
[JsonConverter(typeof(NotSerializableJsonConverterFactory))]
public readonly partial struct Result<TValue, TException> : IResult<TValue?, TException?>
    where TException : Exception
{
    /// <summary>
    /// The stored value
    /// </summary>
    public TValue? Value {
        get
        {
            if (!HasValue && Error == null)
            {
                throw new InvalidOperationException("Cannot access Value on an invalid or default Result");
            }

            return field;
        }
    }

    /// <summary>
    /// The stored exception
    /// </summary>
    public TException? Error {
        get
        {
            if (HasValue && Value == null && !typeof(TValue).IsValueType)
            {
                throw new InvalidOperationException("Cannot access Error on an invalid or default Result");
            }

            return field;
        }
    }
    
    /// True when a value is stored
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }
    
    /// <summary>
    /// Constructs a Result with only a Value stored within
    /// </summary>
    /// <param name="value">The value</param>
    public Result(TValue value)
    {
        this.Value = value;
        this.Error = null;
        
        this.HasValue = true;
    }

    /// <summary>
    /// Constructs a Result with only an Exception stored within
    /// </summary>
    /// <param name="error">The exception</param>
    public Result(TException error)
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
    public Result(TValue? value, TException? exception)
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
    /// Runs a given function and returns the output as a Result, containing either the value if it succeeded, or an exception if the function threw
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns>The result containing either the value if it succeeded, or an exception if the function threw</returns>
    public static Result<TValue, TException> FromWrapped(Func<TValue> func)
    {
        try
        {
            return new Result<TValue, TException>(func());
        }
        catch (TException ex)
        {
            return new Result<TValue, TException>(ex);
        }
    }

    /// <summary>
    /// Runs a given function and returns the output as a Result, containing either the value if it succeeded, or an exception if the function threw
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns>The result containing either the value if it succeeded, or an exception if the function threw</returns>
    public static async Task<Result<TValue, TException>> FromWrappedAsync(Func<Task<TValue>> func)
    {
        try
        {
            return new Result<TValue, TException>(await func());
        }
        catch (TException ex)
        {
            return new Result<TValue, TException>(ex);
        }
    }

    /// <summary>
    /// Asserts that the value is fully expected to nonnull, and will throw the given exception otherwise.
    /// </summary>
    /// <param name="exception">The exception to throw</param>
    /// <returns>The value</returns>
    public TValue Expect(TException exception) => HasValue ? Value : throw exception;

    /// <summary>
    /// Asserts that the value is fully expected to nonnull, and will throw the stored exception otherwise.
    /// </summary>
    /// <returns>The value</returns>
    public TValue ExpectDefault()
    {
        if (HasValue)
        {
            return Value;
        }

        if (Error != null)
        {
            throw Error;
        }

        throw new InvalidOperationException("Result is invalid, neither a value nor an error is contained.");
    }

    /// <summary>
    /// Gets a boolean result from a Result
    /// </summary>
    public BooleanResult<TException> GetBooleanResult() => BooleanResult<TException>.FromResult(this);
    
    /// <summary>
    /// Gets a transportable result from a Result
    /// </summary>
    public TransportableResult<TValue> GetTransportableResult() => TransportableResult<TValue>.FromResult(this);

    /// <summary>
    /// Converts the Result into a TransportableResult
    /// </summary>
    /// <param name="res">The result</param>
    public static implicit operator TransportableResult<TValue>(Result<TValue, TException> res) => res.GetTransportableResult();

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