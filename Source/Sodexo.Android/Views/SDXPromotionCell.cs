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
	public class SDXPromotionCell : Java.Lang.Object
	{
		Activity context;
		View view;

		ImageView pictureIv, statusIv, categoryIv;
		TextView nameTv, dateTv, descTv;

		public SDXPromotionCell (Activity context, View view) : base ()
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			pictureIv = view.FindViewById (Resource.Id.promotioncell_picture_iv) as ImageView;
			statusIv = view.FindViewById (Resource.Id.promotioncell_status_iv) as ImageView;
			categoryIv = view.FindViewById (Resource.Id.promotioncell_category_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.promotioncell_name_tv) as TextView;
			dateTv = view.FindViewById (Resource.Id.promotioncell_date_tv) as TextView;
			descTv = view.FindViewById (Resource.Id.promotioncell_desc_tv) as TextView;
		}

		public void Update (PromotionModel item)
		{
			Console.WriteLine ("Update : " + item.PromotionId.ToString ());

			pictureIv.Visibility = ViewStates.Gone;
			if (item.Photo != null) {
				pictureIv.Visibility = ViewStates.Visible;
				Util.ChangeViewHeight (pictureIv, (int)((float)item.PhotoId * pictureIv.LayoutParameters.Width / 1000.0f));
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					if (context == null)
						return;
					context.RunOnUiThread (delegate {
						if (bytes == null) {
							pictureIv.Visibility = ViewStates.Gone;
							return;
						}
						using (Bitmap bitmap = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bitmap != null)
								pictureIv.SetImageBitmap (bitmap);
						}
					});
				};
				imageManager.LoadImageAsync (item.Photo.ProcessedPhotoBaseUrl, 600, 0);
			}

			nameTv.Text = item.Title;
			dateTv.Text = "START:" + item.StartDate.ToShortDateString () + "-FINISH:" + item.EndDate.ToShortDateString ();
			if (item.PromotionActivated)
				statusIv.SetImageResource (Resource.Drawable.img_active);
			else
				statusIv.SetImageResource (Resource.Drawable.img_inactive);

			descTv.Text = item.Description;

			if (item.PromotionCategories.Count > 0) {
				var categoryName = item.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower ();
				categoryIv.SetImageResource (context.Resources.GetIdentifier (fileName, "drawable", context.PackageName));
			}
		}
	}
}

