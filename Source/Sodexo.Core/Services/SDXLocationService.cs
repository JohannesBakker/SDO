using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sodexo.RetailActivation.Portable.Models;
using System.Web;

namespace Sodexo.Core
{
	public class SDXLocationService : SDXBaseService
	{
		private string LocationServiceBaseUrl = Constants.APIRootUrl + "/locations/";

		public SDXLocationService () : base ()
		{
		}

		protected async Task<LocationModel> GetLocation (string locationID)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var uri = LocationServiceBaseUrl + locationID + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetLocation Success");
				WriteResponseToConsole (responseBody);
				return LocationModel.Get (responseBody);
			} else {
				Console.WriteLine ("GetLocation Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

