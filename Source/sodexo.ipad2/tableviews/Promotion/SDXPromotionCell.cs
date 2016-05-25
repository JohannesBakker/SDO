
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.Ipad2
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

			NameLB.Text = item.Title.ToUpper();
			NameLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 17);

			DateLB.Text = ("START:" + item.StartDate.ToShortDateString () + "-FINISH:" + item.EndDate.ToShortDateString ()).ToUpper();
			DateLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 9);

			if (item.PromotionActivated)
				statusIV.Image = UIImage.FromBundle ("img_active.png");
			else
				statusIV.Image = UIImage.FromBundle ("img_inactive.png");

			DescriptionLB.Text = item.Description;
			DescriptionLB.Font = UIFont.FromName (Constants.HELVETICA_NEUE_LIGHT, 9);

			float textH = Util.GetHeightOfString (item.Description, DescriptionLB.Frame.Width, DescriptionLB.Font) + 10;
			Util.ChangeViewHeight (DescriptionLB, textH);

			float h = DescriptionLB.Frame.Y + DescriptionLB.Frame.Height + 5;

			RectangleF frame = MainView.Frame;
			frame.Y = y;
			frame.Height = h;
			MainView.Frame = frame;

			if (item.PromotionCategories.Count > 0) {
				var categoryName = item.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower () + ".png";
				CategoryIV.Image = UIImage.FromBundle (fileName);

				var highlightedFileName = "promotion_" + categoryName.Replace (" ", "_").ToLower () + "_on.png";
				CategoryIV.HighlightedImage = UIImage.FromBundle (highlightedFileName);
			}
		}



		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			if (selected) {
				NameLB.TextColor = UIColor.White;
				DescriptionLB.TextColor = UIColor.White;
				DateLB.TextColor = UIColor.White;
				MainView.BackgroundColor = UIColor.FromRGB (88, 88, 88);
				HeaderBg.Alpha = 0;

				CategoryIV.Highlighted = true;

			} else {

				NameLB.TextColor = UIColor.FromRGB(60,60,60);
				DescriptionLB.TextColor = UIColor.FromRGB(60,60,60);
				DateLB.TextColor = UIColor.FromRGB(60,60,60);
				MainView.BackgroundColor = UIColor.White;
				HeaderBg.Alpha = 1;

				CategoryIV.Highlighted = false;
			}
		}
	}
}
