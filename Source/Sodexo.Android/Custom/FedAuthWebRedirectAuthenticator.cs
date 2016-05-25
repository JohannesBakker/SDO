using System;
using Xamarin.Auth;
using Android.Webkit;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace Sodexo.Android
{
	public class FedAuthWebRedirectAuthenticator : WebRedirectAuthenticator
	{

		public FedAuthWebRedirectAuthenticator (Uri initialUrl, Uri redirectUrl) : base(initialUrl, redirectUrl)
		{
			//nothing additional here
		}

		protected override void OnRedirectPageLoaded (Uri url, System.Collections.Generic.IDictionary<string, string> query, System.Collections.Generic.IDictionary<string, string> fragment)
		{
			CookieManager cookieManager = CookieManager.Instance;
			var cookiesString = cookieManager.GetCookie (url.ToString());

			if (!string.IsNullOrEmpty(cookiesString))
			{
				var cookiesArray = cookiesString.Split(new char [] {';'});
				var cookies = new List<NameValuePair> ();
				foreach (var cookie in cookiesArray)
				{
					var delimiterIndex = cookie.IndexOf ("=");
					var name = cookie.Substring (0, delimiterIndex).Trim();
					var value = cookie.Substring (delimiterIndex+1).Trim();
					cookies.Add (new NameValuePair { Name=name, Value=value});
				}

				var fedAuthList = (from c in cookies where c.Name.ToLower().StartsWith("fedauth")  select c).ToList();

				if (fedAuthList.Count > 0) {
					var account = new Account ();

					foreach (var fedAuth in fedAuthList)
					{
						account.Properties.Add (fedAuth.Name, fedAuth.Value);
					}
					OnSucceeded (account);
				}
			}
		}

		public class NameValuePair
		{
			public string Name { get; set;}
			public string Value { get; set;}
		}

	}
}

