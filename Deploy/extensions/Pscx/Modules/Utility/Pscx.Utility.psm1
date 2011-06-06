Set-StrictMode -Version 2.0

Set-Alias e     Edit-File              -Description "PSCX alias"
Set-Alias ehp   Edit-HostProfile       -Description "PSCX alias"
Set-Alias ep    Edit-Profile           -Description "PSCX alias"
Set-Alias gpv   Get-PropertyValue      -Description "PSCX alias"
Set-Alias su    Invoke-Elevated        -Description "PSCX alias"
Set-Alias igc   Invoke-GC              -Description "PSCX alias"
Set-Alias ??    Invoke-NullCoalescing  -Description "PSCX alias"
Set-Alias call  Invoke-Method          -Description "PSCX alias"
Set-Alias ?:    Invoke-Ternary         -Description "PSCX alias"
Set-Alias nho   New-HashObject         -Description "PSCX alias"
Set-Alias ql    QuoteList              -Description "PSCX alias"
Set-Alias qs    QuoteString            -Description "PSCX alias"
Set-Alias rver  Resolve-ErrorRecord    -Description "PSCX alias"
Set-Alias rvhr  Resolve-HResult        -Description "PSCX alias"
Set-Alias rvwer Resolve-WindowsError   -Description "PSCX alias"
Set-Alias rf    Invoke-Reflector       -Description "PSCX alias"

# Alias that should be set by PowerShell
Set-Alias sls   Select-String -Description "PSCX alias"

# Initialize the PSCX RegexLib object.
& {
	$RegexLib = new-object psobject
	
	function AddRegex($name, $regex) {
	  Add-Member -Input $RegexLib NoteProperty $name $regex
	}

	AddRegex CDQString           '(?<CDQString>"\\.|[^\\"]*")'
	AddRegex CSQString           "(?<CSQString>'\\.|[^'\\]*')"
	AddRegex CMultilineComment   '(?<CMultilineComment>/\*[^*]*\*+(?:[^/*][^*]*\*+)*/)'
	AddRegex CppEndOfLineComment '(?<CppEndOfLineComment>//[^\n]*)'
	AddRegex CComment            "(?:$($RegexLib.CDQString)|$($RegexLib.CSQString))|(?<CComment>$($RegexLib.CMultilineComment)|$($RegexLib.CppEndOfLineComment))"

	AddRegex PSComment          '(?<PSComment>#[^\n]*)'
	AddRegex PSNonCommentedLine '(?<PSNonCommentedLine>^(?>\s*)(?!#|$))'

	AddRegex EmailAddress       '(?<EmailAddress>[A-Z0-9._%-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4})'
	AddRegex IPv4               '(?<IPv4>)(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))'
	AddRegex RepeatedWord       '\b(?<RepeatedWord>(\w+)\s+\1)\b'
	AddRegex HexDigit           '[0-9a-fA-F]'
	AddRegex HexNumber          '(?<HexNumber>(0[xX])?[0-9a-fA-F]+)'
	AddRegex DecimalNumber      '(?<DecimalNumber>[+-]?(?:\d+\.?\d*|\d*\.?\d+))'
	AddRegex ScientificNotation '(?<ScientificNotation>[+-]?(?<Significand>\d+\.?\d*|\d*\.?\d+)[\x20]?(?<Exponent>[eE][+\-]?\d+)?)'

	$Pscx:RegexLib = $RegexLib
}

<#
.SYNOPSIS
    Creates the registry entries required to create Windows Explorer context 
    menu "Open PowerShell Here" for both Directories and Drives
.NOTES
    Author: Keith Hill
#>
function Enable-OpenPowerShellHere
{
    function New-OpenPowerShellContextMenuEntry
    {
        param($Path)

        New-Item $Path -ItemType RegistryKey -Force
        New-ItemProperty $Path -Name '(Default)' -Value 'Open PowerShell Here'
        New-Item $Path\Command -ItemType RegistryKey
        New-ItemProperty $Path\Command -Name '(Default)' `
            -Value "`"$pshome\powershell.exe`" -NoExit -Command [Environment]::CurrentDirectory=(Set-Location -LiteralPath:'%L' -PassThru).ProviderPath"
    }

    New-OpenPowerShellContextMenuEntry 'HKCU:\Software\Classes\Directory\shell\PowerShell'
    New-OpenPowerShellContextMenuEntry 'HKCU:\Software\Classes\Drive\shell\PowerShell'
}

<#
.SYNOPSIS
    Gets the specified property's value from each input object.
.DESCRIPTION
    Gets the specified property's value from each input object.
	This filter is different from the Select-Object cmdlet in that it
	doesn't create a wrapper object (PSCustomObject) around the property.
	If you just want to get the property's value to assign it to another
	variable this filter will come in handy.  If you assigned the result
	of the Select-Object operation you wouldn't get the property's value.
	You would get an object that wraps that property and its value.
.PARAMETER InputObject
    Any object from which to get the specified property
.EXAMPLE
    C:\PS> $start = Get-History -Id 143 | Get-PropertyValue StartExecutionTime
    Gets the value of the StartExecutionTime property off of each HistoryInfo object.
.NOTES
    Aliases:  gpv
    Author:   Keith Hill  
#>
filter Get-PropertyValue([string] $propertyName) {
    $_.$propertyName
}

<#
.SYNOPSIS
    Create a PSObject from a dictionary such as a hashtable.
.DESCRIPTION
    Create a PSObject from a dictionary such as a hashtable.  The keys-value 
    pairs are turned into NoteProperties.
.PARAMETER InputObject
    Any object from which to get the specified property
.EXAMPLE
    C:\PS> $ht = @{fname='John';lname='Doe';age=42}; $ht | New-HashObject
    Creates a hashtable in $ht and then converts that into a PSObject.
.NOTES
    Aliases:  nho
#>
filter New-HashObject {
    if ($_ -isnot [Collections.IDictionary]) { 
		return $_ 
	}

    $result = new-object PSObject
    $hash = $_

    $hash.Keys | %{ $result | add-member NoteProperty "$_" $hash[$_] -force }

    $result
}

<#
.SYNOPSIS
    Similar to the C# ? : operator e.g. name = (value != null) ? String.Empty : value
.DESCRIPTION
    Similar to the C# ? : operator e.g. name = (value != null) ? String.Empty : value.
    The first script block is tested. If it evaluates to $true then the second scripblock
    is evaluated and its results are returned otherwise the third scriptblock is evaluated
    and its results are returned.
.PARAMETER Condition
    The condition that determines whether the TrueBlock scriptblock is used or the FalseBlock
    is used.
.PARAMETER TrueBlock
	This block gets evaluated and its contents are returned from the function if the Conditon
	scriptblock evaluates to $true.
.PARAMETER FalseBlock
	This block gets evaluated and its contents are returned from the function if the Conditon
	scriptblock evaluates to $false.
.EXAMPLE
    C:\PS> 1..10 | ?: {$_ -gt 5} {"Greater than 5";$_} {"Less than or equal to 5";$_}
    Each input number is evaluated to see if it is > 5.  If it is then "Greater than 5" is
    displayed otherwise "Less than or equal to 5" is displayed.
.NOTES
    Aliases:  ?:
    Author:   Karl Prosser
#>
filter Invoke-Ternary {
	param([scriptblock]$Condition  = $(throw "Parameter '-condition' (position 1) is required"), 
	      [scriptblock]$TrueBlock  = $(throw "Parameter '-trueBlock' (position 2) is required"), 
	      [scriptblock]$FalseBlock = $(throw "Parameter '-falseBlock' (position 3) is required"))
	
	if (&$Condition) { 
		&$TrueBlock
	} 
	else { 
		&$FalseBlock 
	}
}

