using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SpecsFor.Core.Configuration;

namespace SpecsFor.Core.Validation
{
	public class NUnitSpecValidator : ISpecValidator
	{
		//Used to cache validation results for an assembly. 
		private static readonly ConcurrentDictionary<string, bool> _validationResults = new ConcurrentDictionary<string, bool>();

		public void ValidateSpec(ISpecs spec)
		{
			//NOTE: Other validations may be performed in the future.  For now,
			//		just verify that everything in the spec's assembly that derives
			//		from SpecsForConfiguration has the correct attribute.
			var assembly = spec.GetType().Assembly;

			_validationResults.GetOrAdd(assembly.FullName, _ => ValidateAssembly(assembly));
		}

		private bool ValidateAssembly(Assembly assembly)
		{
			var configClassesWithoutSetupFixtureAttribute = (from t in assembly.GetTypes()
			                                                where typeof (SpecsForConfiguration).IsAssignableFrom(t) &&
			                                                      !t.IsAbstract &&
			                                                      Attribute.GetCustomAttribute(t, typeof (SetUpFixtureAttribute)) ==
			                                                      null
			                                                select t).ToArray();

			if (configClassesWithoutSetupFixtureAttribute.Any())
			{
				var message = "Found one or more SpecsForConfiguration types that do not have a SetUpFixtureAttribute. " +
					"You must apply the attribute to each type or make the type abstract.  " + 
					"Offending type(s): \r\n" + string.Join("\r\n", configClassesWithoutSetupFixtureAttribute.Select(t => t.FullName));
				throw new InvalidOperationException(message);
			}

			return true;
		}
	}
}