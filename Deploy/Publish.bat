@echo off
powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\publish.ps1; exit $error.Count}"
pause