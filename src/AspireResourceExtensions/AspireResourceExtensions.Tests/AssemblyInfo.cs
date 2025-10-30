[assembly: LightBddSMyScope]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: ClassCollectionBehavior(AllowTestParallelization = false)]
public class LightBddSMyScopeAttribute : LightBddScopeAttribute
{
    protected override void OnConfigure(LightBddConfiguration configuration)
    {
        
        configuration.ReportWritersConfiguration()
               .AddFileWriter<XmlReportFormatter>("~\\Reports\\FeaturesReport.xml")
                .AddFileWriter<MarkdownReportFormatter>("~\\Reports\\FeaturesReport.md")
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\Feature{TestDateTimeUtc:yyyy-MM-dd}.txt")
                .AddFileWriter<HtmlReportFormatter>("~\\Reports\\FeaturesReport.html")
                ;
        base.OnConfigure(configuration);
    }
}
public static class GlobalTestSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        var playwrightInstallTask = Microsoft.Playwright.Program.Main(new[] { "install" });
;
    }
}