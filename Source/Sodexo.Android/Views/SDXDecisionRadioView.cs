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
	public class SDXDecisionRadioView
	{
		Activity context;
		public View view;
		TextView descTv;
		ImageView pictureIv;
		ImageView radioIv;

		public static SDXDecisionRadioView Create (Activity context, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.DecisionTreeRadioView, parent, false);

			SDXDecisionRadioView self = new SDXDecisionRadioView (context, view);

			return self;
		}

		public SDXDecisionRadioView (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			descTv = view.FindViewById (Resource.Id.radioview_desc_tv) as TextView;
			pictureIv = view.FindViewById (Resource.Id.radioview_picture_iv) as ImageView;
			radioIv = view.FindViewById (Resource.Id.radioview_radio_iv) as ImageView;
		}

		public void Update (DecisionTreeNodeModel node, int index)
		{
			int fX = node.Photo == null ? 80 : 150;
			descTv.SetPadding (fX, descTv.PaddingTop, descTv.PaddingRight, descTv.PaddingBottom);
			descTv.Text = node.Text;

			if (node.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object s, byte[] bytes) => {
					context.RunOnUiThread (delegate {
						if (bytes == null) {
							pictureIv.Visibility = ViewStates.Gone;
							return;
						}
						using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bmp != null)
								pictureIv.SetImageBitmap (bmp);
						}
					});
				};
				imgMgr.LoadImageAsync (node.Photo.ProcessedPhotoBaseUrl, pictureIv.Width, pictureIv.Height);
			} else
				pictureIv.Visibility = ViewStates.Gone;
		}

		public void Select (bool bSelect)
		{
			if (bSelect) {
				radioIv.SetImageResource (Resource.Drawable.radiobtn_full);
			} else {
				radioIv.SetImageResource (Resource.Drawable.radiobtn_empty);
			}
		}
	}
}

