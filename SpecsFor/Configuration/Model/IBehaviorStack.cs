using System;

namespace SpecsFor.Core.Configuration.Model
{
	internal interface IBehaviorStack
	{
		void ApplySpecInitTo(ISpecs specs);
		void ApplyAfterClassUnderTestInitializedTo(ISpecs specs);
		void ApplyGivenTo(ISpecs specs);
		void ApplyAfterSpecTo(ISpecs specs);
		Func<object> GetInitializationMethodFor(ISpecs specs);
	}
}