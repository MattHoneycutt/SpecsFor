using NUnit.Framework;
using SpecsFor.StructureMap;

namespace SpecsFor.Shouldly.Tests.ShouldlyTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "BDD naming style")]
    public class EnumerableExtensionsSpecs
    {
        public class when_checking_for_ascending_lists : SpecsFor<object>
        {
            [Test]
            public void then_it_does_not_throw_for_ascending_lists()
            {
                Assert.DoesNotThrow(() => new[] { 1,2,3,4}.ShouldBeAscending());
            }

            [Test]
            public void then_it_throws_for_lists_that_are_not_ascending()
            {
                Assert.Throws<AssertionException>(() => new[] { 1,1,3,4}.ShouldBeAscending());
            }
        }

        public class when_checking_for_descending_lists : SpecsFor<object>
        {
            [Test]
            public void then_it_does_not_throw_for_descending_lists()
            {
                Assert.DoesNotThrow(() => new[] { 4,3,2,1 }.ShouldBeDescending());
            }

            [Test]
            public void then_it_throws_for_lists_that_are_not_descending()
            {
                Assert.Throws<AssertionException>(() => new[] { 4,4,3,2}.ShouldBeAscending());
            }
        }
    }
}
