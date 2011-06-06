# takes one parameter and somehow notifies user; It could be email, growl message, output to file... 
[scriptblock]$script:Notifier = {}
# todo: read output from xml

function Set-SvnNotifier {
	param(	
		[Parameter(Mandatory=$true)][scriptblock]$notifier
	)
	$script:Notifier = $notifier
}

function Get-SvnInfo
{
	param(
		[Parameter(Mandatory=$true)][string]$dir
	)
	$info = svn info $dir
	$ret = new-object PSObject
	$info | % { 
		if ($_ -match '^Revision')            { $ret | Add-Member NoteProperty Revision ($_ -replace 'Revision[\s:]*','') }
		if ($_ -match '^Last Changed Author') { $ret | Add-Member NoteProperty Author ($_ -replace 'Last Changed Author[\s:]*','') }
		if ($_ -match '^Last Changed Date')   { $ret | Add-Member NoteProperty Date ($_ -replace 'Last Changed Date[\s:]*','') }
	}
	$ret
}

function Update-Svn
{
	param(
		[Parameter(Mandatory=$true)][string]$dir,
		[switch]$gui, 
		[switch]$Wait
	)
	$info = Get-SvnInfo $dir
	if ($gui) {
		Start-Process TortoiseProc.exe -Argument "/command:update", "/path:`"$dir`"" -wait:$wait
	} else {
		svn update $dir | 
		% { 
			$m = $_
			switch -regex ($m) {
				'^(Updated to|At revision)' { write-host $m }
				'^\s*U\s'        { write-host $m -fore Yellow }
				'^\s*A\s'        { write-host $m -fore Green }
				'^\s*D\s'        { write-host $m -fore Red }
				'^\s*G\s'        { write-host $m -fore DarkCyan }
				default       { write-host $m -fore Magenta }
			}
		}
		& $Notifier 'updated from SVN'
	}
	$info2 = Get-SvnInfo $dir
	if ($info.Revision -ne $info2.Revision) {
		#Write-Host ("State before: {0} - {1} - {2}" -f $info.Revision, $info.Author, $info.Date) -fore Green
		#Write-Host ("State after:  {0} - {1} - {2}" -f $info2.Revision, $info2.Author, $info2.Date) -fore Green
		Get-SvnLogInfos -dir $dir -maxCount ($info2.Revision - $info.Revision) | %  {
			write-host $_.Header-fore Green
			$_.Info | % { write-host "    $_" }
		}
	}
	# more colors: 
	#"Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray, DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White" -split ', '|% { write-host $_ -fore $_ }
}

function Commit-Svn
{
	param(
		[Parameter(Mandatory=$true)][string]$dir,
		[switch]$Wait
	)
	Start-Process TortoiseProc.exe -Argument "/command:commit", "/path:`"$dir`"" -wait:$wait
}

function Get-SvnLogInfos
{
	param(
		[Parameter(Mandatory=$true)][string]$dir,
		[Parameter(Mandatory=$false)][int]$maxCount=1
	)
	$ret = @()
	svn log $dir -l $maxCount | % {
		$line = $_
		switch -regex ($_) {
			'^(-*|\s*)$'               { return }
			'^r\d+\s*\|\s*[\w\.]+\s\|' { 
				if ($header -ne $null) { $ret += $header }
				$header = new-object PSObject -property @{Header = $line; Info = @() } 
			}
			default                    { 
				$header.Info += $line 
			}
		}
	}
	if ($header -ne $null) { $ret += $header }
	$ret
}