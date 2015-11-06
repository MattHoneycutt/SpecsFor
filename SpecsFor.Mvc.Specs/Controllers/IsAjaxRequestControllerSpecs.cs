using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Helpers.Web.Mvc;
using SpecsFor.Mvc.Demo.Controllers;

namespace SpecsFor.Mvc.Specs.Controllers
{
	public class IsAjaxRequestControllerSpecs
	{
		public class when_posting_to_index_normally : SpecsFor<IsAjaxRequestController>
		{
			private ActionResult _result;

			protected override void Given()
			{
				this.UseFakeContextForController();
			}

			protected override void When()
			{
				_result = SUT.Index(It.IsAny<string>());
			}
			
			[Test]
			public void then_result_is_ViewResult()
			{
				_result.ShouldBeType(typeof (ViewResult));
			}
		}

		public class when_posting_to_index_through_ajax : SpecsFor<IsAjaxRequestController>
		{
			private ActionResult _result;

			protected override void Given()
			{
				this.UseFakeContextForController();
				this.FakeAjaxRequest();
			}

			protected override void When()
			{
				_result = SUT.Index(It.IsAny<string>());
			}

			[Test]
			public void then_result_is_JsonResult()
			{
				_result.ShouldBeType(typeof(JsonResult));
			}
		}
	}
}
