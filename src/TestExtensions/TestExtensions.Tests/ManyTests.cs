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
        Console.WriteLine("Starting TestOK...");
        Assert.Fail( "This test should NOT pass.");
    }

    [Fact]
    [Trait("Category", "Logic")]
    public async Task TestLogic()
    {
        await Task.Delay(1000, TestContext.Current.CancellationToken); // Simulate some work
        Console.WriteLine("Starting TestOK...");
        Assert.True(true, "This test should always pass.");
    }
}
