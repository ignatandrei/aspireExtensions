namespace AspireResourceExtensions.Tests;

public partial class AspireResourceTests
{

    private (string ASPIRE_LOGIN_URL, string ASPIRE_BASE_URL) Endpoints = (
        Environment.GetEnvironmentVariable("ASPIRE_LOGIN_URL") ?? throw new ArgumentException("Should run from aspire")
        ,
        Environment.GetEnvironmentVariable("ASPIRE_BASE_URL") ?? throw new ArgumentException("Should run from aspire")
    );
    private async Task GotoAspire()
    {
        var (loginUrl, baseUrl) = Endpoints;
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        page = await browser.NewPageAsync();
        await page.GotoAsync(loginUrl);
        //var title = await page.TitleAsync();
        //await Task.Delay(10_000);
        //Assert.Equal("AspireResourceExtensions resources", title);
        await page.GotoAsync(baseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var title = await page.TitleAsync();
        Assert.Equal("AspireResourceExtensions resources", title);



        
    }
    private async Task AspireResourceGraph()
    {
        var (loginUrl, baseUrl) = Endpoints;
        //ArgumentNullException.ThrowIfNull(browser);
        //page = await browser.NewPageAsync();
        //await page.GotoAsync(loginUrl);
        //await Task.Delay(5000);// wait for AspireResource to be fully loaded
        ArgumentNullException.ThrowIfNull(page);
        await page.GotoAsync(baseUrl );
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await page.GetByRole(AriaRole.Tab,new() { Name = "Graph" ,Exact=false} ).ClickAsync();
        await Task.Delay(5000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Zoom Out", Exact = false }).ClickAsync();
        await Task.Delay(5000);
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "AspireResourceGraph.png" });

    }
    private async Task AspireResourceIsRunning()
    {
        await Task.Delay(5000);// wait for AspireResource to be fully loaded
        //await page.GotoAsync("https://localhost:17146/");
        var (loginUrl, baseUrl) = Endpoints;
        ArgumentNullException.ThrowIfNull(browser);
        page = await browser.NewPageAsync();
        await page.GotoAsync(loginUrl);
        await Task.Delay(5000);// wait for AspireResource to be fully loaded
        await page.GotoAsync(baseUrl + "consolelogs/resource/AspireResource");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "AspireResourceDetails10.png" });

        await page.GetByRole(AriaRole.Button, new() { Name = "Resource actions" }).ClickAsync();

        await page.GetByRole(AriaRole.Menuitem, new() { Name = "View Details" }).ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "AspireResourceDetails20.png" });


    }

}
