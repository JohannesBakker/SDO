
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Sodexo.Android
{
	[Activity (Label = "BaseActivity")]

	public class BaseActivity : Activity
	{
		protected bool isViewAdjusted = false;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//Register to with the Update Manager
			HockeyApp.UpdateManager.Register (this, Constants.HOCKEYAPP_APPID);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			//Register for Crash detection / handling
			// You should do this in your main activity
			HockeyApp.CrashManager.Register (this, Constants.HOCKEYAPP_APPID);

			//Start Tracking usage in this activity
			HockeyApp.Tracking.StartUsage (this);
		}

		protected override void OnPause ()
		{
			//Stop Tracking usage in this activity
			HockeyApp.Tracking.StopUsage (this);

			base.OnPause ();
		}
	}
}

