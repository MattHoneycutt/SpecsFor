using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SpecsFor.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.Autofac.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.StructureMap.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.Should.Tests")]
[assembly: InternalsVisibleTo("SpecsFor.Shouldly.Tests")]
namespace SpecsFor.Core.Validation
{
	internal interface ISpecValidator
	{
		void ValidateSpec(ISpecs spec);
	}
}
