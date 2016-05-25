using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	public partial class SDXFeedbackPlanogramCell : UITableViewCell
	{
		OfferModel offer;
		UIColor offerColor;

		public SDXFeedbackPlanogramCell (IntPtr handle) : base (handle)
		{
		}

		public SDXFeedbackPlanogramCell () : base ()
		{
		}

		public void Update (OfferModel item)
		{
			offer = item;

			OfferNameLb.Text = item.Name;
			OfferNameLb.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 14);

			var imgName = "img_offer_categories_" + item.OfferCategory.OfferCategoryId.ToString () + ".png";
			PictureIV.Image = UIImage.FromBundle (imgName);

			StatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
			StatusLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);

			plainLb.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 12);

			if (item.Responses.Count == 0) {
				StatusLB.Text = "NOT SELECTED";
			} else {
				var response = item.Responses [0];
				var planogram = response.AnswerNode.Planogram;
				if (!response.PlanogramActivated) {
					StatusLB.Text = planogram.Name +  "/NOT ACTIVE";
				} else {
					StatusLB.Text = planogram.Name + "/ACTIVE";
					StatusLB.TextColor = UIColor.FromRGB (125 / 255.0f, 244 / 255.0f, 146 / 255.0f);
				}
			}

			offerColor = StatusLB.TextColor;
		}



		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString () + ".png";

			if (selected) {
				OfferNameLb.TextColor = UIColor.White;
				plainLb.TextColor = UIColor.White;
				StatusLB.TextColor = UIColor.White;
				MainView.BackgroundColor = UIColor.FromRGB (88, 88, 88);
				PictureIV.Image = Util.GetColoredImage (imgName, UIColor.White);
				arrow.Image = Util.GetColoredImage ("icon_right_arrow.png", UIColor.White);

			} else {

				OfferNameLb.TextColor = UIColor.FromRGB(60,60,60);
				plainLb.TextColor = UIColor.FromRGB(60,60,60);
				MainView.BackgroundColor = UIColor.White;
				StatusLB.TextColor = offerColor;
				PictureIV.Image = UIImage.FromBundle (imgName);
				arrow.Image = UIImage.FromBundle ("icon_right_arrow.png");
			}
		}

//		private UIImage GetColoredImage(string imageName, UIColor color)
//		{
//			UIImage image = UIImage.FromBundle(imageName);
//			UIImage coloredImage = null;
//
//			UIGraphics.BeginImageContext(image.Size);
//			using (CGContext context = UIGraphics.GetCurrentContext())
//			{
//
//				context.TranslateCTM(0, image.Size.Height);
//				context.ScaleCTM(1.0f, -1.0f);
//
//				var rect = new RectangleF(0, 0, image.Size.Width, image.Size.Height);
//
//				// draw image, (to get transparancy mask)
//				context.SetBlendMode(CGBlendMode.Normal);
//				context.DrawImage(rect, image.CGImage);
//
//				// draw the color using the sourcein blend mode so its only draw on the non-transparent pixels
//				context.SetBlendMode(CGBlendMode.SourceIn);
//				context.SetFillColor(color.CGColor);
//				context.FillRect(rect);
//
//				coloredImage = UIGraphics.GetImageFromCurrentImageContext();
//				UIGraphics.EndImageContext();
//			}
//			return coloredImage;
//		}
	}
}
