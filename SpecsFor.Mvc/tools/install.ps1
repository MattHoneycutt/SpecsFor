param($installPath, $toolsPath, $package, $project)

. (Join-Path $toolsPath "common.ps1")

if (!$project.Properties.Item("PostBuildEvent").Value.Contains($PostBuildEvent)) {
	$project.Properties.Item("PostBuildEvent").Value += $PostBuildEvent
}
