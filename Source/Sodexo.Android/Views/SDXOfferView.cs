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

namespace Sodexo.Android
{
	public class SDXOfferView
	{
		Activity context;
		public View view;
		public EventHandler <int> OfferSelectedEvent;
		public EventHandler <int> DeleteOfferEvent;
		public int OutletIdx;

		int offerIdx;

		TextView nameTv, statusTv;
		ImageView pictureIv;

		public SDXOfferView (Activity context, View view, int offerIdx)
		{
			this.context = context;
			this.view = view;
			this.offerIdx = offerIdx;

			GetInstance ();
		}

		public static SDXOfferView Create (Activity context, ViewGroup parent, int offerIdx)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.Offer_View, parent, false);

			(new Handler ()).Post (new Java.Lang.Runnable (() => {
				context.RunOnUiThread (() => {
					LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
				});
			}));

			SDXOfferView self = new SDXOfferView (context, view, offerIdx);

			return self;
		}

		private void GetInstance ()
		{
			nameTv = view.FindViewById (Resource.Id.offerview_name_tv) as TextView;
			pictureIv = view.FindViewById (Resource.Id.offerview_picture_iv) as ImageView;
			statusTv = view.FindViewById (Resource.Id.offerview_status_tv) as TextView;

			var navigateBtn = view.FindViewById (Resource.Id.offerview_navigate_btn) as Button;
			navigateBtn.Tag = offerIdx.ToString ("00") + "-" + navigateBtn.Tag;
			navigateBtn.Click += NavigateBtn_OnClicked;
			navigateBtn.LongClickable = true;
			navigateBtn.LongClick += NavigateBtn_LongClicked;
		}

		public void Update (OfferModel offer)
		{
			nameTv.Text = offer.Name;

			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString ();
			pictureIv.SetImageResource (context.Resources.GetIdentifier (imgName, "drawable", context.PackageName));

			statusTv.SetTextColor (Color.Rgb (233, 70, 122));
			if (offer.Responses.Count == 0) {
				statusTv.Text = "NOT SELECTED";
			} else {
				var response = offer.Responses [0];
				var planogram = response.AnswerNode.Planogram;
				if (!response.PlanogramActivated) {
					statusTv.Text = planogram.Name +  "/NOT ACTIVE";
				} else {
					statusTv.Text = planogram.Name + "/ACTIVE";
					statusTv.SetTextColor (Color.Rgb (125, 244, 146));
				}
			}
		}

		private void NavigateBtn_OnClicked (object sender, EventArgs e)
		{
			if (OfferSelectedEvent != null)
				OfferSelectedEvent (sender, OutletIdx);
		}

		private void NavigateBtn_LongClicked (object sender, View.LongClickEventArgs e)
		{
			if (DeleteOfferEvent != null)
				DeleteOfferEvent (sender, OutletIdx);
		}
	}
}

