using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpContext : HttpContextBase
	{
		private readonly SessionStateItemCollection _sessionItems;
		private readonly FakeHttpRequest _request;
		private readonly HttpServerUtilityBase _server;
		private readonly HttpResponseBase _response;
		private readonly IHttpContextBehavior _behavior;
		private readonly IDictionary _items;

		public override HttpRequestBase Request
		{
			get { return _request; }
		}

		public override HttpResponseBase Response
		{
			get { return _response; }
		}

		public override IPrincipal User { get; set; }

		public override HttpServerUtilityBase Server
		{
			get { return _server; }
		}

		public override HttpSessionStateBase Session
		{
			get { return new FakeHttpSessionState(_sessionItems); }
		}

		public override bool IsDebuggingEnabled
		{
			get { return _behavior.IsDebuggingEnabled; }
		}

		public override IDictionary Items
		{
			get { return _items; }
		}

		public FakeHttpContext(
			IPrincipal principal,
			IFormParamsProvider formParams,
			IQueryStringParamsProvider queryStringParams,
			ICookieProvider cookies,
			SessionStateItemCollection sessionItems,
			HttpServerUtilityBase server,
			FakeHttpRequest request,
			IHttpContextBehavior contextBehavior)
		{
			User = principal;
			_sessionItems = sessionItems ?? new SessionStateItemCollection();
			_request = request ?? new FakeHttpRequest(formParams, queryStringParams, cookies);
			_request.SetIsAuthenticated(User.Identity != null ? User.Identity.IsAuthenticated : false);
			_server = server ?? new Mock<HttpServerUtilityBase>().Object;
			
			var httpResponse = new Mock<HttpResponseBase>();
			httpResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
			_response = httpResponse.Object;
			
			_behavior = contextBehavior;

			_items = new Hashtable();
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
			(ICookieProvider)null,
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