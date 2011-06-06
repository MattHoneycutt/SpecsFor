Set-StrictMode -Version 2.0

Start-TabExpansion

function TabExpansion($line, $lastWord) 
{
	Get-TabExpansion $line $lastWord
}
