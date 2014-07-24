using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Should;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Helpers.Web.Mvc
{
	public static class ShouldActionResultExtensions
	{
		public static ViewResult ShouldRenderView(this ActionResult result)
		{
			return result.ShouldBeType<ViewResult>();
		}

		public static PartialViewResult ShouldRenderPartialView(this ActionResult result)
		{
			return result.ShouldBeType<PartialViewResult>();
		}

		public static ViewResult ShouldRenderDefaultView(this ActionResult result)
		{
			var viewResult = result.ShouldBeType<ViewResult>();
			viewResult.ViewName.ShouldBeEmpty();
			return viewResult;
		}

		public static PartialViewResult ShouldRenderDefaultPartialView(this ActionResult result)
		{
			var viewResult = result.ShouldBeType<PartialViewResult>();
			viewResult.ViewName.ShouldBeNull();
			return viewResult;
		}

		public static TModel WithModelType<TModel>(this ViewResult result)
		{
			return result.Model.ShouldBeType<TModel>();
		}

		public static void WithModelLike<TModel>(this ViewResult result, TModel model)
		{
			result.Model.ShouldBeType<TModel>()
				.ShouldLookLike(model);
		}

		public static JsonResult ShouldReturnJson(this ActionResult result)
		{
			return result.ShouldBeType<JsonResult>();
		}

		public static TModel WithModelType<TModel>(this JsonResult result)
		{
			return result.Data.ShouldBeType<TModel>();
		}

		public static void ShouldRedirectTo<TController>(this ActionResult result, Expression<Action<TController>> action) where TController : Controller
		{
			var routeResult = result.ShouldBeType<RedirectToRouteResult>();

			var routeData = new RouteData();
			
			foreach (var kvp in routeResult.RouteValues)
			{
				routeData.Values.Add(kvp.Key, kvp.Value);
			}

			routeData.ShouldMapTo(action);
		}

		//TODO: Tests for HTTP  status results

		//TODO: Tests for actions that return a model error.  Probably would be better to return a test helper
		//object that we can chain asserts off of so that we don't lose the model type stuff.

	}

	//TODO: Need some way to initialize a controller with standard/typical mocks
	
	//Need some way to apply model state error to controller:
	//SUT.AddModelErrorFor<SomeForm>(x => x.Name, optionalMessage: "Required!")
}