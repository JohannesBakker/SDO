
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using System.Threading.Tasks;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.iOS
{
	public partial class SDXBaseVC : UIViewController
	{
		public UIViewController PopVC = null;

		string[] headerBgFileNames = {"dashboard_header_bg.png", "accounts_header_bg.png", "promotions_header_bg.png", 
													"feedback_header_bg.png", "prices_header_bg.png", "me_header_bg.png"};

		public SDXBaseVC (IntPtr handle) : base (handle)
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
			base.ViewDidLoad (); //.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal)
			UIBarButtonItem menuItem = new UIBarButtonItem (UIImage.FromBundle("icon_menu.png"), 
															UIBarButtonItemStyle.Plain, (s, e)=>{ OnMenuBtn_Pressed(s, e); });
			NavigationItem.LeftBarButtonItem = menuItem;
		}

		public void AddBackButton (int index)
		{
			UIBarButtonItem backItem = new UIBarButtonItem (UIImage.FromBundle("icon_back.png"), 
				UIBarButtonItemStyle.Plain, (s, e)=>{ OnBackBtn_Pressed(s, e); });
			NavigationItem.LeftBarButtonItem.ImageInsets = new UIEdgeInsets (0, 0, 0, -15);
			backItem.ImageInsets = new UIEdgeInsets (0, -15, 0, 0);
			NavigationItem.LeftBarButtonItems = new UIBarButtonItem[]{NavigationItem.LeftBarButtonItem, backItem};

			SetHeaderBackground (index);
		}

		public void SetHeaderBackground (int index)
		{
			if (index < 0)
				return;

			string headerBgFileName = headerBgFileNames [index];
			NavigationController.NavigationBar.SetBackgroundImage (UIImage.FromBundle (headerBgFileName), UIBarMetrics.Default);

			NSNumber number = new NSNumber (index);
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationHighlightMenuItem, number);
		}

		void OnMenuBtn_Pressed(object sender, EventArgs ea)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationMenuPressed, null);
		}

		void OnBackBtn_Pressed(object sender, EventArgs ea)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (Constants.NotificationMenuPressed, (NSString)"Hide");

			if (PopVC == null)
				NavigationController.PopViewControllerAnimated (true);
			else
				NavigationController.PopToViewController (PopVC, true);
		}

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
				ShowErrorMessage (manager.ErrorMessage);
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

