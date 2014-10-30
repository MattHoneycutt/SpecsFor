param($installPath, $toolsPath, $package, $project)


$MSBuildToolsPathx64 = "${env:ProgramFiles(x86)}\MSBuild\12.0\bin\msbuild.exe"
$MSBuildToolsPathx86 = "${env:ProgramFiles}\MSBuild\12.0\bin\msbuild.exe"

$MissingDependencies = (-not ((Test-Path $MSBuildToolsPathx64) -or (Test-Path $MSBuildToolsPathx86)))
$Url = "http://specsfor.com/SpecsForMvc/installed.cshtml?missingBuildTools=" 

if ($MissingDependencies) {
	$Url = $Url + "true";
} 
$DTE.ItemOperations.Navigate($Url)
#Write-Host "Target URL: $Url"