using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

using System.CodeDom.Compiler;
using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;
using PerpetualEngine.Storage;

namespace Sodexo.Ipad2
{

	partial class SDXMainVC : UIViewController
	{
		private const int MenuItemBaseTag = 11;
		private string[] SegueIDArray = { "SegueToDashboard", "SegueToAccounts", "SegueToPromotions", "SegueToFeedback", "SegueToPrices", "SegueToMe" };

		private UIViewController _curVC;
		private int _selectedMenuItemTag = MenuItemBaseTag;
		private int _highlightedMenuItemTag = MenuItemBaseTag;

		private SDXLoadingView _loadingView;


		public SDXMainVC(IntPtr handle):base(handle)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var loadingFrame = new RectangleF (0, 0, 1024, 768);
			View.Frame = new RectangleF (0, 0, 1024, 768);

			_loadingView = new SDXLoadingView(loadingFrame);

			LoadApplicationSettings ();

			((UIButton)MenuView.ViewWithTag(MenuItemBaseTag)).Selected = true;
			PerformSegue(SegueIDArray[0], this);
			AddNotifications ();
			ClearHeader ();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//NSNotificationCenter.DefaultCenter.AddObserver(this, new MonoTouch.ObjCRuntime.Selector("OnMenuBtn_Pressed:"), Constants.NotificationMenuPressed, null);
			NSNotificationCenter.DefaultCenter.AddObserver(this, new MonoTouch.ObjCRuntime.Selector("OnShowLoadingView:"), Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.AddObserver(this, new MonoTouch.ObjCRuntime.Selector("OnHideLoadingView:"), Constants.NotificationHideLoadingView, null);
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			//NSNotificationCenter.DefaultCenter.RemoveObserver(this, (NSString)Constants.NotificationMenuPressed, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this, (NSString)Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this, (NSString)Constants.NotificationHideLoadingView, null);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			
			if (!MenuView.Hidden)
				return;
		}

