using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using AspireFileDisplayExtension.Templates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
namespace AspireFileDisplayExtension;
record FileToDisplay
{
    public FileToDisplay(string relativePath,string name, params string[] lines)
    {
        this.relativePath = relativePath;
        this.name = name;
        this.lines = lines;
    }
    public readonly string relativePath;
    private readonly string name;
    private readonly string[] lines;
    public int[] indexFound=Array.Empty<int>();
    public string NameFile() => name;
    private string? contentsCache = null;
    public async Task<string> Contents()
    {
        if (contentsCache != null) return contentsCache;
        contentsCache = await File.ReadAllTextAsync(relativePath);
        
        if (lines.Length > 0)
        {
            var linesIndex = 
                contentsCache.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
                .Select((linex,index)=>new KeyValuePair<int,string>(index,linex))
                .ToArray() ;

            indexFound = lines.Select(it=> linesIndex.FirstOrDefault(li=>li.Value.Contains(it,StringComparison.InvariantCultureIgnoreCase)).Key ).ToArray();    

        }
        return contentsCache;
    }
    public int MinLine() => indexFound.Length > 0 ? indexFound.Min() : -1;
    public int MaxLine() => indexFound.Length > 0 ? indexFound.Max() : -1;
    public string MonacoLanguageId()
    {
        var ext = Path.GetExtension(relativePath);
        return Monaco.MonacoLanguageExtensions.TryGetFromExtension(ext, out var lang)
            ? Monaco.MonacoLanguageExtensions.ToMonacoId(lang)
            : "plaintext";
    }
}
public class FileDisplayResource(string name):Resource(name),IResourceWithEndpoints
{
    internal static List<FileToDisplay> files= new ();
    internal static int port;
    public string AddFile(string relativePath,string? name=null, params string[] lines)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            name = Path.GetFileName(relativePath);
        }
        var extension = Path.GetExtension(relativePath);
        if (!Monaco.MonacoLanguageExtensions.TryGetFromExtension(extension, out _))
        {
            throw new ArgumentOutOfRangeException(relativePath,new object[] { extension },$"Cannot find extension {extension}");
        }
        files.Add(new FileToDisplay(relativePath, name, lines));
        return relativePath;
        
    }
    
}

public static class AspireFileDisplayExtensions 
{
    internal static FileDisplayResource? oneAndOnlyAspireFileDisplay = null;
    public static FileDisplayResource CreateFileDisplay(this IDistributedApplicationBuilder builder, int port )
    {        
        FileDisplayResource.port= port;
        if (oneAndOnlyAspireFileDisplay == null)
        {
            oneAndOnlyAspireFileDisplay = new FileDisplayResource("FileDisplay");
            builder.AddResource(oneAndOnlyAspireFileDisplay).ExcludeFromManifest(); 
            builder.Services.TryAddEventingSubscriber<AspireFileDisplaySubscriber>(); ;
            
            
        }
        return oneAndOnlyAspireFileDisplay;
    }
}

public class AspireFileDisplaySubscriber : IDistributedApplicationEventingSubscriber
{
    public Task SubscribeAsync(
        IDistributedApplicationEventing eventing,
        DistributedApplicationExecutionContext context,
        CancellationToken cancellationToken)
    {
        eventing.Subscribe<AfterResourcesCreatedEvent>(async (@event, ct) =>
        {
            if(AspireFileDisplayExtensions.oneAndOnlyAspireFileDisplay == null)
            {
                throw new InvalidOperationException("AspireFileDisplayExtensions.oneAndOnlyAspireFileDisplay is null");
            }
            var res = AspireFileDisplayExtensions.oneAndOnlyAspireFileDisplay;
         
            var notif = @event.Services.GetService(typeof(ResourceNotificationService)) as ResourceNotificationService;
            if (notif == null) return;
            
                await notif.PublishUpdateAsync(AspireFileDisplayExtensions.oneAndOnlyAspireFileDisplay!, s => s with
                {
                    State = KnownResourceStates.Starting

                });
            
            var builder = WebApplication.CreateSlimBuilder();

            builder.WebHost.UseKestrelHttpsConfiguration();
            builder.Logging.ClearProviders();

            //builder.Logging.AddProvider(new ResourceLoggerProvider(fileRes.GetLogger(fileRes.Name)));
            var app = builder.Build();
            app.Urls.Add("http://127.0.0.1:"+FileDisplayResource.port);
            DisplayAllFiles displayAll= new (); 
            displayAll.AllFiles = [.. FileDisplayResource.files];
            var template =new DisplayAllFilesTemplate(displayAll);
            var result = await template.RenderAsync(ct);
            app.MapGet("/", () => new HtmlResult(result));
            app.UseStaticFiles(new StaticFileOptions
            {
                // EmbeddedFileProvider mangles folder-name segments of the request path
                // into Everett identifiers (e.g. "basic-languages" -> "basic_languages")
                // but keeps the file name verbatim. That matches MSBuild's default
                // resource naming exactly, so we let MSBuild name the resources and pass
                // the full "<RootNamespace>.vs" as the base namespace here.
                // NOTE: GetCallingAssembly() would return Aspire.Hosting inside this
                // eventing callback — use the extension assembly explicitly.
                FileProvider = new EmbeddedFileProvider(
                    typeof(FileDisplayResource).Assembly,
                    baseNamespace: "AspireFileDisplayExtension.vs"),
                RequestPath = "/vs"
            });
            app.MapGet("/files/{nameFile}", async ([FromRoute]string nameFile) =>
            {
                var countFiles = FileDisplayResource.files.Count(it =>
                string.Equals(it.NameFile(), nameFile, StringComparison.InvariantCultureIgnoreCase)
                );
                if(countFiles !=1 ) 
                {
                    return new HtmlResult($"xfile {nameFile} to display count {countFiles}");
                }
                var fileToDisplay = FileDisplayResource.files.First(it => it.NameFile() == nameFile);
                var result = await new DisplayFileTemplate(fileToDisplay).RenderAsync(ct);
                return new HtmlResult(result);
            });
            await app.StartAsync(ct);
            string[] adresses = [];
            var features = app.Services.GetService(typeof(IServer)) is IServer server ? server.Features: null;
            if (features != null) {
                foreach (var feature in features)
                {
                    if(feature.Value is IServerAddressesFeature addressesFeature)
                    {
                        adresses = [..adresses,..addressesFeature.Addresses.ToArray()];
                    }
                }
            }
            if (adresses.Length > 0)
            {
                await notif.PublishUpdateAsync(res, s => s with
                {
                    State = "Running",
                    Urls = adresses.Select(it=> new UrlSnapshot(it, $"{it}", IsInternal: false))
                    .ToImmutableArray()
                });
            }
            return ;
        });

        return Task.CompletedTask;
    }
}

class ResourceLoggerProvider(ILogger logger) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ResourceLogger(logger);
    }

    public void Dispose()
    {
    }

    private class ResourceLogger(ILogger logger) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    
}
class HtmlResult : IResult
{
    private readonly string _html;

    public HtmlResult(string html)
    {
        _html = html;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_html);
        return httpContext.Response.WriteAsync(_html);
    }
}
