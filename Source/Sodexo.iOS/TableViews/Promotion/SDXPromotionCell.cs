
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.iOS
{
	public partial class SDXPromotionCell : UITableViewCell
	{
		private PromotionModel promotion;

		public SDXPromotionCell (IntPtr handle) : base (handle)
		{
		}

		public SDXPromotionCell () : base ()
		{
			
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			Console.WriteLine ("LayoutSubviews : " + promotion.PromotionId.ToString ());

			if (promotion == null)
				return;

			float y = 0;
			if (promotion.PhotoId != null) {
				Util.ChangeViewHeight (PictureIV, ((float)promotion.PhotoId * 300 / 1000.0f));
				y = PictureIV.Frame.Height;
			}
			RectangleF frame = MainView.Frame;
			frame.Y = y;
			frame.Height = ContentView.Frame.Height - y - 10;
			MainView.Frame = frame;
		}

		public void Update (PromotionModel item)
		{
			promotion = item;

			Console.WriteLine ("Update : " + item.PromotionId.ToString ());

			float y = 0;
			PictureIV.Hidden = true;
			if (item.Photo != null) {
				PictureIV.Hidden = false;
				Util.ChangeViewHeight (PictureIV, ((float)item.PhotoId * 300 / 1000.0f));
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						PictureIV.Alpha = 0.5f;
						PictureIV.Image = Util.GetImageFromByteArray (bytes);
						UIView.Animate (0.5f, () => {
							PictureIV.Alpha = 1;
						});
					});
				};
				imageManager.LoadImageAsync (item.Photo.ProcessedPhotoBaseUrl, 600, 0);

				y += PictureIV.Frame.Height;
			}

			NameLB.Text = item.Title;
			DateLB.Text = "START:" + item.StartDate.ToShortDateString () + "-FINISH:" + item.EndDate.ToShortDateString ();
			if (item.PromotionActivated)
				statusIV.Image = UIImage.FromBundle ("img_active.png");
			else
				statusIV.Image = UIImage.FromBundle ("img_inactive.png");

			DescriptionLB.Text = item.Description;
			DescriptionLB.SizeToFit ();

			float h = DescriptionLB.Frame.Y + DescriptionLB.Frame.Height + 5;

			RectangleF frame = MainView.Frame;
			frame.Y = y;
			frame.Height = h;
			MainView.Frame = frame;

			if (item.PromotionCategories.Count > 0) {
				var categoryName = item.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower () + ".png";
				CategoryIV.Image = UIImage.FromBundle (fileName);
			}
		}

		public float GetHeightOfCell (PromotionModel item)
		{
			float h = 0;
			if (item.Photo != null) {
				h += item.PhotoId != null ? (float)(item.PhotoId * 300 / 1000.0f) : 0;
			}

			h += 3 + 60 + 5;
			DescriptionLB.Text = item.Description;
			DescriptionLB.SizeToFit ();
			h += DescriptionLB.Frame.Height + 5;
			
			return h + 10;
		}
	}
}

