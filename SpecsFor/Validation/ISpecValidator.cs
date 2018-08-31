﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SpecsFor.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.Autofac.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.StructureMap.Tests")]
namespace SpecsFor.Validation
{
	internal interface ISpecValidator
	{
		void ValidateSpec(ISpecs spec);
	}
}