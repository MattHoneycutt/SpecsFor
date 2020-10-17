if (Test-path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg)
}
if (Test-path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg)
}
if (Test-path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg)
}
if (Test-path .\SpecsFor.Should\bin\Debug\SpecsFor.Should.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Should\bin\Debug\SpecsFor.Should.*.nupkg)
}
if (Test-path .\SpecsFor.Shouldly\bin\Debug\SpecsFor.Shouldly.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Shouldly\bin\Debug\SpecsFor.Shouldly.*.nupkg)
}
if (Test-path .\SpecsFor.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.*.nupkg)
}
dotnet pack .\SpecsFor.Core\SpecsFor.Core.csproj
dotnet pack .\SpecsFor.StructureMap\SpecsFor.StructureMap.csproj
dotnet pack .\SpecsFor.Autofac\SpecsFor.Autofac.csproj
dotnet pack .\SpecsFor.Should\SpecsFor.Should.csproj
dotnet pack .\SpecsFor.Shouldly\SpecsFor.Shouldly.csproj
nuget pack .\metapackage\specsfor-metapackage.nuspec

Write-Host "Packages built, publish?"
pause

dotnet nuget push  (Resolve-Path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.Should\bin\Debug\SpecsFor.Should.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.Shouldly\bin\Debug\SpecsFor.Shouldly.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.*.nupkg)
