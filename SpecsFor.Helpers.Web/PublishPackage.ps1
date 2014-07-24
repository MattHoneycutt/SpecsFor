#Remove existing packages
Remove-Item *.nupkg
#Build release
#Create package
nuget pack SpecsFor.Helpers.Web.csproj -Build
#Push
$PackageName = gci "*.nupkg"
nuget push $PackageName
Remove-Item *.nupkg