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
				directory = directory.Parent;
			}

			return Path.Combine(directory.FullName, projectName);
		}
	}
}