@echo off
powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\publish.ps1 -PackageName 'SpecsFor'; exit $error.Count}"
pause