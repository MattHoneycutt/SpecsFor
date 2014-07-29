using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpContext : HttpContextBase
	{
		private FakePrincipal _principal;

		private readonly NameValueCollection _formParams;

		private readonly NameValueCollection _queryStringParams;

		private readonly HttpCookieCollection _cookies;

		private readonly SessionStateItemCollection _sessionItems;

		private FakeHttpRequest _request;

		private HttpServerUtilityBase _server;

		private HttpResponseBase _response;

		public FakeHttpContext(FakePrincipal principal, NameValueCollection formParams,
			NameValueCollection queryStringParams, HttpCookieCollection cookies,
			SessionStateItemCollection sessionItems)
		{
			_principal = principal ?? new FakePrincipal(new FakeIdentity(null), null);
			_formParams = formParams ?? new NameValueCollection();
			_queryStringParams = queryStringParams ?? new NameValueCollection();
			_cookies = cookies ?? new HttpCookieCollection();
			_sessionItems = sessionItems ?? new SessionStateItemCollection();

			_request = new FakeHttpRequest(_formParams, _queryStringParams, _cookies);
			_request.SetIsAuthenticated(_principal.Identity.IsAuthenticated);

			_server = new Mock<HttpServerUtilityBase>().Object;

			var responseMock = new Mock<HttpResponseBase>();
			//NOTE: Without this, URL generation will *not* work. 
			responseMock.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>()))
				.Returns<string>(s => s);
			_response = responseMock.Object;
		}

		public FakeHttpContext(string url)
			: this()
		{
			_request.SetUrl(url);
		}

		public FakeHttpContext(FakeHttpRequest request)
			: this()
		{
			_request = request;
		}

		public FakeHttpContext()
			: this(null, null, null, null, null)
		{
		}

		public override HttpRequestBase Request
		{
			get
			{
				return _request;
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				return _response;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return _principal;
			}
			set
			{
				if (value is FakePrincipal)
				{
					_principal = (FakePrincipal)value;
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		public override HttpServerUtilityBase Server
		{
			get
			{
				return _server;
			}
		}

		public override HttpSessionStateBase Session
		{
			get
			{
				return new FakeHttpSessionState(_sessionItems);
			}
		}
	}
}