using Autofac;
using Autofac.Extras.Moq;

namespace SpecsFor.Autofac
{
    public class SpecsFor<T> : SpecsForBase<T> where T : class
    {
        /// <summary>
        /// Override this in your specs if you need to provide additional configuration, such as
        /// supplying a concrete implmentation of an interface.
        /// </summary>
        /// <param name="mocker"></param>
        public virtual void ConfigureMocker(AutoMock mocker)
        {

        }

        /// <summary>
        /// Override this in your specs if you want to use a different setting of Autofac's AutoMocker.
        /// The default is AutoMock.GetLoose(). You can also use AutoMock.GetStrict(), or provide your own repository with
        /// AutoMock.GetFromRepository(myRepository);
        /// </summary>
        /// <returns></returns>
        public virtual AutoMock CreateMocker()
        {
            return AutoMock.GetLoose();
        }

        /// <summary>
        /// Do not override this unless you really know what you're doing. If you're trying to adjust Moq behavior,
        /// override CreateMocker() instead.
        /// </summary>
        /// <returns></returns>
        protected override IAutoMocker CreateAutoMocker()
        {
            return new AutofacAutoMocker();
        }
    }
}
