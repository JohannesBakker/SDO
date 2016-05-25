using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using Xamarin.Auth;
using Sodexo.Core;
using TinyIoC;
using System.Threading.Tasks;

namespace Sodexo.iOS
{
	public partial class SDXLoginVC : SDXBaseVC
	{

		public AVPlayer Player;
//		AVPlayerLayer PlayerLayer;
//		UIView IntroView;

//		private bool IsApplicationSettingsLoaded = false;
		private bool IsSplashVideoFinished = false;

		public SDXLoginVC (IntPtr handle) : base (handle)
		{
		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (UIScreen.MainScreen.Bounds.Height <= 480)
				BackgroundIV.Image = UIImage.FromBundle ("Splash.png");

			IsSplashVideoFinished = true;
			LoadApplicationSettings ();

//			AVAsset asset = AVAsset.FromUrl(NSUrl.FromFilename ("intro.mp4"));
//			AVPlayerItem playerItem = new AVPlayerItem (asset);
//			Player = new AVPlayer (playerItem);
//			if (Player != null) {
//				IntroView = new UIView (View.Frame);
//				IntroView.BackgroundColor = UIColor.FromRGB (194 / 255.0f, 194 / 255.0f, 204 / 255.0f);
//				View.AddSubview (IntroView);
//
//				PlayerLayer = AVPlayerLayer.FromPlayer (Player);
//				PlayerLayer.Frame = IntroView.Frame;
//				IntroView.Layer.AddSublayer (PlayerLayer);
//
//				NSNotificationCenter.DefaultCenter.AddObserver (AVPlayerItem.DidPlayToEndTimeNotification, PlayerItemDidReachEnd, playerItem);
//
//				Player.Play ();
//			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			PresentAuthScreen ();
		}
		#endregion

		#region Notifications
		private void PlayerItemDidReachEnd (NSNotification notification)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (AVPlayerItem.DidPlayToEndTimeNotification);

//			UIView.Animate (1.0f, () => {
//				IntroView.Alpha = 2.0f;
//			}, () => {
//				if (IsApplicationSettingsLoaded)
//					PresentAuthScreen ();
//				IsSplashVideoFinished = true;
//			});
		}
		#endregion


		#region Private Functions
		async void LoadApplicationSettings ()
		{
			SDXSettingManager manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			await manager.LoadApplicationSettings (false);
			if (!manager.IsSuccessed) {
				new UIAlertView ("", "Network Error", null, "OK", null).Show ();
				return;
			}

			if (IsSplashVideoFinished)
				PresentAuthScreen ();
//			IsApplicationSettingsLoaded = true;
		}

		void PresentAuthScreen ()
		{
			SDXSettingManager settingManager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			if (settingManager.AdminPortalCookieDomain == "")
				return;

			var loginUrl =  settingManager.InitialLoginUrl;
			var redirectUrl =  settingManager.RedirectLoginUrl;
			var auth = new FedAuthWebRedirectAuthenticator (new Uri(loginUrl) ,new Uri(redirectUrl));

			auth.Title = "";
			auth.AllowCancel = false;

			auth.Completed += async (object sender, AuthenticatorCompletedEventArgs e) => {

				this.DismissViewController(true, null);

				if (!e.IsAuthenticated) {
					//should not get here as long as auth cancel is disabled
					return;
				}

				var fedAuthTokens = new Dictionary<string,string>();
				foreach (var token in e.Account.Properties) {
					fedAuthTokens.Add(token.Key, token.Value);
				}

				var authManager = TinyIoCContainer.Current.Resolve<SDXAuthManager>();
				authManager.SetSecurityToken(fedAuthTokens);
				authManager.AuthenticationTime = DateTime.Now;
				authManager.IsAuthenticated = true;


				var userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
				if (userManager.Me == null)
				{
					await LoadMe();
				}

				//register for push notifications now that we're authenticated
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			};

			var vc = auth.GetUI ();

			this.PresentViewController (vc, false, () => {
				this.PerformSegue ("SegueToMain", this);
			});
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
		#endregion
	}
}
