using System;
using System.IO;

namespace SpecsFor.Mvc
{
	public static class Project
	{
		public static string Named(string projectName)
		{
			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			while (directory.GetFiles("*.sln").Length == 0)
			{
				if (directory.Parent == null)
				{
					var errorMessage = string.Format("Unable to find solution file, traversed up to '{0}'.  Your test runner may be using shadow-copy to create a clone of your working directory.  The Project.Named method does not currently support this behavior.  You must manually specify the path to the project to be tested instead.", directory.FullName);

					throw new InvalidOperationException(errorMessage);
				}

				directory = directory.Parent;

			}

			return Path.Combine(directory.FullName, projectName);
		}
	}
}