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
	public class SDXPlanogramCell : Java.Lang.Object
	{
		Activity context;
		View view;

		LinearLayout offersView;
		ImageView pictureIv;
		TextView nameTv;

		public EventHandler <int> OfferSelectedEvent;

		int outletIdx;

		public SDXPlanogramCell (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		private void GetInstance ()
		{
			pictureIv = view.FindViewById (Resource.Id.planogramcell_outlet_picture_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.planogramcell_outlet_name_tv) as TextView;
			offersView = view.FindViewById (Resource.Id.planogramcell_offers_ll) as LinearLayout;
		}

		public void Update (OutletModel outlet, int outletIdx)
		{
			this.outletIdx = outletIdx;

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
		}

		private void AddOffers (IList<OfferModel> offers)
		{
			offersView.RemoveAllViews ();
			int i = 0;
			foreach (OfferModel offer in offers) {
				SDXOfferView offerV = SDXOfferView.Create (context, (ViewGroup)view, i++);
				offerV.OfferSelectedEvent = OfferSelectedEvent;
				offerV.OutletIdx = outletIdx;
				offerV.Update (offer);
				offersView.AddView (offerV.view);
			}
		}
	}
}

