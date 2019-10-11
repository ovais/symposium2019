param(
    [Parameter(Mandatory = $true)][string] $intanceName,
    [Parameter(Mandatory = $false)][string] $instancesRoot = "C:\inetpub\wwwroot",
    [Parameter(Mandatory = $false)][string] $configuration = "debug"
)

$cmsRoot = Join-Path $instancesRoot $intanceName | Join-Path -ChildPath "website"
$xconnectRoot = Join-Path $instancesRoot "$intanceName`_xconnect" | Join-Path -ChildPath "website"
$indexerRoot = Join-Path $instancesRoot "$intanceName`_IndexWorkerService"
$peRoot = Join-Path $instancesRoot "$intanceName`_ProcessingEngineService"
$maRoot = Join-Path $instancesRoot "$intanceName`_AutomationEngineService"
$utRoot = Join-Path $instancesRoot "$intanceName`_ut"

$nameMap = @{
    MarketingAutomationEngine = $maRoot
    ProcessingEngine          = $peRoot
    UniversalTracker          = $utRoot
}

Write-Host "Deploy model..." -ForegroundColor "Green"
"$cmsRoot\bin", "$xconnectRoot\bin", $indexerRoot, $processingEngineRoot, $maRoot |
    ForEach-Object {
        Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Model\bin\$configuration\Sitecore.Demo.Model.dll" $_ -Force
    }

Get-ChildItem "source\Sitecore.Demo\Sitecore.Demo.Model\configs" |
    ForEach-Object {
        Copy-Item  $_.FullName "$($nameMap[$_.Name])\App_Data\Model" -Recurse -Force
    }

Write-Host "Deploy model...Done" -ForegroundColor "Green"

Write-Host "Deploy CMS helpers..." -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\App_Config\**" "$cmsRoot\App_Config\" -Recurse -Force
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\bin\$configuration\Sitecore.Demo.Cms.dll" "$cmsRoot\bin" -Force
Write-Host "Deploy CMS helpers...Done" -ForegroundColor "Green"

Write-Host "Deploy Workers..." -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.CortexWorkers\App_Data\**" "$peRoot\App_Data\" -Recurse -Force
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.CortexWorkers\bin\$configuration\Sitecore.Demo.CortexWorkers.dll" "$cmsRoot\bin" -Force
Write-Host "Deploy Workers...Done" -ForegroundColor "Green"
