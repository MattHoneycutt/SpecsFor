$loadedPsake = $false;
if ((Get-Module psake) -eq $null) {
	import-module .\extensions\psake.psm1
	$loadedPsake = $true;
}

$loadedPscx = $false;
if ((Get-Module pscx) -eq $null) {
	import-module .\extensions\pscx	
	$loadedPscx = $true;
}

invoke-psake SpecsFor.ps1 -taskList Publish,Archive
invoke-psake SpecsFor.Mvc.ps1 -taskList Publish,Archive

if ($loadedPscx) { 
	remove-module pscx
}
	
if ($loadedPsake) {
	remove-module psake
}