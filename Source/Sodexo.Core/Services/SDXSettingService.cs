using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;

namespace Sodexo.Core
{
	public class SDXSettingService : SDXBaseService
	{
		public SDXSettingService () : base ()
		{

		}

		protected async Task <List<ApplicationSettingModel>> GetApplicationSettings (bool isAuthorized)
		{
			#if DEBUG
			var uri = "https://sodexodevapi.azure-api.net/itz-api/settings/Test?subscription-key=b63ab65e3d9d41eda7791fccb17bb159";
			#else
			var uri = "https://sodexoapi.azure-api.net/itz-api/settings/Prod?subscription-key=ae56a4b329644dc7a6603608821fbcb5";
			#endif

			var responseBody = "";
			if (isAuthorized)
				responseBody = await GetAsync (uri);
			else
				responseBody = await GetAsyncWithoutAuth (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetApplicationSettings Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<List<ApplicationSettingModel>> (responseBody);
			} else {
				Console.WriteLine ("GetApplicationSettings Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

