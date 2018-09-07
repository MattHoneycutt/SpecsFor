if (Test-path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg)
}
if (Test-path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg)
}
if (Test-path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg) {
	rm (Resolve-Path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg)
}

dotnet pack .\SpecsFor.Core\SpecsFor.Core.csproj
dotnet pack .\SpecsFor.StructureMap\SpecsFor.StructureMap.csproj
dotnet pack .\SpecsFor.Autofac\SpecsFor.Autofac.csproj

Write-Host "Packages built, publish?"
pause

dotnet nuget push  (Resolve-Path .\SpecsFor.Core\bin\Debug\SpecsFor.Core.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.StructureMap\bin\Debug\SpecsFor.StructureMap.*.nupkg)
dotnet nuget push  (Resolve-Path .\SpecsFor.Autofac\bin\Debug\SpecsFor.Autofac.*.nupkg)

