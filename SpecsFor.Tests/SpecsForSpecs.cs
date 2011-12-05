using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Should;
using System.Linq;
using SpecsFor.Tests.TestObjects;

namespace SpecsFor.Tests
{
	public class SpecsForSpecs
	{
		public class when_running_specs_with_no_given_and_no_when : SpecsFor<object>
		{
			[Test]
			public void then_the_SUT_is_still_initialized()
			{
				SUT.ShouldNotBeNull();
			}
		}

		public class when_running_specs_with_multiple_thens : SpecsFor<object>
		{
			private static int _whenCount;
			private static int _givenCount;

			protected override void Given()
			{
				_givenCount++;
			}

			protected override void When()
			{
				_whenCount++;
			}

			[Test]
			public void then_the_when_should_only_be_executed_once()
			{
				_whenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_when_should_still_only_be_executed_once()
			{
				_whenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_given_should_only_be_executed_once()
			{
				_givenCount.ShouldEqual(1);
			}

			[Test]
			public void then_the_given_should_still_only_be_executed_once()
			{
				_givenCount.ShouldEqual(1);
			}
		}
		
		public class when_tearing_down_after_a_test_given_SUT_implements_IDisposable : SpecsFor<IDisposable>
		{
			protected override void InitializeClassUnderTest()
			{
				SUT = GetMockFor<IDisposable>().Object;
			}

			protected override void When()
			{
				TearDown();
			}

			[Test]
			public void then_it_should_call_Dispose()
			{
				GetMockFor<IDisposable>()
					.Verify(d => d.Dispose());
			}
		}

		public class when_injecting_into_a_service_that_depends_on_IEnumerable : SpecsFor<ConsumeEnumerableService>
		{
			private IEnumerable<IWidget> _testWidgets;

			protected override void ConfigureContainer(StructureMap.IContainer container)
			{
				base.ConfigureContainer(container);

				container.Inject(typeof (string), "blah");

				var mocks = GetMockForEnumerableOf<IWidget>(10);

				var widgets = new IWidget[10];

				for (var i = 0; i < mocks.Length; i++)
				{
					var widget = mocks[i];
					widget.Setup(w => w.Name).Returns("Widget " + i);

					widgets[i] = widget.Object;
				}

				_testWidgets = widgets;
			}

			[Test]
			public void then_it_provides_the_enumerable()
			{
				SUT.Widgets.Count().ShouldEqual(_testWidgets.Count());
			}

			[Test]
			public void then_expectations_set_up_on_the_mocks_are_preserved()
			{
				SUT.Widgets.All(w => w.Name.StartsWith("Widget ")).ShouldBeTrue();
			}
	
			[Test]
			public void then_I_can_retrieve_the_equivalent_mock_enumerable()
			{
				var mocks = GetMockForEnumerableOf<IWidget>(10);

				var injectedNames = SUT.Widgets.Select(w => w.Name);
				var mockNames = mocks.Select(m => m.Object.Name);

				mockNames.ShouldEqual(injectedNames);
			}
		}
		
		public class when_requesting_a_mock_IEnumerable_of_a_different_size_than_originally_requested: SpecsFor<ConsumeEnumerableService>
		{
			private InvalidOperationException _exception;

			protected override void ConfigureContainer(StructureMap.IContainer container)
			{
				GetMockForEnumerableOf<IWidget>(10);
			}

			protected override void When()
			{
				_exception = Assert.Throws<InvalidOperationException>(() => GetMockForEnumerableOf<IWidget>(5));
			}

			[Test]
			public void then_it_throws_an_exception()
			{
				_exception.ShouldNotBeNull();
			}
		}
	}
}