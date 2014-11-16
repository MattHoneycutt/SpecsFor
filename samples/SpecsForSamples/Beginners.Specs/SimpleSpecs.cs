using NUnit.Framework;
using Should;
using SpecsFor;

namespace Beginners
{
	public class SimpleSpecs
	{
		public class WidgetFactory
		{
			public Widget CreateWidget(string color, int size)
			{
				return new Widget
				{
					Color = color,
					Size = size
				};
			}
		}

		public class Widget
		{
			public string Color { get; set; }
			public int Size { get; set; }
		}

		public class when_creating_a_widget : SpecsFor<WidgetFactory>
		{
			private Widget _widget;

			protected override void When()
			{
				_widget = SUT.CreateWidget(color: "Red", size: 500);
			}

			[Test]
			public void then_it_sets_the_color()
			{
				_widget.Color.ShouldEqual("Red");
			}

			[Test]
			public void then_it_sets_the_size()
			{
				_widget.Size.ShouldEqual(500);
			}
		}
	}
}