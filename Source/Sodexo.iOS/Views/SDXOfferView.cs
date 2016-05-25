
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
	public partial class SDXOfferView : UIView
	{
		public static readonly UINib Nib = UINib.FromName ("SDXOfferView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SDXOfferView");

		public EventHandler <int> OfferSelected;

		private int outletIndex;
		public SDXOfferView (IntPtr handle) : base (handle)
		{
		}

		public static SDXOfferView Create ()
		{
			return (SDXOfferView)Nib.Instantiate (null, null) [0];
		}

		public void Update (OfferModel offer, int offerIndex, int outletIndex)
		{
			this.outletIndex = outletIndex;

			NameLB.Text = offer.Name;

			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString () + ".png";
			PictureIV.Image = UIImage.FromBundle (imgName);

			StatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
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

			NavigationBtn.Tag = offerIndex;
		}

		partial void OnNavigationBtn_Pressed (MonoTouch.Foundation.NSObject sender)
		{
			OfferSelected (sender, outletIndex);
		}
	}
}

