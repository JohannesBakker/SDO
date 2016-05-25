
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;
using System.Collections.Generic;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Ipad2
{
	public partial class SDXLookupAccountVC : SDXBaseVC
	{
		public AccountModel Account = null;
		public bool IsFromDashboard = false;

		private IList <AccountModel> accountList;
		private AccountModel forDetailAccount = null;
		private LocationModel locationModel = null;
		private int selectedConsumerTypeId = -1;
		private int selectedAccountIndex = -1;

		private UIViewController _curVC;

		public SDXLookupAccountVC (IntPtr handle) : base (handle)
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

			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (manager.Me == null || manager.Me.Accounts == null)
				accountList = new List <AccountModel> ();
			else
				accountList = manager.Me.Accounts;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_Appear:"), UIKeyboard.DidShowNotification, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_WillDisappear:"), UIKeyboard.WillHideNotification, null);
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await LoadReferenceData ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.DidShowNotification, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.WillHideNotification, null);
		}
		#endregion

		#region Button Actions
		async partial void OnLookupAccountBtn_Pressed (SDXButton sender)
		{
			if (Account != null)
				return;

			// Check If Account Number has 8 length
			if (UnitNumberTF.Text.Length != 8) {
				UnitNumberTF.BecomeFirstResponder ();
				new UIAlertView ("Retail Ranger", "The length of account number should be 8.", null, "OK", null).Show ();
				return;
			}

			UnitNumberTF.ResignFirstResponder ();

			// Check If Account is Already Associated.
			AccountModel associatedAccount = null;
			int index = 0;
			if (accountList.Count > 0) {
				foreach (AccountModel myaccount in accountList) {
					if (myaccount.LocationId == UnitNumberTF.Text) {
						associatedAccount = myaccount;
						break;
					}
					index ++;
				}
			}
			if (associatedAccount != null) {
				UIAlertView alert = new UIAlertView ("Account is already associated", "Would like to go to details?", null, "Yes", new string[] {"No"});
				alert.Clicked += ((object s, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						// Go to Account Detail Screen
						selectedAccountIndex = index;
						forDetailAccount = associatedAccount;
						PerformSegue ("SegueToAccountDetail", this);
					} else {
						UnitNumberTF.Text = "";
						UnitNumberTF.BecomeFirstResponder ();
					}
				});
				alert.Show ();
				return;
			}

			ShowLoading ("Searching...");
			SDXAccountManager accountMgr = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			AccountModel account = await accountMgr.LoadAccount (UnitNumberTF.Text, false);

			if (accountMgr.IsSuccessed) {
				HideLoading ();
				UIAlertView alert = new UIAlertView ("This Account is already set", "Do you want to add this account?", null, "Add", new string[] {"Cancel"});
				alert.Clicked += ((object s, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						AddMeToAccount (account);
					} else {
						UnitNumberTF.Text = "";
						UnitNumberTF.BecomeFirstResponder ();
					}
				});
				alert.Show ();
				return;
			}

			LookupLocation ();
		}
		#endregion

		#region Private Functions
		async private void LookupLocation ()
		{
			ShowLoading ("Looking up...");
			SDXLocationManager manager = TinyIoCContainer.Current.Resolve <SDXLocationManager> ();
			locationModel = await manager.LoadLocation (UnitNumberTF.Text);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", "Can't find location. Please try another account", null, "OK", null).Show ();
				UnitNumberTF.Text = "";
				UnitNumberTF.BecomeFirstResponder ();
				return;
			}

			PerformSegue("SegueToAddOutlet", this);
			//FillLocationInfoToViews (location);
		}

		async private void AddMeToAccount (AccountModel account)
		{
			ShowLoading ("Adding...");
			SDXUserManager manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			await manager.AddMeToAccount (account.LocationId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", "Can't add this account. Please try again or another.", null, "OK", null).Show ();
				return;
			}

			// Go To Account Detail
			selectedAccountIndex = -1;
			forDetailAccount = account;
			PerformSegue ("SegueToAccountDetail", this);
		}
		#endregion

		#region Segue
		async public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAddOutlet") {
				SDXAccountsVC vc = segue.DestinationViewController as SDXAccountsVC;
				vc.Location = locationModel;
				vc.AccountIndex = selectedAccountIndex >= 0 ? selectedAccountIndex : -1;
			} else if (segue.Identifier == "SegueToAccountDetail") {
				SDXAccountDetailVC vc = segue.DestinationViewController as SDXAccountDetailVC;
				vc.Account = forDetailAccount;
			}

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

				RectangleF newFrame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Height);

				_curVC.View.Frame = newFrame;
				_curVC.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				_curVC.View.Hidden = false;
				View.Add (_curVC.View);

				_curVC.DidMoveToParentViewController (this);
			}
		}
		#endregion

		#region Notifications
		[Export ("OnKeyboard_Appear:")]
		void OnKeyboard_Appear (NSNotification notification)
		{
			RectangleF frame = ViewMain.Frame;
			frame.Y -= 216;
			ViewMain.Frame = frame;
		}

		[Export ("OnKeyboard_WillDisappear:")]
		void OnKeyboard_WillDisappear (NSNotification notification)
		{
			ViewMain.Frame = new RectangleF (256, 100, 512, 384);
		}
		#endregion
	}
}

