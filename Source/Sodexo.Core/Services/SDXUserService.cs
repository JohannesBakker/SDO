using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXUserService : SDXBaseService
	{
		private string UserServiceBaseUri = Constants.APIRootUrl + "/users/";

		public SDXUserService () : base ()
		{

		}

		protected async Task<UserModel> GetUser(string domainUniqueIdentifier, bool isIncludeAccounts)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);

			queryString ["includeAccounts"] = isIncludeAccounts.ToString ();
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var uri = UserServiceBaseUri + domainUniqueIdentifier + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetUser Success");
				WriteResponseToConsole (responseBody);
				return UserModel.Get (responseBody);
			} else {
				Console.WriteLine ("GetUser Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task<string> AddUserToAccount(string domainUniqueIdentifier, string locationId)
		{
			var uri = UserServiceBaseUri + domainUniqueIdentifier + "/accounts/" + locationId + "?subscription-key=" + Constants.APISubscriptionKey;

			HttpContent content = new StringContent ("", System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddUserToAccount Success");
				Console.WriteLine (responseBody);
				return "Success";
			} else {
				Console.WriteLine ("AddUserToAccount Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

