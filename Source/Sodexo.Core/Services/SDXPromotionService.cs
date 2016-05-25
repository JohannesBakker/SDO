using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sodexo.Core
{
	public class SDXPromotionService : SDXBaseService
	{
		private string BaseUrl = Constants.APIRootUrl + "/promotions";
		public SDXPromotionService () : base ()
		{

		}

		protected async Task<List<PromotionModel>> DoLoadPromotions (bool isIncludeLocalActivations, bool isIncludePromotionCategories) 
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["includeLocalActivations"] = isIncludeLocalActivations.ToString ();
			queryString ["includePromotionCategories"] = isIncludePromotionCategories.ToString ();
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var url = BaseUrl + "?" + queryString;

			var responseBody = await GetAsync (url);

			if (IsSuccessed) {
				Console.WriteLine ("LoadPromotions Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject <List<PromotionModel>> (responseBody);
			} else {
				Console.WriteLine ("LoadPromotions Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task<PromotionModel> DoLoadPromotion (int promotionId, bool isIncludeLocalActivations, bool isIncludePromotionCategories)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["promotionId"] = promotionId.ToString ();
			queryString ["includeLocalActivations"] = isIncludeLocalActivations.ToString ();
			queryString ["includePromotionCategories"] = isIncludePromotionCategories.ToString ();
			queryString ["subscription-key"] = Constants.APISubscriptionKey;

			var url = BaseUrl + "?" + queryString;

			var responseBody = await GetAsync (url);

			if (IsSuccessed) {
				Console.WriteLine ("LoadPromotion Success:");
				WriteResponseToConsole (responseBody);
				return PromotionModel.Get (responseBody);
			} else {
				Console.WriteLine ("LoadPromotion Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <PromotionResponseModel> DoAcceptPromotion (int promotionId, string domainUniqueIdentifier)
		{
			var uri = BaseUrl + "/" + promotionId.ToString () + "/users/" + domainUniqueIdentifier + "?subscription-key=" + Constants.APISubscriptionKey;

			HttpContent content = new StringContent ("", System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("DoAcceptPromotion Success:");
				WriteResponseToConsole (responseBody);
				return PromotionResponseModel.Get (responseBody);
			} else {
				Console.WriteLine ("DoAcceptPromotion Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <PromotionResponseModel> DoDeclinePromotion (int promotionId, string domainUniqueIdentifier)
		{
			var uri = BaseUrl + "/" + promotionId.ToString () + "/users/" + domainUniqueIdentifier + "?subscription-key=" + Constants.APISubscriptionKey;

			var responseBody = await DeleteAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("DoDeclinePromotion Success:");
				WriteResponseToConsole (responseBody);
				return PromotionResponseModel.Get (responseBody);
			} else {
				Console.WriteLine ("DoDeclinePromotion Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <string> DoSendPromotionViaEmail (int promotionId, string email)
		{
			var uri = BaseUrl + "/sendbyemail?subscription-key=" + Constants.APISubscriptionKey;

			JObject jObj = new JObject ();
			jObj ["promotionId"] = promotionId.ToString ();
			JArray jAry = new JArray ();
			jAry.Add (email);
			jObj ["emailAddresses"] = jAry;

			Console.WriteLine (jObj.ToString ());

			var content = new StringContent (jObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("DoSendPromotionViaEamil Success:");
				return "Success";
			} else {
				Console.WriteLine ("DoSendPromotionViaEamil Failed:");
				return "Failed";
			}
		}
	}
}

