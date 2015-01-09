Param(
  [switch]$DoNotPush
)

$ProjectName = "SpecsFor.csproj"

#Remove existing packages
Remove-Item *.nupkg
#Create package
nuget pack $ProjectName -Build -Properties Configuration=Debug

if ($DoNotPush) {
	Write-Host -ForegroundColor Yellow "Skipping 'nuget push' step!"
}
else {
	#Push
	$PackageName = gci "*.nupkg"
	nuget push $PackageName
	Remove-Item *.nupkg
}