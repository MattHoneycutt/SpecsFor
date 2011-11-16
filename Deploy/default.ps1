properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\SpecsFor.sln"
	$OutputDir = "$BaseDir\Deploy\Package\"
	$SpecsForOutput = "$BaseDir\Deploy\Package\_PublishedApplications\SpecsFor"
	#Gets the number of commits since the last tag. 
	$Version = "1.1." + (git describe --tags --long).split('-')[1]
	$Debug="false"
	
	$NuGetPackageName = "SpecsFor"
	$NuGetPackDir = "$OutputDir" + "Pack"
	$NuSpecFileName = "NuGet\SpecsFor.nuspec"
	
	$ArchiveDir = "$OutputDir" + "Archive"
}

$framework = '4.0'

task default -depends Pack,Archive

task Init {
	cls
}

task Clean -depends Init {
	if (Test-Path $OutputDir) {
		ri $OutputDir -Recurse
	}
	
	ri SpecsFor.*.nupkg
	ri specsfor.zip -ea SilentlyContinue
}

task Build -depends Init,Clean {
	exec { msbuild $SolutionFile "/p:OutDir=$OutputDir" }
}

task Archive -depends Build {
	mkdir $ArchiveDir
	
	cp "$SpecsForOutput\Moq.dll" "$ArchiveDir"
	cp "$SpecsForOutput\nunit.framework.dll" "$ArchiveDir"
	cp "$SpecsForOutput\SpecsFor.dll" "$ArchiveDir"
	cp "$SpecsForOutput\StructureMap.AutoMocking.dll" "$ArchiveDir"
	cp "$SpecsForOutput\StructureMap.dll" "$ArchiveDir"
	
	cp "$BaseDir\Templates" "$ArchiveDir" -Recurse
	Remove-Item -Force "$ArchiveDir\Templates\.gitignore"

	Write-Zip -Path "$ArchiveDir\*" -OutputPath specsfor.zip
}

task Pack -depends Build {

	mkdir $NuGetPackDir
	cp "$NuSpecFileName" "$NuGetPackDir"

	mkdir "$NuGetPackDir\lib"
	cp "$SpecsForOutput\SpecsFor.dll" "$NuGetPackDir\lib"

	cp "$BaseDir\Templates" "$NuGetPackDir" -Recurse
	Remove-Item -Force "$NuGetPackDir\Templates\.gitignore"
	
	$Spec = [xml](get-content "$NuGetPackDir\$NuSpecFileName")
	$Spec.package.metadata.version = ([string]$Spec.package.metadata.version).Replace("{Version}",$Version)
	$Spec.Save("$NuGetPackDir\$NuSpecFileName")

	exec { nuget pack "$NuGetPackDir\$NuSpecFileName" }
}

task Publish -depends Pack {
	$PackageName = gci *.nupkg
	#We don't care if deleting fails..
	nuget delete $NuGetPackageName $Version -NoPrompt
	exec { nuget push $PackageName }
}