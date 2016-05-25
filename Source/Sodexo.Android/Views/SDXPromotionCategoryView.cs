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
	public class SDXPromotionCategoryView
	{
		private Activity context;
		public View view;

		ImageView pictureIv;
		TextView nameTv;
		Button btn;

		public bool IsSelected = false;
		public EventHandler<int> SelectedEvent;

		public static SDXPromotionCategoryView Create (Activity context, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.PromotionCategoryView, parent, false);

			SDXPromotionCategoryView self = new SDXPromotionCategoryView (context, view);

			return self;
		}

		public SDXPromotionCategoryView (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			pictureIv = view.FindViewById (Resource.Id.promotionview_picture_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.promotionview_name_tv) as TextView;
			btn = view.FindViewById (Resource.Id.promotionview_btn) as Button;

			btn.Click += OnButton_Clicked;
		}

		public void Update (string name)
		{
			nameTv.Text = name;
			var imgName = "promotion_" + name.Replace (" ", "_").ToLower ();
			pictureIv.SetImageResource (context.Resources.GetIdentifier (imgName, "drawable", context.PackageName));
		}

		public void Select (bool isSelect)
		{
			IsSelected = isSelect;
			if (isSelect) {
				btn.SetBackgroundColor (Color.Black);
				btn.Background.SetAlpha (127);
			} else
				btn.SetBackgroundColor (Color.Transparent);
		}

		private void OnButton_Clicked (object sender, EventArgs args)
		{
			Select (!IsSelected);
			int tag = int.Parse (((string)view.Tag).Substring (0, 2));
			SelectedEvent (this, tag);
		}
	}
}

