using System;
using Xamarin.Auth;
using System.Linq;
using System.Collections.Generic;


namespace Sodexo.iOS
{
	public class FedAuthWebRedirectAuthenticator : WebRedirectAuthenticator
	{

		public FedAuthWebRedirectAuthenticator (Uri initialUrl, Uri redirectUrl) : base(initialUrl, redirectUrl)
		{
			//nothing additional here
		}

		protected override void OnRedirectPageLoaded (Uri url, System.Collections.Generic.IDictionary<string, string> query, System.Collections.Generic.IDictionary<string, string> fragment)
		{

			var store = MonoTouch.Foundation.NSHttpCookieStorage.SharedStorage;
			var cookies = store.Cookies;
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
}