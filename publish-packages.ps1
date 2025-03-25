if (Test-path .\SpecsFor.Core\bin\Release\SpecsFor.Core.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Core\bin\Release\SpecsFor.Core.*.nupkg)
}
if (Test-path .\SpecsFor.StructureMap\bin\Release\SpecsFor.StructureMap.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.StructureMap\bin\Release\SpecsFor.StructureMap.*.nupkg)
}
if (Test-path .\SpecsFor.Autofac\bin\Release\SpecsFor.Autofac.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Autofac\bin\Release\SpecsFor.Autofac.*.nupkg)
}
if (Test-path .\SpecsFor.Lamar\bin\Release\SpecsFor.Lamar.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Lamar\bin\Release\SpecsFor.Lamar.*.nupkg)
}
if (Test-path .\SpecsFor.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.*.nupkg)
}
dotnet pack .\SpecsFor.Core\SpecsFor.Core.csproj
dotnet pack .\SpecsFor.StructureMap\SpecsFor.StructureMap.csproj
dotnet pack .\SpecsFor.Autofac\SpecsFor.Autofac.csproj
dotnet pack .\SpecsFor.Lamar\SpecsFor.Lamar.csproj
nuget pack .\metapackage\specsfor-metapackage.nuspec

Write-Host "Packages built, publish?"
pause

nuget push (Resolve-Path .\SpecsFor.Core\bin\Release\SpecsFor.Core.*.nupkg) -Source https://api.nuget.org/v3/index.json
nuget push (Resolve-Path .\SpecsFor.StructureMap\bin\Release\SpecsFor.StructureMap.*.nupkg) -Source https://api.nuget.org/v3/index.json
nuget push (Resolve-Path .\SpecsFor.Autofac\bin\Release\SpecsFor.Autofac.*.nupkg) -Source https://api.nuget.org/v3/index.json
nuget push (Resolve-Path .\SpecsFor.Lamar\bin\Release\SpecsFor.Lamar.*.nupkg) -Source https://api.nuget.org/v3/index.json
nuget push (Resolve-Path .\SpecsFor.*.nupkg) -Source https://api.nuget.org/v3/index.json

