using System.Collections.Generic;

namespace SpecsFor.Tests.TestObjects
{
	public class ConsumeEnumerableService
	{
		public ConsumeEnumerableService(IEnumerable<IWidget> widgets)
		{
			Widgets = widgets;
		}

		public IEnumerable<IWidget> Widgets { get; set; }
	}
}