using StructureMap.AutoMocking;

namespace SpecsFor.StructureMap
{
    public class MoqAutoMocker<T> : AutoMocker<T> where T : class
    {
        public MoqAutoMocker() : base(new MoqServiceLocator())
        {
            ServiceLocator = new MoqServiceLocator();
        }
    }
}