using System.Runtime.CompilerServices;
using Shiki.Common.Result;

namespace Shiki.Common.Extensions;

public static class ObjectDisposedExceptionExtensions
{
    extension(ObjectDisposedException ex)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectDisposedException GetFor(object? instance) => new ObjectDisposedException(instance?.GetType().FullName).WithCurrentStackTrace();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIf(bool condition, object? instance, ref BooleanResult<Exception> result)
        {
            if (condition)
            {
                result = ObjectDisposedException.GetFor(instance);
            }

            return condition;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIf(bool condition, object? instance, ref BooleanResult<SystemException> result)
        {
            if (condition)
            {
                result = ObjectDisposedException.GetFor(instance);
            }

            return condition;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIf(bool condition, object? instance, ref BooleanResult<InvalidOperationException> result)
        {
            if (condition)
            {
                result = ObjectDisposedException.GetFor(instance);
            }

            return condition;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIf(bool condition, object? instance, ref BooleanResult<ObjectDisposedException> result)
        {
            if (condition)
            {
                result = ObjectDisposedException.GetFor(instance);
            }

            return condition;
        }
    }
}