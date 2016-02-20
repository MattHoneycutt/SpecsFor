using System.Transactions;
using SpecsFor;
using SpecsFor.Configuration;

namespace Conventions.Basics
{
    public interface INeedATransaction : ISpecs
    {
    }

    public class TransactionScopeWrapperBehavior : Behavior<INeedATransaction>
    {
        private TransactionScope _scope;

        public override void SpecInit(INeedATransaction instance)
        {
            _scope = new TransactionScope();
        }

        public override void AfterSpec(INeedATransaction instance)
        {
            _scope.Dispose();
        }
    }
}