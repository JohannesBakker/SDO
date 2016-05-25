
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
	public partial class SDXOfferCategoryView : UIView
	{
		public static readonly UINib Nib = UINib.FromName ("SDXOfferCategoryView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SDXOfferCategoryView");

		public bool IsSelected = true;
		public EventHandler <int> SelectedEvent;

		public SDXOfferCategoryView (IntPtr handle) : base (handle)
		{
		}

		public static SDXOfferCategoryView Create ()
		{
			return (SDXOfferCategoryView)Nib.Instantiate (null, null) [0];
		}

		public void Update (OfferCategoryModel offerCategory)
		{
			NameLB.Text = offerCategory.Description;
			var imgName = "img_offer_categories_" + offerCategory.OfferCategoryId.ToString () + ".png";
			PictureIV.Image = UIImage.FromBundle (imgName);
			Select (IsSelected);
		}

		public void Select (bool isSelect)
		{
			if (isSelect) {
				Button.BackgroundColor = UIColor.FromWhiteAlpha (0, 0.5f);
				Button.SetImage (UIImage.FromBundle ("selected_checkmark.png"), UIControlState.Normal);
				Button.ImageEdgeInsets = new UIEdgeInsets(0, 30, 30, 0);

			} else {
				Button.BackgroundColor = UIColor.Clear;
				Button.SetImage (null, UIControlState.Normal);
			}
		}

		partial void OnBtn_Pressed (MonoTouch.Foundation.NSObject sender)
		{
			IsSelected = !IsSelected;
			Select (IsSelected);
			SelectedEvent (this, Tag);
		}
	}
}

