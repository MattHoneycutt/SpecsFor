Param(
  [switch]$DoNotPush
)
#Remove existing packages
Remove-Item *.nupkg
#Create package
msbuild SpecsFor.Mvc.csproj "/p:Configuration=Release"
nuget pack SpecsFor.Mvc.csproj -Properties Configuration=Release
if ($DoNotPush) {
	Write-Host -ForegroundColor Yellow "Skipping 'nuget push' step!"
}
else {
	#Push
	$PackageName = gci "*.nupkg"
	nuget push $PackageName
	Remove-Item *.nupkg
}
