Set-StrictMode -Version 2.0

# Only prepend format data once - unloading a module doesn't undo the effects of prepending format data
if (!$Pscx:Session['FileSystem_PrependedFormatData'])
{
	$ScriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
	Write-Verbose "PSCX prepending format data $ScriptDir\Pscx.FileSystem.Format.ps1xml."
	Update-FormatData -PrependPath "$ScriptDir\Pscx.FileSystem.Format.ps1xml"
	$Pscx:Session['FileSystem_PrependedFormatData'] = $true
}

Set-Alias sro Set-ReadOnly -Description "PSCX alias"
Set-Alias swr Set-Writable -Description "PSCX alias"

<#
.SYNOPSIS
    Calculates the sizes of the specified directory and adds that size
    as a "Length" NoteProperty to the input DirectoryInfo object.
.DESCRIPTION
    Calculates the sizes of the specified directory and adds that size
    as a "Length" NoteProperty to the input DirectoryInfo object.  NOTE: Computing the
    size of a directory can noticeably impact performance. 
.PARAMETER InputObject
    The directory object (System.IO.DirectoryInfo) on which to add the Length property
.EXAMPLE
    C:\PS> Get-ChildItem . -Recurse | Add-DirectoryLength | Sort Length
    This example shows how you can compute the directory size for each directory passed via the pipeline
    and add that info to each DirectoryInfo object.
#>
function Add-DirectoryLength
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [AllowNull()]
        [PSObject]
        $InputObject
    )

    Begin 
    {
    	function ProcessFile([string]$path) {
    		(get-item -LiteralPath $path -Force).Length
    	}
    	
    	function ProcessDirectory([string]$path) {
    		$dirSize = 0
    		$items = get-childitem -LiteralPath $path -Force -ea $ErrorActionPreference | sort @{e={$_.PSIsContainer}}
    		if ($items -eq $null) {
    			return $null
    		}
    		foreach ($item in $items) {
    			if ($item.PSIsContainer) {
    				$dirSize += ProcessDirectory($item.FullName)
    			}
    			else {
    				$dirSize += ProcessFile($item.FullName)
    			}
    		}
    		$dirSize
    	}
    }

    Process {
    	if ($InputObject -is [System.IO.DirectoryInfo]) {
    		$dirSize = ProcessDirectory($InputObject.FullName)
    		Add-Member NoteProperty Length $dirSize -InputObject $InputObject
    	}
    	$InputObject
    }
}

<#
.SYNOPSIS
    Adds the file or directory's short path as a "ShortPath" NoteProperty to each input object. 
.DESCRIPTION
    Adds the file or directory's short path as a "ShortPath" NoteProperty to each input object.
    NOTE: This filter requires the PSCX cmdlet Get-ShortPath
.PARAMETER InputObject
    A DirectoryInfo or FileInfo object on which to add the ShortPath property
.EXAMPLE
    C:\PS> Get-ChildItem | Add-ShortPath | Format-Table ShortPath,FullName
    This example shows how you can add the short path to each DirectoryInfo or FileInfo object in the pipeline.
#>
function Add-ShortPath
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [AllowNull()]
        [PSObject]
        $InputObject
    )

    Process {
    	if ($InputObject -is [System.IO.FileSystemInfo]) {
    		$shortPathInfo = Get-ShortPath -LiteralPath $_.Fullname 
    		Add-Member NoteProperty ShortPath $shortPathInfo.ShortPath -InputObject $InputObject
    	}
    	$InputObject
    }
}

<#
.SYNOPSIS
    Sets a file's read only status to false making it writable.
.DESCRIPTION
    Sets a file's read only status to false making it writable.
.PARAMETER LiteralPath
    Specifies the path to a file make writable. Unlike Path, the value of LiteralPath is used exactly as it is typed. 
    No characters are interpreted as wildcards. If the path includes escape characters, enclose it in 
    single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters 
    as escape sequences.
.PARAMETER Path
    The path to the file make writable.  Wildcards are accepted.
.PARAMETER PassThru
	Passes the pipeline input object down the pipeline. By default, this cmdlet does not generate any output.
.EXAMPLE
    C:\PS> Set-Writable foo.txt
    Makes foo.txt writable.
.EXAMPLE
    C:\PS> Set-Writable [a-h]*.txt -passthru
    Makes any .txt file start with the letters a thru h writable and passes the filenames down the pipeline.
.EXAMPLE
    C:\PS> Get-ChildItem bar[0-9].txt | Set-Writable
    Set-Writable can accept pipeline input corresponding to files and make them all writable.
