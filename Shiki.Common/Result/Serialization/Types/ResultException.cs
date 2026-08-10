namespace Shiki.Common.Result.Serialization.Types;

/// <summary>
/// An exception that can be created from a ResultExceptionDto and thrown
/// </summary>
/// <param name="ex"></param>
public sealed class ResultException(ResultExceptionDto ex) : Exception(ex.Message, ex.InnerException != null ? new ResultException(ex.InnerException) : null)
{
    /// <summary>
    /// The type of exception stored in the given ResultExceptionDto
    /// </summary>
    public string Type { get; } = ex.Type;
}