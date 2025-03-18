using Lamar;
using SpecsFor.Core;

namespace SpecsFor.Lamar;

public class SpecsFor<T> : SpecsForBase<T> where T : class
{
    public virtual void ConfigureContainer(Container container)
    {
    }

    protected override IAutoMocker CreateAutoMocker()
    {
        return new LamarAutoMocker<T>(this);
    }
}