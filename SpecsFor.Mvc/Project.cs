using System;
using System.Diagnostics;
using System.IO;

namespace SpecsFor.Mvc
{
	public static class Project
	{
		public static string Named(string projectName)
		{
		    return DiscoverProject(projectName);
		}

	    private static string DiscoverProject(string projectName, string currentFolder = null)
        {
            currentFolder = currentFolder ?? Environment.CurrentDirectory;

            Debug.WriteLine($"Discovering path to project {projectName} for Specs4MVC tests. Starting in {currentFolder}");

            string projectPath = null;

            var directoryInfo = new DirectoryInfo(currentFolder);

            while (projectPath == null)
            {
                var solutionFiles = directoryInfo.GetFiles("*.sln", SearchOption.AllDirectories);

                foreach (var solutionFile in solutionFiles)
                {
                    Debug.WriteLine($"Found a possible solution file {solutionFile.FullName}");

                    projectPath = Path.Combine(solutionFile.Directory.FullName, projectName);

                    if (Directory.Exists(projectPath))
                    {
                        Debug.WriteLine($"Found suitable project path {projectPath}");
                        break;
                    }
                    else
                    {
                        Debug.WriteLine($"Assumed project path {projectPath} does not exist. Continuing search...");
                        projectPath = null;
                    }
                }

                if (projectPath != null) continue;

                if (directoryInfo.Parent == null)
                {
                    string message = $"Unable to find project file {projectName}. Started at {currentFolder}, Search all the way up to {directoryInfo.FullName}";
                    throw new InvalidOperationException(message);
                }

                directoryInfo = directoryInfo.Parent;
            }

            return projectPath;
        }

    }
}