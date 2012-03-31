@echo off
powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\publish.ps1 -PackageName 'SpecsFor.Mvc'; exit $error.Count}"
pause