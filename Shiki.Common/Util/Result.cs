using System.Diagnostics.CodeAnalysis;

namespace Shiki.Common.Util;

/// <summary>
/// A small impl similar to Rust's Result type
///
/// Mostly used for serializing a value, making a request, etc.
/// </summary>
/// <typeparam name="TValue">The expected value type</typeparam>
/// <typeparam name="TException">The expected Exception type</typeparam>
public class Result<TValue, TException>
    where TException : Exception
{
    /// <summary>
    /// The stored value
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// The stored exception
    /// </summary>
    public TException? Exception { get; }

    /// <summary>
    /// Returns true if a value is stored
    /// </summary>
    [MemberNotNullWhen(false, nameof(Exception))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => Value != null;

    /// <summary>
    /// Constructs a Result with only a Value stored within
    /// </summary>
    /// <param name="value">The value</param>
    public Result(TValue value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Constructs a Result with only an Exception stored within
    /// </summary>
    /// <param name="exception">The exception</param>
    public Result(TException exception)
    {
        this.Exception = exception;
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
    public TValue ExpectDefault() => HasValue ? Value : throw Exception;
}

/// <summary>
/// A small impl similar to Rust's Result type
///
/// Mostly used for serializing a value, making a request, etc.
/// </summary>
/// <typeparam name="TValue">The expected value type</typeparam>
public class Result<TValue> : Result<TValue, Exception>
{
    /// <summary>
    /// Constructs a Result with only a Value stored within
    /// </summary>
    /// <param name="value">The value</param>
    public Result(TValue value) : base(value)
    {
    }
    
    /// <summary>
    /// Constructs a Result with only an Exception stored within
    /// </summary>
    /// <param name="exception">The exception</param>
    public Result(Exception exception) : base(exception)
    {
    }

    /// <summary>
    /// Runs a given function and returns the output as a Result, containing either the value if it succeeded, or an exception if the function threw
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns>The result containing either the value if it succeeded, or an exception if the function threw</returns>
    public new static Result<TValue> FromWrapped(Func<TValue> func)
    {
        try
        {
            return new Result<TValue>(func());
        }
        catch (Exception ex)
        {
            return new Result<TValue>(ex);
        }
    }

    /// <summary>
    /// Runs a given function and returns the output as a Result, containing either the value if it succeeded, or an exception if the function threw
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns>The result containing either the value if it succeeded, or an exception if the function threw</returns>
    public new static async Task<Result<TValue>> FromWrappedAsync(Func<Task<TValue>> func)
    {
        try
        {
            return new Result<TValue>(await func());
        }
        catch (Exception ex)
        {
            return new Result<TValue>(ex);
        }
    }

}