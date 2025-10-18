Write-Host "Starting SqlServerExtensions AppHost..."
cd ..
Write-Host "Current directory: $(Get-Location)"
cd SqlServerExtensions
cd SqlServerExtensions.AppHost
dotnet run --project SqlServerExtensions.AppHost.csproj --profile http