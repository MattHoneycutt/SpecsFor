using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace SpecsFor.Mvc.Helpers
{
	public class FakeHttpContext : HttpContextBase
	{
		private readonly HttpCookieCollection _cookies;
		private readonly NameValueCollection _formParams;
		private IPrincipal _principal;
		private readonly NameValueCollection _queryStringParams;
		private readonly string _relativeUrl;
		private readonly string _method;
		private readonly SessionStateItemCollection _sessionItems;
		private HttpResponseBase _response;
		private HttpRequestBase _request;
		private readonly Dictionary<object, object> _items;

		public override HttpRequestBase Request
		{
			get
			{
				return this._request ?? (HttpRequestBase)new FakeHttpRequest(this._relativeUrl, this._method, this._formParams, this._queryStringParams, this._cookies);
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				return this._response ?? (HttpResponseBase)new FakeHttpResponse();
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._principal;
			}
			set
			{
				this._principal = value;
			}
		}

		public override HttpSessionStateBase Session
		{
			get
			{
				return (HttpSessionStateBase)new FakeHttpSessionState(this._sessionItems);
			}
		}

		public override IDictionary Items
		{
			get
			{
				return (IDictionary)this._items;
			}
		}

		public override bool SkipAuthorization { get; set; }

		public FakeHttpContext(string relativeUrl, string method)
			: this(relativeUrl, method, (IPrincipal)null, (NameValueCollection)null, (NameValueCollection)null, (HttpCookieCollection)null, (SessionStateItemCollection)null)
		{
		}

		public FakeHttpContext(string relativeUrl)
			: this(relativeUrl, (IPrincipal)null, (NameValueCollection)null, (NameValueCollection)null, (HttpCookieCollection)null, (SessionStateItemCollection)null)
		{
		}

		public FakeHttpContext(string relativeUrl, IPrincipal principal, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies, SessionStateItemCollection sessionItems)
			: this(relativeUrl, (string)null, principal, formParams, queryStringParams, cookies, sessionItems)
		{
		}

		public FakeHttpContext(string relativeUrl, string method, IPrincipal principal, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies, SessionStateItemCollection sessionItems)
		{
			this._relativeUrl = relativeUrl;
			this._method = method;
			this._principal = principal;
			this._formParams = formParams;
			this._queryStringParams = queryStringParams;
			this._cookies = cookies;
			this._sessionItems = sessionItems;
			this._items = new Dictionary<object, object>();
		}

		public static FakeHttpContext Root()
		{
			return new FakeHttpContext("~/");
		}

		public void SetRequest(HttpRequestBase request)
		{
			this._request = request;
		}

		public void SetResponse(HttpResponseBase response)
		{
			this._response = response;
		}

		public override object GetService(Type serviceType)
		{
			return (object)null;
		}
	}
}