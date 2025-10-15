using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Diagnostics;
using System.Threading;

namespace SqlServerExtensions.Tests;
[CollectionDefinition("SerialTests", DisableParallelization = true)]
public class SerialTestsCollection { }

public class WebTestsFixture :IAsyncDisposable
{
    private CreateAspireHost<Projects.SqlServerExtensions_AppHost>? HostWithData { get; set; }

    public WebTestsFixture()
    {
    }
    static Lock lockHost = new Lock();
    public async Task<CreateAspireHost<Projects.SqlServerExtensions_AppHost>> GetHostWithData()
    {
    
        
        lock (lockHost)
        {
            if (HostWithData != null) return HostWithData;

        }

        HostWithData = await CreateAspireHost<Projects.SqlServerExtensions_AppHost>.Create(
            TimeSpan.FromSeconds(59), TestContext.Current.CancellationToken);
        Process.Start(new ProcessStartInfo() { FileName = HostWithData.urlDashboard, UseShellExecute = true });
        return HostWithData;
    }

    public async ValueTask DisposeAsync()
    {
        if (HostWithData != null) await HostWithData.DisposeAsync();
    }

   
}

[Collection("SerialTests")]
public class WebTests : IClassFixture<WebTestsFixture>
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(59);
    //static CreateAspireHost<Projects.SqlServerExtensions_AppHost>? hostWithData;


    private readonly WebTestsFixture _fixture;

    public WebTests(WebTestsFixture fixture)
    {
        _fixture = fixture;
        
    }
    

    public async ValueTask DisposeAsync()
    {
        if(_fixture != null)
            await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task DatabaseIsCorrectlyConfigured()
    {
        var hostWithData = await _fixture.GetHostWithData();
        var cancellationToken = TestContext.Current.CancellationToken;

        ArgumentNullException.ThrowIfNull(hostWithData);

        await hostWithData.app!.ResourceNotifications.WaitForResourceHealthyAsync("DepEmp", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        var cnString = await hostWithData.app!.GetConnectionStringAsync("DepEmp", cancellationToken);
        await Task.Delay(20000,cancellationToken);
        await Task.Yield();
        var message = string.Join("\r\n", hostWithData.fakeLoggerProvider!.Collector.GetSnapshot().Select(it => it.Message).ToArray());
        using var cn = new SqlConnection(cnString);
        await cn.OpenAsync(cancellationToken);
        using var cmd = cn.CreateCommand();
        cmd.CommandText = "select count(*) from Employee";
        var res = await cmd.ExecuteScalarAsync(cancellationToken);
        Assert.NotNull(res);
        Assert.Equal("2", res.ToString());
    }
    [Fact]
    public async Task SqlCommandWorks()
    {
        var hostWithData = await _fixture.GetHostWithData();
        var ct = TestContext.Current.CancellationToken;
        await DatabaseIsCorrectlyConfigured();
        var message = string.Join("\r\n", hostWithData.fakeLoggerProvider!.Collector.GetSnapshot(true).Select(it => it.Message).ToArray());
        var cnString = await hostWithData.app!.GetConnectionStringAsync("DepEmp", ct);
        var result= await hostWithData.app.ResourceCommands.ExecuteCommandAsync("DepEmp", "deleteEmployee", ct);
        //TODO: see fake logger messages
        Assert.True(result.Success);
        //await Task.Delay(20000, ct);
        using var cn = new SqlConnection(cnString);
        await cn.OpenAsync(ct);
        using var cmd = cn.CreateCommand();
        cmd.CommandText = "select count(*) from Employee";
        var res = await cmd.ExecuteScalarAsync(ct);
        Assert.NotNull(res);
        Assert.Equal("0", res.ToString());
    }
    [Fact]
    public async Task RestoreDatabaseWorks()
    {
        var hostWithData = await _fixture.GetHostWithData();
        var ct = TestContext.Current.CancellationToken;
        await SqlCommandWorks();
        var message = string.Join("\r\n", hostWithData.fakeLoggerProvider!.Collector.GetSnapshot(true).Select(it => it.Message).ToArray());
        var result = await hostWithData.app.ResourceCommands.ExecuteCommandAsync("DepEmp", "reset-all", ct);
        //TODO: see fake logger messages
        Assert.True(result.Success);
        //await Task.Delay(30000, ct);
        var cnString = await hostWithData.app!.GetConnectionStringAsync("DepEmp", ct);
        using var cn = new SqlConnection(cnString);
        await cn.OpenAsync(ct);
        using var cmd = cn.CreateCommand();
        cmd.CommandText = "select count(*) from Employee";
        var res = await cmd.ExecuteScalarAsync(ct);
        Assert.NotNull(res);
        Assert.Equal("2", res.ToString());
    }
    [Fact]
    public async Task DashboardIsAvailable()
    {
        var hostWithData = await _fixture.GetHostWithData();
        var cancellationToken = TestContext.Current.CancellationToken;
        ArgumentNullException.ThrowIfNull(hostWithData);
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(hostWithData.urlDashboard, cancellationToken);
        response.EnsureSuccessStatusCode();
        //var content = await response.Content.ReadAsStringAsync(cancellationToken);
        //Assert.Contains("Aspire Hosting Dashboard", content);
    }
    //[Theory]
    //[InlineData(60)]
    //public async Task EnsureDashboardAvailable(int seconds)
    //{
    //    Process.Start(new ProcessStartInfo() { FileName = hostWithData!.urlDashboard, UseShellExecute = true });
    //    await Task.Delay(seconds*1000, TestContext.Current.CancellationToken);
    //}

}
