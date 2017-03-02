using System.Web.Mvc;
using Base.Controllers;
using NUnit.Framework;
using Should;
using SpecsFor;

namespace gb.specs.Controllers
{
    public class HomeControllerSpecs
    {
        public class when_viewing_index_action : SpecsFor<HomeController>
        {
            private ActionResult _result;

            protected override void When()
            {
                _result = SUT.Index();
            }
            [Test]
            public void then_it_returns_a_view_result()
            {
                _result.ShouldBeType<ActionResult>();
            }
        }
    }
}
