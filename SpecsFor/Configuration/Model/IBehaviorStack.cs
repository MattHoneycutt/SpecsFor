using System;

namespace SpecsFor.Configuration.Model
{
	internal interface IBehaviorStack
	{
		void ApplySpecInitTo(object specs);
		void ApplyAfterClassUnderTestInitializedTo(object specs);
		void ApplyGivenTo(object specs);
		void ApplyAfterSpecTo(object specs);
		Func<object> GetInitializationMethodFor(object specs);
	}
}