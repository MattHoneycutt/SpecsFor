properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\SpecsFor.sln"
	$SpecsForOutput = "$BaseDir\SpecsFor\bin\Debug"
	$ProjectPath = "$BaseDir\SpecsFor\SpecsFor.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "SpecsFor"

	$ZipFiles =  @("$SpecsForOutput\Moq.dll",
		"$SpecsForOutput\nunit.framework.dll",
		"$SpecsForOutput\SpecsFor.dll",
		"$SpecsForOutput\StructureMap.AutoMocking.dll",
		"$SpecsForOutput\StructureMap.dll",
		"$SpecsForOutput\Should.dll",
		"$SpecsForOutput\ExpectedObjects.dll")
	$ZipName = "SpecsFor.zip"
}

. .\common.ps1

function OnArchiving {
	cp "$BaseDir\SpecsFor\Templates" "$ArchiveDir" -Recurse
	Remove-Item -Force "$ArchiveDir\Templates\.gitignore"
}
