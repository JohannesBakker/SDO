using System;
using System.IO;


using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using Android.Graphics;
using Android.Database;
using Android.Provider;
using Uri = Android.Net.Uri;
using Java.Nio;
using Android.Media;

namespace Sodexo.Android
{
	public static class Constants
	{
		public const string HOCKEYAPP_APPID = "aa8e31eca75393e448b6d3cae40d58e4";//"f435f30b3f82f1527fd29d8bc30ba54a";

		// Error Messages
		public const string ERROR_NETWORK = "Network Error";


	}

	public enum  AnimationType {
		TransitionToLeft = 0,
		TransitionToRight,
		FadeIn,
		FadeOut
	}

	public static class Data
	{
		// Screen Size Rate & Density
		public static float XRate = 1.0f;
		public static float YRate = 1.0f;
		public static float WRate = 1.0f;
		public static float HRate = 1.0f;
		public static float Density = 24.0f;
	}

	public static class Fonts
	{
		public static Typeface HelveticaNenueLTStd_Lt;
		public static Typeface Karla_Bold;
		public static Typeface Karla_Regular;
		public static Typeface Oswald_Light;
		public static Typeface Oswald_Regular;
	}

	public static class Util
	{
		static ProgressDialog loadingDlg;
		public static void ShowAlert (string msg, Context context)
		{
			var builder = new AlertDialog.Builder (context);
			builder.SetTitle ("Retail Ranger");
			builder.SetMessage (msg);
			builder.SetPositiveButton ("Ok", (o, e) => {});
			builder.Create().Show();
		}

		public static void MoveViewToX (View view, int xPos)
		{
			var param = (ViewGroup.MarginLayoutParams) view.LayoutParameters;
			param.LeftMargin = xPos;
			view.LayoutParameters = param;
		}

		public static void MoveViewToY (View view, int yPos)
		{
			var param = (ViewGroup.MarginLayoutParams) view.LayoutParameters;
			param.TopMargin = yPos;
			view.LayoutParameters = param;
		}

		public static void ChangeViewWidth (View view, int width)
		{
			var param = view.LayoutParameters;
			param.Width = width;
			view.LayoutParameters = param;
		}

		public static void ChangeViewHeight (View view, int height)
		{
			var param = view.LayoutParameters;
			param.Height = height;
			view.LayoutParameters = param;
		}

		public static void ShowLoading (Context context)
		{
			if (loadingDlg != null) {
//				if (!loadingDlg.IsShowing)
//					loadingDlg.Show ();
				return;
			}
//			loadingDlg = new ProgressDialog (context, AlertDialog.ThemeTraditional);
//			loadingDlg.RequestWindowFeature (0);
//			loadingDlg.Window.SetBackgroundDrawable (new global::Android.Graphics.Drawables.ColorDrawable (Color.Transparent));
//			loadingDlg.SetCancelable (false);

			loadingDlg = new ProgressDialog(context);
			try {
				loadingDlg.Show ();
			}catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			loadingDlg.SetCancelable(false);
//			loadingDlg.SetCanceledOnTouchOutside (false);
			loadingDlg.SetContentView(Resource.Layout.progressdlg);

//			loadingDlg.Show ();
		}

		public static void HideLoading ()
		{
			if (loadingDlg == null)
				return;
			loadingDlg.Dismiss ();
			loadingDlg = null;
		}

		public static void StartFadeAnimation (View view, long duration)
		{
			Animation fadeIn = new AlphaAnimation (0, 1);
			fadeIn.Interpolator = new DecelerateInterpolator ();
			fadeIn.Duration = duration;
			view.StartAnimation (fadeIn);
		}

		public static void StartFadeAnimation (View view, long duration, float startAlpha, float endAlpha)
		{
			Animation fadeIn = new AlphaAnimation (startAlpha, endAlpha);
			fadeIn.Interpolator = new DecelerateInterpolator ();
			fadeIn.Duration = duration;
			view.StartAnimation (fadeIn);
		}

		public static byte[] GetByteArrayFromBitmap (Bitmap bitmap)
		{
			MemoryStream stream = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
			byte[] bitmapData = stream.ToArray();

			return bitmapData;
		}

		public static Bitmap GetScaledBitmap(Bitmap realImage, float maxImageSize, bool filter = true) 
		{
			float ratio = Math.Min(
				(float) maxImageSize / realImage.Width,
				(float) maxImageSize / realImage.Height);
			int width = (int)Math.Round((float) ratio * realImage.Width);
			int height = (int)Math.Round((float) ratio * realImage.Height);

			Bitmap newBitmap = Bitmap.CreateScaledBitmap (realImage, width, height, filter);
			realImage.Recycle ();

			return newBitmap;
		}

		public static Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
		{
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile (fileName, options);

			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width)
			{
				inSampleSize = Math.Max (outHeight / height, outWidth / width);
//				inSampleSize = outWidth > outHeight
//					? outHeight / height
//					: outWidth / width;
			}

			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}

		public static Bitmap GetOrientedBitmap (Bitmap bmp, string path)
		{
			Bitmap resultBmp = bmp;
			try {
				ExifInterface exif = new ExifInterface (path);
				int orientation = exif.GetAttributeInt (ExifInterface.TagOrientation, (int)global::Android.Media.Orientation.Normal);
				int rotation = 0;
				if (orientation == (int)global::Android.Media.Orientation.Rotate90) rotation = 90;
				else if (orientation == (int)global::Android.Media.Orientation.Rotate180) rotation = 180;
				else if (orientation == (int)global::Android.Media.Orientation.Rotate270) rotation = 270;
				if (rotation != 0) {
					Matrix mat = new Matrix ();
					mat.PostRotate (rotation);
					Bitmap rotatedBmp = Bitmap.CreateBitmap (bmp, 0, 0, bmp.Width, bmp.Height, mat, true);
					bmp.Recycle ();
					resultBmp = rotatedBmp;
				}
			}catch (Exception e) {
				Console.WriteLine ("Error in rotate bitmap : " + e.Message);
			}
			return resultBmp;
		}

		public static string GetPathFromImageUri (Activity context, Uri uri)
		{
			string path = null;
			// The projection contains the columns we want to return in our query.
			string[] projection = new[] { MediaStore.Images.Media.InterfaceConsts.Data };
			using (ICursor cursor = context.ManagedQuery(uri, projection, null, null, null))
			{
				if (cursor != null)
				{
					int columnIndex = cursor.GetColumnIndexOrThrow (MediaStore.Images.Media.InterfaceConsts.Data);
					cursor.MoveToFirst();
					path = cursor.GetString(columnIndex);
				}
			}
			return path;
		}

		public static void AddEvent(Context ctx, String title, String desc, DateTime start, DateTime end)
		{
			var intent = new Intent(Intent.ActionEdit);
			intent.SetType ("vnd.android.cursor.item/event");
			intent.PutExtra ("title", title);
			intent.PutExtra ("beginTime", TimeInMillis(start));
			intent.PutExtra ("endTime", TimeInMillis(end));
			intent.PutExtra ("allDay", false);
			intent.PutExtra ("description", desc);
			ctx.StartActivity(intent);
		}

		private readonly static DateTime jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static Int64 TimeInMillis(DateTime dateTime)
		{
			return (Int64)(dateTime - jan1970).TotalMilliseconds;
		}
	}
}

