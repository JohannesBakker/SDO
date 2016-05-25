
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Ipad2
{
	public partial class SDXPlanogramCell : UITableViewCell
	{
		public EventHandler<int> OfferSelected;

		public SDXPlanogramCell (IntPtr handle) : base (handle)
		{
		}

		public SDXPlanogramCell ()
		{

		}

		public void Update (OutletModel outlet, int outletIndex)
		{
			for (int i = 0;; i++) {
				var view = ContentView.ViewWithTag (i + 1);
				if (view != null)
					view.RemoveFromSuperview ();
				else
					break;
			}

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

			for (int i = 0; i < outlet.Offers.Count; i++) {
				var view = SDXOfferView.Create ();

				view.Update (outlet.Offers [i], i, outletIndex);
				view.OfferSelected = OfferSelected;

				RectangleF frame = view.Frame;
				frame.X = 10;
				frame.Y = i * 50 + 65;
				view.Frame = frame;

				view.Tag = i + 1;
				ContentView.AddSubview (view);
			}
		}

		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			if (selected) {
				NameLB.TextColor = UIColor.White;
				MainView.BackgroundColor = UIColor.FromRGB (88, 88, 88);
				PictureIV.TintColor = UIColor.White;

			} else {

				NameLB.TextColor = UIColor.FromRGB(60,60,60);
				MainView.BackgroundColor = UIColor.White;
				PictureIV.TintColor = UIColor.Clear;
			}
		}
	}
}

