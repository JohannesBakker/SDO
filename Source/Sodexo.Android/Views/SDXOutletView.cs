
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Views.Animations;
using Android.Graphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;


namespace Sodexo.Android
{
	public class SDXOutletView
	{
		Activity context;
		public View view;

		public SDXOutletView (Activity context, View view)
		{
			this.context = context;
			this.view = view;
		}

		public static SDXOutletView Create (Activity context, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.Outlet_View, parent, false);

			(new Handler ()).Post (new Java.Lang.Runnable (() => {
				context.RunOnUiThread (() => {
					LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
				});
			}));

			SDXOutletView self = new SDXOutletView (context, view);

			return self;
		}

		public void Update (OutletModel outlet)
		{
			var nameTv = view.FindViewById (Resource.Id.outlet_view_name_tv) as TextView;
			var pictureIv = view.FindViewById (Resource.Id.outlet_view_picture_iv) as ImageView;

			nameTv.Text = outlet.Name;

			if (outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					context.RunOnUiThread (delegate {
						if (bytes == null || pictureIv == null)
							return;
						using (var bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Count ())) {
							if (bmp != null)
								pictureIv.SetImageBitmap (bmp);
						}
					});
				};
				imgManager.LoadImageAsync (outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}
		}
	}
}

