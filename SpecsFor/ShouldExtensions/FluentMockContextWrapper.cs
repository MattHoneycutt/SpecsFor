using System;
using System.Linq;
using System.Reflection;
using Moq;

namespace SpecsFor.ShouldExtensions
{
	public class FluentMockContextWrapper
	{
		private static readonly Type MockContextType;
		private static readonly PropertyInfo GetCurrentProp;
		private static readonly MethodInfo MatchesMethod;
		private static readonly PropertyInfo LastMatchProp;

		static FluentMockContextWrapper()
		{
			MockContextType = typeof(Mock).Assembly.GetTypes().Single(x => x.Name == "FluentMockContext");
			GetCurrentProp = MockContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
			LastMatchProp = MockContextType.GetProperty("LastMatch", BindingFlags.Public | BindingFlags.Instance);
			MatchesMethod = typeof(Match).GetMethod("Matches", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public FluentMockContextWrapper()
		{
			
			Activator.CreateInstance(MockContextType);
		}

		public bool LastMatcherMatches(object actualValue)
		{

			var currentContext = GetCurrentProp.GetValue(null, null);

			var match = (Match)LastMatchProp.GetValue(currentContext, null);

			return (bool)MatchesMethod.Invoke(match, new[] { actualValue });
		}
	}
}