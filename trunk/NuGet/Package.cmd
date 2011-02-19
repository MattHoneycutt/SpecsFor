@ECHO OFF
del *.nupkg
xcopy ..\SpecsFor\bin\debug\SpecsFor.dll .\lib /Y
nuget pack
pause