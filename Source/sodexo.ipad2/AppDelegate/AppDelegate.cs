using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;

using HockeyApp;
using TinyIoC;

using Sodexo.Core;
using WindowsAzure.Messaging;

namespace Sodexo.Ipad2
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window
		{
			get;
			set;
		}

		public UIViewController AccountsVC { get; set; }
		public UIViewController AccountDetailVC {get; set;}

		private SBNotificationHub Hub { get; set; }

		const string HOCKEYAPP_APPID = "8501ff6f88c8f9e6e216d77b10e44f10";

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			UIBarButtonItem.Appearance.SetBackgroundImage(new UIImage(), UIControlState.Normal | UIControlState.Highlighted, UIBarMetrics.Default);
			UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			UINavigationBar.Appearance.ShadowImage = new UIImage();

			UITextAttributes navTitleAttributes = UINavigationBar.Appearance.GetTitleTextAttributes();
			navTitleAttributes.Font = UIFont.FromName(Constants.KARLA_REGULAR, 20);
			navTitleAttributes.TextColor = UIColor.White;
			UINavigationBar.Appearance.SetTitleTextAttributes(navTitleAttributes);

			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

			HockeyApp.Setup.EnableCustomCrashReporting(() =>
			{

				//Get the shared instance
				var manager = BITHockeyManager.SharedHockeyManager;

				//Configure it to use our APP_ID
				manager.Configure(HOCKEYAPP_APPID);

				//Start the manager
				manager.StartManager();

				//Authenticate (there are other authentication options)
				manager.Authenticator.AuthenticateInstallation();

				//Rethrow any unhandled .NET exceptions as native iOS 
				// exceptions so the stack traces appear nicely in HockeyApp
				AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
					Setup.ThrowExceptionAsNative(e.ExceptionObject);

				TaskScheduler.UnobservedTaskException += (sender, e) =>
					Setup.ThrowExceptionAsNative(e.Exception);
			});

			TinyIoCContainer.Current.Register<SDXSettingManager>().AsSingleton();
			TinyIoCContainer.Current.Register<SDXAuthManager>().AsSingleton();
			TinyIoCContainer.Current.Register<SDXDashboardManager>().AsSingleton();
			TinyIoCContainer.Current.Register <SDXReferenceDataManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXUserManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXPromotionManager> ().AsSingleton ();

			// don't use the following line
			// per http://stackoverflow.com/questions/9911934/how-to-implement-unregisterforremotenotifications-in-ios
			//UIApplication.SharedApplication.UnregisterForRemoteNotifications ();

			UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.None;
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);

			return true;
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			NSSet tags = null;

			var userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (userManager.Me != null)
			{
				tags = new NSSet (userManager.Me.Tags.ToArray());
			}

			var settingManager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();

			if (!settingManager.IsUnAuthorizedLoaded)
			{
				Console.WriteLine("Unable to locate required setings to enable push notifications");
			}
			else
			{
				Console.WriteLine("NotificationHubName: {0}", settingManager.NotificationHubName);
				Console.WriteLine("NotificationHubListenSAS: {0}", settingManager.NotificationHubListenSAS);

				Hub = new SBNotificationHub (settingManager.NotificationHubListenSAS, settingManager.NotificationHubName);
				Hub.UnregisterAllAsync(deviceToken, (error) => {
					if (error != null) {
						Console.WriteLine("UnregisterAllAsync error: {0}", error.ToString());
						new UIAlertView("UnregisterAllAsync error", error.LocalizedDescription, null, "OK", null).Show();
						return;
					}

					Console.WriteLine("UnregisterAllAsync successfull.");

					if (!settingManager.EnablePushNotifications) {
						Console.WriteLine("EnablePushNotifications is not set to true, abortng re-registrations.");
						return;
					}

					if (tags != null) {
						Hub.RegisterNativeAsync(deviceToken, tags,(errorCallback) => {
							if (errorCallback != null) {
								Console.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
								new UIAlertView("RegisterNativeAsync error", error.LocalizedDescription, null, "OK", null).Show();
								return;
							}

							Console.WriteLine("RegisterNativeAsync successfull.");
						});					
					}
				});				
			}
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
		{
			new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine("ReceivedRemoteNotification entered.");

			ProcessNotification(userInfo, false);
		}

		void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying 
				// "  aps:{alert:"alert msg here"}  " this will work fine.
				// But if you're using a complex alert with Localization keys, etc., 
				// your "alert" object from the aps dictionary will be another NSDictionary. 
				// Basically the json gets dumped right into a NSDictionary, 
				// so keep that in mind.
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps [new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}           
			}
		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation(UIApplication application)
		{
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground(UIApplication application)
		{
		}

		/// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground(UIApplication application)
		{
			try{
				Console.WriteLine("Checking Account");
				var manager = TinyIoCContainer.Current.Resolve<SDXUserManager> ();
				var me = manager.LoadMe ();

				//throw new Exception();
			}
			catch (Exception e) {
				UIStoryboard board = UIStoryboard.FromName ("MainStoryboard", null);
				var vc =  (UINavigationController)board.InstantiateViewController ("LoginParent");
				Window.RootViewController.PresentViewController (vc, false, null);
	
			}
		}

		/// This method is called when the application is about to terminate. Save data, if needed. 
		public override void WillTerminate(UIApplication application)
		{
		}
	}
}