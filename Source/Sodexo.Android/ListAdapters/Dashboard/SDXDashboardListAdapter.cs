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
	public class SDXDashboardListAdapter : BaseAdapter <string>
	{
		Activity context;
		public IList <DashboardItemModel> DashboardItems;

		public SDXDashboardListAdapter (Activity context) : base ()
		{
			this.context = context;
		}

		public override int Count {
			get {
				int cnt = DashboardItems != null ? DashboardItems.Count : 0;
				return cnt;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;

			if (view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.Dashboard_Cell, parent, false) as LinearLayout;

				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					context.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
						Update (DashboardItems [position], view);
					});
				}));
			} else {
				Update (DashboardItems [position], view);
			}

			return view;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override string this[int position] {  
			get { 
				return DashboardItems [position].Title;
			}
		}

		private void Update (DashboardItemModel item, View view)
		{
			// Header
			var indicatorIv = view.FindViewById (Resource.Id.dashboard_cell_indicator_iv) as ImageView;
			var symbolIv = view.FindViewById (Resource.Id.dashboard_cell_symbol_iv) as ImageView;
			var titleTv = view.FindViewById (Resource.Id.dashboard_cell_title_tv) as TextView;
			var dateTv = view.FindViewById (Resource.Id.dashboard_cell_date_tv) as TextView;

			var indicatorResId = "indicator_" + item.DasboardItemType.ToString ().ToLower ();
			indicatorIv.SetImageResource (context.Resources.GetIdentifier (indicatorResId, "drawable", context.PackageName));

			var symbolResid = "symbol_" + item.DasboardItemType.ToString ().ToLower ();
			symbolIv.SetImageResource (context.Resources.GetIdentifier (symbolResid, "drawable", context.PackageName));

			titleTv.Text = item.Title;

			if (item.CalloutDate != null) {
				DateTime date = (DateTime) item.CalloutDate;
				dateTv.Text = date.Month.ToString ("00") + "." + date.Day.ToString ("00");
			} else {
				dateTv.Text = "";
			}

			// Picture & Progress View
			var pictureIv = view.FindViewById (Resource.Id.dashboard_cell_picture_iv) as ImageView;
			var progressView = view.FindViewById (Resource.Id.dashboard_cell_progress_fl);

			pictureIv.Visibility = ViewStates.Gone;
			progressView.Visibility = ViewStates.Gone;

			if (item.Photo != null) {
				pictureIv.Visibility = ViewStates.Visible;
				Util.ChangeViewHeight (pictureIv, (int)(item.PhotoId * pictureIv.LayoutParameters.Width / 1000.0f));

				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					if (context == null)
						return;
					context.RunOnUiThread (delegate {
						if (bytes == null)
							return;
						using (var bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Count ())) {
							if (bmp != null)
								pictureIv.SetImageBitmap (bmp);
						}
					});
				};
				imageManager.LoadImageAsync (item.Photo.ProcessedPhotoBaseUrl, 600, 0);
			} else if (item.PercentageComplete > 0 && item.PercentageComplete < 100) {
				progressView.Visibility = ViewStates.Visible;

				var fullProgressView = view.FindViewById (Resource.Id.dashboard_cell_progress_full_ll) as LinearLayout;
				var emptyProgressIv = view.FindViewById (Resource.Id.dashboard_cell_progress_empty_iv) as ImageView;
				var progressValueView = view.FindViewById (Resource.Id.dashboard_cell_progress_value_fl) as FrameLayout;
				var progressValueTv = view.FindViewById (Resource.Id.dashboard_cell_progress_value_tv) as TextView;

				Util.ChangeViewWidth (fullProgressView, (int)(emptyProgressIv.LayoutParameters.Width * item.PercentageComplete / 100.0f));
				progressValueTv.Text = item.PercentageComplete.ToString () + "%";
				Util.MoveViewToX (progressValueView, fullProgressView.Left + fullProgressView.LayoutParameters.Width - progressValueView.LayoutParameters.Width / 2);
			}
			
			// Description View
			var descTv = view.FindViewById (Resource.Id.dashboard_cell_desc_tv) as TextView;
			descTv.Text = item.Text;
		}
	}
}

