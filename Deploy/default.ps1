properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\SpecsFor.sln"
	$SpecsForOutput = "$BaseDir\SpecsFor\bin\Debug"
	$ProjectPath = "$BaseDir\SpecsFor\SpecsFor.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	#TODO: Once NuGet fully supports semantic versioning, this can go away.
	$Version = Read-Host -Prompt "Please enter the version number"
	
	$NuGetPackageName = "SpecsFor"
}

$framework = '4.0'

task default -depends Pack,Archive

task Init {
	cls
}

task Clean -depends Init {
	
	if (Test-Path $ArchiveDir) {
		ri $ArchiveDir -Recurse
	}
	
	ri SpecsFor.*.nupkg
	ri specsfor.zip -ea SilentlyContinue
}

task Build -depends Init,Clean {
	exec { msbuild $SolutionFile }
}

task Archive -depends Build {
	mkdir $ArchiveDir
	
	cp "$SpecsForOutput\Moq.dll" "$ArchiveDir"
	cp "$SpecsForOutput\nunit.framework.dll" "$ArchiveDir"
	cp "$SpecsForOutput\SpecsFor.dll" "$ArchiveDir"
	cp "$SpecsForOutput\StructureMap.AutoMocking.dll" "$ArchiveDir"
	cp "$SpecsForOutput\StructureMap.dll" "$ArchiveDir"
	cp "$SpecsForOutput\Should.dll" "$ArchiveDir"
	cp "$SpecsForOutput\ExpectedObjects.dll" "$ArchiveDir"
	
	cp "$BaseDir\Templates" "$ArchiveDir" -Recurse
	Remove-Item -Force "$ArchiveDir\Templates\.gitignore"

	Write-Zip -Path "$ArchiveDir\*" -OutputPath specsfor.zip
}

task Pack -depends Build {

	exec { nuget pack "$ProjectPath" -Version "$Version" }
}

task Publish -depends Pack {
	$PackageName = gci *.nupkg
	#We don't care if deleting fails..
	nuget delete $NuGetPackageName $Version -NoPrompt
	exec { nuget push $PackageName }
}