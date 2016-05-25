using System;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;
using System.Web;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXReferenceDataService : SDXBaseService
	{
		public SDXReferenceDataService () : base ()
		{

		}

		protected async Task<ReferenceDataModel> GetReferenceData (bool isIncludeConsumerTypes, bool isIncludeAnnualSalesRanges, bool isIncludeConsumersOnSiteRanges, bool isIncludeLocalCompetitionTypes, bool isIncludeOfferCategories, bool isIncludeFeedbackTypes)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);

			queryString ["includeConsumerTypes"] = isIncludeConsumerTypes.ToString ();
			queryString ["includeAnnualSalesRanges"] = isIncludeAnnualSalesRanges.ToString ();
			queryString ["includeConsumersOnSiteRanges"] = isIncludeConsumersOnSiteRanges.ToString ();
			queryString ["includeLocalCompetitionTypes"] = isIncludeLocalCompetitionTypes.ToString ();
			queryString ["includeOfferCategories"] = isIncludeOfferCategories.ToString ();
			queryString ["includeFeedbackTypes"] = isIncludeFeedbackTypes.ToString ();
			
			var baseUrl = Constants.APIRootUrl + "/lookups/referencedata?subscription-key=" + Constants.APISubscriptionKey;

			var uri = baseUrl + "&" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetReferenceData Success");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<ReferenceDataModel> (responseBody);
			} else {
				Console.WriteLine ("GetReferenceData Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task <DecisionTreeModel> DoLoadDecisionTree (int offerCategoryId, bool isIncludeVersions, bool isIncludeNodes, bool isIncludeOnlyCurrentVersion)
		{
			var queryString = HttpUtility.ParseQueryString (string.Empty);

			queryString ["includeVerions"] = isIncludeVersions.ToString ();
			queryString ["includeNodes"] = isIncludeNodes.ToString ();
			queryString ["includeOnlyCurrentVersion"] = isIncludeOnlyCurrentVersion.ToString ();

			var uri = Constants.APIRootUrl + "/offercategories/" + offerCategoryId.ToString () + "/decisiontree?subscription-key=" + Constants.APISubscriptionKey + "&" + queryString;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("LoadDecisionTree Success:");
				WriteResponseToConsole (responseBody);
				return DecisionTreeModel.Get (responseBody);
			} else {
				Console.WriteLine ("LoadDecisionTree Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