#>
function Set-Writable
{
	[CmdletBinding(DefaultParameterSetName="Path", SupportsShouldProcess=$true)]
	param(
		[Parameter(Position=0, Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="Path")]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$Path,
	    
		[Alias("PSPath")]
		[Parameter(Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true, ParameterSetName="LiteralPath")]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$LiteralPath,
		
		[switch]
		$PassThru
	)
	
	Process 
	{
		$resolvedPaths = @()
		if ($psCmdlet.ParameterSetName -eq "Path")
		{
		    # In the non-literal case we may need to resolve a wildcarded path
			foreach ($apath in $Path) 
			{
			    if (Test-Path $apath) 
			    {
				    $resolvedPaths += @(Resolve-Path $apath | Foreach { $_.Path })
			    }
			    else
			    {
			        Write-Error "File $apath does not exist"
			    }
			}
		}
		else
		{
			$resolvedPaths += $LiteralPath
		}
		
		foreach ($rpath in $resolvedPaths) 
		{
			$PathIntrinsics = $ExecutionContext.SessionState.Path
			if ($PathIntrinsics.IsProviderQualified($rpath))
			{
				$rpath = $PathIntrinsics.GetUnresolvedProviderPathFromPSPath($rpath)
			}
			
			if (!(Test-Path $rpath -PathType Leaf))
			{
			    Write-Error "$rpath is not a file."
			    continue
			}
			
			$fileInfo = New-Object System.IO.FileInfo $rpath				
			if ($pscmdlet.ShouldProcess("$fileInfo"))
			{
				$fileInfo.IsReadOnly = $false
			}
			
			if ($PassThru)
			{
			    $fileInfo
			}			
		}				
	}
}

<#
.SYNOPSIS
    Sets a file's read only status to true making it read only.
.DESCRIPTION
    Sets a file's read only status to true making it read only.
.PARAMETER LiteralPath
    Specifies the path to a file make read only. Unlike Path, the value of LiteralPath is used exactly as it is typed. 
    No characters are interpreted as wildcards. If the path includes escape characters, enclose it in 
    single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters 
    as escape sequences.
.PARAMETER Path
    The path to the file make read only.  Wildcards are accepted.
.PARAMETER PassThru
	Passes the pipeline input object down the pipeline. By default, this cmdlet does not generate any output.
.EXAMPLE
    C:\PS> Set-ReadOnly foo.txt
    Makes foo.txt read only.
.EXAMPLE
    C:\PS> Set-ReadOnly [a-h]*.txt -passthru
    Makes any .txt file start with the letters a thru h read only and passes the filenames down the pipeline.
.EXAMPLE
    C:\PS> Get-ChildItem bar[0-9].txt | Set-ReadOnly
    Set-ReadOnly can accept pipeline input corresponding to files and make them all read only.
#>
function Set-ReadOnly
{
	[CmdletBinding(DefaultParameterSetName="Path", SupportsShouldProcess=$true)]
	param(
		[Parameter(Position=0, Mandatory=$true, ValueFromPipeline=$true, ParameterSetName="Path")]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$Path,
	    
		[Alias("PSPath")]
		[Parameter(Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true, ParameterSetName="LiteralPath")]
		[ValidateNotNullOrEmpty()]
		[string[]]
		$LiteralPath,
		
		[switch]
		$PassThru
	)
		
	Process 
	{
		$resolvedPaths = @()
		if ($psCmdlet.ParameterSetName -eq "Path")
		{
		    # In the non-literal case we may need to resolve a wildcarded path
			foreach ($apath in $Path) 
			{
			    if (Test-Path $apath) 
			    {
				    $resolvedPaths += @(Resolve-Path $apath | Foreach { $_.Path })
			    }
			    else
			    {
			        Write-Error "File $apath does not exist"
			    }
			}
		}
		else
		{
			$resolvedPaths += $LiteralPath
		}
		
		foreach ($rpath in $resolvedPaths) 
		{
			$PathIntrinsics = $ExecutionContext.SessionState.Path
			if ($PathIntrinsics.IsProviderQualified($rpath))
			{
				$rpath = $PathIntrinsics.GetUnresolvedProviderPathFromPSPath($rpath)
			}
			
			if (!(Test-Path $rpath -PathType Leaf))
			{
			    Write-Error "$rpath is not a file."
			    continue
			}
			
			$fileInfo = New-Object System.IO.FileInfo $rpath				
			if ($pscmdlet.ShouldProcess("$fileInfo"))
			{
				$fileInfo.IsReadOnly = $true
			}
			
			if ($PassThru)
			{
			    $fileInfo
			}						
		}				
	}
}

Export-ModuleMember -Alias * -Function *