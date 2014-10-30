#Remove existing packages
Remove-Item *.nupkg
#Create package
msbuild SpecsFor.Mvc.csproj "/p:Configuration=Release"
nuget pack SpecsFor.Mvc.csproj -Properties Configuration=Release
#Push
$PackageName = gci "*.nupkg"
nuget push $PackageName
Remove-Item *.nupkg