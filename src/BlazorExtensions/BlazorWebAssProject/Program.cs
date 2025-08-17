using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWebAssProject;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// name of the apiService in the AppHost project
//var apiService = builder.AddProject<Projects.BlazorExtensions_ApiService>("apiservice")
var hostApi = builder.Configuration["apiservice_host"];
if (string.IsNullOrEmpty(hostApi))
{
    hostApi = builder.HostEnvironment.BaseAddress;
    if (!hostApi.EndsWith("/"))
    {
        hostApi += "/";
    }
    //uncomment the following lines to set a default value for the hostApi
    //    var dict = new Dictionary<string, string?> { { "your name here", hostApi } };
    //    builder.Configuration.AddInMemoryCollection(dict.ToArray());
}

builder.Services.AddKeyedScoped("api",(sp,_) => new HttpClient { BaseAddress = new Uri(hostApi) });
builder.Services.AddKeyedScoped("local_host", (sp, _) => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
