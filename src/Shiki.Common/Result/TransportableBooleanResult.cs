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
public readonly partial record struct TransportableBooleanResult : IResultState<ResultExceptionDto?>
{
    public static readonly TransportableBooleanResult Successful = new();

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;

    /// <inheritdoc/>
    public ResultExceptionDto? Error { get; }

    /// <summary>
    /// Creates a new successful TransportableBooleanResult
    /// </summary>
    public TransportableBooleanResult() : this(null)
    {
    }

    /// <summary>
    /// Creates a new TransportableBooleanResult with the held error if present
    /// </summary>
    /// <param name="error">The error</param>
    [FactoryConstructable]
    public TransportableBooleanResult(ResultExceptionDto? error = null)
    {
        Error = error;
    }

    /// <summary>
    /// Creates a new TransportableBooleanResult with the held error if present
    /// </summary>
    /// <param name="error">The error</param>
    [FactoryConstructable]
    public TransportableBooleanResult(Exception? error = null)
    {
        Error = error != null ? new ResultExceptionDto(error) : null;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(nameof(TransportableBooleanResult))
          .Append(" { ");

        if (IsSuccessful)
        {
            sb.Append("Success = ").Append(IsSuccessful);
        }
        else
        {
            sb.Append("Error = ").Append(Error);
        }

        return sb.Append(" }").ToString();
    }

    /// <summary>
    /// Converts the TransportableBooleanResult back into a normal BooleanResult
    /// </summary>
    public BooleanResult<ResultException> ToBooleanResult() =>
        IsSuccessful ? BooleanResult<ResultException>.Successful : new BooleanResult<ResultException>(Error);
    
    /// <summary>
    /// Converts the TransportableBooleanResult back into a normal BooleanResult
    /// </summary>
    /// <param name="res">The result</param>
    public static implicit operator BooleanResult<ResultException>(TransportableBooleanResult res) => res.ToBooleanResult();
}