<#
.SYNOPSIS
    Similar to the C# ?? operator e.g. name = value ?? String.Empty
.DESCRIPTION
	Similar to the C# ?? operator e.g. name = value ?? String.Empty;
	where value would be a Nullable&lt;T&gt; in C#.  Even though PowerShell
	doesn't support nullables yet we can approximate this behavior.
	In the example below, $LogDir will be assigned the value of $env:LogDir
	if it exists and it's not null, otherwise it get's assigned the
	result of the second script block (C:\Windows\System32\LogFiles).
	This behavior is also analogous to Korn shell assignments of this form:
	LogDir = ${$LogDir:-$WinDir/System32/LogFiles}
.PARAMETER PrimaryExpr
    The condition that determines whether the TrueBlock scriptblock is used or the FalseBlock
    is used.
.PARAMETER AlternateExpr
	This block gets evaluated and its contents are returned from the function if the Conditon
	scriptblock evaluates to $true.
.EXAMPLE
    C:\PS> $LogDir = ?? {$env:LogDir} {"$env:windir\System32\LogFiles"}
	$LogDir is set to the value of $env:LogDir unless it doesn't exist, in which case it 
	will then default to "$env:windir\System32\LogFiles".
.NOTES
    Aliases:  ??
    Author:   Keith Hill
#>
filter Invoke-NullCoalescing {
	param([scriptblock]$PrimaryExpr   = $(throw "Parameter '-primaryExpr' (position 1) required"), 
	      [scriptblock]$AlternateExpr = $(throw "Parameter '-alternateExpr' (position 2) required"))
	      
	if ($primaryExpr -ne $null) {
		$result = &$primaryExpr
		if ($result -ne $null -and "$result" -ne '') {
			$result
		}
		else {
			&$alternateExpr
		}
	}
	else {
		&$alternateExpr
	}
}

<#
.FORWARDHELPTARGETNAME Get-Help
.FORWARDHELPCATEGORY Cmdlet
#>
function help
{
    [CmdletBinding(DefaultParameterSetName='AllUsersView')]
    param(
        [Parameter(Position=0, ValueFromPipelineByPropertyName=$true)]
        [System.String]
        ${Name},

        [System.String]
        ${Path},

        [System.String[]]
        ${Category},

        [System.String[]]
        ${Component},

        [System.String[]]
        ${Functionality},

        [System.String[]]
        ${Role},

        [Parameter(ParameterSetName='DetailedView')]
        [Switch]
        ${Detailed},

        [Parameter(ParameterSetName='AllUsersView')]
        [Switch]
        ${Full},

        [Parameter(ParameterSetName='Examples')]
        [Switch]
        ${Examples},

        [Parameter(ParameterSetName='Parameters')]
        [System.String]
        ${Parameter},

        [Switch]
        ${Online}
    )

    $outputEncoding=[System.Console]::OutputEncoding

	if ($Pscx:Preferences["PageHelpUsingLess"]) 
	{
        Get-Help @PSBoundParameters | less
    }
    else
    {
        Get-Help @PSBoundParameters | more
    }
}

<#
.SYNOPSIS
    Less provides better paging of output from cmdlets.
.DESCRIPTION
    Less provides better paging of output from cmdlets.
    By default PowerShell uses more.com for paging which is a pretty minimal paging app that doesn't support advanced
    navigation features.  This function uses Less.exe ie Less394 as the replacement for more.com.  Less can navigate
    down as well as up and can be scrolled by page or by line and responds to the Home and End keys. Less also 
    supports searching the text by pressing the "/" key followed by the term to search for then the "Enter" key.  
    One of the primary keyboard shortcuts to know with less.exe is the key to exit. Pressing the "q" key will exit 
    less.exe.  For more help on less.exe press the "h" key.  If you prefer to use more.com set the PSCX preference 
    variable PageHelpUsingLess to $false e.g. $Pscx:Preferences['PageHelpUsingLess'] = $false
.PARAMETER LiteralPath
    Specifies the path to a file to view. Unlike Path, the value of LiteralPath is used exactly as it is typed. 
    No characters are interpreted as wildcards. If the path includes escape characters, enclose it in 
    single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters 
    as escape sequences.
.PARAMETER Path
    The path to the file to view.  Wildcards are accepted.
.EXAMPLE
    C:\PS> man about_profiles -full
    This sends the help output of the about_profiles topic to the help function which pages the output.
    Man is an alias for the "help" function. PSCX overrides the help function to page help using either 
    the built-in PowerShell "more" function or the PSCX "less" function depending on the value of the 
    PageHelpUsingLess preference variable.
.EXAMPLE
    C:\PS> less *.txt
    Opens each text file in less.exe in succession.  Pressing ':n' moves to the next file.
.NOTES
    This function is just a passthru in all other hosts except for the PowerShell.exe console host.
.LINK
	http://en.wikipedia.org/wiki/Less_(Unix)
#>
function less
{
	param([string[]]$Path, [string[]]$LiteralPath)
	
	if ($host.Name -ne 'ConsoleHost')
	{
	    # The rest of this function only works well in PowerShell.exe
	    $input
	    return
	}
	
	$OutputEncoding = [System.Console]::OutputEncoding
		
	$resolvedPaths = $null
	if ($LiteralPath)
	{
		$resolvedPaths = $LiteralPath
	}
	elseif ($Path)
	{
		$resolvedPaths = @()
		# In the non-literal case we may need to resolve a wildcarded path
		foreach ($apath in $Path) 
		{
			if (Test-Path $apath) 
			{
				$resolvedPaths += @(Resolve-Path $apath | Foreach { $_.Path })
			}
			else
			{
				$resolvedPaths += $apath
			}
		}
	}
		
	# Tricky to get this just right.  
	# Here are three test cases to verify all works as it should:
	# less *.txt      : Should bring up named txt file in less in succession, press q to go to next file
	# man gcm -full   : Should open help topic in less, press q to quit
	# man gcm -online : Should open help topic in web browser but not open less.exe		
	if ($resolvedPaths)
	{
		& "$Pscx:Home\Apps\less.exe" $resolvedPaths
	}
	elseif ($input.MoveNext())
	{
		$input.Reset()
		$input | & "$Pscx:Home\Apps\less.exe"
	}
}

<#
.SYNOPSIS
    Opens up the specified text file in a text editor.
.DESCRIPTION
	Opens up the specified text file in the text editor specified by 
	$Pscx:Preferences['TextEditor'] variable.  If not specified or the 
	specified editor isn't found then notepad is used.
.PARAMETER LiteralPath
    Specifies the path to a file to edit. Unlike Path, the value of LiteralPath is used exactly as it is typed. 
    No characters are interpreted as wildcards. If the path includes escape characters, enclose it in 
    single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters 
    as escape sequences.
.PARAMETER Path
    The path to the file to edit.  Wildcards are accepted.
.EXAMPLE
    C:\PS> Edit-File foo.txt
    Opens foo.txt in a text editor.
.EXAMPLE
    C:\PS> Edit-File *.txt
    Opens all .txt files in a text editor.
.EXAMPLE
    C:\PS> Get-ChildItem . -r *.cs | Edit-File
    Opens all .cs files in the current dir and all subdirs in a text editor.
.NOTES
    Aliases:  e
    Author:   Keith Hill
