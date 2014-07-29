using System;
using System.Collections.Specialized;
using System.Web;
using Moq;

namespace SpecsFor.Helpers.Web.Mvc
{
	public class FakeHttpRequest : HttpRequestBase
	{
		private readonly NameValueCollection _formParams;

		private readonly NameValueCollection _queryStringParams;

		private readonly HttpCookieCollection _cookies;

		private Uri _url;

		private string _method;

		private bool _isAuthenticated;

		private string _pathInfo;

		private Mock<HttpBrowserCapabilitiesBase> _browser;

		public FakeHttpRequest(string method)
			: this(null, null, null)
		{
			_method = method;
		}

		public FakeHttpRequest(NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies)
		{
			_browser = new Mock<HttpBrowserCapabilitiesBase>();
			_formParams = formParams;
			_queryStringParams = queryStringParams;
			_cookies = cookies;
		}

		public override HttpBrowserCapabilitiesBase Browser
		{
			get
			{
				return _browser.Object;
			}
		}

		public void SetBrowser(string name, string version)
		{
			_browser.SetupGet(b => b.Browser).Returns(name);
			_browser.SetupGet(b => b.Version).Returns(version);
		}

		public override NameValueCollection Form
		{
			get
			{
				return _formParams;
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _queryStringParams;
			}
		}

		public override HttpCookieCollection Cookies
		{
			get
			{
				return _cookies;
			}
		}

		public override Uri Url
		{
			get
			{
				return _url;
			}
		}

		public override string ApplicationPath
		{
			get
			{
				return "/";
			}
		}

		public void SetUrl(string url)
		{
			if (url.StartsWith("~/"))
			{
				//Grab path info.
				_pathInfo = url.Remove(0, 2);
				url = "http://www.example.com" + url.Remove(0, 1);
			}
			else if (url.StartsWith("/"))
			{
				//Grab path info
				_pathInfo = url;
				url = "http://www.example.com" + url;
			}

			_url = new Uri(url);
		}

		public override string HttpMethod
		{
			get
			{
				return _method ?? base.HttpMethod;
			}
		}

		public void SetHttpMethod(string method)
		{
			_method = method;
		}

		public override bool IsAuthenticated
		{
			get
			{
				return _isAuthenticated;
			}
		}


		public void SetIsAuthenticated(bool isAuthenticated)
		{
			_isAuthenticated = isAuthenticated;
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
				return _pathInfo ?? "";
			}
		}
	}
}