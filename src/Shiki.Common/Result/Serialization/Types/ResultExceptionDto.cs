using System.Text.Json.Serialization;

namespace Shiki.Common.Result.Serialization.Types;

/// <summary>
/// A portable exception-like type.
/// </summary>
/// <param name="Type">The exception type</param>
/// <param name="Message">The exception message</param>
/// <param name="InnerException">An inner exception, if present</param>
[method: JsonConstructor]
public record ResultExceptionDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("message")]
    string Message,
    [property: JsonPropertyName("inner")] ResultExceptionDto? InnerException)
{
    /// <summary>
    /// Creates a new ResultExceptionDto from a given Exception
    /// </summary>
    /// <param name="ex">The exception</param>
    public ResultExceptionDto(Exception ex) : this(ex.GetType().FullName ?? "", ex.Message, ex.InnerException != null ? new ResultExceptionDto(ex.InnerException) : null) {}

    /// <summary>
    /// Throws the DTO as a new exception
    /// </summary>
    /// <exception cref="ResultException">The exception</exception>
    public void Throw() => throw new ResultException(this);

    /// <summary>
    /// Converts the DTO to a ResultException
    /// </summary>
    /// <param name="dto">this</param>
    /// <returns>The DTO as a throwable exception</returns>
    public static implicit operator ResultException(ResultExceptionDto dto) => new(dto);
}