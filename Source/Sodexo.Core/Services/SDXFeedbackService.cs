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
	public class SDXFeedbackService : SDXBaseService
	{
		string baseUrl = Constants.APIRootUrl + "/feedback";

		public SDXFeedbackService () : base ()
		{

		}

		protected async Task <FeedbackModel> DoAddFeedback (int feedbackTypeId, int starRatingCount, string comment, int modelId, bool isResponseRequested)
		{
			var uri = baseUrl + "?subscription-key=" + Constants.APISubscriptionKey;

			JObject contentJObj = new JObject ();
			contentJObj ["feedbackTypeId"] = feedbackTypeId.ToString ();
			contentJObj ["starRatingCount"] = starRatingCount.ToString ();
			contentJObj ["comment"] = comment;
			contentJObj ["responseRequested"] = isResponseRequested.ToString ();
			if (modelId > 0)
				contentJObj ["modelId"] = modelId.ToString ();

			Console.WriteLine ("RequestBody = " + contentJObj.ToString ());
			var content = new StringContent (contentJObj.ToString (), System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddFeedback Success:");
				WriteResponseToConsole (responseBody);
				return FeedbackModel.Get (responseBody);
			} else {
				Console.WriteLine ("AddFeedback Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

