
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using TinyIoC;
using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using System.Collections.Generic;

namespace Sodexo.iOS
{
	public partial class SDXDashboardVC : SDXBaseVC
	{
		private List <DashboardItemModel> dashboardItems = null;

		public SDXDashboardVC (IntPtr handle) : base (handle)
		{

		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.

			TableView.Delegate = new SDXDashboardTableDelegate ();
			TableView.DataSource = new SDXDashboardTableDataSource ();

			((SDXDashboardTableDelegate)TableView.Delegate).RowSelectedEvent += OnRowSelected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (0);

			LoadDashboard ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}
		#endregion

		#region Private Functions
		private async void LoadDashboard ()
		{
			ShowLoading ("Loading...");
			SDXDashboardManager manager = TinyIoCContainer.Current.Resolve <SDXDashboardManager> ();
			dashboardItems = await manager.LoadDashboard ();

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			List<string> photoUrls = new List<string> ();
			foreach (DashboardItemModel item in dashboardItems) {
				if (item.Photo != null)
					photoUrls.Add (item.Photo.ProcessedPhotoBaseUrl);
			}
			if (photoUrls.Count == 0) {
				HideLoading ();

				((SDXDashboardTableDelegate)TableView.Delegate).DashboardItems = dashboardItems;
				((SDXDashboardTableDataSource)TableView.DataSource).DashboardItems = dashboardItems;

				TableView.ReloadData ();
				return;
			}
			int iTotalCount = photoUrls.Count;
			int iCounter = 0;
			foreach (string url in photoUrls) {
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object sender, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						iCounter ++;
						foreach (DashboardItemModel item in dashboardItems) {
							if (item.Photo != null) {
								if(item.Photo.ProcessedPhotoBaseUrl + "?autorotate=true&w=600" == (string)sender) {
									UIImage img = Util.GetImageFromByteArray (bytes);
									item.PhotoId = (int)(1000 * img.Size.Height / img.Size.Width);
									break;
								}
							}
						}
						if (iCounter == iTotalCount) 
						{
							HideLoading ();

							((SDXDashboardTableDelegate)TableView.Delegate).DashboardItems = dashboardItems;
							((SDXDashboardTableDataSource)TableView.DataSource).DashboardItems = dashboardItems;

							TableView.ReloadData ();
						}
					});
				};
				imageManager.LoadImageAsync (url, 600, 0);
			}
		}
		#endregion

		#region Event Handler
		private void OnRowSelected (object sender, int row)
		{
			DashboardItemModel item = dashboardItems [row];
			if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.AccountSetup) {
				if (item.ModelId <= 0) {
					var vc = Storyboard.InstantiateViewController ("SDXLookupAccountVC") as SDXLookupAccountVC;
					vc.IsFromDashboard = true;
					NavigationController.PushViewController (vc, true);
				} else {
					var vc = Storyboard.InstantiateViewController ("SDXAccountDetailVC") as SDXAccountDetailVC;
					vc.LocationId = item.ModelId.ToString ();
					NavigationController.PushViewController (vc, true);
				}
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.UserSetup) {
				var vc = Storyboard.InstantiateViewController ("SDXMeVC") as SDXMeVC;
				vc.IsFromDashboard = true;
				NavigationController.PushViewController (vc, true);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Promotion) {
				if (item.ModelId <= 0)
					return;
				var vc = Storyboard.InstantiateViewController ("SDXPromotionDetailVC") as SDXPromotionDetailVC;
				vc.PromotionId = item.ModelId;
				vc.PromotionTitle = item.Title;
				NavigationController.PushViewController (vc, true);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.SurveyOrFeedback) {
				if (item.FeedbackTypeId == null || item.FeedbackTypeId <= 0)
					return;
				var vc = Storyboard.InstantiateViewController ("SDXLeaveFeedbackVC") as SDXLeaveFeedbackVC;
				vc.FeedbackTypeId = (int)item.FeedbackTypeId;
				vc.ModelId = item.ModelId;
				NavigationController.PushViewController (vc, true);
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Planogram) {
				var vc = Storyboard.InstantiateViewController ("SDXAccountsVC") as SDXAccountsVC;
				NavigationController.PushViewController (vc, true);
			}
		}
		#endregion
	}
}