using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Auth;
using MonoTouch.Foundation;

namespace Sodexo.Ipad2
{
	class FedAuthWebRedirectAuthenticator : WebRedirectAuthenticator
	{
		public FedAuthWebRedirectAuthenticator(Uri initialUrl, Uri redirectUrl) : base(initialUrl, redirectUrl) { }

		protected override void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
		{
			//base.OnRedirectPageLoaded(url, query, fragment);

			var store = NSHttpCookieStorage.SharedStorage;
			var cookies = store.Cookies;
			var fedAuthList = cookies.Where(c => c.Name.ToLower().StartsWith("fedauth")).ToList();

			if(fedAuthList.Count>0)
			{
				var account = new Account();
				foreach(var fedAuth in fedAuthList)
				{
					account.Properties.Add(fedAuth.Name, fedAuth.Value);
				}
				OnSucceeded(account);
			}
		}
	}
}