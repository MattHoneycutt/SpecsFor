using System;
using System.Collections.Specialized;
using System.Web;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpRequest : HttpRequestBase
	{
		private Uri _url;
		private string _method;
		private bool _isAuthenticated;
		private string _pathInfo;
		private Mock<HttpBrowserCapabilitiesBase> _browser;
		private readonly IFormParamsProvider _formParams;
		private readonly IQueryStringParamsProvider _queryStringParams;
		private readonly ICookieProvider _cookies;

		public override HttpBrowserCapabilitiesBase Browser
		{
			get
			{
				return this._browser.Object;
			}
		}

		public override NameValueCollection Form
		{
			get
			{
				return _formParams.Values ?? new NameValueCollection();
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _queryStringParams.Values ?? new NameValueCollection();
			}
		}

		public override HttpCookieCollection Cookies
		{
			get
			{
				return _cookies.Cookies ?? new HttpCookieCollection();
			}
		}

		public override Uri Url
		{
			get
			{
				return this._url;
			}
		}

		public override string ApplicationPath
		{
			get
			{
				return "/";
			}
		}

		public override string HttpMethod
		{
			get
			{
				return this._method ?? base.HttpMethod;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._isAuthenticated;
			}
		}

		public override string AppRelativeCurrentExecutionFilePath
		{
			get
			{
				return "~/";
			}
		}

		public override string PathInfo
		{
			get
			{
				return this._pathInfo ?? "";
			}
		}

		public FakeHttpRequest(IFormParamsProvider formParams = null, IQueryStringParamsProvider queryStringParams = null, ICookieProvider cookies = null)
		{
			_browser = new Mock<HttpBrowserCapabilitiesBase>();
			_formParams = formParams ?? new EmptyFormsParamProvider();
			_queryStringParams = queryStringParams ?? new EmptyQueryStringParamProvider();
			_cookies = cookies ?? new EmptyCookieProvider();
		}

		public void SetBrowser(string name, string version)
		{
			_browser.SetupGet(b => b.Browser).Returns(name);
			_browser.SetupGet(b => b.Version).Returns(version);
		}

		public void SetUrl(string url)
		{
			if (url.StartsWith("~/"))
			{
				_pathInfo = url.Remove(0, 2);
				url = "http://www.example.com" + url.Remove(0, 1);
			}
			else if (url.StartsWith("/"))
			{
				this._pathInfo = url;
				url = "http://www.example.com" + url;
			}
			this._url = new Uri(url);
		}

		public void SetHttpMethod(string method)
		{
			this._method = method;
		}

		public void SetIsAuthenticated(bool isAuthenticated)
		{
			this._isAuthenticated = isAuthenticated;
		}
	}
}