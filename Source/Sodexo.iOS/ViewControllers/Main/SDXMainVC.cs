using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.iOS
{
	partial class SDXMainVC : UIViewController
	{
		private const int 	MenuItemBaseTag 	= 11;

		private string[] 	MenuBtnOnImageNameArray = {"menu_dashboard_on.png", "menu_accounts_on.png", "menu_promotions_on.png", "menu_feedback_on.png", "menu_prices_on.png", "menu_me_on.png"};
		private string[] 	SegueIDArray = {"SegueToDashboard", "SegueToAccounts", "SegueToPromotions", "SegueToFeedback", "SegueToPrices", "SegueToMe"};

		private UIViewController _curVC;
		private int _selectedMenuItemTag = MenuItemBaseTag;
		private int _highlightedMenuItemTag = MenuItemBaseTag;
		private SDXLoadingView _loadingView;

		public SDXMainVC (IntPtr handle) : base (handle)
		{

		}

		#region View Lifecycle
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UITextField.Appearance.TintColor = UIColor.DarkGray;

			_loadingView = new SDXLoadingView (View.Frame);

			LoadApplicationSettings ();

			MenuView.Hidden = true;

			for (int i = 0; i < MenuBtnOnImageNameArray.Length; i++) {
				((UIButton)MenuView.ViewWithTag (MenuItemBaseTag + i)).SetBackgroundImage (UIImage.FromBundle (MenuBtnOnImageNameArray[i]), UIControlState.Highlighted | UIControlState.Selected);
			}

			((UIButton)MenuView.ViewWithTag (MenuItemBaseTag)).Selected = true;

			PerformSegue (SegueIDArray[0], this);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (!MenuView.Hidden)
				return;

			Console.WriteLine ("ViewDidLayoutSubviews");

			float menuItemH = UIScreen.MainScreen.Bounds.Height > 480 ? 76 : 63;
			float itemLogoH = UIScreen.MainScreen.Bounds.Height > 480 ? 48 : 38;
			RectangleF frame = MenuView.Frame;
			frame.Width = menuItemH;
			frame.X = -frame.Width;
			frame.Y = 64;
			MenuView.Frame = frame;
			for (int i = 0; i < 7; i++) {
				UIButton btn = (UIButton) MenuView.ViewWithTag (MenuItemBaseTag + i);
				btn.Frame = new RectangleF (0, i * menuItemH, menuItemH, i != 6 ? menuItemH : itemLogoH);
			}
			MenuView.Hidden = false;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			RemoveNotifications ();
			AddNotifications ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			RemoveNotifications ();
		}
		#endregion

		#region Private Functions
		private void AddNotifications ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnMenuBtn_Pressed:"), Constants.NotificationMenuPressed, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnShowLoadingView:"), Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnHideLoadingView:"), Constants.NotificationHideLoadingView, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnLogout:"), Constants.NotificationLogout, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnHighlightMenuItem:"), Constants.NotificationHighlightMenuItem, null);
		}

		private void RemoveNotifications ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationMenuPressed, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationHideLoadingView, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationLogout, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationHighlightMenuItem, null);
		}

		async private void LoadApplicationSettings ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			await manager.LoadApplicationSettings (true);
			if (!manager.IsSuccessed) {
				return;
			}

			LoadReferenceData ();
		}

		async private void LoadReferenceData ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel != null)
				return;
			await manager.LoadReferenceData ();
		}
		#endregion

		#region Actions of Buttons
		partial void OnMenuItem_Pressed (UIButton sender)
		{
			UIButton pressedBtn = (UIButton)sender;

			if (pressedBtn.Tag == 17)
				return;

			if (_selectedMenuItemTag == pressedBtn.Tag) {
				OnMenuBtn_Pressed (null);
				if (_highlightedMenuItemTag != _selectedMenuItemTag) {
					((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = false;
					((UIButton) MenuView.ViewWithTag(_selectedMenuItemTag)).Selected = true;
					((UINavigationController)_curVC).PopToRootViewController (false);
					_highlightedMenuItemTag = _selectedMenuItemTag;
				}
				return;
			}

			Console.WriteLine ("OnMenuItem_Pressed : " + ((UIButton)sender).Tag.ToString());

			if (_selectedMenuItemTag > -1) {
				((UIButton) MenuView.ViewWithTag(_selectedMenuItemTag)).Selected = false;
				((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = false;
			}

			((UIButton) MenuView.ViewWithTag(pressedBtn.Tag)).Selected = true;
			_selectedMenuItemTag = _highlightedMenuItemTag = pressedBtn.Tag;

			PerformSegue (SegueIDArray[_selectedMenuItemTag - MenuItemBaseTag], this);

			OnMenuBtn_Pressed (null);
		}
		#endregion

		#region Notifications
		[Export ("OnMenuBtn_Pressed:")]
		void OnMenuBtn_Pressed (NSNotification notification)
		{
			if (MenuView.Hidden)
				return;

			NSString obj = notification != null ? (NSString) notification.Object : null;

			RectangleF menuFrame = MenuView.Frame;
			RectangleF curViewFrame = ((UINavigationController)_curVC).VisibleViewController.View.Frame;
			if (obj == null) {
				if (menuFrame.X >= 0) {
					menuFrame.X = -menuFrame.Width;
					curViewFrame.X = 0;
				} else {
					menuFrame.X = 0;
					curViewFrame.X = menuFrame.Width;
				}
			} else if (obj.ToString() == "Hide") {
				if (menuFrame.X < 0)
					return;
				menuFrame.X = -menuFrame.Width;
				curViewFrame.X = 0;
			} else {		// "Show"
				if (menuFrame.X >= 0)
					return;
				menuFrame.X = 0;
				curViewFrame.X = menuFrame.Width;
			}

			UIView.Animate (0.3f,
				() => {
					MenuView.Frame = menuFrame;
					((UINavigationController)_curVC).VisibleViewController.View.Frame = curViewFrame;
				}, () => {

				});
		}

		[Export ("OnShowLoadingView:")]
		void OnShowLoadingView (NSNotification notification)
		{
			NSString text = notification == null ? (NSString)"Loading..." : notification.Object as NSString;

			_loadingView.Text = text.ToString ();
			_loadingView.Alpha = 1.0f;
			if (_loadingView.Superview == null)
				View.AddSubview (_loadingView);
			View.BringSubviewToFront (_loadingView);
		}

		[Export ("OnHideLoadingView:")]
		void OnHideLoadingView (NSNotification notification)
		{
			if (_loadingView.Superview == null)
				return;

			UIView.Animate (0.3f, () => {
				_loadingView.Alpha = 0.0f;
			}, () => {
				_loadingView.RemoveFromSuperview ();
			});
		}

		[Export ("OnLogout:")]
		void OnLogout (NSNotification notification)
		{
			if (NavigationController != null)
				NavigationController.PopViewControllerAnimated (true);
			var manager = TinyIoCContainer.Current.Resolve <SDXAuthManager> ();
			manager.Logout ();

//			var settingsMgr = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
//			settingsMgr.IsAuthorizedLoaded = false;
//			settingsMgr.IsUnAuthorizedLoaded = false;
		}

		[Export ("OnHighlightMenuItem:")]
		void OnHighlightMenuItem (NSNotification notification)
		{
			int index = ((NSNumber)notification.Object).IntValue;
			((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = false;
			_highlightedMenuItemTag = MenuItemBaseTag + index;
			((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = true;
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountsVC = null;
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = null;

			if (_curVC != null) {
				_curVC.WillMoveToParentViewController (null);
				_curVC.View.RemoveFromSuperview ();
				_curVC.RemoveFromParentViewController ();
				_curVC.DidMoveToParentViewController (null);
			}

			_curVC = segue.DestinationViewController;

			if (_curVC != null) {
				_curVC.WillMoveToParentViewController (this);
				this.AddChildViewController (_curVC);

				_curVC.View.Frame = View.Frame;
				_curVC.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				_curVC.View.Hidden = false;
				View.Add (_curVC.View);

				_curVC.DidMoveToParentViewController (this);
			}

			View.BringSubviewToFront (MenuView);
		}
		#endregion
	}
}
