using System;
using System.Linq;
using NUnit.Framework;

namespace SpecsFor
{
	public class GivenAttribute : TestFixtureAttribute
	{
		public GivenAttribute(params Type[] contextTypes)
			: base(new[] { contextTypes })
		{
			var badTypes = from type in contextTypes
			               where !type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContext<>))
			               select type;

			if (badTypes.Any())
			{
				throw new InvalidContextException(badTypes.ToArray());
			}
		}
	}
}