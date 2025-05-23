using System;

namespace SpecsFor.Core.Configuration.Model
{
    internal interface IConditionalBehavior
    {
        bool CanBeAppliedTo(Type targetType);
        void ApplyGivenTo(object specs);
        void ApplyAfterGivenTo(object specs);
        void ApplyAfterSpecTo(object specs);
        void ApplySpecInitTo(object specs);
        void ApplyAfterTestTo(object specs);
        void ApplyBeforeTestTo(object specs);
        void ApplyAfterClassUnderTestInitializedTo(object specs);
    }
}