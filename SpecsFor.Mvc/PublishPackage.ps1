#Remove existing packages
Remove-Item *.nupkg
#Create package
nuget pack SpecsFor.Mvc.csproj -Build -Properties Configuration=Debug
#Push
$PackageName = gci "*.nupkg"
nuget push $PackageName
Remove-Item *.nupkg