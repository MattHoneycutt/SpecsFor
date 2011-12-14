properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\SpecsFor.sln"
	$SpecsForOutput = "$BaseDir\SpecsFor.Mvc\bin\Debug"
	$ProjectPath = "$BaseDir\SpecsFor.Mvc\SpecsFor.Mvc.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "SpecsFor.Mvc"
	$ZipFiles =  @("$SpecsForOutput\Ionic.Zip.dll", 
		"$SpecsForOutput\Microsoft.Web.Mvc.dll",
		"$SpecsForOutput\MvcContrib.TestHelper.dll",
		"$SpecsForOutput\Newtonsoft.Json.dll",
		"$SpecsForOutput\SpecsFor.Mvc.dll",
		"$SpecsForOutput\SpecsFor.Mvc.pdb",
		"$SpecsForOutput\WebDriver.dll")
	$ZipName = "SpecsFor.Mvc.zip"
}

. .\common.ps1