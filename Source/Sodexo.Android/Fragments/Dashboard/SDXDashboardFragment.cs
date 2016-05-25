
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Android
{
	public class SDXDashboardFragment : SDXBaseFragment
	{
		IList <DashboardItemModel> dashboardItems; 
		SDXDashboardListAdapter listAdapter;
		ListView listView;

		View view;

		#region View Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Dashboard", 0);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Dashboard, container, false);
			LayoutView (view);

			listAdapter = new SDXDashboardListAdapter (Activity);
			listView = view.FindViewById (Resource.Id.dashboard_listview) as ListView;
			listView.CacheColorHint = Color.Transparent;
			listView.DividerHeight = 0;
			listView.Adapter = listAdapter;
			listView.ItemClick += ListViewRow_OnSelected;
			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			LoadDashboard ();
		}
		#endregion

		#region Private Functions
		private async void LoadDashboard ()
		{
			Util.ShowLoading (Activity);
			SDXDashboardManager manager = TinyIoCContainer.Current.Resolve <SDXDashboardManager> ();
			dashboardItems = await manager.LoadDashboard ();

			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			List<string> photoUrls = new List<string> ();
			foreach (DashboardItemModel item in dashboardItems) {
				if (item.Photo != null)
					photoUrls.Add (item.Photo.ProcessedPhotoBaseUrl);
			}
			if (photoUrls.Count == 0) {
				Util.HideLoading ();
				listAdapter.DashboardItems = dashboardItems;
				listAdapter.NotifyDataSetChanged ();
				Util.StartFadeAnimation (listView, 500);
				return;
			}
			int iTotalCount = photoUrls.Count;
			int iCounter = 0;
			foreach (string url in photoUrls) {
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object sender, byte[] bytes) => {
					if (Activity == null)
						return;
					Activity.RunOnUiThread (() => {
						if (dashboardItems == null)
							return;
						iCounter ++;
						foreach (DashboardItemModel item in dashboardItems) {
							if (item.Photo == null) {
								continue;
							}
							if(item.Photo.ProcessedPhotoBaseUrl + "?autorotate=true&w=600" == (string)sender) {
								if (bytes == null) {
									item.PhotoId = 0;
								} else {
									using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Count ())) {
										if (bmp == null) {
											item.PhotoId = 0;
										} else {
											item.PhotoId = (int)(1000.0f * bmp.Height / (float)bmp.Width);
											bmp.Recycle ();
										}
									}
								}
								break;
							}
						}
						if (iCounter == iTotalCount) 
						{
							Util.HideLoading ();
							listAdapter.DashboardItems = dashboardItems;
							listAdapter.NotifyDataSetChanged ();
							Util.StartFadeAnimation (listView, 500);
						}
					});
				};
				imageManager.LoadImageAsync (url, 600, 0);
			}
		}
		#endregion

		#region ListView
		private void ListViewRow_OnSelected (object sender, AdapterView.ItemClickEventArgs e)
		{
			DashboardItemModel item = dashboardItems [e.Position];
			if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.AccountSetup) {
				if (item.ModelId <= 0) {
					var fragment = new SDXLookupAccountFragment ();
					fragment.IsFromDashboard = true;
					PushFragment (fragment, this.Class.SimpleName);
				} else {
					var fragment = new SDXAccountDetailFragment ();
					fragment.LocationId = item.ModelId.ToString ();
					PushFragment (fragment, this.Class.SimpleName);
				}
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.UserSetup) {
				var fragment = new SDXMeFragment ();
				fragment.IsFromDashboard = true;
				PushFragment (fragment, this.Class.SimpleName);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Promotion) {
				if (item.ModelId <= 0)
					return;
				var fragment = new SDXPromotionDetailFragment ();
				fragment.PromotionId = item.ModelId;
				fragment.PromotionTitle = item.Title;
				PushFragment (fragment, this.Class.SimpleName);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.SurveyOrFeedback) {
				if (item.FeedbackTypeId == null || item.FeedbackTypeId <= 1)
					return;
				var fragment = new LeaveFeedbackFragment ();
				fragment.FeedbackTypeId = (int)item.FeedbackTypeId;
				fragment.ModelId = item.ModelId;
				PushFragment (fragment, this.Class.SimpleName);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Planogram) {
				var fragment = new SDXAccountsFragment ();
				fragment.isNeededBack = true;
				PushFragment (fragment, this.Class.SimpleName);
			}
		}
		#endregion
	}
}

