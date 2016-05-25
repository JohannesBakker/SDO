using System;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;
using System.Threading.Tasks;

namespace Sodexo.Core
{
	public class SDXSettingManager : SDXSettingService
	{
		// For Login
		public string AdminPortalCookieDomain = "";
		public string APIAuthenticationCookieDomain = "";
		public string InitialLoginUrl = "";
		public string RedirectLoginUrl = "";

		public string APIRootUrl = "";
		public string APISubscriptionKey = "";
		public string MimimumAppVersionAndroid = "";
		public string MimimumAppVersioniOS = "";
		public string UpgradeUrlAndroid = "";
		public string UpgradeUrliOS = "";

		public string SiteRootUrl = "";
		public string StorageAccountKey = "";
		public string StorageAccountName = "";
		public string StorageHostUrl = "";

		public string NotificationHubName = "";
		public string NotificationHubFullSAS = "";
		public string NotificationHubListenSAS = "";

		public int MaxPhotoUploadDimension = 1024;

		public bool IsUnAuthorizedLoaded = false;
		public bool IsAuthorizedLoaded = false;

		public string CalendarEventText = "";

		public bool EnablePushNotifications = false;
		public bool DeserializeObjectsToConsole = true;


		public SDXSettingManager () : base ()
		{

		}

		public async Task<bool> LoadApplicationSettings (bool isAuthorized)
		{
			if (!isAuthorized && IsUnAuthorizedLoaded)
			{
				return true;
			}
			if (isAuthorized && IsAuthorizedLoaded)
			{
				return true;
			}

			List<ApplicationSettingModel> settings = await GetApplicationSettings (isAuthorized);

			if (settings == null)
				return false;

			Dictionary<string,string> settingsDict = settings.ToDictionary(name => name.SettingName, value => value.SettingValue);

			AdminPortalCookieDomain = settingsDict ["AdminPortalCookieDomain"];
			Constants.APIAuthenticationCookieDomain = APIAuthenticationCookieDomain = settingsDict ["APIAuthenticationCookieDomain"];
			Constants.APIRootUrl = APIRootUrl = settingsDict ["APIRootUrl"];
			Constants.APISubscriptionKey = APISubscriptionKey = settingsDict ["APISubscriptionKey"];
			InitialLoginUrl = settingsDict ["InitialLoginUrl"];
			MimimumAppVersionAndroid = settingsDict ["MimimumAppVersionAndroid"];
			MimimumAppVersioniOS = settingsDict ["MimimumAppVersioniOS"];
			RedirectLoginUrl = settingsDict ["RedirectLoginUrl"];
			UpgradeUrlAndroid = settingsDict ["UpgradeUrlAndroid"];
			UpgradeUrliOS = settingsDict ["UpgradeUrliOS"];
			#if __IPAD__
			NotificationHubName = settingsDict ["NotificationHubiPadName"];
			NotificationHubListenSAS = settingsDict ["NotificationHubiPadListenSAS"];
			#else
			NotificationHubListenSAS = settingsDict ["NotificationHubListenSAS"];
			NotificationHubName = settingsDict ["NotificationHubName"];
			#endif
			#if !DEBUG
			DeserializeObjectsToConsole = bool.Parse((string)settingsDict ["DeserializeObjectsToConsole"]);
			#endif
			if (isAuthorized) {
				SiteRootUrl = settingsDict ["SiteRootUrl"];
				StorageAccountKey = settingsDict ["StorageAccountKey"];
				StorageAccountName = settingsDict ["StorageAccountName"];
				StorageHostUrl = settingsDict ["StorageHostUrl"];
				UpgradeUrlAndroid = settingsDict ["UpgradeUrlAndroid"];
				UpgradeUrliOS = settingsDict ["UpgradeUrliOS"];
				#if __IPAD__
				NotificationHubFullSAS = settingsDict ["NotificationHubiPadFullSAS"];
				#else
				NotificationHubFullSAS = settingsDict ["NotificationHubFullSAS"];
				#endif
				MaxPhotoUploadDimension = Int32.Parse((string)settingsDict ["MaxPhotoUploadDimension"]);
				CalendarEventText = settingsDict ["CalendarEventText"];
				EnablePushNotifications = bool.Parse((string)settingsDict ["EnablePushNotifications"]);

				IsAuthorizedLoaded = true;
			}

			IsUnAuthorizedLoaded = true;
			return IsSuccessed;
		}
	}
}

