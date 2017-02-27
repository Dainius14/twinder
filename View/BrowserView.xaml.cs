using CefSharp;
using System.Windows;
using System;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Twinder.View
{
	internal delegate void TokenRetrieved(object sender, string e);

	public partial class BrowserView : Window
	{
		private const string URL = @"https://www.facebook.com/v2.6/dialog/oauth?redirect_uri=fb464891386855067%3A%2F%2Fauthorize%2F&display=touch&state=%7B%22challenge%22%3A%22IUUkEUqIGud332lfu%252BMJhxL4Wlc%253D%22%2C%220_auth_logger_id%22%3A%2230F06532-A1B9-4B10-BB28-B29956C71AB1%22%2C%22com.facebook.sdk_client_state%22%3Atrue%2C%223_method%22%3A%22sfvc_auth%22%7D&scope=user_birthday%2Cuser_photos%2Cuser_education_history%2Cemail%2Cuser_relationship_details%2Cuser_friends%2Cuser_work_history%2Cuser_likes&response_type=token%2Csigned_request&default_audience=friends&return_scopes=true&auth_type=rerequest&client_id=464891386855067&ret=login&sdk=ios&logger_id=30F06532-A1B9-4B10-BB28-B29956C71AB1&ext=1470840777&hash=AeZqkIcf-NEW6vBd";
		private const string USERAGENT = @"Mozilla/5.0 (Linux; U; Android 4.0.2; en-us; Galaxy Nexus Build/ICL53F) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
		private const string FBID_URL = @"https://graph.facebook.com/v2.8/me?fields=id&access_token=";

		public string AccessToken { get; private set; }
		public string FbId { get; private set; }


		public BrowserView()
		{
			if (!Cef.IsInitialized)
			{
				var settings = new CefSettings();
				settings.UserAgent = USERAGENT;
				settings.LogSeverity = LogSeverity.Disable;
				Cef.Initialize(settings);
			}

			InitializeComponent();

			var requestHandler = new RequestHandler();
			requestHandler.TokenRetrievedEvent += BrowserView_TokenRetrievedEvent;
			
			Browser.RequestHandler = requestHandler;
			Browser.Address = URL;
			Browser.Load(URL);
			
		}

		private void BrowserView_TokenRetrievedEvent(object sender, string e)
		{
			// Gets app-scope Fb Id using Graph API

			AccessToken = e;

			var request = (HttpWebRequest) WebRequest.Create(FBID_URL + e);
			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				string responseString = reader.ReadToEnd();
				int start = responseString.IndexOf(':') + 2;
				int end = responseString.Length - 2;
				FbId = responseString.Substring(start, end - start);
			}
			
			Dispatcher.Invoke(() =>
			{
				DialogResult = true;
				Close();
			});
		}
	}

	internal class RequestHandler : IRequestHandler
	{

		public event TokenRetrieved TokenRetrievedEvent;

		public string AccessToken { get; private set; }

		public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
		{
			return false;
		}

		public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
		{
			// This is where the magic happens
			int tokenStart = url.IndexOf("access_token=") + "access_token=".Length;
			int tokenEnd = url.LastIndexOf('&');
			string accessToken = url.Substring(tokenStart, tokenEnd - tokenStart);
			TokenRetrievedEvent.Invoke(this, accessToken);
			return false;
		}

		public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
		{
			callback.Dispose();
			return false;
		}

		public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
		{
			return null;
		}

		public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
		{
			return false;
		}

		public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
		{
			callback.Dispose();
			return CefReturnValue.Continue;
		}

		public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
		{
			callback.Dispose();
			return false;
		}

		public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
		{
			return false;
		}

		public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
		{
		}


		public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
		{
			callback.Dispose();
			return false;
		}

		public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
		{
		}

		public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
		{
		}

		public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
		{
		}

		public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
		{
		}

		public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
		{
			callback.Dispose();
			return false;
		}
	}
}
