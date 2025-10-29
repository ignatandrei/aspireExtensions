namespace AspireResourceExtensions.PlaywrightTests;

public class BasicPlaywrightTest
{
    [Fact]
    public async Task Should_Open_Google_Homepage()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://www.google.com");
        var title = await page.TitleAsync();
        Assert.Contains("Google", title);
    }
}
