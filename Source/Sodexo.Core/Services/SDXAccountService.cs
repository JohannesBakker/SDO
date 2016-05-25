using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXAccountService : SDXBaseService
	{
		private string AccountServiceBaseUri = Constants.APIRootUrl + "/accounts";

		public SDXAccountService () : base ()
		{

		}

		protected async Task<List<AccountModel>> GetAccountList(int itemsToReturn, bool isIncludeUsers, bool isIncludeLocation, bool isIncludeOutlets)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["subscription-key"] = Constants.APISubscriptionKey;
			queryString ["itemsToReturn"] = itemsToReturn.ToString ();
			queryString ["includeUsers"] = isIncludeUsers.ToString ();
			queryString ["includeLocation"] = isIncludeLocation.ToString ();
			queryString ["includeOutlets"] = isIncludeOutlets.ToString ();

			var uri = AccountServiceBaseUri + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetAccountList Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<List<AccountModel>> (responseBody);
			} else {
				Console.WriteLine ("GetAccountList Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task<AccountModel> GetAccount(string locationID, bool isIncludeUsers, bool isIncludeLocation, bool isIncludeOutlets)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);
			queryString ["subscription-key"] = Constants.APISubscriptionKey;
			queryString ["includeUsers"] = isIncludeUsers.ToString ();
			queryString ["includeLocation"] = isIncludeLocation.ToString ();
			queryString ["includeOutlets"] = isIncludeOutlets.ToString ();

			var uri = AccountServiceBaseUri + "/" + locationID + "?" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetAccount Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<AccountModel> (responseBody);
			} else {
				Console.WriteLine ("GetAccount Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task<AccountModel> AddAccount (AccountModel account)
		{
			JObject contentJObj = new JObject ();
			contentJObj ["locationId"] = account.LocationId;
			contentJObj ["consumerTypeId"] = account.ConsumerTypeId;

			JArray outletsJAry = new JArray ();
			foreach (OutletModel outlet in account.Outlets) {
				JObject obj = new JObject ();
				obj ["name"] = outlet.Name;
				obj ["annualSalesRangeId"] = outlet.AnnualSalesRangeId;
				obj ["consumersOnSiteRangeId"] = outlet.ConsumersOnSiteRangeId;
				obj ["localCompetitionTypeId"] = outlet.LocalCompetitionTypeId;

				outletsJAry.Add (obj);
			}
			contentJObj ["outlets"] = outletsJAry;

			var uri = AccountServiceBaseUri + "?subscription-key=" + Constants.APISubscriptionKey;

			System.Console.WriteLine (contentJObj.ToString ());

			HttpContent content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddAccount Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<AccountModel> (responseBody);
			} else {
				Console.WriteLine ("AddAccount Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <string> DoDeleteOffer (string locationId, string outletId, string offerId)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets/" + outletId + "/offers/" + offerId + "?subscription-key=" + Constants.APISubscriptionKey;

			var responseBody = await DeleteAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("DeleteOffer Success:");
				Console.WriteLine (responseBody);
				return "Success";
			} else {
				Console.WriteLine ("DeleteOffer Failed: " + ErrorMessage);
				return "";
			}
		}

		protected async Task <AccountModel> DoUpdateAccount (string locationId, string consumerTypeId, string rowVersion)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "?subscription-key=" + Constants.APISubscriptionKey;

			JObject contentJObj = new JObject ();
			contentJObj ["consumerTypeId"] = consumerTypeId;
			contentJObj ["rowVersion"] = rowVersion;

			System.Console.WriteLine (contentJObj.ToString ());

			HttpContent content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PatchAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("UpdateAccount Success:");
				WriteResponseToConsole (responseBody);
				return AccountModel.Get (responseBody);
			} else {
				Console.WriteLine ("UpdateAccount Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <OutletModel> DoUpdateOutlet (string locationId, OutletModel outlet)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets/" + outlet.OutletId + "?subscription-key=" + Constants.APISubscriptionKey;

			JObject contentJObj = new JObject ();
			contentJObj ["name"] = outlet.Name;
			contentJObj ["annualSalesRangeId"] = outlet.AnnualSalesRangeId;
			contentJObj ["consumersOnSiteRangeId"] = outlet.ConsumersOnSiteRangeId;
			contentJObj ["localCompetitionTypeId"] = outlet.LocalCompetitionTypeId;
			contentJObj ["rowVersion"] = System.Convert.ToBase64String(outlet.RowVersion);

			System.Console.WriteLine (contentJObj.ToString ());

			HttpContent content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PatchAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("UpdateOutlet Success:");
				WriteResponseToConsole (responseBody);
				return OutletModel.Get (responseBody);
			} else {
				Console.WriteLine ("UpdateOutlet Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <OutletModel> DoAddOutlet (string locationId, OutletModel outlet)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets" + "?subscription-key=" + Constants.APISubscriptionKey;

			JObject contentJObj = new JObject ();
			contentJObj ["name"] = outlet.Name;
			contentJObj ["annualSalesRangeId"] = outlet.AnnualSalesRangeId;
			contentJObj ["consumersOnSiteRangeId"] = outlet.ConsumersOnSiteRangeId;
			contentJObj ["localCompetitionTypeId"] = outlet.LocalCompetitionTypeId;
			contentJObj ["offers"] = new JArray ();

			System.Console.WriteLine (contentJObj.ToString ());

			HttpContent content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddOutlet Success:");
				WriteResponseToConsole (responseBody);
				return OutletModel.Get (responseBody);
			} else {
				Console.WriteLine ("AddOutlet Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <OfferModel> DoAddOffer (string locationId, string outletId, string offerName, string offerCategoryId)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets/" + outletId + "/offers" + "?subscription-key=" + Constants.APISubscriptionKey;

			JObject contentJObj = new JObject ();
			contentJObj ["name"] = offerName;
			contentJObj ["offerCategoryId"] = offerCategoryId;

			System.Console.WriteLine (contentJObj.ToString ());

			HttpContent content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddOffer Success:");
				WriteResponseToConsole (responseBody);
				return OfferModel.Get (responseBody);
			} else {
				Console.WriteLine ("AddOffer Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <OfferResponseModel> DoSetPlanogram (string locationId, int outletId, int offerId, int decisionTreeNodeId)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets/" + outletId.ToString () 
					+ "/offers/" + offerId.ToString () + "/responses/" + decisionTreeNodeId.ToString () + "?subscription-key=" + Constants.APISubscriptionKey;
			HttpContent content = new StringContent ("", System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("SetPlanogram Success:");
				WriteResponseToConsole (responseBody);
				return OfferResponseModel.Get (responseBody);
			} else {
				Console.WriteLine ("SetPlanogram Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <string> DoDeleteOutlet (string locationId, int outletId)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "/outlets/" + outletId + "?subscription-key=" + Constants.APISubscriptionKey;

			var responseBody = await DeleteAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("DeleteOutlet Success:");
				Console.WriteLine (responseBody);
				return "Success";
			} else {
				Console.WriteLine ("DeleteOutlet Failed: " + ErrorMessage);
				return "";
			}
		}

		protected async Task <string> DoDeleteAccount (string locationId)
		{
			var uri = AccountServiceBaseUri + "/" + locationId + "?subscription-key=" + Constants.APISubscriptionKey;

			var responseBody = await DeleteAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("DeleteAccount Success:");
				Console.WriteLine (responseBody);
				return "Success";
			} else {
				Console.WriteLine ("DeleteAccount Failed: " + ErrorMessage);
				return "";
			}
		}

		protected async Task <string> DoSendPlanogramViaEmail (int planogramVersionId, string email)
		{
			var uri = Constants.APIRootUrl + "/planograms/sendbyemail?subscription-key=" + Constants.APISubscriptionKey;

			JObject jObj = new JObject ();
			jObj ["planogramVersionId"] = planogramVersionId.ToString ();
			JArray jAry = new JArray ();
			jAry.Add (email);
			jObj ["emailAddresses"] = jAry;

			Console.WriteLine (jObj.ToString ());

			var content = new StringContent (jObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("DoSendPlanogramViaEamil Success:");
				return "Success";
			} else {
				Console.WriteLine ("DoSendPlanogramViaEamil Failed:");
				return "Failed";
			}
		}
	}
}

