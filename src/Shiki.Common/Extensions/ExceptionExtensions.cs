using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Shiki.Common.Extensions;

public static class ExceptionExtensions
{
    extension<TException>(TException exception)
    where TException : Exception
    {
        /// <summary>
        /// Sets the stacktrace on a given exception
        /// </summary>
        /// <returns>The exception</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]//hopefully will inline and fix the stacktraces
        public TException WithCurrentStackTrace()
        {
            ExceptionDispatchInfo.SetCurrentStackTrace(exception);
            return exception;
        }
    }
}