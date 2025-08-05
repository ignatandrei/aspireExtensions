namespace TestExtensions.Tests;

public class ManyTests
{
 
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TestOK()
    {
        await Task.Delay(1000, TestContext.Current.CancellationToken); // Simulate some work
        Console.WriteLine("Starting TestOK...");
        Assert.True(true, "This test should always pass.");
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TestNotOK()
    {
        await Task.Delay(1000, TestContext.Current.CancellationToken); // Simulate some work
        Console.WriteLine("Starting TestNotOK...");
        Assert.Fail( "This test should NOT pass.");
    }

    [Fact]
    [Trait("Category", "Logic")]
    public async Task TestLogic()
    {
        await Task.Delay(1000, TestContext.Current.CancellationToken); // Simulate some work
        Console.WriteLine("Starting TestLogic...");
        Assert.True(true, "This test should always pass.");
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TestEnvironment()
    {
        await Task.Delay(1000, TestContext.Current.CancellationToken); // Simulate some work
        Console.WriteLine("Starting TestEnvironment...");
        string? https = null;
        var envData = Environment.GetEnvironmentVariables();
        var keys = envData.Keys;

        foreach (var env in keys)
        {
            if (env == null) continue;
            if (env.ToString() == null) continue;
            if (env.ToString()!.Contains("http"))
            {
                https = env.ToString();
                Assert.True(envData[env] is string, $"Environment variable {env} should be a string.");

            }
            Console.WriteLine($"Environment Variable: {env} - value {envData[env]}");
        }

        Assert.NotNull(https);
    }
}
