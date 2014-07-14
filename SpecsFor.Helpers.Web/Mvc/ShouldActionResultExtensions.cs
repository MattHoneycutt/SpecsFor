using System;
using System.Web.Mvc;

namespace SpecsFor.Helpers.Web.Mvc
{
	public static class ShouldActionResultExtensions
	{
		public static ViewResult ShouldRenderView(this ActionResult result)
		{
			throw new Exception();
		}

		public static PartialViewResult ShouldRenderPartialView(this ActionResult result)
		{
			throw new Exception();
		}

		public static ViewResult ShouldRenderDefaultView(this ActionResult result)
		{
			throw new Exception();
		}

		public static ViewResult ShouldRenderDefaultPartialView(this ActionResult result)
		{
			throw new Exception();
		}

		public static TModel WithModelType<TModel>(this ViewResult result)
		{
			throw new Exception();
		}

		public static JsonResult ShouldReturnJson(this ActionResult result)
		{
			throw new Exception();
		}

		public static TModel WithModelType<TModel>(this JsonResult result)
		{
			throw new Exception();
		}

		//TODO: Do we need to split this?  One that returns a RedirectToRouteResult, and one
		//		that returns a more generic RedirectResult?
		public static RedirectResult ShouldRedirect(this ActionResult result)
		{
			throw new Exception();
		}

		//Tests for HTTP  status results

		//Tests for actions that return a model error.  Probably would be better to return a test helper
		//object that we can chain asserts off of so that we don't lose the model type stuff.

	}

	//Need some way to initialize a controller with standard/typical mocks
	
	//Need some way to apply model state error to controller:
	//SUT.AddModelErrorFor<SomeForm>(x => x.Name, optionalMessage: "Required!")
}