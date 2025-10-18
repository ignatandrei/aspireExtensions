using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Diagnostics;

namespace SqlServerExtensions.Tests;

public class CreateAspireHost<TEntryPoint>: IAsyncDisposable
    where TEntryPoint : class
{
    private CreateAspireHost()
    {
        
    }
    public static async Task<CreateAspireHost<TEntryPoint>> Create(
        TimeSpan DefaultTimeout ,string? WaitForResource=null, CancellationToken cancellationToken= CancellationToken.None)
    {
        CreateAspireHost<TEntryPoint> instance = new CreateAspireHost<TEntryPoint>();
        instance.fakeLoggerProvider = new FakeLoggerProvider();
        instance.appHost = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>([],
            (dao, habs) =>
            {
                dao.DisableDashboard = false;
                dao.AllowUnsecuredTransport = true;                
            },
            cancellationToken);
        instance.appHost.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddProvider(instance.fakeLoggerProvider);
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        instance.appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        instance.app = await instance.appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);


        await instance.app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        var tsWait =TimeSpan.FromSeconds(1);
        if(WaitForResource != null)
        {
            await instance.app.ResourceNotifications.WaitForResourceHealthyAsync(WaitForResource, cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        }
        while (true)
        {
            await Task.Delay(tsWait, cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            DefaultTimeout= DefaultTimeout.Subtract(tsWait);            
            if( DefaultTimeout <= TimeSpan.Zero) throw new TimeoutException("Timeout waiting for Aspire Host to start");

            var messages= instance.fakeLoggerProvider.Collector.GetSnapshot(false).Select(it => it.Message).ToArray();
            var mes = messages.FirstOrDefault(it => it.Contains("\"Message\":\"Now listening on:"));
            if (mes == null) continue;
            var match = System.Text.RegularExpressions.Regex.Match(mes, @"Now listening on: (?<url>http://[^:]+:(?<port>\d+))");
            if (match.Success)
            {
                instance.urlDashboard = match.Groups["url"].Value;
//                Process.Start(new ProcessStartInfo() { FileName = urlDashboard, UseShellExecute = true });
                break;
            }

            
        }
        return instance;
    }

    public async ValueTask DisposeAsync()
    {
        if (app != null)
        {
            await app.StopAsync();
            app = null;
        }
        if (appHost != null)
        {
            await appHost.DisposeAsync();
            appHost = null;
        }
    }

    public FakeLoggerProvider? fakeLoggerProvider;
    public DistributedApplication? app = null;
    public IDistributedApplicationTestingBuilder? appHost = null;
    public string? urlDashboard;


}