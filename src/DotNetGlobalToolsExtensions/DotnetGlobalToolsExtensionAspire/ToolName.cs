
namespace DotnetGlobalToolsExtensionAspire;

[Flags]
[EnumExtensions]
public enum ToolName: UInt64
{
    None = 0,
    [Display(Name = "programmerall")]
    programmerall = 1 << 0,
    [Display(Name ="dotnet-depends")]
    dotnet_depends  =1 << 1,
    [Display(Name = "dotnet-ef")]
    dotnet_ef = 1 << 2,
    [Display(Name = "dotnet-outdated")]
    dotnet_outdated = 1 << 3,
    [Display(Name = "dotnet-project-licenses")]
    dotnet_project_licenses =1 << 4,
    [Display(Name = "dotnet-property")]
    dotnet_property = 1 << 5,
    [Display(Name = "dotnet-repl")]
    dotnet_repl = 1 << 6,
    [Display(Name = "dotnetthx")]
    dotnetthx = 1 << 7,
    [Display(Name = "httprepl")]
    httprepl=1 << 8,
    [Display(Name = "netpackageanalyzerconsole")]
    netpackageanalyzerconsole=1 << 9,
    [Display(Name = "powershell")]
    powershell = 1 << 10,
    [Display(Name = "run-script")]
    run_script = 1 << 11,
    [Display(Name = "watch2")]
    watch2=1 << 12,
    
}
