using System;

namespace SpecsFor.Core.Configuration.Model
{
	internal interface IBehaviorStack
	{
		void ApplySpecInitTo(ISpecs specs);
		void ApplyAfterClassUnderTestInitializedTo(ISpecs specs);
		void ApplyGivenTo(ISpecs specs);
		void ApplyAfterGivenTo(ISpecs specs);
		void ApplyAfterSpecTo(ISpecs specs);
		void ApplyAfterTestTo(ISpecs specs);
		void ApplyBeforeTestTo(ISpecs specs);
		Func<object> GetInitializationMethodFor(ISpecs specs);
	}
}