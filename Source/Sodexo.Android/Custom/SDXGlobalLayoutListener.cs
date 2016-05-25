using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;

namespace Sodexo.Android
{
	public class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		Context context;
		View view;
		bool isLayoutAdjusted = false;

		public GlobalLayoutListener (Context context, View view)
		{
			this.context = context;
			this.view = view;
		}

		public void OnGlobalLayout()
		{
			if (isLayoutAdjusted)
				return;

			LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
			isLayoutAdjusted = true;
		}
	}
}

