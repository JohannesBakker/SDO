using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;

using Xamarin.Auth;
using Sodexo.Core;
using TinyIoC;
using System.CodeDom.Compiler;
using PerpetualEngine.Storage;
using System.Threading.Tasks;

namespace Sodexo.Ipad2
{
	[Register ("SDXLoginVC")]
	partial class SDXLoginVC : SDXBaseVC
	{
		//public AVPlayer Player;
		private bool IsSplashVideoFinished = false;
		private SDXSettingManager manager = TinyIoCContainer.Current.Resolve<SDXSettingManager>();

		public SDXLoginVC (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			IsSplashVideoFinished = true;

			//LoadApplicationSettings();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			//UINavigationBar.Appearance.BackgroundColor = UIColor.FromRGB (40, 56, 151);
		
			LoadApplicationSettings();
		}

		async void LoadApplicationSettings()
		{
			await manager.LoadApplicationSettings(false);
			if(!manager.IsSuccessed)
			{
				new UIAlertView("", "Network Error", null, "Ok", null).Show();
				return;
			}

			if (IsSplashVideoFinished) {

				//if login token was set and is not older than 90 min
				var storage = SimpleStorage.EditGroup ("Sodexo");
				var loginTime = storage.Get <DateTime> ("Login Data time limit", DateTime.Now.AddMinutes (-100));

				//if (DateTime.Now > loginTime.AddMinutes (90)) {

					PresentAuthScreen ();
					
//				} else {
//					//proceed with the cache
//					var authManager = TinyIoCContainer.Current.Resolve<SDXAuthManager> ();
//					authManager.SetSecurityToken (storage.Get <Dictionary<string,string>>("Security Token"));
//					authManager.AuthenticationTime = DateTime.Now;
//					authManager.IsAuthenticated = true;	
//
//					this.PerformSegue("SegueToMain", this);
//				}
			}
		}

		private void PresentAuthScreen()
		{
			if(manager.AdminPortalCookieDomain == "")
			{
				return;
			}

			var loginUrl = manager.InitialLoginUrl;
			var redirectUrl = manager.RedirectLoginUrl;
			var auth = new FedAuthWebRedirectAuthenticator(new Uri(loginUrl), new Uri(redirectUrl));

			auth.Title = "";
			auth.AllowCancel = false;
			auth.Completed += async (object sender, AuthenticatorCompletedEventArgs e) =>
				{
					this.DismissViewController(true, null);
					if(!e.IsAuthenticated)
					{
						return;
					}

					var authManager = TinyIoCContainer.Current.Resolve<SDXAuthManager>();
					authManager.SetSecurityToken(e.Account.Properties);
					authManager.AuthenticationTime = DateTime.Now;
					authManager.IsAuthenticated = true;	
					
					//cache the auth tokens
					var storage = SimpleStorage.EditGroup("Sodexo");
					storage.Put<Dictionary<string,string>> ("Security Token", e.Account.Properties);
					storage.Put<DateTime> ("Login Data time limit", DateTime.Now);

				var userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
				if (userManager.Me == null)
				{
					await LoadMe();
				}

				//register for push notifications now that we're authenticated
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);

				};

			var vc = auth.GetUI();
			this.PresentViewController(vc, false, () => { this.PerformSegue("SegueToMain", this);  });

		}

		private async Task LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null) {
				ShowLoading ("Loading...");
				await manager.LoadMe (true);

				if (!manager.IsSuccessed) {
					HideLoading ();
					new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
					return;
				}
			}
		}
	}
}
