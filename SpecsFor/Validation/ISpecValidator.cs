using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SpecsFor.Tests")]
namespace SpecsFor.Validation
{
	internal interface ISpecValidator
	{
		void ValidateSpec(ISpecs spec);
	}
}