using System.Diagnostics;

namespace SqlServerExtensions.Tests;

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
