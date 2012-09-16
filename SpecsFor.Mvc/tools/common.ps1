
$solutionDir = [System.IO.Path]::GetDirectoryName($dte.Solution.FullName) + "\"
$path = $toolsPath.Replace($solutionDir, "`$(SolutionDir)")

$PostBuildEvent = "
xcopy /D /Y `"$path\IEDriverServer.exe`" `"`$(TargetDir)`"
xcopy /D /Y `"$path\ChromeDriver.exe`" `"`$(TargetDir)`"
"
