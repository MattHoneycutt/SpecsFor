using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpContext : HttpContextBase
	{
		private readonly IFormParamsProvider _formParams;
		private readonly IQueryStringParamsProvider _queryStringParams;
		private readonly HttpCookieCollection _cookies;
		private readonly SessionStateItemCollection _sessionItems;
		private FakeHttpRequest _request;
		private HttpServerUtilityBase _server;
		private HttpResponseBase _response;
		private readonly IHttpContextBehavior _behavior;

		public override HttpRequestBase Request
		{
			get
			{
				return (HttpRequestBase)this._request;
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				return this._response;
			}
		}

		public override IPrincipal User { get; set; }

		public override HttpServerUtilityBase Server
		{
			get
			{
				return this._server;
			}
		}

		public override HttpSessionStateBase Session
		{
			get
			{
				return (HttpSessionStateBase)new FakeHttpSessionState(this._sessionItems);
			}
		}

		public override bool IsDebuggingEnabled
		{
			get { return _behavior.IsDebuggingEnabled; }
		}

		public FakeHttpContext(
			IPrincipal principal,
			IFormParamsProvider formParams,
			IQueryStringParamsProvider queryStringParams,
			HttpCookieCollection cookies,
			SessionStateItemCollection sessionItems,
			HttpServerUtilityBase server,
			FakeHttpRequest request,
			IHttpContextBehavior contextBehavior)
		{
			User = principal;
			_formParams = formParams ?? new EmptyFormsParamProvider();
			_queryStringParams = queryStringParams ?? new EmptyQueryStringParamProvider();
			_cookies = cookies ?? new HttpCookieCollection();
			_sessionItems = sessionItems ?? new SessionStateItemCollection();
			_request = request ?? new FakeHttpRequest();
			_request.SetIsAuthenticated(User.Identity != null ? User.Identity.IsAuthenticated : false);
			_server = server ?? new Mock<HttpServerUtilityBase>().Object;
			var httpResponse = new Mock<HttpResponseBase>();
			httpResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
			this._response = httpResponse.Object;
			this._behavior = contextBehavior;
		}

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
			(HttpCookieCollection)null,
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