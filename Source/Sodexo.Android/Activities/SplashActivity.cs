
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Xamarin.Auth;

using System.Threading.Tasks;

using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Android
{
	[Activity (MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashActivity : BaseActivity
	{
		const int AuthRequestCode = 1000;

		#region Activity Lifecycle
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Splash);

			LoadApplicationSettings ();
		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);

			if (!hasFocus || isViewAdjusted)
				return;

			var view = FindViewById (Resource.Id.splash_view) as FrameLayout;

			Data.XRate = view.Width / 640.0f;
			Data.YRate = view.Height / 1096.0f;
			Data.WRate = Data.XRate;
			Data.HRate = Data.YRate;
			Data.Density = Resources.DisplayMetrics.Density;

			LayoutAdjuster.FitToScreen (this, view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);

			isViewAdjusted = true;
		}
		#endregion

		#region Private Functions
		async void LoadApplicationSettings ()
		{
			SDXSettingManager manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			if (manager.AdminPortalCookieDomain == "") {
				await manager.LoadApplicationSettings (false);
				if (!manager.IsSuccessed) {
					Util.ShowAlert (manager.ErrorMessage, this);
					return;
				}
			}
			PresentAuthScreen ();
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

			auth.Completed += (object sender, AuthenticatorCompletedEventArgs ee) => {

				FinishActivity (AuthRequestCode);

				if (!ee.IsAuthenticated) {
					Util.ShowAlert ("Not Authenticated", this);
					return;
				}

				Finish ();

				var fedAuthTokens = new Dictionary<string,string>();
				foreach (var token in ee.Account.Properties) {
					fedAuthTokens.Add(token.Key, token.Value);
				}
				
				var authManager = TinyIoCContainer.Current.Resolve<SDXAuthManager>();
				authManager.SetSecurityToken(fedAuthTokens);
				authManager.AuthenticationTime = DateTime.Now;
				authManager.IsAuthenticated = true;

				var mainIntent = new Intent (this, typeof(MainActivity));
				StartActivity (mainIntent);
			} ;

			var intent = auth.GetUI (this);
			StartActivityForResult (intent, AuthRequestCode);
		}
		#endregion
	}
}

