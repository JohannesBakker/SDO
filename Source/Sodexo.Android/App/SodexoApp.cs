
using System;
using System.Threading.Tasks;

using Android.App;
using Android.Runtime;
using Android.Graphics;

using TinyIoC;
using Sodexo.Core;

namespace Sodexo.Android
{
	#if DEBUG
	[Application(Debuggable=true, Label="Sodexo ITZ")]
	#else
	[Application(Debuggable=false, Label="Sodexo ITZ")]
	#endif

	public class SodexoApp : Application
	{
		public SodexoApp (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();

			RegisterManagers ();
			InitFonts ();

			// Handle the events and Save the Managed Exceptions to HockeyApp		
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
				HockeyApp.ManagedExceptionHandler.SaveException (e.ExceptionObject);
			TaskScheduler.UnobservedTaskException += (sender, e) => 
				HockeyApp.ManagedExceptionHandler.SaveException (e.Exception);
		}

		void RegisterManagers ()
		{
			TinyIoCContainer.Current.Register <SDXSettingManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXAuthManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXUserManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXAccountManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXReferenceDataManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXLocationManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXImageManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXDashboardManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXPromotionManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXFeedbackManager> ().AsSingleton ();
			TinyIoCContainer.Current.Register <SDXPriceManager> ().AsSingleton ();
		}

		void InitFonts ()
		{
			Fonts.HelveticaNenueLTStd_Lt = Typeface.CreateFromAsset (Assets, "Fonts/HelveticaNeueLTStd-Lt.otf");
			Fonts.Karla_Bold = Typeface.CreateFromAsset (Assets, "Fonts/Karla-Bold.ttf");
			Fonts.Karla_Regular = Typeface.CreateFromAsset (Assets, "Fonts/Karla-Regular.ttf");
			Fonts.Oswald_Light = Typeface.CreateFromAsset (Assets, "Fonts/Oswald-Light.ttf");
			Fonts.Oswald_Regular = Typeface.CreateFromAsset (Assets, "Fonts/Oswald-Regular.ttf");
		}
	}
}

