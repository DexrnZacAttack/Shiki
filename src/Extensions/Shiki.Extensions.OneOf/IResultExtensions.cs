using OneOf;
using Shiki.Common.Result;

namespace Shiki.Extensions.OneOf;

public static class IResultExtensions
{
    extension<TSuccess, TFailure>(IResult<TSuccess, TFailure> result)
    {
        public OneOf<TSuccess, TFailure> SuccessOrFailure()
        {
            return result.HasValue ? result.Value : result.Error;
        }
    }
}