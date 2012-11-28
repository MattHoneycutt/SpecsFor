using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace SpecsFor.Mvc.Helpers
{
	public class FakeHttpRequest : HttpRequestBase
	{
		private readonly HttpCookieCollection _cookies;
		private readonly NameValueCollection _formParams;
		private readonly NameValueCollection _queryStringParams;
		private readonly NameValueCollection _serverVariables;
		private readonly string _relativeUrl;
		private readonly Uri _url;
		private readonly Uri _urlReferrer;
		private readonly string _httpMethod;

		public override NameValueCollection ServerVariables
		{
			get
			{
				return this._serverVariables;
			}
		}

		public override NameValueCollection Form
		{
			get
			{
				return this._formParams;
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return this._queryStringParams;
			}
		}

		public override HttpCookieCollection Cookies
		{
			get
			{
				return this._cookies;
			}
		}

		public override string AppRelativeCurrentExecutionFilePath
		{
			get
			{
				return this._relativeUrl;
			}
		}

		public override Uri Url
		{
			get
			{
				return this._url;
			}
		}

		public override Uri UrlReferrer
		{
			get
			{
				return this._urlReferrer;
			}
		}

		public override string PathInfo
		{
			get
			{
				return string.Empty;
			}
		}

		public override string ApplicationPath
		{
			get
			{
				return "";
			}
		}

		public override string HttpMethod
		{
			get
			{
				return this._httpMethod;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return new NameValueCollection();
			}
		}

		public FakeHttpRequest(string relativeUrl, string method, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies)
		{
			this._httpMethod = method;
			this._relativeUrl = relativeUrl;
			this._formParams = formParams;
			this._queryStringParams = queryStringParams;
			this._cookies = cookies;
			this._serverVariables = new NameValueCollection();
		}

		public FakeHttpRequest(string relativeUrl, string method, Uri url, Uri urlReferrer, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies)
			: this(relativeUrl, method, formParams, queryStringParams, cookies)
		{
			this._url = url;
			this._urlReferrer = urlReferrer;
		}

		public FakeHttpRequest(string relativeUrl, Uri url, Uri urlReferrer)
			: this(relativeUrl, HttpVerbs.Get.ToString("g"), url, urlReferrer, (NameValueCollection)null, (NameValueCollection)null, (HttpCookieCollection)null)
		{
		}
	}
}