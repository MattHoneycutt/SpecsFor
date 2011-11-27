properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\SpecsFor.sln"
	$SpecsForOutput = "$BaseDir\SpecsFor.Mvc\bin\Debug"
	$ProjectPath = "$BaseDir\SpecsFor.Mvc\SpecsFor.Mvc.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	#TODO: Once NuGet fully supports semantic versioning, this can go away.
	$Version = Read-Host -Prompt "Please enter the version number"
	
	$NuGetPackageName = "SpecsFor.Mvc"
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
	ri specsfor*.zip -ea SilentlyContinue
}

task Build -depends Init,Clean {
	exec { msbuild $SolutionFile }
}

task Archive -depends Build {
	mkdir $ArchiveDir
	
	cp "$SpecsForOutput\Ionic.Zip.dll" "$ArchiveDir"
	cp "$SpecsForOutput\Microsoft.Web.Mvc.dll" "$ArchiveDir"
	cp "$SpecsForOutput\MvcContrib.TestHelper.dll" "$ArchiveDir"
	cp "$SpecsForOutput\Newtonsoft.Json.dll" "$ArchiveDir"
	cp "$SpecsForOutput\Rhino.Mocks.dll" "$ArchiveDir"
	cp "$SpecsForOutput\SpecsFor.Mvc.dll" "$ArchiveDir"
	cp "$SpecsForOutput\SpecsFor.Mvc.pdb" "$ArchiveDir"
	cp "$SpecsForOutput\WebDriver.dll" "$ArchiveDir"
	
	Write-Zip -Path "$ArchiveDir\*" -OutputPath "SpecsFor.Mvc.zip"
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