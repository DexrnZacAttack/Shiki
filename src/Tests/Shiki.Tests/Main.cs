using Serilog;

[SetUpFixture]
public class Main
{
    [OneTimeSetUp]
    public void InitializeLogger()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }
}