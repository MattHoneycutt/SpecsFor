using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpecsFor.Helpers.Web.Mvc
{
	public static class RouteTableTestingExtensions
	{
		public static void MapMvcAttributeRoutesForTestingFromAssemblyContaining<TType>(this RouteCollection routeCollection)
		{
			var targetAssembly = typeof(TType).Assembly;

			var controllerTypes = (from type in targetAssembly.GetExportedTypes()
								   where
								   type != null && type.IsPublic
								   && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
								   && !type.IsAbstract && typeof(IController).IsAssignableFrom(type)
								   select type).ToList();

			var attributeRoutingAssembly = typeof(RouteCollectionAttributeRoutingExtensions).Assembly;
			var attributeRoutingMapperType =
			attributeRoutingAssembly.GetType("System.Web.Mvc.Routing.AttributeRoutingMapper");

			var mapAttributeRoutesMethod = attributeRoutingMapperType.GetMethod(
			"MapAttributeRoutes",
			BindingFlags.Public | BindingFlags.Static,
			null,
			new[] { typeof(RouteCollection), typeof(IEnumerable<Type>) },
			null);

			mapAttributeRoutesMethod.Invoke(null, new object[] { routeCollection, controllerTypes });
		}

	}


}