param(
    [Parameter(Mandatory = $false)][string] $instancesRoot = "C:\inetpub\wwwroot\",
    [Parameter(Mandatory = $false)][string] $configuration = "debug"
)
& {iisreset /Stop}

#define paths and names
$services = "habitathome_xconnect.dev.local-ProcessingEngineService"
$xconnectRoot = Join-Path $instancesRoot "habitathome_xconnect.dev.local" 
$utProcessing = Join-Path $instancesRoot "UniversalTrackerutprocessing"
$utCollection = Join-Path $instancesRoot "UniversalTrackerutcollection"
$cmsRoot = Join-Path $instancesRoot "habitathome.dev.local"
$processingEngineRoot = Join-Path $xconnectRoot "App_Data\jobs\continuous\ProcessingEngine"

# deploy model to xconnect
Write-Host "Executing Exe to create the model json" -ForegroundColor "Green"
$modelcreator = "source\Sitecore.Demo\XConnectModelSerialization\bin\Debug\XConnectModelSerialization.exe";

& $modelcreator;

$modelJson = "source\Sitecore.Demo\XConnectModelSerialization\bin\Debug\LetsPlay.XConnectModel, 1.0.json";

Write-Host "Coping model json to xconnect" -ForegroundColor "Green"
Copy-Item $modelJson "$xconnectRoot\App_Data\Models" -Force
$modelDll = "source\Sitecore.Demo\Sitecore.Demo.Model\bin\$configuration\netstandard2.0\Sitecore.Demo.Model.dll" 
Copy-Item $modelDll "$xconnectRoot\bin" -Force

# Universal Traker
#Universal Tracker Paths 

Write-Host "Coping model dll to Universal tracker" -ForegroundColor "Green"
Copy-Item $modelDll $utCollection -Force
Copy-Item $modelDll $utProcessing -Force

Write-Host "Coping event mapping to Universal tracker" -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Model\configs\UniversalTracker\config" $utProcessing -Force -Recurse
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Model\configs\UniversalTracker\config" $utCollection -Force -Recurse

If(!(test-path "$utCollection\sitecoreruntime\Production"))
{
      New-Item -ItemType Directory -Force -Path "$utCollection\sitecoreruntime\Production"
}
If(!(test-path "$utProcessing\sitecoreruntime\Production"))
{
      New-Item -ItemType Directory -Force -Path "$utProcessing\sitecoreruntime\Production"
}

Copy-Item $modelDll "$utCollection\sitecoreruntime\Production" -Force
Copy-Item $modelDll "$utProcessing\sitecoreruntime\Production" -Force

#copying model to indexer

$indexerRoot = Join-Path $xconnectRoot "App_Data\jobs\continuous\IndexWorker"

Write-Host "copying model to indexer($indexerRoot) " -ForegroundColor "Green"

Copy-Item $modelJson "$indexerRoot\App_Data\Models" -Force

# copy stuff to CMS 

Write-Host "copying model dll to CMS ($cmsRoot\bin) " -ForegroundColor "Green"
Copy-Item $modelDll "$cmsRoot\bin" -Force

Write-Host "Sitecore.Demo.Personalization.dll CMS ($cmsRoot\bin) " -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Personalization\bin\Debug\Sitecore.Demo.Personalization.dll" "$cmsRoot\bin" -Force

Write-Host "Copy cms configs " -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\App_Config" "$cmsRoot" -Force -Recurse

Write-Host "Copy cms configs " -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\App_Config\ContactInfo.aspx" "$cmsRoot" -Force

Write-Host "Copy items to  " -ForegroundColor "Green"
#Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\serialization" "$cmsRoot\App_Data" -Force -Recurse

Write-Host "Sitecore.Demo.Cms " -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Cms\bin\Debug\net471\Sitecore.Demo.Cms.dll" "$cmsRoot\bin" -Force

# copy stuff to Processing engine


Write-Host "Stopping the Cortex Service this may take a while " -ForegroundColor "Green"


Stop-Service $services
$maxRepeat = 20
$status = "Running" # change to Stopped if you want to wait for services to start

do 
{
    $count = (Get-Service $services | ? {$_.status -eq $status}).count
    $maxRepeat--
    sleep -Milliseconds 5000
} until ($count -eq 0 -or $maxRepeat -eq 0)

Write-Host "copying model dll to processing engine ($processingEngineRoot) " -ForegroundColor "Green"
Copy-Item $modelDll $processingEngineRoot -Force

$processingworkerDll = "source\Sitecore.Demo\Sitecore.Demo.CortexWorkers\bin\Debug\netstandard2.0\Sitecore.Demo.CortexWorkers.dll";

Write-Host "copying model dll to processing engine ($processingEngineRoot) " -ForegroundColor "Green"
Copy-Item $processingworkerDll $processingEngineRoot -Force

Write-Host "copying processing engine configs ($processingEngineRoot) " -ForegroundColor "Green"
Copy-Item "source\Sitecore.Demo\Sitecore.Demo.CortexWorkers\App_Data" $processingEngineRoot -Force -Recurse

Write-Host "copying processing engine dll to cms " -ForegroundColor "Green"
Copy-Item $processingworkerDll "$cmsRoot\bin" -Force

#copy the ProcessingEngine from Sitecore.Demo.Model to the processing engine. 

Copy-Item "source\Sitecore.Demo\Sitecore.Demo.Model\configs\ProcessingEngine\app_data" $processingEngineRoot -Force -Recurse

Write-Host "Starting the Cortex Service" -ForegroundColor "Green"
#Start the processing engine service.
Start-Service $services


& {iisreset /Start}