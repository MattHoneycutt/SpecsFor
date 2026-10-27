using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpecsFor.Core.Configuration.Model
{
	internal class BehaviorStack : IBehaviorStack
	{
		private static readonly ConcurrentDictionary<string, ConcurrentBag<SpecsForConfiguration>> ConfigurationsByNamespace = new ConcurrentDictionary<string, ConcurrentBag<SpecsForConfiguration>>();

		private static readonly ConcurrentHashSet<Type> ProcessedConfigurationTypes = new ConcurrentHashSet<Type>();

		public static BehaviorStack Current { get; } = new BehaviorStack();

		private BehaviorStack()
		{
			//Prevents the class from being instantiated from outside.
		}

		public static void RegisterConfiguration(SpecsForConfiguration config)
		{
			var configType = config.GetType();

			if (!ProcessedConfigurationTypes.Add(configType)) return;

			var configNamespace = configType.Namespace ?? string.Empty;

			ConfigurationsByNamespace.AddOrUpdate(configNamespace,
				new ConcurrentBag<SpecsForConfiguration> { config },
				(_, existing) =>
				{
					existing.Add(config);
					return existing;
				});
		}

		private IEnumerable<IConditionalBehavior> FindBehaviorsFor(object specs)
		{
			var specType = specs.GetType();
			var testNamespace = specType.Namespace ?? string.Empty;
			var namespaces = BuildNamespaceHierarchy(testNamespace);
			var behaviors = new List<IConditionalBehavior>();

			foreach (var ns in namespaces)
			{
				if (!ConfigurationsByNamespace.TryGetValue(ns, out var configs)) continue;

				foreach (var config in configs)
				{
					behaviors.AddRange(config.GetBehaviorsFor(specType));
				}
			}

			return behaviors;
		}

		private List<string> BuildNamespaceHierarchy(string testNamespace)
		{
			var hierarchy = new List<string>();

			if (string.IsNullOrEmpty(testNamespace))
			{
				hierarchy.Add(string.Empty);
				return hierarchy;
			}

			var parts = testNamespace.Split('.');
			var current = "";

			for (var i = 0; i < parts.Length; i++)
			{
				if (i == 0)
				{
					current = parts[0];
				}
				else
				{
					current = current + "." + parts[i];
				}

				hierarchy.Add(current);
			}

			return hierarchy;
		}

		public void ApplyGivenTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyGivenTo(specs);
			}
		}

		public void ApplyAfterGivenTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterGivenTo(specs);
			}
		}

		public void ApplyAfterSpecTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterSpecTo(specs);
			}
		}

		public void ApplyAfterTestTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterTestTo(specs);
			}
		}

		public void ApplyBeforeTestTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyBeforeTestTo(specs);
			}
		}

		public void ApplySpecInitTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplySpecInitTo(specs);
			}
		}

		public void ApplyAfterClassUnderTestInitializedTo(ISpecs specs)
		{
			var behaviors = FindBehaviorsFor(specs);

			foreach (var behavior in behaviors)
			{
				behavior.ApplyAfterClassUnderTestInitializedTo(specs);
			}
		}

		public Func<object> GetInitializationMethodFor(ISpecs specs)
		{
			var specType = specs.GetType();
			var testNamespace = specType.Namespace ?? string.Empty;

			foreach (var ns in BuildNamespaceHierarchy(testNamespace))
			{
				if (!ConfigurationsByNamespace.TryGetValue(ns, out var configs)) continue;

				foreach (var config in configs)
				{
					var initializer = config.GetInitializationMethodFor(specType);

					if (initializer != null) return initializer;
				}
			}

			return null;
		}
	}
}
