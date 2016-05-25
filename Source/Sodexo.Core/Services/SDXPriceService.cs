using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXPriceService : SDXBaseService
	{
		string BASE_URL = Constants.APIRootUrl + "/productprices/";
		public SDXPriceService ()
		{
		}

		protected async Task <List<ProductPriceModel>> DoLoadPricesAdvance (int divisonId, int pricingBandId, int localCompetitionTypeId, int consumerTypeId, int offerCategoryId, string searchText)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			//queryString ["offerCategoryId"] = offerCategoryId.ToString ();
			queryString ["searchText"] = searchText;
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var uri = BASE_URL + divisonId.ToString () + "/" + pricingBandId.ToString () + "/" + localCompetitionTypeId.ToString () 
				+ "/" + consumerTypeId.ToString () + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("LoadPricesAdvance Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<List<ProductPriceModel>> (responseBody);
			} else {
				Console.WriteLine ("LoadPricesAdvance Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <List<ProductPriceModel>> DoLoadPricesBasic (int outletId, string searchText)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["searchText"] = searchText;
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var uri = BASE_URL + "/" + outletId.ToString () + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("LoadPricesBasic Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<List<ProductPriceModel>> (responseBody);
			} else {
				Console.WriteLine ("LoadPricesBasic Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

