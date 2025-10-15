using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Diagnostics;

namespace SqlServerExtensions.Tests;

public class WebTests :  IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(59);
    CreateAspireHost<Projects.SqlServerExtensions_AppHost>? hostWithData;
    public async ValueTask InitializeAsync()
    {

        hostWithData = await CreateAspireHost<Projects.SqlServerExtensions_AppHost>.Create(DefaultTimeout, TestContext.Current.CancellationToken);
        Process.Start(new ProcessStartInfo() { FileName = hostWithData.urlDashboard, UseShellExecute = true });
     
    }

    public async ValueTask DisposeAsync()
    {
        if (hostWithData != null) await hostWithData.DisposeAsync();
    }

    [Fact]
    public async Task DatabaseIsCorrectlyConfigured()
    {
        ArgumentNullException.ThrowIfNull(hostWithData);
        var dashboardUrl = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_URLS");


        var cancellationToken = TestContext.Current.CancellationToken;
        await hostWithData.app.ResourceNotifications.WaitForResourceHealthyAsync("DepEmp", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        var cnString = await hostWithData.app.GetConnectionStringAsync("DepEmp", cancellationToken);
        await Task.Delay(20000,cancellationToken);
        await Task.Yield();
        var message = string.Join("\r\n", hostWithData.fakeLoggerProvider!.Collector.GetSnapshot().Select(it => it.Message).ToArray());
        using var cn = new SqlConnection(cnString);
        await cn.OpenAsync(cancellationToken);
        using var cmd = cn.CreateCommand();
        cmd.CommandText = "select count(*) from Department";
        var res = await cmd.ExecuteScalarAsync(cancellationToken);
        Assert.NotNull(res);
        Assert.Equal("2", res.ToString());
    }

    
}
