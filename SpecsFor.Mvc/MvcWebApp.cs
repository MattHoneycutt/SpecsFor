﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using OpenQA.Selenium;
using Microsoft.Web.Mvc;
using SpecsFor.Mvc.Authentication;
using SpecsFor.Mvc.Helpers;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace SpecsFor.Mvc
{
    //NOTE: MvcWebApp has definitely picked up too many responsibilites.  It's in need of 
    //		refactoring.  The project could probably benefit from a simple IoC container 
    //		or service locator for handling some of these things. 
    public class MvcWebApp
    {
        //TODO: Move to Service Locator class?  

        public static readonly IList<Action<MvcWebApp>> PreTestCallbacks = new List<Action<MvcWebApp>>();
        public static string BaseUrl { get; set; }
        public static BrowserDriver Driver { get; set; }
        public static IHandleAuthentication Authentication { get; set; }
        public static TimeSpan Delay { get; set; }

        public IWebDriver Browser { get; private set; }

        static MvcWebApp()
        {
            BaseUrl = "http://localhost";
            Driver = BrowserDriver.InternetExplorer;
        }

        public MvcWebApp()
        {
            Browser = Driver.GetDriver();

            try
            {
                foreach (var callback in PreTestCallbacks)
                {
                    callback(this);
                }

                if (Authentication != null)
                {
                    Authentication.Authenticate(this);
                }
            }
            //If something happens and the class can't be created, we still need to destroy the browser.
            catch (Exception)
            {
                Browser.Quit();
                Browser.Dispose();
                throw;
            }
        }

        public FluentForm<T> FindFormFor<T>()
        {
            return new FluentForm<T>(this);
        }

        public FluentDisplay<T> FindDisplayFor<T>()
        {
            return new FluentDisplay<T>(this);
        }

        public IWebElement ValidationSummary
        {
            get
            {
                return Browser.FindElement(By.ClassName("validation-summary-errors"));
            }
        }

        public RouteData Route
        {
            get
            {
                //Strip the host, port, etc. off the route.  The routing helpers
                //expect the URL to look like "~/virtual/path"
                var url = Browser.Url.Replace(BaseUrl, "~");

                var queryStringParams = new Dictionary<string, string>();

                //Parse out the query string params. 
                if (url.Contains("?"))
                {
                    var parts = url.Split('?');
                    url = parts[0];

                    queryStringParams = parts[1].Split('&')
                        .Select(v => v.Split('='))
                        .ToDictionary(a => a[0], a => a[1]);
                }

                var context = new FakeHttpContext(url, null, null, null, null, null);
                context.SetRequest(new FakeHttpRequest(url, new Uri(Browser.Url), null));

                var routeData = RouteTable.Routes.GetRouteData(context);

                //Add in query string params.  This will allow the ShouldMapTo extension method to work 
                //with query string parameters.
                foreach (var kvp in queryStringParams.Where(kvp => !routeData.Values.ContainsKey(kvp.Key)))
                {
                    routeData.Values.Add(kvp.Key, kvp.Value);
                }

                return routeData;
            }
        }

        public void NavigateTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());
            //TODO: workaround to fixes MattHoneycutt/SpecsFor#25
            var url = BuildUrlFromExpression(helper.ViewContext.RequestContext, helper.RouteCollection, action);

            Browser.Navigate().GoToUrl(BaseUrl + url);
        }

        public IWebElement FindLinkTo<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var helper = new HtmlHelper(new ViewContext { HttpContext = FakeHttpContext.Root() }, new FakeViewDataContainer());
            //TODO: workaround to fixes MattHoneycutt/SpecsFor#25
            var url = BuildUrlFromExpression(helper.ViewContext.RequestContext, helper.RouteCollection, action);
            var element = Browser.FindElement(By.CssSelector("a[href='" + url + "']"));

            return element;
        }

        internal void Pause()
        {
            if (Delay != default(TimeSpan))
            {
                Thread.Sleep(Delay);
            }
        }

        public static void AddPreTestCallback(Action action)
        {
            AddPreTestCallback(_ => action());
        }

        public static void AddPreTestCallback(Action<MvcWebApp> action)
        {
            PreTestCallbacks.Add(action);
        }

        public IWebElement FindElementByExpression<T, TProp>(Expression<Func<T, TProp>> property)
        {
            var name = ExpressionHelper.GetExpressionText(property);
            name = TagBuilder.CreateSanitizedId(name);

            var field = Browser.FindElement(By.Id(name));
            return field;
        }

        public string AllText()
        {
            return Browser.FindElement(By.TagName("body")).Text;
        }

        #region | workaround to fixes MattHoneycutt/SpecsFor#25 |
        private string BuildUrlFromExpression<TController>(RequestContext context, RouteCollection routeCollection, Expression<Action<TController>> action) where TController : Controller
        {
            RouteValueDictionary routeValues = GetRouteValuesFromExpression(action);
            VirtualPathData vpd = routeCollection.GetVirtualPathForArea(context, routeValues);
            return (vpd == null) ? null : vpd.VirtualPath;
        }

        private static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            MethodCallExpression call = action.Body as MethodCallExpression;
            if (call == null)
            {
                throw new ArgumentException("MvcResources.ExpressionHelper_MustBeMethodCall", "action");
            }

            string controllerName = typeof(TController).Name;
            if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("MvcResources.ExpressionHelper_TargetMustEndInController", "action");
            }
            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            if (controllerName.Length == 0)
            {
                throw new ArgumentException("MvcResources.ExpressionHelper_CannotRouteToController", "action");
            }

            // TODO: How do we know that this method is even web callable?
            //      For now, we just let the call itself throw an exception.

            string actionName = GetTargetActionName(call.Method);

            var rvd = new RouteValueDictionary();
            rvd.Add("Controller", controllerName);
            rvd.Add("Action", actionName);

            ActionLinkAreaAttribute areaAttr = typeof(TController).GetCustomAttributes(typeof(ActionLinkAreaAttribute), true /* inherit */).FirstOrDefault() as ActionLinkAreaAttribute;
            if (areaAttr != null)
            {
                string areaName = areaAttr.Area;
                rvd.Add("Area", areaName);
            }

            AddParameterValuesFromExpressionToDictionary(rvd, call);
            return rvd;
        }

        // This method contains some heuristics that will help determine the correct action name from a given MethodInfo
        // assuming the default sync / async invokers are in use. The logic's not foolproof, but it should be good enough
        // for most uses.
        private static string GetTargetActionName(MethodInfo methodInfo)
        {
            string methodName = methodInfo.Name;

            // do we know this not to be an action?
            if (methodInfo.IsDefined(typeof(NonActionAttribute), true /* inherit */))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "MvcResources.ExpressionHelper_CannotCallNonAction : {0}", methodName));
            }

            // has this been renamed?
            ActionNameAttribute nameAttr = methodInfo.GetCustomAttributes(typeof(ActionNameAttribute), true /* inherit */).OfType<ActionNameAttribute>().FirstOrDefault();
            if (nameAttr != null)
            {
                return nameAttr.Name;
            }

            // targeting an async action?
            if (methodInfo.DeclaringType.IsSubclassOf(typeof(AsyncController)))
            {
                if (methodName.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
                {
                    return methodName.Substring(0, methodName.Length - "Async".Length);
                }
                if (methodName.EndsWith("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        "MvcResources.ExpressionHelper_CannotCallCompletedMethod : {0}", methodName));
                }
            }

            // fallback
            return methodName;
        }

        private static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary rvd, MethodCallExpression call)
        {
            ParameterInfo[] parameters = call.Method.GetParameters();

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Expression arg = call.Arguments[i];
                    object value = null;
                    ConstantExpression ce = arg as ConstantExpression;
                    if (ce != null)
                    {
                        // If argument is a constant expression, just get the value
                        value = ce.Value;
                    }
                    else
                    {
                        value = CachedExpressionCompiler.Evaluate(arg);
                    }
                    rvd.Add(parameters[i].Name, value);
                }
            }
        }
        #endregion
    }
}