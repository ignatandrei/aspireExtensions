
namespace AspireResourceExtensions.Tests
{
    [FeatureDescription("Integration")]
    public partial class AspireResourceTests : FeatureFixture, IAsyncLifetime
    {
        IPlaywright? playwright;
        IBrowser? browser;
        IPage? page;
        public async Task DisposeAsync()
        {
            if(page != null) await page.CloseAsync();
            if(browser != null) await browser.CloseAsync();
            if(playwright != null) playwright.Dispose();
            
        }

        public async Task InitializeAsync()
        {
            var (loginUrl, baseUrl) = Endpoints;
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            page = await browser.NewPageAsync();
            await page.GotoAsync(loginUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.GotoAsync(baseUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        }

        [Scenario]
        [Trait("Category", "Integration")]
        public async Task SeeFirstPage()
        {
            //by the fact that this test runs, we know that AspireResource is running
            //and sending endpoints to the test project
            await Runner.RunScenarioAsync(  
            _ => GotoAspire(),
            _ => AspireResourceIsRunning()
            );
        }
        [Scenario]
        [Trait("Category", "Integration")]
        public async Task ScreenshotGraph()
        {
            //by the fact that this test runs, we know that AspireResource is running
            //and sending endpoints to the test project
            await Runner.RunScenarioAsync(
            _ => GotoAspire(),
            _ => AspireResourceGraph()
            );
        }

    }
}
