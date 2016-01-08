using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using JetBrains.Annotations;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpContext : HttpContextBase
	{
		private readonly SessionStateItemCollection _sessionItems;
		private readonly FakeHttpRequest _request;
	    private readonly IHttpContextBehavior _behavior;

	    public override HttpRequestBase Request => _request;

	    public override HttpResponseBase Response { get; }

	    public override IPrincipal User { get; set; }

		public override HttpServerUtilityBase Server { get; }

	    public override HttpSessionStateBase Session => new FakeHttpSessionState(_sessionItems);

	    public override bool IsDebuggingEnabled => _behavior.IsDebuggingEnabled;

	    public override IDictionary Items { get; }

	    public FakeHttpContext(
			IPrincipal principal,
			IFormParamsProvider formParams,
			IQueryStringParamsProvider queryStringParams,
			ICookieProvider cookies,
			IServerVariablesParamsProvider serverVariablesParams,
			IHeadersParamsProvider headersParams,
			SessionStateItemCollection sessionItems,
			HttpServerUtilityBase server,
			FakeHttpRequest request,
			IHttpContextBehavior contextBehavior)
		{
			User = principal;
			_sessionItems = sessionItems ?? new SessionStateItemCollection();
			_request = request ?? new FakeHttpRequest(formParams, queryStringParams, cookies, serverVariablesParams, headersParams);
			_request.SetIsAuthenticated(User?.Identity?.IsAuthenticated ?? false);
			Server = server ?? new Mock<HttpServerUtilityBase>().Object;
			
			var httpResponse = new Mock<HttpResponseBase>();
			httpResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
			Response = httpResponse.Object;
			
			_behavior = contextBehavior;

			Items = new Hashtable();
		}

	    [UsedImplicitly]
	    public FakeHttpContext(string url)
			: this()
		{
			this._request.SetUrl(url);
		}

		public FakeHttpContext(FakeHttpRequest request)
			: this()
		{
			this._request = request;
		}

		public FakeHttpContext()
			: this(
			(IPrincipal)null,
			(IFormParamsProvider)null,
			(IQueryStringParamsProvider)null,
			(ICookieProvider)null,
			(IServerVariablesParamsProvider)null,
			(IHeadersParamsProvider)null,
			(SessionStateItemCollection)null,
			(HttpServerUtilityBase)null,
			(FakeHttpRequest)null,
			(IHttpContextBehavior)null)
		{
		}

		public override object GetService(Type service)
		{
			object obj = null;

			if (service == typeof(HttpRequest))
				obj = Request;
			else if (service == typeof(HttpResponse))
				obj = Response;
			else if (service == typeof(HttpApplication))
				obj = ApplicationInstance;
			else if (service == typeof(HttpApplicationState))
				obj = Application;
			else if (service == typeof(HttpSessionState))
				obj = Session;
			else if (service == typeof(HttpServerUtility))
				obj = Server;

			return obj;

		}
	}
}