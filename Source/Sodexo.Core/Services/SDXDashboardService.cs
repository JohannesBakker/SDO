using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXDashboardService : SDXBaseService
	{
		private string BaseUrl = Constants.APIRootUrl + "/dashboard";
		public SDXDashboardService () : base ()
		{

		}

		protected async Task<List<DashboardItemModel>> DoLoadDashboard ()
		{
			var uri = BaseUrl + "?subscription-key=" + Constants.APISubscriptionKey;

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("Dashboard Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<List<DashboardItemModel>> (responseBody);
			} else {
				Console.WriteLine ("Dashboard Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

