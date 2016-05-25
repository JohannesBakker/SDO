﻿
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.iOS
{
	public partial class SDXOutletCell : UITableViewCell
	{
		bool isAddDetailViewAppear = false;
		List <OfferModel> offersNo = new List<OfferModel> ();
		List <OfferModel> offersWith = new List<OfferModel> ();
		IList <OfferCategoryModel> offerCategories = null;

		public EventHandler<OfferModel> OfferRowSelectedEvent;
		public int SelectedOfferCategoryIdx = -1;

		public SDXOutletCell (IntPtr handle) : base (handle)
		{

		}

		public SDXOutletCell () : base ()
		{

		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			Console.WriteLine ("AwakeFromNib");

			TableView.DataSource = new SDXOfferTableViewDataSource ();
			TableView.Delegate = new SDXOfferTableViewDelegate ();
			TableView.ScrollEnabled = false;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			UpdateAddDetailView ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			System.Console.WriteLine ("LayoutSubviews");

			float fHeightAdditional = 0;
			if (offersWith.Count != 0)
				fHeightAdditional += 30;
			if (offersNo.Count != 0)
				fHeightAdditional += 30;

			RectangleF frame = TableView.Frame;
			frame.Height = fHeightAdditional + (offersWith.Count + offersNo.Count) * 50;
			TableView.Frame = frame;
			ContentView.BringSubviewToFront (TableView);

			if (isAddDetailViewAppear) {
				AddDetailView.Hidden = false;
				AddDetailView.Frame = new RectangleF (10, frame.Y + frame.Height, 300, AddDetailView.Frame.Height);
			} else {
				AddNewView.Hidden = false;
				AddNewView.Frame = new RectangleF (10, frame.Y + frame.Height, 300, AddNewView.Frame.Height);
			}
		}

		public void Update (int index, OutletModel outlet, bool isAddDetailViewAppear)
		{
			System.Console.WriteLine ("Update");
		
			SelectedOfferCategoryIdx = -1;

			AddDetailView.Hidden = true;
			AddNewView.Hidden = true;

			PictureIV.Image = UIImage.FromBundle ("outlet_default.png");
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
			NameLB.Text = outlet.Name;

			this.isAddDetailViewAppear = isAddDetailViewAppear;

			offersWith.Clear ();
			offersNo.Clear ();

			foreach (OfferModel offer in outlet.Offers) {
				if (offer.OfferCategory.RequiresPlanogram)
					offersWith.Add (offer);
				else
					offersNo.Add (offer);
				if (offerCategories == null)
					continue;
			}

			var tableDataSource = (SDXOfferTableViewDataSource) TableView.DataSource;
			tableDataSource.SetOffers (offersWith, offersNo);
			var tableDelegate = (SDXOfferTableViewDelegate) TableView.Delegate;
			tableDelegate.SetOffers (offersWith, offersNo);
			tableDelegate.RowSelectedEvent = OfferRowSelectedEvent;
			TableView.ReloadData ();

			AddNewBtn.Tag = 100 + index;
			DetailAddBtn.Tag = 200 + index;
			DetailCancelBtn.Tag = 300 + index;
			EditBtn.Tag = 400 + index;

			UpdateAddDetailView ();
		}

		private void UpdateAddDetailView ()
		{
			if (offerCategories != null) {
				for (int i = 0; i < 8; i++) {
					UIButton btn = AddDetailView.ViewWithTag (30 + i) as UIButton;
					btn.BackgroundColor = UIColor.Clear;
				}
				return;
			}

			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				return;
			}
			offerCategories = manager.DataModel.OfferCategories;

			// For AddOffer View
			for (int i = 0; i < 8; i++) {
				UIImageView iv = AddDetailView.ViewWithTag (10 + i) as UIImageView;
				UILabel lb = AddDetailView.ViewWithTag (20 + i) as UILabel;
				UIButton btn = AddDetailView.ViewWithTag (30 + i) as UIButton;

				btn.BackgroundColor = UIColor.Clear;
				if (i >= offerCategories.Count) {
					iv.Image = null;
					lb.Text = "";
					btn.Enabled = false;
				} else {
					string imgName = "img_offer_categories_" + offerCategories [i].OfferCategoryId.ToString () + ".png";
					iv.Image = UIImage.FromBundle (imgName);
					lb.Text = offerCategories [i].Description;
					btn.Enabled = true;
				}
			}
		}

		partial void OnOfferCategoryBtn_Pressed (UIButton sender)
		{
			if (SelectedOfferCategoryIdx != -1) {
				UIButton preSelectedBtn = AddDetailView.ViewWithTag (30 + SelectedOfferCategoryIdx) as UIButton;
				preSelectedBtn.BackgroundColor = UIColor.Clear;
			}
			UIButton selectedBtn = sender as UIButton;
			selectedBtn.BackgroundColor = UIColor.FromWhiteAlpha (0, 0.5f);
			SelectedOfferCategoryIdx = selectedBtn.Tag - 30;
		}
	}
}

