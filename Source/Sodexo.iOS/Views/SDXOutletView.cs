
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXOutletView : UIView
	{
		public static readonly UINib Nib = UINib.FromName ("SDXOutletView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SDXOutletView");

		public SDXOutletView (IntPtr handle) : base (handle)
		{
		}

		public static SDXOutletView Create ()
		{
			return (SDXOutletView)Nib.Instantiate (null, null) [0];
		}

		public void Update (OutletModel outlet)
		{
			NameLB.Text = outlet.Name;

			if (outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (bytes == null || PictureIV == null)
							return;
						var img = Util.GetImageFromByteArray (bytes);
						PictureIV.Image = img;
					});
				};
				imgManager.LoadImageAsync (outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}
		}
	}
}

