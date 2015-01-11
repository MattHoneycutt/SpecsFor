Param(
  [switch]$DoNotPush,
  [switch]$PushToLocalFeed
)

$ProjectName = "SpecsFor.csproj"

$LocalTestFeedDir = "C:\Projects\LocalNuGetFeed\"

#Remove existing packages
Remove-Item *.nupkg
#Create package
nuget pack $ProjectName -Build -Properties Configuration=Debug
$PackageName = gci "*.nupkg"

if ($PushToLocalFeed) {
	Write-Host -ForegroundColor Yellow "Deploying to local NuGet test feed!"
	Copy-Item $PackageName $LocalTestFeedDir
}
elseif ($DoNotPush) {
	Write-Host -ForegroundColor Yellow "Skipping 'nuget push' step!"
}
else {
	#Push
	Write-Host "About to push to NuGet.org!  Press CTRL+C to abort."
	pause
	nuget push $PackageName
	Remove-Item *.nupkg
}
