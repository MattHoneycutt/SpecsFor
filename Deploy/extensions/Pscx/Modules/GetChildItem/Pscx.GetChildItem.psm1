<#
.ForwardHelpTargetName Get-ChildItem
.ForwardHelpCategory Cmdlet
#>
function Get-ChildItem
{
    [CmdletBinding(DefaultParameterSetName='Items', SupportsTransactions=$true)]
    param(
        [Parameter(ParameterSetName='Items', Position=0, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true)]
        [System.String[]]
        ${Path},

        [Parameter(ParameterSetName='LiteralItems', Mandatory=$true, Position=0, ValueFromPipelineByPropertyName=$true)]
        [Alias('PSPath')]
        [System.String[]]
        ${LiteralPath},

        [Parameter(Position=1)]
        [System.String]
        ${Filter},

        [System.String[]]
        ${Include},

        [System.String[]]
        ${Exclude},

        [Switch]
        ${Recurse},

        [Switch]
        ${Force},

        [Switch]
        ${Name},

        [Switch]
        ${ContainerOnly},
        
        [Switch]
        ${LeafOnly}
    )
    
    DynamicParam
    {
        if ($path -match ".*CERT.*:")
        {
            $attributes = new-object System.Management.Automation.ParameterAttribute
            $attributes.ParameterSetName = "__AllParameterSets"
            $attributes.Mandatory = $false
            $attributeCollection = new-object -Type System.Collections.ObjectModel.Collection[System.Attribute]
            $attributeCollection.Add($attributes)
            $dynParam = new-object -Type System.Management.Automation.RuntimeDefinedParameter("CodeSigningCert", [switch], $attributeCollection)
            $paramDictionary = new-object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
            $paramDictionary.Add("CodeSigningCert", $dynParam)
            return $paramDictionary
        }
    }

    begin
    {
        try 
        {
            $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand('Get-ChildItem', [System.Management.Automation.CommandTypes]::Cmdlet)

            $outBuffer = $null
            if ($PSBoundParameters.TryGetValue('OutBuffer', [ref]$outBuffer))
            {
                $PSBoundParameters['OutBuffer'] = 1
            }
            
            if ($ContainerOnly -and $LeafOnly)
            {
                throw "The parameters ContainerOnly and LeafOnly are mutually exclusive"
            }
            elseif ($ContainerOnly)
            {
                [void]$PSBoundParameters.Remove("ContainerOnly")
                $scriptCmd = {& $wrappedCmd @PSBoundParameters | Where-Object {$_.PSIsContainer}}                    
            }
            elseif ($LeafOnly)
            {
                [void]$PSBoundParameters.Remove("LeafOnly")
                $scriptCmd = {& $wrappedCmd @PSBoundParameters | Where-Object {!$_.PSIsContainer}}        
            }
            else
            {
                $scriptCmd = {& $wrappedCmd @PSBoundParameters }
            }      
            
            $steppablePipeline = $scriptCmd.GetSteppablePipeline($myInvocation.CommandOrigin)
            $steppablePipeline.Begin($PSCmdlet)
        } 
        catch 
        {
            throw
        }
    }

    process
    {
        try 
        {
            $steppablePipeline.Process($_)
        } 
        catch 
        {
            throw
        }
    }

    end
    {
        try 
        {
            $steppablePipeline.End()
        } 
        catch 
        {
            throw
        }
    }
}