using Shiki.Common.Result;
using Shiki.Common.Result.Serialization.Types;

namespace Shiki.Tests;

public class ResultConversionTests
{
    private enum TestErrorCode { Ok = 0, Garbage = 1 }

    #region Result<TValue, TException> Conversions

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(TransportableResult<object>)}")]
    public void SuccessfulResultToTransportableResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        var c = res.ToTransportableResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(TransportableResult<object>)}")]
    public void UnsuccessfulResultToTransportableResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        var c = res.ToTransportableResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(BooleanResult<Exception>)}")]
    public void SuccessfulResultToBooleanResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(BooleanResult<Exception>)}")]
    public void UnsuccessfulResultToBooleanResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(Result<object, Exception>)} to {nameof(TransportableBooleanResult)}")]
    public void SuccessfulResultToTransportableBooleanResult()
    {
        var res = new Result<string, Exception>("Hello, world!");
        TransportableBooleanResult c = res.ToTransportableBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(Result<object, Exception>)} to {nameof(TransportableBooleanResult)}")]
    public void UnsuccessfulResultToTransportableBooleanResult()
    {
        var res = new Result<string, Exception>(new InvalidOperationException("error"));
        TransportableBooleanResult c = res.ToTransportableBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region BooleanResult<TException> Conversions

    [Test(Description = $"Convert successful {nameof(BooleanResult<Exception>)} to {nameof(TransportableBooleanResult)}")]
    public void SuccessfulBooleanResultToTransportableBooleanResult()
    {
        var res = BooleanResult<Exception>.Successful;
        TransportableBooleanResult c = res.ToTransportableBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(BooleanResult<Exception>)} to {nameof(TransportableBooleanResult)}")]
    public void UnsuccessfulBooleanResultToTransportableBooleanResult()
    {
        var res = new BooleanResult<Exception>(new InvalidOperationException("error"));
        TransportableBooleanResult c = res.ToTransportableBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    #endregion

    #region TransportableResult<TValue> Conversions

    [Test(Description = $"Convert successful {nameof(TransportableResult<object>)} to {nameof(Result<object, Exception>)}")]
    public void SuccessfulTransportableResultToResult()
    {
        var res = new TransportableResult<string>("Hello, world!");
        var c = res.ToResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableResult<object>)} to {nameof(Result<object, Exception>)}")]
    public void UnsuccessfulTransportableResultToResult()
    {
        var res = new TransportableResult<string>(new ResultExceptionDto(new InvalidOperationException("error")));
        var c = res.ToResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ResultException>());
            Assert.That(c.Error!.Message, Is.EqualTo("error"));
        }
    }

    [Test(Description = $"Convert successful {nameof(TransportableResult<object>)} to {nameof(BooleanResult<Exception>)}")]
    public void SuccessfulTransportableResultToBooleanResult()
    {
        var res = new TransportableResult<string>("Hello, world!");
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableResult<object>)} to {nameof(BooleanResult<Exception>)}")]
    public void UnsuccessfulTransportableResultToBooleanResult()
    {
        var res = new TransportableResult<string>(new ResultExceptionDto(new InvalidOperationException("error")));
        var c = res.ToBooleanResult();

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

    [Test(Description = $"Convert successful {nameof(TransportableBooleanResult)} to {nameof(BooleanResult<Exception>)}")]
    public void SuccessfulTransportableBooleanResultToBooleanResult()
    {
        TransportableBooleanResult res = TransportableBooleanResult.Successful;
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(TransportableBooleanResult)} to {nameof(BooleanResult<Exception>)}")]
    public void UnsuccessfulTransportableBooleanResultToBooleanResult()
    {
        TransportableBooleanResult res = new(new ResultExceptionDto(new InvalidOperationException("error")));
        var c = res.ToBooleanResult();

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

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(Result<object, Exception>)}")]
    public void SuccessfulErrorCodeResultToResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        var c = res.ToResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(Result<object, Exception>)}")]
    public void UnsuccessfulErrorCodeResultToResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        var c = res.ToResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.False);
            Assert.That(c.Value, Is.Null);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ErrorCodeException<TestErrorCode>>());
            Assert.That(c.Error!.ErrorCode, Is.EqualTo(TestErrorCode.Garbage));
        }
    }

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(BooleanResult<Exception>)}")]
    public void SuccessfulErrorCodeResultToBooleanResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.True);
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(BooleanResult<Exception>)}")]
    public void UnsuccessfulErrorCodeResultToBooleanResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        var c = res.ToBooleanResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.IsSuccessful, Is.False);
            Assert.That(c.Error, Is.Not.Null);
            Assert.That(c.Error, Is.InstanceOf<ErrorCodeException<TestErrorCode>>());
            Assert.That(c.Error!.ErrorCode, Is.EqualTo(TestErrorCode.Garbage));
        }
    }

    [Test(Description = $"Convert successful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(TransportableResult<object>)}")]
    public void SuccessfulErrorCodeResultToTransportableResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>("Hello, world!");
        var c = res.ToTransportableResult();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.HasValue, Is.True);
            Assert.That(c.Value, Is.EqualTo("Hello, world!"));
            Assert.That(c.Error, Is.Null);
        }
    }

    [Test(Description = $"Convert unsuccessful {nameof(ErrorCodeResult<object, TestErrorCode>)} to {nameof(TransportableResult<object>)}")]
    public void UnsuccessfulErrorCodeResultToTransportableResult()
    {
        var res = new ErrorCodeResult<string, TestErrorCode>(TestErrorCode.Garbage);
        var c = res.ToTransportableResult();

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