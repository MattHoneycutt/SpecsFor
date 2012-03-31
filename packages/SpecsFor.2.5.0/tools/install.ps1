param($rootPath, $toolsPath, $package, $project)

Write-Host 
Write-Host 

Write-Host "---------------------------READ ME---------------------------------------------------"
Write-Host 
Write-Host "Thanks for installing SpecsFor, the Behavior-Driven Development framework that is "
Write-Host "optimized for *your* productivity.  If there's a common testing pattern, utility, "
Write-Host "or library that you think should be bundled with SpecsFor, let me know at "
Write-Host "http://trycatchfail.com."
Write-Host
Write-Host "For more information about getting started, check out the examples at "
Write-Host "http://matthoneycutt.github.com/SpecsFor/"
Write-Host 
Write-Host "------------------------Known Issues------------------------------------------------"
Write-Host 
Write-Host "Note that there is a bug in NuGet 1.5 that prevents binding redirects from being "
Write-Host "added to class library projects.  You may receive an error message that looks like this: "
Write-Host 
Write-Host "System.IO.FileLoadException : Could not load file or assembly 'StructureMap.AutoMocking, "
Write-Host " Version=2.6.2.0, Culture=neutral, PublicKeyToken=e60ad81abae3c223' or one of its dependencies. "
Write-Host
Write-Host "If you receive this error, then you will need to add an App.config file to your test "
Write-Host "project with the appropriate binding redirects.  You can copy, paste, and customize "
Write-Host "This block of XML: "
Write-Host 
Write-Host @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="StructureMap.AutoMocking" publicKeyToken="e60ad81abae3c223" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.6.3.0" newVersion="2.6.3.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="StructureMap" publicKeyToken="e60ad81abae3c223" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.6.3.0" newVersion="2.6.3.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
"@
Write-Host
Write-Host -------------------------------------------------------------------------------------
# In NuGet 1.6, this command will automatically add any necessary binding redirects, even to a class libary project. 
Add-BindingRedirect $project.Name