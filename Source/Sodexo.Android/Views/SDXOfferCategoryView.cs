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
	public class SDXOfferCategoryView
	{
		public bool IsSelected = false;
		public EventHandler<int> SelectedEvent;

		Activity context;
		public View view;

		ImageView pictureIv;
		TextView nameTv;
		Button btn;

		public static SDXOfferCategoryView Create (Activity context, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.OfferCategoryView, parent, false);

//			(new Handler ()).Post (new Java.Lang.Runnable (() => {
//				LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
//			}));

			SDXOfferCategoryView self = new SDXOfferCategoryView (context, view);

			return self;
		}

		public SDXOfferCategoryView (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			pictureIv = view.FindViewById (Resource.Id.offercategoryview_picture_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.offercategoryview_name_tv) as TextView;

			btn = view.FindViewById (Resource.Id.offercategoryview_btn) as Button;
			btn.Click += OnButton_Clicked;
		}

		public void Update (OfferCategoryModel model)
		{
			nameTv.Text = model.Description;
			var imgName = "img_offer_categories_" + model.OfferCategoryId.ToString ();
			pictureIv.SetImageResource (context.Resources.GetIdentifier (imgName, "drawable", context.PackageName));
		}

		public void Select (bool isSelect)
		{
			IsSelected = isSelect;
			if (isSelect) {
				btn.SetBackgroundColor (Color.Black);
				btn.Background.SetAlpha (127);
			} else {
				btn.SetBackgroundColor (Color.White);
				btn.Background.SetAlpha (0);
			}
		}

		private void OnButton_Clicked (object sender, EventArgs args)
		{
			IsSelected = !IsSelected;
			Select (IsSelected);
			int tag = int.Parse (((string)view.Tag).Substring (0, 2));
			SelectedEvent (this, tag);
		}
	}
}