		private void AddNotifications ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnShowLoadingView:"), Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnHideLoadingView:"), Constants.NotificationHideLoadingView, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnLogout:"), Constants.NotificationLogout, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnHighlightMenuItem:"), Constants.NotificationHighlightMenuItem, null);

			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnPhotoTaking_Start:"), Constants.NotificationStartPhotoTaking, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnPhotoTaking_End:"), Constants.NotificationEndPhotoTaking, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnSectionHeader_Change:"), Constants.NotificationChangeSectionHeader, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("PhotoJustUpdated:"), Constants.NotificationUserPhotoUpdated, null);
		}

		private void RemoveNotifications ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationMenuPressed, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationShowLoadingView, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationHideLoadingView, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationLogout, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationHighlightMenuItem, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationStartPhotoTaking, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationEndPhotoTaking, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationChangeSectionHeader, null);
		}

		async private void LoadApplicationSettings()
		{
			var manager = TinyIoCContainer.Current.Resolve<SDXSettingManager>();
			await manager.LoadApplicationSettings(true);

			if(!manager.IsSuccessed)
			{
				new UIAlertView("", "Network Error", null, "OK", null).Show();
				return;
			}

			LoadReferenceData();
			LoadMe ();
		}

		private void ClearHeader()
		{
			HeaderNameLB.Text = "";
			HeaderJobLB.Text = "";
			HeaderUserBorder.Alpha = 0;
			HeaderUserImage.Alpha = 0;
			HeaderNameLB.Alpha = 0;
			HeaderJobLB.Alpha = 0;

			SectionHeader.Text = "";
			SectionHeader.Font = UIFont.FromName (Constants.KARLA_REGULAR, 21);
			SectionHeader.TextColor = UIColor.White;
			SectionHeader.ShadowColor = UIColor.FromRGBA (214, 214, 214, 100);
			SectionHeader.ShadowOffset = new SizeF (1, 1); 
		}

		async private void LoadReferenceData()
		{
			var manager = TinyIoCContainer.Current.Resolve<SDXReferenceDataManager>();
			if(manager.DataModel!=null)
			{
				return;
			}
			await manager.LoadReferenceData();
		}

		[Export ("PhotoJustUpdated:")]
		private void PhotoJustUpdated(NSNotification notification)
		{
			LoadMe ();
		}

		private async void LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null) {

				await manager.LoadMe (false);

				if (!manager.IsSuccessed) {
					new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
					return;
				}
			}

			HeaderNameLB.Text = manager.Me.FirstName + " " + manager.Me.LastName;
			HeaderJobLB.Text = manager.Me.Title;

			HeaderNameLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 25);
			HeaderNameLB.TextColor = UIColor.White;
			HeaderNameLB.ShadowColor = UIColor.FromRGBA (214, 214, 214, 100);
			HeaderNameLB.ShadowOffset = new SizeF (1, 1); 

			HeaderJobLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 11);
			HeaderJobLB.TextColor = UIColor.White;
			HeaderJobLB.ShadowColor = UIColor.FromRGBA (214, 214, 214, 100);
			HeaderJobLB.ShadowOffset = new SizeF (1, 1); 

			if (HeaderJobLB.Text == null) {
				HeaderNameLB.Frame = new RectangleF (HeaderNameLB.Frame.X, HeaderNameLB.Frame.Y, HeaderNameLB.Frame.Size.Width, 47);
			}

			PhotoModel photo = manager.Me.Photos [manager.Me.Photos.Count - 1];
			JHImageManager imgManager = new JHImageManager ();
			imgManager.LoadCompleted += (object sender, byte[] bytes) => {
				InvokeOnMainThread (delegate {
					//AvatarIV.Alpha = 0.5f;
					HeaderUserImage.Image = Util.GetImageFromByteArray (bytes);

					UIView.Animate (0.5f, () => {
						HeaderUserImage.Alpha = 1;
						HeaderUserBorder.Alpha = 1;
						HeaderNameLB.Alpha = 1;
						HeaderJobLB.Alpha = 1;
					});

				});
			};

			SizeF size = new SizeF (50 * 2, 50 * 2);
			imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int)size.Width, (int)size.Height);
		}
		partial void OnMenuItem_Pressed(UIButton sender)
		{
			UIButton pressedBtn = (UIButton)sender;
			if(_selectedMenuItemTag == pressedBtn.Tag)
			{
				return;
			}

			if(_selectedMenuItemTag>-1)
			{
				((UIButton)MenuView.ViewWithTag(_selectedMenuItemTag)).Selected = false;
			}
			pressedBtn.Selected = true;
			_selectedMenuItemTag = pressedBtn.Tag;
			PerformSegue(SegueIDArray[_selectedMenuItemTag - MenuItemBaseTag], this);

//			var vc = new UIViewController();
//			switch(pressedBtn.Tag - MenuItemBaseTag) {
//			case 0:vc = Storyboard.InstantiateViewController ("SDXDashboardVC") as SDXDashboardVC;
//					break;
//			case 1:vc = Storyboard.InstantiateViewController ("SDXAccountsDetailVC") as SDXAccountDetailVC;
//					break;
//			case 2:vc = Storyboard.InstantiateViewController ("SDXPromotionsVC") as SDXPromotionsVC;
//					break;
//			case 3:vc = Storyboard.InstantiateViewController ("SDXFeedbackVC") as SDXFeedbackVC;
//					break;
//			case 4:vc = Storyboard.InstantiateViewController ("SDXPricesVC") as SDXPricesVC;
//					break;
//			case 5:vc = Storyboard.InstantiateViewController ("SDXMeVC") as SDXMeVC;
//					break;
//			}
//			
//			((UINavigationController)this.ChildViewControllers[0]).PushViewController (vc, true);
		
		}

		[Export ("OnShowLoadingView:")]
		void OnShowLoadingView(NSNotification notification)
		{
			Console.WriteLine ("IM LOADING");
			NSString text = notification == null ? (NSString)"Loading..." : notification.Object as NSString;

			_loadingView.Text = text.ToString();
			_loadingView.Alpha = 1.0f;
			if (_loadingView.Superview == null)
				View.AddSubview(_loadingView);

			View.BringSubviewToFront(_loadingView);
			View.BringSubviewToFront (MenuView);
			View.BringSubviewToFront (HeaderView);
		}

		[Export("OnHideLoadingView:")]
		void OnHideLoadingView(NSNotification notification)
		{
			Console.WriteLine ("DONE LOADING");
			if (_loadingView.Superview == null)
				return;

			UIView.Animate(0.3f, () =>
			{
				_loadingView.Alpha = 0.0f;
			}, () =>
			{
				_loadingView.RemoveFromSuperview();
			});
		}

		[Export ("OnLogout:")]
		void OnLogout (NSNotification notification)
		{
			var storage = SimpleStorage.EditGroup ("Sodexo");
			storage.Put<DateTime> ("Login Data time limit", DateTime.Now.AddMinutes (-200));

			NavigationController.PopViewControllerAnimated (true);
		}

		[Export ("OnHighlightMenuItem:")]
		void OnHighlightMenuItem (NSNotification notification)
		{
			int index = ((NSNumber)notification.Object).IntValue;

			string[]headerBgFileNames = {"dashboard_header_bg.png", "planograms_header_bg.png", "promotions_header_bg.png", 
				"feedback_header_bg.png", "prices_header_bg.png", "me_header_bg.png"};

			string headerBgFileName = headerBgFileNames [index];
			MainHeadedBackground.Image = UIImage.FromBundle (headerBgFileName);

			((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = false;
			_highlightedMenuItemTag = MenuItemBaseTag + index;
			((UIButton) MenuView.ViewWithTag(_highlightedMenuItemTag)).Selected = true;
			_selectedMenuItemTag = _highlightedMenuItemTag;
		}

		[Export ("OnPhotoTaking_Start:")]
		void OnPhotoTaking_Start(NSNotification notification)
		{
			View.SendSubviewToBack (MenuView);
			View.SendSubviewToBack (HeaderView);
			ContainerView.Frame = new RectangleF (0, 0, 1024, 768);
			View.BringSubviewToFront (ContainerView);
		}

		[Export ("OnPhotoTaking_End:")]
		void OnPhotoTaking_End(NSNotification notification)
		{
			View.BringSubviewToFront (MenuView);
			View.BringSubviewToFront (HeaderView);
			ContainerView.Frame = new RectangleF (0,79,1024,596); 
			View.SendSubviewToBack (ContainerView);
		}

		[Export ("OnSectionHeader_Change:")]
		void OnSectionHeader_Change(NSNotification notification)
		{
			SectionHeader.Text = ((NSString)notification.Object);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

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

				//_curVC.View.Frame = View.Frame;
				//alter the height of the subview, due to presence of horizontal menu
				var height = View.Frame.Height;
				height = height - 95-79;
				RectangleF newFrame = new RectangleF (0,0, View.Frame.Width, height);

				_curVC.View.Frame = newFrame;
				_curVC.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				_curVC.View.Hidden = false;
				//View.Add (_curVC.View);
				ContainerView.Frame = new RectangleF(0,79,1024,596); 
				ContainerView.Add (_curVC.View);
				_curVC.DidMoveToParentViewController (this);
			}

			//ContainerView.Frame = new RectangleF(0,79,1024,768); 

			View.BringSubviewToFront (ContainerView);
			View.BringSubviewToFront (MenuView);
			View.BringSubviewToFront (HeaderView);

		}

	
	}
}