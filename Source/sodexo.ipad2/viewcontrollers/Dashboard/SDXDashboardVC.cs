using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using TinyIoC;
using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using System.Collections.Generic;
using PerpetualEngine.Storage;

namespace Sodexo.Ipad2
{
	public partial class SDXDashboardVC : SDXBaseVC
	{
		private List <DashboardItemModel> dashboardItems = null;

		private DashboardItemModel selectedDashboard;

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



			UITableView.Appearance.BackgroundColor = UIColor.Clear;
			UITableViewCell.Appearance.BackgroundColor = UIColor.Clear;
			UITableView.Appearance.SeparatorInset.InsetRect(new RectangleF(10, 10, 320, 10));

			// Perform any additional setup after loading the view, typically from a nib.
			TableView.Delegate = new SDXDashboardTableDelegate ();
			TableView.DataSource = new SDXDashboardTableDataSource ();

			((SDXDashboardTableDelegate)TableView.Delegate).RowSelectedEvent += OnRowSelected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (0);
			SetSectionName ("Dashboard");
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
				new UIAlertView ("Sodexo", manager.ErrorMessage, null, "OK", null).Show ();
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
			selectedDashboard = item;

			var vc = new SDXBaseVC ();

			int menuIndex = 0;

			if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.AccountSetup) {
				if (item.ModelId <= 0) {
					vc = (SDXLookupAccountVC)Storyboard.InstantiateViewController ("SDXLookupAccountVC") as SDXLookupAccountVC;
				
				} else {
					vc = Storyboard.InstantiateViewController ("SDXAccountsDetailVC") as SDXAccountDetailVC;
				}

				menuIndex = 1;
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Planogram) {

				vc = Storyboard.InstantiateViewController ("SDXAccountDetailVC") as SDXAccountDetailVC;
				menuIndex = 1;
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.UserSetup) {
				 
				vc = Storyboard.InstantiateViewController ("SDXMeVC") as SDXMeVC;
				menuIndex = 5;
			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.Promotion) {

				vc = Storyboard.InstantiateViewController ("SDXPromotionsVC") as SDXPromotionsVC;
				menuIndex = 2;

			} else if (item.DasboardItemType == Sodexo.RetailActivation.Portable.Models.Constants.DashboardItemType.SurveyOrFeedback) {
								
				vc = Storyboard.InstantiateViewController ("SDXFeedbackVC") as SDXFeedbackVC;
				menuIndex = 3;

			} 

			vc.dashboardItem = item;
			NavigationController.PushViewController (vc, true);

//			vc.dashboardItem = item;
//			vc.View.Frame = View.Bounds;
//			vc.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
//
//			this.AddChildViewController (vc);
//			vc.DidMoveToParentViewController (this);
//			View.AddSubview (vc.View);

			NSNumber number = new NSNumber (menuIndex);
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationHighlightMenuItem,number);
			//NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationEndPhotoTaking, null);
		}
		#endregion

	}
}