using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Views.Animations;
using Android.Graphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Android
{
	public class SDXOutletDetailCell : Java.Lang.Object
	{
		Activity context;
		View view;

		FrameLayout offerCategoryView, addNewView;
		LinearLayout addDetailView, offersView;
		ImageView pictureIv;
		TextView nameTv;
		Button addOfferBtn;

		bool isOfferCategoryViewAppeared = false;

		IList <OfferCategoryModel> offerCategories;
		List <SDXOfferCategoryView> viewList = new List<SDXOfferCategoryView> ();

		private int selectedOfferCategoryIdx = -1;

		public EventHandler <int> OfferSelectedEvent;
		public EventHandler <int> OutletEditEvent;
		public EventHandler <int> AddOfferEvent;
		public EventHandler <int> DeleteOfferEvent;

		int outletIdx;

		public SDXOutletDetailCell (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();

			FillOfferCategoriesView ();
		}

		private void GetInstance ()
		{
			addDetailView = view.FindViewById (Resource.Id.outletdetail_offercategories_ll) as LinearLayout;
			offerCategoryView = view.FindViewById (Resource.Id.outletdetail_offercategories_fl) as FrameLayout;
			addNewView = view.FindViewById (Resource.Id.outletdetail_addnew_fl) as FrameLayout;
			pictureIv = view.FindViewById (Resource.Id.outletdetail_picture_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.outletdetail_name_tv) as TextView;
			offersView = view.FindViewById (Resource.Id.outletdetail_offers_ll) as LinearLayout;

			var addNewBtn = view.FindViewById (Resource.Id.outletdetail_addnew_btn) as Button;
			addNewBtn.Click += AddNewBtn_OnClicked;

			var cancelBtn = view.FindViewById (Resource.Id.outletdetail_cancel_btn) as Button;
			cancelBtn.Click += CancelBtn_OnClicked;

			addOfferBtn = view.FindViewById (Resource.Id.outletdetail_addoffer_btn) as Button;
			addOfferBtn.Click += AddOfferBtn_OnClicked;

			var editBtn = view.FindViewById (Resource.Id.outletdetail_edit_imgbtn) as ImageButton;
			editBtn.Click += EditBtn_OnClicked;
		}

		public void Update (OutletModel outlet, int outletIdx)
		{
			this.outletIdx = outletIdx;

			if (isOfferCategoryViewAppeared) {
				addDetailView.Visibility = ViewStates.Visible;
				addNewView.Visibility = ViewStates.Gone;
			} else {
				addDetailView.Visibility = ViewStates.Gone;
				addNewView.Visibility = ViewStates.Visible;
			}

			pictureIv.SetImageResource (Resource.Drawable.outlet_default);
			if (outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					context.RunOnUiThread (delegate {
						if (bytes == null)
							return;
						using (var bitmap = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bitmap != null)
								pictureIv.SetImageBitmap (bitmap);
						}
					});
				};
				imgManager.LoadImageAsync (outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}
			nameTv.Text = outlet.Name;

			AddOffers (outlet.Offers);

			addOfferBtn.Tag = outletIdx.ToString ("00") + "-sw-sh-sfont-or";
		}

		private void AddNewBtn_OnClicked (object sender, EventArgs args)
		{
			isOfferCategoryViewAppeared = true;
			addNewView.Visibility = ViewStates.Gone;
			addDetailView.Visibility = ViewStates.Visible;
		}

		private void CancelBtn_OnClicked (object sender, EventArgs arg)
		{
			isOfferCategoryViewAppeared = false;
			addNewView.Visibility = ViewStates.Visible;
			addDetailView.Visibility = ViewStates.Gone;
		}

		private void AddOfferBtn_OnClicked (object sender, EventArgs arg)
		{
			if (selectedOfferCategoryIdx < 0)
				return;
			AddOfferEvent (sender, selectedOfferCategoryIdx);
			CancelBtn_OnClicked (null, null);
		}

		private void CategoryBtn_OnClicked (object sender, int tag)
		{
			if (selectedOfferCategoryIdx != -1) {
				viewList [selectedOfferCategoryIdx].Select (false);
			}
			viewList [tag].Select (true);
			selectedOfferCategoryIdx = tag;
		}

		private void EditBtn_OnClicked (object sender, EventArgs arg)
		{
			OutletEditEvent (sender, outletIdx);
		}

		private void AddOffers (IList<OfferModel> offers)
		{
			offersView.RemoveAllViews ();
			int i = 0;
			foreach (OfferModel offer in offers) {
				SDXOfferView offerV = SDXOfferView.Create (context, (ViewGroup)view, i++);
				offerV.OfferSelectedEvent = OfferSelectedEvent;
				offerV.DeleteOfferEvent = DeleteOfferEvent;
				offerV.OutletIdx = outletIdx;
				offerV.Update (offer);
				offersView.AddView (offerV.view);
			}
		}
		
		private void FillOfferCategoriesView ()
		{
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				return;
			}
			offerCategories = manager.DataModel.OfferCategories;
			for (int i = 0; i < offerCategories.Count; i++) {
				OfferCategoryModel model = offerCategories [i];
				SDXOfferCategoryView categoryV = SDXOfferCategoryView.Create (context, (ViewGroup)view);
				categoryV.view.Tag = i.ToString ("00") + "-" + categoryV.view.Tag;
				categoryV.SelectedEvent = CategoryBtn_OnClicked;
				offerCategoryView.AddView (categoryV.view);
				var orgParams = (FrameLayout.LayoutParams) categoryV.view.LayoutParameters;
				orgParams.LeftMargin = (int)((i % 4) * (150));
				orgParams.TopMargin = (int)((i / 4) * (150));
				orgParams.Width = (int)((i % 4) == 3 ? 150 : 148);
				orgParams.Height = (int)(148);
				categoryV.view.LayoutParameters = orgParams;
				categoryV.view.Tag += "-sl-st-glt";
				categoryV.Update (model);
				viewList.Add (categoryV);
			}
		}
	}
}

