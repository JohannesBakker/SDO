// This file has been autogenerated from a class added in the UI designer.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public partial class SDXOfferCell : UITableViewCell
	{
		public SDXOfferCell (IntPtr handle) : base (handle)
		{
		}

		public SDXOfferCell() : base ()
		{

		}

		public void Update (OfferModel offer, bool isNoPlanogram)
		{
			NameLB.Text = offer.Name;
			//previous
			//var imgName = "img_" + offer.OfferCategory.Description.Replace (" ", "_").ToLower () + ".png";

			//currently visible in iphone
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString () + ".png";
			//Console.WriteLine ("now-------"+imgName);
			PictureIV.Image = UIImage.FromBundle (imgName);

			StatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
			if (isNoPlanogram) {
				PlanogramLB.Text = "STATUS:";
				StatusLB.Text = "NOT ACTIVE";
				Util.MoveViewToX (StatusLB, 120);
			}
			if (offer.Responses.Count == 0) {
				StatusLB.Text = "NOT SELECTED";
			} else {
				var response = offer.Responses [0];
				var planogram = response.AnswerNode.Planogram;
				if (!response.PlanogramActivated) {
					StatusLB.Text = planogram.Name +  "/NOT ACTIVE";
				} else {
					StatusLB.Text = planogram.Name + "/ACTIVE";
					StatusLB.TextColor = UIColor.FromRGB (125 / 255.0f, 244 / 255.0f, 146 / 255.0f);
				}
			}
		}
	}
}
