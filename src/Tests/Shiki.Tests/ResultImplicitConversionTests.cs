using Shiki.Common.Result;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Tests;

public class ResultImplicitConversionTests
{
    private enum TestErrorCode { Ok = 0, Garbage = 1 }

    #region Result<TValue, TException> Conversions

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(TransportableResult<object>)} implicitly")]
    public void SuccessfulResultToTransportableResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        TransportableResult<string> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(TransportableResult<object>)} implicitly")]
    public void UnsuccessfulResultToTransportableResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        TransportableResult<string> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void SuccessfulResultToBooleanResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        BooleanResult<Exception> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void UnsuccessfulResultToBooleanResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        BooleanResult<Exception> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(TransportableBooleanResult)} implicitly")]
    public void SuccessfulResultToTransportableBooleanResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        TransportableBooleanResult c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(TransportableBooleanResult)} implicitly")]
    public void UnsuccessfulResultToTransportableBooleanResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        TransportableBooleanResult c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region BooleanResult<TException> Conversions

    [Test(Description = $"Convert successful {nameof(BooleanResult<Exception>)} to {nameof(TransportableBooleanResult)} implicitly")]
    public void SuccessfulBooleanResultToTransportableBooleanResult()
    {
        var res = BooleanResult<Exception>.Successful;
        TransportableBooleanResult c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(BooleanResult<Exception>)} to {nameof(TransportableBooleanResult)} implicitly")]
    public void UnsuccessfulBooleanResultToTransportableBooleanResult()
    {
        var res = new BooleanResult<Exception>(new InvalidOperationException("error"));
        TransportableBooleanResult c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region TransportableResult<TValue> Conversions

    [Test(Description = $"Convert successful {nameof(TransportableResult<object>)} to {nameof(Result<object, Exception>)} implicitly")]
    public void SuccessfulTransportableResultToResult()
    {
        var res = new TransportableResult<string>("Hello, world!");
        Result<string, ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableResult<object>)} to {nameof(Result<object, Exception>)} implicitly")]
    public void UnsuccessfulTransportableResultToResult()
    {
        var res = new TransportableResult<string>(new ResultExceptionDto(new InvalidOperationException("error")));
        Result<string, ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ResultException>());
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(TransportableResult<object>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void SuccessfulTransportableResultToBooleanResult()
    {
        var res = new TransportableResult<string>("Hello, world!");
        BooleanResult<ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableResult<object>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void UnsuccessfulTransportableResultToBooleanResult()
    {
        var res = new TransportableResult<string>(new ResultExceptionDto(new InvalidOperationException("error")));
        BooleanResult<ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ResultException>());
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region TransportableBooleanResult Conversions

    [Test(Description = $"Convert successful {nameof(TransportableBooleanResult)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void SuccessfulTransportableBooleanResultToBooleanResult()
    {
        TransportableBooleanResult res = TransportableBooleanResult.Successful;
        BooleanResult<ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableBooleanResult)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void UnsuccessfulTransportableBooleanResultToBooleanResult()
    {
        TransportableBooleanResult res = new(new ResultExceptionDto(new InvalidOperationException("error")));
        BooleanResult<ResultException> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ResultException>());
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region ErrorCodeResult<TValue, TError> Conversions

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(Result<object, Exception>)} implicitly")]
    public void SuccessfulErrorCodeResultToResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        Result<string, ErrorCodeException<TestErrorCode>> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(Result<object, Exception>)} implicitly")]
    public void UnsuccessfulErrorCodeResultToResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        Result<string, ErrorCodeException<TestErrorCode>> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ErrorCodeException<TestErrorCode>>());
            Assert.That(c.Error!.ErrorCode, Is.EqualTo(TestErrorCode.Garbage));
        }
    }

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void SuccessfulErrorCodeResultToBooleanResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        BooleanResult<ErrorCodeException<TestErrorCode>> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(BooleanResult<Exception>)} implicitly")]
    public void UnsuccessfulErrorCodeResultToBooleanResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        BooleanResult<ErrorCodeException<TestErrorCode>> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ErrorCodeException<TestErrorCode>>());
            Assert.That(c.Error!.ErrorCode, Is.EqualTo(TestErrorCode.Garbage));
        }
    }

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(TransportableResult<object>)} implicitly")]
    public void SuccessfulErrorCodeResultToTransportableResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        TransportableResult<string> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(TransportableResult<object>)} implicitly")]
    public void UnsuccessfulErrorCodeResultToTransportableResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        TransportableResult<string> c = res;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            
            Assert.That(c.Error!.Message, Does.Contain(nameof(TestErrorCode.Garbage)));
        }
    }

    #endregion
}