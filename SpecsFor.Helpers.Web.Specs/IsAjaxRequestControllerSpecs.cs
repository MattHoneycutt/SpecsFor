using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Helpers.Web.Mvc;

namespace SpecsFor.Helpers.Web.Specs
{
	public class IsAjaxRequestControllerSpecs
	{
		public class IsAjaxRequestController : Controller
		{
			[HttpPost]
			public ActionResult Index(string name)
			{
				string message = string.Format("Hi {0}!", name);

				if (Request.IsAjaxRequest())
				{
					return Json(message);
				}

				ViewBag.Message = message;
				// ReSharper disable once Mvc.ViewNotResolved
				return View();
			}
		}

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
