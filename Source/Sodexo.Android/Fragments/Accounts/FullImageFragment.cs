
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;
using Android.Media;
using Android.Webkit;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;
namespace Sodexo.Android
{
	public class FullImageFragment : SDXBaseFragment
	{
		View view;

		public Bitmap bitmap;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.FullImage, container, false);

			LayoutView (view);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			LoadImage ();
		}

		private void LoadImage ()
		{
			String html="<html><body><img src='{IMAGE_URL}' /></body></html>";

			Console.WriteLine ("Bitmap Size = " + bitmap.Width.ToString () + "-" + bitmap.Height.ToString ());

			byte[] bytes = Util.GetByteArrayFromBitmap (bitmap);
			String imgageBase64 = Base64.EncodeToString(bytes, Base64Flags.Default);
			String image = "data:image/png;base64," + imgageBase64;

			html = html.Replace("{IMAGE_URL}", image);
			var webView = view.FindViewById (Resource.Id.fullimage_webview) as WebView;
			webView.Settings.SetSupportZoom (true);
			webView.Settings.BuiltInZoomControls = true;
			webView.LoadDataWithBaseURL ("file:///android_asset/", html, "text/html", "utf-8", "");
		}
	}
}

