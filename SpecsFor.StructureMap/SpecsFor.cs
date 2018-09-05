using SpecsFor.Core;
using StructureMap;

namespace SpecsFor.StructureMap
{
    public class SpecsFor<T> : SpecsForBase<T> where T : class
    {
        public virtual void ConfigureContainer(Container container)
        {
        }

        protected override IAutoMocker CreateAutoMocker()
        {
            return new StructureMapAutoMocker<T>();
        }
    }
}