#>
function Edit-File 
{
	[CmdletBinding(DefaultParameterSetName="Path", SupportsShouldProcess=$true)]
	param(
		[Parameter(Position=0, 
				   ParameterSetName="Path", 
				   ValueFromPipeline=$true, 
				   ValueFromPipelineByPropertyName=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$Path,
	    
		[Alias("PSPath")]
		[Parameter(Position=0, 
				   ParameterSetName="LiteralPath", 
				   ValueFromPipelineByPropertyName=$true)]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$LiteralPath
	)
	
	Begin {
		$editor = 'Notepad.exe'
		$preferredEditor = $Pscx:Preferences['TextEditor']
		if ($preferredEditor) {
			Get-Command $preferredEditor 2>&1 | out-null
			if ($?) {
				$editor = $Pscx:Preferences['TextEditor']
			}
			else {
				$pscmdlet.WriteDebug("Edit-File editor preference '$preferredEditor' not found, defaulting to $editor")	
			}
		}
		
		$pscmdlet.WriteDebug("Edit-File editor is $editor")
		function EditFileImpl($path) {
			& $editor $path
		}
	}
	
	Process 
	{
		if ($psCmdlet.ParameterSetName -eq "Path")
		{
			$resolvedPaths = $null
			if ($Path)
			{
				$resolvedPaths = @()
				# In the non-literal case we may need to resolve a wildcarded path
				foreach ($apath in $Path) 
				{
					if (Test-Path $apath) 
					{
						$resolvedPaths += @(Resolve-Path $apath | Foreach { $_.Path })
					}
					else
					{
						$resolvedPaths += $apath
					}
				}
			}
		}
		else 
		{
			$resolvedPaths = $LiteralPath
		}
	    	            
		foreach ($rpath in $resolvedPaths) 
		{
			$PathIntrinsics = $ExecutionContext.SessionState.Path
	        
			if ($PathIntrinsics.IsProviderQualified($rpath))
			{
				$rpath = $PathIntrinsics.GetUnresolvedProviderPathFromPSPath($rpath)
			}
	        
			$pscmdlet.WriteVerbose("Edit-File opening $rpath")
		    if ($pscmdlet.ShouldProcess("$rpath"))
			{
				EditFileImpl $rpath
			}
		}
	}
}

<#
.SYNOPSIS
    Opens the current user's "all hosts" profile in a text editor.
.DESCRIPTION
    Opens the current user's "all hosts" profile ($Profile.CurrentUserAllHosts) in a text editor.
.EXAMPLE
    C:\PS> Edit-Profile
    Opens the current user's "all hosts" profile in a text editor.
.NOTES
    Aliases:  ep
    Author:   Keith Hill
#>
function Edit-Profile {
	Edit-File $Profile.CurrentUserAllHosts
}

<#
.SYNOPSIS
    Opens the current user's profile for the current host in a text editor.
.DESCRIPTION
    Opens the current user's profile for the current host ($Profile.CurrentUserCurrentHost) in a text editor.
.EXAMPLE
    C:\PS> Edit-HostProfile
    Opens the current user's profile for the current host in a text editor.
.NOTES
    Aliases:  ehp
    Author:   Keith Hill    
#>
function Edit-HostProfile {
	Edit-File $Profile.CurrentUserCurrentHost
}

<#
.SYNOPSIS
    Runs the specified command in an elevated context.
.DESCRIPTION
    Runs the specified command in an elevated context.  This is useful on Windows Vista
    and Windows 7 when you run with a standard user token but can elevate to Admin when needed.
.EXAMPLE
    C:\PS> Invoke-Elevated
    Opens a new PowerShell instance as admin.
.EXAMPLE
    C:\PS> Invoke-Elevated Notepad C:\windows\system32\drivers\etc\hosts
    Opens notepad elevated with the hosts file so that you can save changes to the file.
.EXAMPLE
    C:\PS> Invoke-Elevated {gci c:\windows\temp | export-clixml tempdir.xml; exit}
    Executes the scriptblock in an elevated PowerShell instance.
.EXAMPLE
    C:\PS> Invoke-Elevated {gci c:\windows\temp | export-clixml tempdir.xml; exit} | %{$_.WaitForExit(5000)} | %{Import-Clixml tempdir.xml}
    Executes the scriptblock in an elevated PowerShell instance, waits for that elevated process to execute, then
    retrieves the results.
.NOTES
    Aliases:  su
    Author:   Keith Hill
#>
function Invoke-Elevated 
{
	$ndx=0
	if ($MyInvocation.PositionMessage -match 'char:(\d+)') {
		$ndx = [int]$matches[1]
	}
	
	$setDirCmd = "[Environment]::CurrentDirectory = (set-location -LiteralPath '$pwd' -PassThru).ProviderPath"
	
	$OFS = ","
    $filePath = 'PowerShell.exe'
    $suargs   = "-noexit -command & {$setDirCmd}"

	Write-Debug "Invoke-Elevated: `$MyInvocation.Line args index is $ndx; `$args are <$args>"
	
	if ($args.Length -eq 0)	
    {
        Write-Debug "  Starting Powershell without args"
	}
	elseif ($args[0] -is [Scriptblock]) 
    {
        $script = $args[0]
        Write-Debug "  Starting PowerShell with scriptblock: {$script}"
        $suargs = "-noexit -command & {$setDirCmd; $script}"
	}
	elseif ($args[0].Length -gt 0)
    {
        $startProcessArgs = $MyInvocation.Line.Substring($ndx)
        $app = Get-Command $args[0] | Select -First 1 | ? {$_.CommandType -eq 'Application'}
        if ($app) {
            Write-Debug "  Starting app with args"
            $filePath = $app.Path
            $suargs = $startProcessArgs.Substring($args[0].Length).Trim()
        }
        else {
            Write-Debug "  Starting PowerShell with args"
            $suargs = "-noexit -command & {$setDirCmd; $startProcessArgs}"
        }
	}
    
    Write-Debug "  Invoking Start-Process on filepath $filePath with args: '$suargs'"
    Microsoft.PowerShell.Management\Start-Process $filePath -Verb runas -WorkingDir $pwd -Arg $suargs -PassThru
}

<#
.SYNOPSIS
    Resolves the PowerShell error code to a textual description of the error.
.DESCRIPTION
    Use when reporting an error or ask a question about a exception you
    are seeing.  This function provides all the information we have 
    about the error message making it easier to diagnose what is 
    actually going on.
.PARAMETER ErrorRecord
	The ErrorRecord to resolve into a useful error report. The default value
	is $Error[0] - the last error that occurred.
.EXAMPLE
    C:\PS> Resolve-ErrorRecord
    Resolves the most recent PowerShell error code to a textual description of the error.
.NOTES
    Aliases:  rver
#>
function Resolve-ErrorRecord
{
	param(
		[Parameter(Position=0, ValueFromPipeline=$true)]
		[ValidateNotNull()]
		[System.Management.Automation.ErrorRecord[]]
		$ErrorRecord
	)
	
    Process
    {
        if (!$ErrorRecord) 
        {
            if ($global:Error.Count -eq 0)
            {
                Write-Host "The `$Error collection is empty."
                return
            }
            else
            { 
                $ErrorRecord = @($global:Error[0])
            }
        }
        foreach ($record in $ErrorRecord)
        {
            $record | Format-List * -Force
            $record.InvocationInfo | Format-List *
            $Exception = $record.Exception
            for ($i = 0; $Exception; $i++, ($Exception = $Exception.InnerException))
            {   
                "$i" * 80
               $Exception | Format-List * -Force
            }
        }
    }
}

<#
.SYNOPSIS
    Resolves the hresult error code to a textual description of the error.
.DESCRIPTION
    Resolves the hresult error code to a textual description of the error.
.PARAMETER HResult
	The hresult error code to resolve.    
.EXAMPLE
    C:\PS> Resolve-HResult -2147023293
    Fatal error during installation. (Exception from HRESULT: 0x80070643)
.NOTES
    Aliases:  rvhr
#>
function Resolve-HResult
{
	param(
		[Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
		[long[]]
		$HResult
	)
	
	Process 
	{
		foreach ($hr in $HResult)
		{
			$comEx = [System.Runtime.InteropServices.Marshal]::GetExceptionForHR($hr)
			if ($comEx)
			{
				$comEx.Message
			}
			else
			{
				Write-Error "$hr doesn't correspond to a known HResult"
			}
		}
	}
}

<#
.SYNOPSIS
    Resolves a Windows error number a textual description of the error.
.DESCRIPTION
    Resolves a Windows error number a textual description of the error. The Windows
    error number is typically retrieved via the Win32 API GetLastError() but it is
    typically displayed in messages to the end user.
.PARAMETER ErrorNumber
	The Windows error code number to resolve.    
.EXAMPLE
    C:\PS> Resolve-WindowsError 5
    Access is denied
.NOTES
    Aliases:  rvwer
#>
function Resolve-WindowsError
{
	param(
		[Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
		[int[]]
		$ErrorNumber
	)
	
	Process
	{
		foreach ($num in $ErrorNumber)
		{
			$win32Ex = new-object ComponentModel.Win32Exception $num
			if ($win32Ex)
			{
				$win32Ex.Message
			}
			else
			{
				Write-Error "$num does not correspond to a known Windows error code"
			}
		}
	}
}

<#
.SYNOPSIS
    Convenience function for creating an array of strings without requiring quotes or commas.
.DESCRIPTION
    Convenience function for creating an array of strings without requiring quotes or commas.
.EXAMPLE
    C:\PS> QuoteList foo bar baz 
    This is the equivalent of 'foo', 'bar', 'baz'
.EXAMPLE
    C:\PS> ql foo bar baz 
    This is the equivalent of 'foo', 'bar', 'baz'. Same example as above but using the alias
    for QuoteList.
.NOTES
    Aliases:  ql
#>
function QuoteList { $args }

<#
.SYNOPSIS
    Creates a string from each parameter by concatenating each item using $OFS as the separator.
.DESCRIPTION
    Creates a string from each parameter by concatenating each item using $OFS as the separator.
.EXAMPLE
    C:\PS> QuoteString $a $b $c 
    This is the equivalent of "$a $b $c".
.EXAMPLE
    C:\PS> qs $a $b $c 
    This is the equivalent of "$a $b $c".  Same example as above but using the alias for QuoteString.
.NOTES
    Aliases:  qs
#>
function QuoteString { "$args" }

<#
.SYNOPSIS
    Invokes the .NET garbage collector to clean up garbage objects.
.DESCRIPTION
    Invokes the .NET garbage collector to clean up garbage objects. Invoking
    a garbage collection can be useful when .NET objects haven't been disposed 
    and is causing a file system handle to not be released.
.EXAMPLE
    C:\PS> Invoke-GC 
    Invokes a garbage collection to free up resources and memory.
#>
function Invoke-GC 
{
	[System.GC]::Collect()
}

<#
.SYNOPSIS
    Invokes the specified batch file and retains any environment variable changes it makes.
.DESCRIPTION
    Invoke the specified batch file (and parameters), but also propagate any  
    environment variable changes back to the PowerShell environment that  
    called it.
.PARAMETER Path
    Path to a .bat or .cmd file.
.PARAMETER Parameters
    Parameters to pass to the batch file.
.EXAMPLE
    C:\PS> Invoke-BatchFile "$env:ProgramFiles\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
    Invokes the vcvarsall.bat file.  All environment variable changes it makes will be
    propagated to the current PowerShell session.
.NOTES
    Author: Lee Holmes    
#>
function Invoke-BatchFile
{
    param([string]$Path, [string]$Parameters)  

    $tempFile = [IO.Path]::GetTempFileName()  

    ## Store the output of cmd.exe.  We also ask cmd.exe to output   
    ## the environment table after the batch file completes  
    cmd.exe /c " `"$Path`" $Parameters && set > `"$tempFile`" " 

    ## Go through the environment variables in the temp file.  
    ## For each of them, set the variable in our local environment.  
    Get-Content $tempFile | Foreach-Object {   
        if ($_ -match "^(.*?)=(.*)$")  
        { 
            Set-Content "env:\$($matches[1])" $matches[2]  
        } 
    }  

    Remove-Item $tempFile
}

<#
.SYNOPSIS
    Gets the possible alternate views for the specified object.
.DESCRIPTION
    Gets the possible alternate views for the specified object.
.PARAMETER TypeName
    Name of the type for which to retrieve the view definitions.
.PARAMETER Path
    Path to a specific format data PS1XML file.  Wildcards are accepted.  The default
    value is an empty array which will load the default .ps1xml files and exported
    format files from modules loaded in the current session
.PARAMETER IncludeSnapInFormatting
	Include the exported format information from v1 PSSnapins.
.EXAMPLE
    C:\PS> Get-ViewDefinition
    Retrieves all view definitions from the PowerShell format files.
.EXAMPLE
    C:\PS> Get-ViewDefinition System.Diagnostics.Process
    Retrieves all view definitions for the .NET type System.Diagnostics.Process.
.EXAMPLE
    C:\PS> Get-Process | Get-ViewDefinition | ft Name,Style -groupby SelectedBy
    Retrieves all view definitions for the .NET type System.Diagnostics.Process.
.EXAMPLE
    C:\PS> Get-ViewDefinition Pscx.Commands.Net.PingHostStatistics $Pscx:Home\Modules\Net\Pscx.Net.Format.ps1xml
    Retrieves all view definitions for the .NET type Pscx.Commands.Net.PingHostStatistics.
.NOTES
    Author: Joris van Lier and Keith Hill    
#>
function Get-ViewDefinition
{
 	[CmdletBinding(DefaultParameterSetName="Name")]
 	param(
 		[Parameter(Position=0, ParameterSetName="Name")]
 		[string]
 		$TypeName,
	 	
 		[Parameter(Position=0, ParameterSetName="Object", ValueFromPipeline=$true)]
 		[psobject]
 		$InputObject,
	 	
 		[Parameter(Position=1)]
 		[string[]]
 		$Path = @(),
	 	
 		[Parameter(Position=2)]
 		[switch]
 		$IncludeSnapInFormatting
 	)

 	Begin
 	{
 		# Setup arrays to hold Format XMLDocument objects and the paths to them
 		$arrFormatFiles = @()
 		$arrFormatFilePaths = @()
 		# If a specific Path is specified, use that, otherwise load all defaults
 		# which consist of the default formatting files, and exported format files
 		# from modules
 		if ($Path.count -eq 0)
 		{
 			# Populate the arrays with the standard ps1xml format file information
 			gci $PsHome *.format.ps1xml | % `
 			{
 				if (Test-Path $_.fullname)
 				{
 					$x = New-Object xml.XmlDocument
 					$x.Load($_.fullname)
 					$arrFormatFiles += $x
 					$arrFormatFilePaths += $_.fullname
 				}
 			}
 			# Populate the arrays with format info from loaded modules
 			Get-Module | Select -ExpandProperty exportedformatfiles | % `
 			{
 				if (Test-Path $_)
 				{
 					$x = New-Object xml.XmlDocument
 					$x.load($_)
 					$arrFormatFiles += $x
 					$arrFormatFilePaths += $_
 				}
 			}
 			# Processing snapin formatting seems to be slow, and snapins are more or less
 			# deprecated with modules in v2, so exclude them by default
 			if ($IncludeSnapInFormatting)
 			{
 				# Populate the arrays with format info from loaded snapins
 				Get-PSSnapin | ? { $_.name -notmatch "Microsoft\." } | select applicationbase,formats | % `
 				{
 					foreach ($f in $_.formats)
 					{
 						$x = New-Object xml.xmlDocument
 						if ( test-path $f )
 						{
 							$x.load($f)
 							$arrFormatFiles += $x
 							$arrFormatFilePaths += $f
 						}
 						else
 						{
 							$fpath = "{0}\{1}" -f $_.ApplicationBase,$f
 							if (Test-Path $fpath)
 							{
 								$x.load($fpath)
 								$arrFormatFiles += $x
 								$arrFormatFilePaths += $fpath
 							}
 						}
 					} 
 				}
 			}
 		}
 		else
 		{
 			foreach ($p in $path)
 			{
 				$x = New-Object xml.xmldocument
 				if (Test-Path $p)
 				{
 					$x.load($p)
 					$arrFormatFiles += $x
 					$arrFormatFilePaths += $p
 				}
 			}
 		}
 		$TypesSeen = @{}
 		
 		# The functions below reference's object members that may not exist
 		Set-StrictMode -Version 1.0
 		
 		function IsViewSelectedByTypeName($view, $typeName, $formatFile)
 		{
 			if ($view.ViewSelectedBy.TypeName)
 			{
 				foreach ($t in @($view.ViewSelectedBy.TypeName))
 				{
 					if ($typeName -eq $t) { return $true }
 				}
 				$false
 			}
 			elseif ($view.ViewSelectedBy.SelectionSetName)
 			{
 				$typeNameNodes = $formatFile.SelectNodes('/Configuration/SelectionSets/SelectionSet/Types')
 				$typeNames = $typeNameNodes | foreach {$_.TypeName}
 				$typeNames -contains $typeName
 			}
 			else
 			{
 				$false
 			}
 		}
 		
 		function GenerateViewDefinition($typeName, $view, $path)
 		{
 			$ViewDefinition = new-object psobject
 			
 			Add-Member NoteProperty Name $view.Name -Input $ViewDefinition
 			Add-Member NoteProperty Path $path -Input $ViewDefinition
 			Add-Member NoteProperty TypeName $typeName -Input $ViewDefinition
 			$selectedBy = ""
 			if ($view.ViewSelectedBy.TypeName)
 			{
 				$selectedBy = $view.ViewSelectedBy.TypeName
 			}
 			elseif ($view.ViewSelectedBy.SelectionSetName)
 			{
 				$selectedBy = $view.ViewSelectedBy.SelectionSetName
 			}
 			Add-Member NoteProperty SelectedBy $selectedBy -Input $ViewDefinition
 			Add-Member NoteProperty GroupBy $view.GroupBy.PropertyName -Input $ViewDefinition
 			if ($view.TableControl)
 			{
 				Add-Member NoteProperty Style 'Table' -Input $ViewDefinition
 			}
 			elseif ($view.ListControl)
 			{
 				Add-Member NoteProperty Style 'List' -Input $ViewDefinition
 			}
 			elseif ($view.WideControl)
 			{
 				Add-Member NoteProperty Style 'Wide' -Input $ViewDefinition
 			}
 			elseif ($view.CustomControl)
 			{
 				Add-Member NoteProperty Style 'Custom' -Input $ViewDefinition
 			}
 			else
 			{
 				Add-Member NoteProperty Style 'Unknown' -Input $ViewDefinition
 			}
 			
 			$ViewDefinition
 		}
 		
 		function GenerateViewDefinitions($typeName, $path)
 		{
 			for ($i = 0 ; $i -lt $arrFormatFiles.count ; $i++)
 			{
 				$formatFile = $arrFormatFiles[$i]
 				$path		= $arrFormatFilePaths[$i]
 				foreach ($view in $formatFile.Configuration.ViewDefinitions.View)
 				{
 					if ($typeName)
 					{
 						if (IsViewSelectedByTypeName $view $typeName $formatFile)
 						{
 							GenerateViewDefinition $typeName $view $path
 						}
 					}
 					else
 					{
 					GenerateViewDefinition $typeName $view $path
 					}
 				}
 			}
 		}
 	}
 	
 	Process
 	{
 		if ($pscmdlet.ParameterSetName -eq 'Name')
 		{
 			GenerateViewDefinitions $TypeName #$Path
 		}
 		elseif (!$TypesSeen[$InputObject.PSObject.TypeNames[0]])
 		{
 			if ($InputObject -is [string])
 			{
 				GenerateViewDefinitions $InputObject
 			}
 			else
 			{
 				GenerateViewDefinitions $InputObject.PSObject.TypeNames[0]
 			}
 			$TypesSeen[$InputObject.PSObject.TypeNames[0]] = $true
 		}
 	}
}

<#
.SYNOPSIS
    Outputs text as spoken words.
.DESCRIPTION
    Outputs text as spoken words.
.PARAMETER InputObject
    One or more objects to speak.
.PARAMETER Wait
    Wait for the machine to read each item (NOT asynchronous).
.PARAMETER Purge
    Purge all other speech requests before making this call.
.PARAMETER ReadFiles
    Read the contents of the text files indicated.
.PARAMETER ReadXml
    Treat input as speech XML markup.
.PARAMETER NotXml
    Do NOT parse as XML (if text starts with "<" but is not XML).
.EXAMPLE
    C:\PS> Out-Speech "Hello World"
    Speaks "hello world".
.EXAMPLE
    C:\PS> Get-Content quotes.txt | Get-Random | Out-Speech -wait
    Speaks a random quote from a file.
.EXAMPLE
    C:\PS> Out-Speech -readfiles "Hitchhiker's Guide To The Galaxy.txt"
    Speaks the entire contents of a file.
.NOTES
    Author: Joel "Jaykul" Bennett    
#>
function Out-Speech
{
    param(
        [Parameter(Position=0, Mandatory=$true, ValueFromPipeline=$true)]
        [psobject[]]
        $InputObject, 
        
        [switch]
        $Wait, 
        
        [switch]
        $Purge, 
        
        [switch]
        $ReadFiles, 
        
        [switch]
        $ReadXml, 
        
        [switch]
        $NotXml
    )
     
    begin 
    {  
	    # To override this default, use the other flag values given below.
	    $SPF_DEFAULT = 0          # Specifies that the default settings should be used.  
	    ## The defaults are:
	    #~ * Speak the given text string synchronously
	    #~ * Not purge pending speak requests
	    #~ * Parse the text as XML only if the first character is a left-angle-bracket (<)
	    #~ * Not persist global XML state changes across speak calls
	    #~ * Not expand punctuation characters into words.
	    $SPF_ASYNC = 1            # Specifies that the Speak call should be asynchronous.
	    $SPF_PURGEBEFORESPEAK = 2 # Purges all pending speak requests prior to this speak call.
	    $SPF_IS_FILENAME = 4      # The string passed is a file name, and the file text should be spoken.
	    $SPF_IS_XML = 8           # The input text will be parsed for XML markup. 
	    $SPF_IS_NOT_XML= 16       # The input text will not be parsed for XML markup.
      
	    $SPF = $SPF_DEFAULT
	    if (!$wait)    { $SPF += $SPF_ASYNC }
	    if ($purge)    { $SPF += $SPF_PURGEBEFORESPEAK }
	    if ($readfiles){ $SPF += $SPF_IS_FILENAME }
	    if ($readxml)  { $SPF += $SPF_IS_XML }
	    if ($notxml)   { $SPF += $SPF_IS_NOT_XML }

	    $Voice = New-Object -Com SAPI.SpVoice
    }
     
    process 
    {
        foreach ($obj in $InputObject)
        {
            $str = $obj | Out-String
		    $exit = $Voice.Speak($str, $SPF)
	    }
    }
}

<#
.SYNOPSIS
    Stops a process on a remote machine.
.DESCRIPTION
    Stops a process on a remote machine.
    This command uses WMI to terminate the remote process.
.PARAMETER ComputerName
    The name of the remote computer that the process is executing on.
    Type the NetBIOS name, an IP address, or a fully qualified domain name of the remote computer.
.PARAMETER Name
    The process name of the remote process to terminate.
.PARAMETER Id
    The process id of the remote process to terminate.
.PARAMETER Credential    
    Specifies a user account that has permission to perform this action. The default is the current user. 
    Type a user name, such as "User01", "Domain01\User01", or User@Contoso.com. Or, enter a PSCredential 
    object, such as an object that is returned by the Get-Credential cmdlet. When you type a user name, 
    you will be prompted for a password.    
.EXAMPLE
    C:\PS> Stop-RemoteProcess server1 notepad.exe
    Stops all processes named notepad.exe on the remote computer server1.
.EXAMPLE
    C:\PS> Stop-RemoteProcess server1 3478
    Stops the process with process id 3478 on the remote computer server1.
.EXAMPLE
    C:\PS> 3478,4005 | Stop-RemoteProcess server1
    Stops the processes with process ids 3478 and 4005 on the remote computer server1.
.NOTES
    Author: Jachym Kouba and Keith Hill
#>
function Stop-RemoteProcess
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0, Mandatory=$true)]
        [string]
        $ComputerName, 
        
        [Parameter(Position=1, Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="Name")]
        [string[]]
        $Name,
        
        [Parameter(Position=1, Mandatory=$true, ValueFromPipeline=$true, 
                   ValueFromPipelineByPropertyName=$true, ParameterSetName="Id")]
        [int[]]
        $Id,
        
        [System.Management.Automation.PSCredential]
        $Credential
    ) 
    
    Process
    {
        $params = @{
            Class = 'Win32_Process'
            ComputerName = $ComputerName
        }
        
        if ($Credential)
        {
            $params.Credential = $Credential
        }
        
        if ($pscmdlet.ParameterSetName -eq 'Name')
        {
            foreach ($item in $Name)
            {
                if (!$pscmdlet.ShouldProcess("process $item on computer $ComputerName"))
                {
                    continue
                }
                $params.Filter = "Name LIKE '%$item%'"
                Get-WmiObject @params | Foreach {
	                if ($_.Terminate().ReturnValue -ne 0) {
		                Write-Error "Failed to stop process $item on $ComputerName."
	                }
                }
            }
        }
        else
        {
            foreach ($item in $Id)
            {
                if (!$pscmdlet.ShouldProcess("process id $item on computer $ComputerName"))
                {
                    continue
                }
                $params.Filter = "ProcessId = $item"
                Get-WmiObject @params | Foreach {
	                if ($_.Terminate().ReturnValue -ne 0) {
		                Write-Error "Failed to stop process id $item on $ComputerName."
	                }
                }
            }
        }
    }
}

<#
.SYNOPSIS
    Generate CSS header for HTML "screen shot" of the host buffer.
.DESCRIPTION
    Generate CSS header for HTML "screen shot" of the host buffer.
.EXAMPLE
    C:\PS> $css = Get-ScreenCss
    Gets the color info of the host's screen into CSS form.
.NOTES
    Author: Jachym Kouba
#>
function Get-ScreenCss
{
    param()
    
    Process
    {
        '<style>'
        [Enum]::GetValues([ConsoleColor]) | Foreach {
	        "  .F$_ { color: $_; }"
	        "  .B$_ { background-color: $_; }"
        }
        '</style>'
    }
}

<#
.SYNOPSIS
    Functions to generate HTML "screen shot" of the host buffer.
.DESCRIPTION
    Functions to generate HTML "screen shot" of the host buffer.
.PARAMETER Count
    The number of lines of the host buffer to create a screen shot from.
.EXAMPLE
    C:\PS> Stop-RemoteProcess server1 notepad.exe
    Stops all processes named notepad.exe on the remote computer server1.
.EXAMPLE
    C:\PS> Get-ScreenHtml > screen.html
    Generates an HTML representation of the host's screen buffer and saves it to file.
.EXAMPLE
    C:\PS> Get-ScreenHtml 25 > screen.html
    Generates an HTML representation of the first 25 lines of the host's screen buffer and saves it to file.
.NOTES
    Author: Jachym Kouba
#>
function Get-ScreenHtml
{
    param($Count = $Host.UI.RawUI.WindowSize.Height)

    Begin
    {
        # Required by HttpUtility
        Add-Type -Assembly System.Web

        $raw = $Host.UI.RawUI
        $buffsz = $raw.BufferSize
        
        function BuildHtml($out, $buff) 
        {
	        function OpenElement($out, $fore, $back) 
	        {
		        & { 
			        $out.Append('<span class="F').Append($fore)
			        $out.Append(' B').Append($back).Append('">')
		        } | out-null
	        }
        	
	        function CloseElement($out) {
		        $out.Append('</span>') | out-null
	        }

	        $height = $buff.GetUpperBound(0)
	        $width  = $buff.GetUpperBound(1)
        	
	        $prev = $null
	        $whitespaceCount = 0
        	
	        $out.Append("<pre class=`"B$($Host.UI.RawUI.BackgroundColor)`">") | out-null
        	
	        for ($y = 0; $y -lt $height; $y++) 
	        {
		        for ($x = 0; $x -lt $width; $x++) 
		        {
			        $current = $buff[$y, $x] 
        		
			        if ($current.Character -eq ' ') 
			        {
				        $whitespaceCount++
				        write-debug "whitespaceCount: $whitespaceCount"
			        }
			        else 
			        {
				        if ($whitespaceCount) 
				        {
					        write-debug "appended $whitespaceCount spaces, whitespaceCount: 0"					
					        $out.Append((new-object string ' ', $whitespaceCount)) | out-null
					        $whitespaceCount = 0
				        }
        	
				        if ((-not $prev) -or 
				            ($prev.ForegroundColor -ne $current.ForegroundColor) -or
				            ($prev.BackgroundColor -ne $current.BackgroundColor)) 
				        {
					        if ($prev) { CloseElement $out }
        					
					        OpenElement $out $current.ForegroundColor $current.BackgroundColor
				        } 
        						
                        $char = [System.Web.HttpUtility]::HtmlEncode($current.Character)
				        $out.Append($char) | out-null
				        $prev =	$current	
			        }
		        }
        		
		        $out.Append("`n") | out-null
		        $whitespaceCount = 0
	        }
        	
	        if($prev) { CloseElement $out }
        	
	        $out.Append('</pre>') | out-null
        }
    }
    
    Process
    {
        $cursor = $raw.CursorPosition

        $rect = new-object Management.Automation.Host.Rectangle 0, ($cursor.Y - $Count), $buffsz.Width, $cursor.Y
        $buff = $raw.GetBufferContents($rect)

        $out = new-object Text.StringBuilder
        BuildHtml $out $buff
        $out.ToString()
    }
}

<#
.SYNOPSIS
    Function to call a single method on an incoming stream of piped objects.
.DESCRIPTION
    Function to call a single method on an incoming stream of piped objects. Methods can be static or instance and
    arguments may be passed as an array or individually. 
.PARAMETER InputObject
    The object to execute the named method on. Accepts pipeline input.
.PARAMETER MemberName
	The member to execute on the passed object.
.PARAMETER Arguments
    The arguments to pass to the named method, if any.
.PARAMETER Static
    The member name will be treated as a static method call on the incoming object.
.EXAMPLE
    C:\PS> 1..3 | invoke-method gettype
    Calls GetType() on each incoming integer.
.EXAMPLE
    C:\PS> dir *.txt | invoke-method moveto "c:\temp\"
    Calls the MoveTo() method on all txt files in the current directory passing in "C:\Temp" as the destFileName.
.NOTES
    Aliases:  call
#>
function Invoke-Method {
    [CmdletBinding()]
    param(        
        [parameter(valuefrompipeline=$true, mandatory=$true)]
        [allownull()]
        [allowemptystring()]
        $InputObject,
        
        [parameter(position=0, mandatory=$true)]
        [validatenotnullorempty()]
        [string]$MethodName,
        
        [parameter(valuefromremainingarguments=$true)]
        [allowemptycollection()]
        [object[]]$Arguments,

        [parameter()]
        [switch]$Static
    )
    
    Process 
    {
        if ($InputObject) 
        {
            if ($InputObject | Get-Member $methodname -static:$static) 
            {
                $flags = "ignorecase,public,invokemethod"
                
                if ($Static) {
                    $flags += ",static"
                } 
                else {
                    $flags += ",instance"
                }
                
                if ($InputObject -is [type]) {
                    $target = $InputObject
                } 
                else {
                    $target = $InputObject.gettype()
                }
                
                try {
					$target.invokemember($methodname, $flags, $null, $InputObject, $arguments)
                } 
                catch {
					if ($_.exception.innerexception -is [missingmethodexception]) {
						write-warning "Method argument count (or type) mismatch."
					}
                }
            } 
            else {
                write-warning "Method $methodname not found."
            }
        }
    }
}

<#
.SYNOPSIS
    Shows the specified path as a tree.
.DESCRIPTION
    Shows the specified path as a tree.  This works for any type of PowerShell provider and can be used to explore providers used for configuration like the WSMan provider.
.PARAMETER Path
    The path to the root of the tree that will be shown.
.PARAMETER Depth
    Specifies how many levels of the specified path are recursed and shown.
.PARAMETER IndentSize
    The size of the indent per level. The default is 3.  Minimum value is 1.
.PARAMETER Force
    Allows the command to show items that cannot otherwise not be accessed by the user, such as hidden or system files.
    Implementation varies from provider to provider. For more information, see about_Providers. Even using the Force 
    parameter, the command cannot override security restrictions.
.PARAMETER ShowLeaf
    Shows the leaf items in each container.
.PARAMETER ShowProperty
    Shows the properties on containers and items (if -ShowLeaf is specified).
.PARAMETER ExcludeProperty
    List of properties to exclude from output.  Only used when -ShowProperty is specified.
.PARAMETER Width
    Specifies the number of characters in each line of output. Any additional characters are truncated, not wrapped.
    If you omit this parameter, the width is determined by the characteristics of the host. The default for the 
    PowerShell.exe host is 80 (characters).
.PARAMETER UseAsciiLineArt
    Displays line art using only ASCII characters.
.EXAMPLE
    C:\PS> Show-Tree C:\Users -Depth 2
    Shows the directory tree structure, recursing down two levels.
.EXAMPLE
    C:\PS> Show-Tree HKLM:\SOFTWARE\Microsoft\.NETFramework -Depth 2 -ShowProperty -ExcludeProperty 'SubKeyCount','ValueCount'
    Shows the hierarchy of registry keys and values (-ShowProperty), recursing down two levels.  Excludes the standard regkey extended properties SubKeyCount and ValueCount from the output.
.EXAMPLE
    C:\PS> Show-Tree WSMan: -ShowLeaf
    Shows all the container and leaf items in the WSMan: drive.
#>
function Show-Tree
{
    [CmdletBinding(DefaultParameterSetName="Path")]
    param(
        [Parameter(Position=0, 
                   ParameterSetName="Path", 
                   ValueFromPipeline=$true, 
                   ValueFromPipelineByPropertyName=$true)]
        [ValidateNotNullOrEmpty()]
        [string[]]
        $Path,
        
        [Alias("PSPath")]
        [Parameter(Position=0, 
                   ParameterSetName="LiteralPath", 
                   ValueFromPipelineByPropertyName=$true)]
        [ValidateNotNullOrEmpty()]
        [string[]]
        $LiteralPath,
            
        [Parameter(Position = 1)]
        [ValidateRange(0, 2147483647)]
        [int]
        $Depth = [int]::MaxValue, 
        
        [Parameter()]
        [switch]
        $Force, 
        
        [Parameter()]
        [ValidateRange(1, 100)]
        [int]
        $IndentSize = 3, 
        
        [Parameter()]
        [switch]
        $ShowLeaf, 
        
        [Parameter()]
        [switch]
        $ShowProperty,
        
        [Parameter()]
        [string[]]
        $ExcludeProperty,
        
        [Parameter()]
        [ValidateRange(0, 2147483647)]
        [int]
        $Width,
        
        [Parameter()]
        [switch]
        $UseAsciiLineArt
    )

    Begin
    {
        Set-StrictMode -Version 2.0
        
        # Set default path if not specified
        if (!$Path -and $psCmdlet.ParameterSetName -eq "Path")
        {
            $Path = Get-Location
        }
        
        if ($Width -eq 0)
        {
            $Width = $host.UI.RawUI.BufferSize.Width
        }
        
        $asciiChars = @{
            EndCap        = '\'
            Junction      = '|'
            HorizontalBar = '-'
            VerticalBar   = '|'
        }
        
        $cp437Chars = @{
            EndCap        = '└'
            Junction      = '├'
            HorizontalBar = '─'
            VerticalBar   = '│'
        }
        
        if (($Host.CurrentCulture.TextInfo.OEMCodePage -eq 437) -and !$UseAsciiLineArt)
        {
            $lineChars = $cp437Chars
        }
        else
        {
            $lineChars = $asciiChars
        }
               
        function GetIndentString([bool[]]$IsLast)
        {
            $str = ''
            for ($i=0; $i -lt $IsLast.Count - 1; $i++)
            {
                $str += if ($IsLast[$i]) {' '} else {$lineChars.VerticalBar}
                $str += " " * ($IndentSize - 1)
            }
            $str += if ($IsLast[-1]) {$lineChars.EndCap} else {$lineChars.Junction}
            $str += $lineChars.HorizontalBar * ($IndentSize - 1)
            $str
        }
        
        function CompactString([string]$String, [int]$MaxWidth = $Width)
        {
            $updatedString = $String
            if ($String.Length -ge $MaxWidth)
            {
                $ellipsis = '...'
                $updatedString = $String.Substring(0, $MaxWidth - $ellipsis.Length - 1) + $ellipsis
            }
            $updatedString    
        }

        function ShowItemText([string]$ItemPath, [string]$ItemName, [bool[]]$IsLast)
        {
            if ($IsLast.Count -eq 0) 
            {
                $itemText = Resolve-Path -LiteralPath $ItemPath | Foreach {$_.Path}
                CompactString $itemText
            }
            else
            {
                $itemText = $ItemName                
                if (!$itemText)
                {
                    if ($ExecutionContext.SessionState.Path.IsProviderQualified($ItemPath))
                    {
                        $itemText = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ItemPath)
                    }
                }
                CompactString "$(GetIndentString $IsLast)$itemText"
            }
        }
        
        function ShowPropertyText ([string]$Name, [string]$Value, [bool[]]$IsLast)
        {
            $cookedValue = @($Value -split "`n")[0].Trim()
            CompactString "$(GetIndentString $IsLast)Property: $Name = $cookedValue"
        }
        
        function ShowItem([string]$ItemPath, [string]$ItemName='', [bool[]]$IsLast=@())
        {        
            $isContainer = Test-Path -LiteralPath $ItemPath -Type Container
            if (!($isContainer -or $ShowLeaf))
            {
                Write-Warning "Path is not a container, use the ShowLeaf parameter to show leaf items."
                return
            }
            
            # Show current item
            ShowItemText $ItemPath $ItemName $IsLast
            
            # If the item is a container, grab its children.  This let's us know if there
            # will be items after the last property at the same level.
            $childItems = @()
            if ($isContainer -and ($IsLast.Count -lt $Depth))
            {
                $childItems = @(Get-ChildItem -LiteralPath $ItemPath -Force:$Force -ErrorAction $ErrorActionPreference | 
                                Where {$ShowLeaf -or $_.PSIsContainer} | Select PSPath, PSChildName)            
            }

            # Track parent's "last item" status to determine which level gets a vertical bar
            $IsLast += @($false)
            
            # If requested, show item properties
            if ($ShowProperty)
            {
                $excludedProviderNoteProps =  'PSIsContainer','PSChildName','PSDrive','PSParentPath','PSPath','PSProvider','Name','Property'
                $excludedProviderNoteProps += $ExcludeProperty
                $props = @()
                $itemProp = Get-ItemProperty -LiteralPath $ItemPath -ErrorAction SilentlyContinue
                if ($itemProp)
                {
                    $props = @($itemProp.psobject.properties | Sort Name | Where {$excludedProviderNoteProps -notcontains $_.Name})
                }
                else
                {
                    $item = $null
                    # Have to use try/catch here because Get-Item cert: error caught be caught with -EA
                    try { $item = Get-Item -LiteralPath $ItemPath -ErrorAction SilentlyContinue } catch {}
                    if ($item)
                    {
                        $props = @($item.psobject.properties | Sort Name | Where {$excludedProviderNoteProps -notcontains $_.Name})
                    }
                }
                
                for ($i=0; $i -lt $props.Count; $i++)
                {
                    $IsLast[-1] = ($i -eq $props.count -1) -and ($childItems.Count -eq 0)

                    $prop = $props[$i]
                    ShowPropertyText $prop.Name $prop.Value $IsLast
                }
            }
            
            # Recurse through child items
            for ($i=0; $i -lt $childItems.Count; $i++)
            {
                $childItemPath = $childItems[$i].PSPath
                $childItemName = $childItems[$i].PSChildName
                $IsLast[-1] = ($i -eq $childItems.Count - 1)
                if ($ShowLeaf -or (Test-Path -LiteralPath $childItemPath -Type Container))
                {
                    ShowItem $childItemPath $childItemName $IsLast
                }
            }
        }
    }

    Process
    {
        if ($psCmdlet.ParameterSetName -eq "Path")
        {
            # In the -Path (non-literal) resolve path in case it is wildcarded.
            $resolvedPaths = @($Path | Resolve-Path | Foreach {"$_"})
        }
        else 
        {
            # Must be -LiteralPath
            $resolvedPaths = @($LiteralPath)
        }
 
        foreach ($rpath in $resolvedPaths) 
        {
            Write-Verbose "Processing $rpath"            
            ShowItem $rpath
        }  
    }        
}

function Invoke-Reflector {
<#
    .SYNOPSIS
        Quickly load Reflector, with the specified Type or Command selected. 
    .DESCRIPTION
        Quickly load Reflector, with the specified Type or Command selected. The function will also
        ensure that Reflector has the Type or Command's containing Assembly loaded.
    .EXAMPLE
        # Opens System.String in Reflector. Will load its Assembly into Reflector if required.
        ps> [string] | invoke-reflector    
    .EXAMPLE
        # Opens GetChildItemCommand in Reflector. Will load its Assembly into Reflector if required.
        ps> gcm ls | invoke-reflector        
    .EXAMPLE
        # Opens GetChildItemCommand in Reflector. Will load its Assembly into Reflector if required.
        ps> invoke-reflector dir        
    .PARAMETER CommandName
        Accepts name of command. Does not accept pipeline input.
    .PARAMETER CommandInfo
        Accepts output from Get-Command (gcm). Accepts pipeline input.
    .PARAMETER Type
        Accepts a System.Type (System.RuntimeType). Accepts pipeline input.
    .PARAMETER ReflectorPath
        Optional. Defaults to Reflector.exe's location if it is found in your $ENV:PATH. If not found, you must specify.
    .INPUTS
        [System.Type]
        [System.Management.Automation.CommandInfo]
    .OUTPUTS
        None
#>
     [cmdletbinding(defaultparametersetname="name")]
     param(
         [parameter(
            parametersetname="name",
            position=0,
            mandatory=$true
         )]
         [validatenotnullorempty()]
         [string]$CommandName,
         
         [parameter(
            parametersetname="command",
            position=0,
            valuefrompipeline=$true,
            mandatory=$true
         )]
         [validatenotnull()]
         [management.automation.commandinfo]$CommandInfo,
         
         [parameter(
            parametersetname="type",
            position=0,
            valuefrompipeline=$true,
            mandatory=$true
         )]
         [validatenotnull()]
         [type]$Type,
         
         [parameter(
            position=1
         )]
         [validatenotnullorempty()]
         [string]$ReflectorPath = $((gcm reflector.exe -ea 0).definition)
     )
     
    end {
        # no process block; i only want
        # a single reflector instance
        
        if ($ReflectorPath -and (test-path $reflectorpath)) {

            $typeName = $null            
            $assemblyLocation = $null
            
            switch ($pscmdlet.parametersetname) {
            
                 { "name","command" -contains $_ } {
                
                    if ($CommandName) {
                        $CommandInfo = gcm $CommandName -ea 0
                    } else {
                        $CommandName = $CommandInfo.Name
                    }
                    
                    if ($CommandInfo -is [management.automation.aliasinfo]) {
                        
                        # expand aliases
                        while ($CommandInfo.CommandType -eq "Alias") {
                            $CommandInfo = gcm $CommandInfo.Definition
                        }                                                
                    }
                    
                    # can only reflect cmdlets, obviously.
                    if ($CommandInfo.CommandType -eq "Cmdlet") {
                    
                        $typeName = $commandinfo.implementingtype.fullname
                        $assemblyLocation = $commandinfo.implementingtype.assembly.location
                    
                    } elseif ($CommandInfo) {
                        write-warning "$CommandInfo is not a Cmdlet."
                    } else {                    
                        write-warning "Cmdlet $CommandName does not exist in current scope. Have you loaded its containing module or snap-in?"
                    }
                }
                
                "type" {
                    $typeName = $type.fullname
                    $assemblyLocation = $type.assembly.location
                }                                
            } # end switch
            
            
            if ($typeName -and $assemblyLocation) {
                & $reflectorPath /select:$typeName $assemblyLocation
            }
            
        } else {
            write-warning "Unable to find Reflector.exe. Please specify full path via -ReflectorPath."
        }
    } # end end
} # end function

Export-ModuleMember -Alias * -Function *
