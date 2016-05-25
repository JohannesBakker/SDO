using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;

namespace Sodexo.iOS
{
	public static class Constants
	{
		public const string NotificationMenuPressed 		= "Notification_Menu_Pressed";
		public const string NotificationShowLoadingView 	= "NotificationShowLoadingView";
		public const string NotificationHideLoadingView		= "NotificationHideLoadingView";
		public const string NotificationDeleteOffer 		= "NotificationDeleteOffer";
		public const string NotificationLogout 				= "NotificationLogout";
		public const string NotificationHighlightMenuItem	= "NotificationHighlightMenuItem";

		public const int	AccountCellNavigateBtnTagBase 	= 100;

		// Font Names
		public const string HELVETICA_REGULAR 				= "Helvetica";
		public const string HELVETICA_BOLD					= "Helvetica-Bold";
		public const string HELVETICA_NEUE 					= "HelveticaNeue";
		public const string HELVETICA_NEUE_LIGHT			= "HelveticaNeue-Light";
		public const string HELVETICA_NEUE_MEDIUM 			= "HelveticaNeue-Medium";
		public const string	HELVETICA_NEUE_CONDENSED_BOLD	= "HelveticaNeue-CondensedBold";
		public const string HELVETICA_NEUE_CONDENSED_BLACK  = "HelveticaNeue-CondensedBlack";
		public const string KARLA_BOLD			 = "Karla-Bold";
		public const string KARLA_ITALIC		 = "Karla-Italic";
		public const string KARLA_REGULAR		 = "Karla-Regular";
		public const string OSWALD_BOLD			 = "Oswald-Bold";
		public const string OSWALD_LIGHT		 = "Oswald-Light";
		public const string OSWALD_REGULAR		 = "Oswald";
	}

	public static class Common
	{

	}

	public static class Util
	{
		public static void MoveViewToY (UIView view, float y)
		{
			RectangleF frame = view.Frame;
			frame.Y = y;
			view.Frame = frame;
		}

		public static void MoveViewToX (UIView view, float x)
		{
			RectangleF frame = view.Frame;
			frame.X = x;
			view.Frame = frame;
		}

		public static void ChangeViewWith (UIView view, float w)
		{
			RectangleF frame = view.Frame;
			frame.Width = w;
			view.Frame = frame;
		}

		public static void ChangeViewHeight (UIView view, float h)
		{
			RectangleF frame = view.Frame;
			frame.Height = h;
			view.Frame = frame;
		}

		public static UIImage GetImageFromByteArray (byte[] bytes)
		{
			if (bytes == null)
				return new UIImage ();
			using (NSData imgData = NSData.FromArray (bytes)) {
				return UIImage.LoadFromData (imgData);
			}
		}

		public static byte[] GetByteArrayFromImage (UIImage image)
		{
			using (NSData imageData = image.AsPNG()) {
				Byte[] bytes = new Byte[imageData.Length];
				System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, bytes, 0, Convert.ToInt32(imageData.Length));
				return bytes;
			}
		}

		public static UIImage GetScaledImage (UIImage image, SizeF maxScaleSize)
		{
			bool scaleImage = true;
			if (image.Size.Width < maxScaleSize.Width && image.Size.Height < maxScaleSize.Height)
			{
				scaleImage = false;
			}

			SizeF scaledSize = new SizeF (image.Size);
			if (scaleImage)
			{
				float scaleRate = image.Size.Height / image.Size.Width;
				if (scaledSize.Width >= maxScaleSize.Width) {
					scaledSize.Width = maxScaleSize.Width;
					scaledSize.Height = maxScaleSize.Width * scaleRate;
				}
				if (scaledSize.Height >= maxScaleSize.Height) {
					scaledSize.Width = maxScaleSize.Height / scaleRate;
					scaledSize.Height = maxScaleSize.Height;
				}				
			}

			UIGraphics.BeginImageContext (scaledSize);
			image.Draw(new RectangleF(0, 0, scaledSize.Width, scaledSize.Height)); //this also corrects orientation if needed
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return resultImage;
		}

		public static string GetFileNameFromUrl (string url)
		{
			int num = url.LastIndexOfAny (new char[]{'/', ':'});
			if (num < 0) {
				return url;
			}
			return url.Substring (num + 1);
		}

		public static void ShowAlert (string msg)
		{
//			var appNameAndVersion = string.Format("{0} {1}", NSBundle.MainBundle.InfoDictionary ["CFBundleDisplayName"],
//				NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"]);
			new UIAlertView ("Retail Ranger", msg, null, "OK", null).Show ();
		}

		public static UIImage GetOrientationFixedImage(UIImage aImage) 
		{
			if (aImage.Orientation == UIImageOrientation.Up) 
				return aImage;

			UIGraphics.BeginImageContextWithOptions (aImage.Size, false, aImage.CurrentScale);
			aImage.Draw (new RectangleF (0, 0, aImage.Size.Width, aImage.Size.Height));
			UIImage normalizedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return normalizedImage;
		}
	}
}

