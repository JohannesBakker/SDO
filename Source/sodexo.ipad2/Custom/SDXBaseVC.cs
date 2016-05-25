
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using System.Threading.Tasks;
using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public partial class SDXBaseVC : UIViewController
	{
		public UIViewController PopVC = null;
		public DashboardItemModel dashboardItem = null;

		public SDXBaseVC (IntPtr handle) : base (handle)
		{

		}

		public SDXBaseVC () : base ()
		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.NavigationItem.SetHidesBackButton(true,false);
			if (NavigationController != null) {
				this.NavigationController.NavigationBarHidden = true;
			}
		}

//		public void AddBackButton (int index)
//		{
//			UIBarButtonItem backItem = new UIBarButtonItem (UIImage.FromBundle("icon_back.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), 
//				UIBarButtonItemStyle.Plain, (s, e)=>{ OnBackBtn_Pressed(s, e); });
//			NavigationItem.LeftBarButtonItem.ImageInsets = new UIEdgeInsets (0, 0, 0, -15);
//			backItem.ImageInsets = new UIEdgeInsets (0, -15, 0, 0);
//			NavigationItem.LeftBarButtonItems = new UIBarButtonItem[]{NavigationItem.LeftBarButtonItem, backItem};
//
//			SetHeaderBackground (index);
//		}

		public void SetHeaderBackground (int index)
		{
			if (index < 0)
				return;

			NSNumber number = new NSNumber (index);
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationHighlightMenuItem, number);
		}

		public void SetSectionName(string name)
		{
			NSString nm = new NSString (name);
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationChangeSectionHeader, nm);
		}
//		void OnMenuBtn_Pressed(object sender, EventArgs ea)
//		{
//			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationMenuPressed, null);
//		}
//
//		void OnBackBtn_Pressed(object sender, EventArgs ea)
//		{
//			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationMenuPressed, (NSString)"Hide");
//
//			if (PopVC == null)
//				NavigationController.PopViewControllerAnimated (true);
//			else
//				NavigationController.PopToViewController (PopVC, true);
//		}

		public void ShowLoading (string text)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationShowLoadingView, (NSString)text);
		}

		public void HideLoading ()
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationHideLoadingView, null);
		}

		async public Task <bool> LoadReferenceData ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel != null)
				return true;

			ShowLoading ("Loading...");
			await manager.LoadReferenceData ();
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("", manager.ErrorMessage, null, "OK", null).Show ();
				return false;
			}
			return true;
		}

		protected void Logout ()
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationLogout, null);
		}

		protected void ShowErrorMessage (string errorMsg)
		{
			if (errorMsg.Contains (Sodexo.Core.Constants.ERROR_MSG_NEED_REAUTH)) {
				UIAlertView alertV = new UIAlertView ("Retail Ranger", errorMsg, null, "OK", null);
				alertV.Clicked += (object sender, UIButtonEventArgs e) => {
					Logout ();
				};
				alertV.Show ();
			} else {
				Util.ShowAlert (errorMsg);
			}
		}
	}
}