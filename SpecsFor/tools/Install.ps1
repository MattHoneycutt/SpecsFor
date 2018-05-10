param($rootPath, $toolsPath, $package, $project)

# In NuGet 1.6+, this command will automatically add any necessary binding redirects, even to a class libary project. 
Add-BindingRedirect $project.Name

$DTE.ItemOperations.Navigate("http://specsfor.com/installed.cshtml")