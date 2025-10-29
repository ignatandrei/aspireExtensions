namespace AspireResourceExtensionsAspire;

public static class AddAspire
{
    static AspireResource resource = new();
    static bool added = false;
    public static IResourceBuilder<AspireResource>? AddAspireResource(this IDistributedApplicationBuilder builder)
    {
        if (added)
        {
            return null;
        }
        added = true;
        var res = builder.AddResource(resource);
        builder.Services.AddFakeLogging();
        return res;
    }
    
    static string? LoginUrl = null;
    
    internal static async Task<string?> ViewData(DistributedApplication da)
    {
        await Task.Delay(1000);
        var fake = typeof(FakeLoggerProvider);
        var logger = da.Services.GetServices<ILoggerProvider>();
        FakeLoggerProvider? fakeLogger = null;
        foreach (var service in logger)
        {
            if (service.GetType() == fake)
            {
                fakeLogger = service as FakeLoggerProvider;
            }
        }
        ArgumentNullException.ThrowIfNull(fakeLogger);
        var tsWait = TimeSpan.FromSeconds(5);
        string? url = null;
        while (url == null)
        {

            var messages = fakeLogger.Collector.GetSnapshot().Select(x => x.Message).ToArray();
            //Console.WriteLine("wating"+string.Join(',',messages));
            await Task.Delay(tsWait);
            url = FindUrl(messages);
        }
        string? login = null;
        var nrRetry = 10;
        while (url != null && login == null && nrRetry > 0)
        {
            nrRetry--;
            var messages = fakeLogger.Collector.GetSnapshot().Select(x => x.Message).ToArray();
            login = FindLogin(messages);
            await Task.Delay(tsWait);
        }
        LoginUrl = login ?? url;
        return LoginUrl;
    }
    private static string? FindLogin(string[] messages)
    {
        var mes = messages.FirstOrDefault(it => it.Contains("Login to the dashboard at"));
        if (mes == null) return null;
        var url = mes.Replace("Login to the dashboard at", "");
        return url.Trim();
    }
    private static string? FindUrl(string[] messages)
    {
        var mes = messages.FirstOrDefault(it => it.Contains("Now listening on:"));
        if (mes == null) return null;
        var url = mes.Replace("Now listening on:", "");
        return url.Trim();

    }
}
