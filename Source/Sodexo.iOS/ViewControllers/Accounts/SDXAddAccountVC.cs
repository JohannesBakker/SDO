
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.iOS
{
	public partial class SDXAddAccountVC : SDXBaseVC
	{
		private AccountModel forDetailAccount = null;

		private int selectedConsumerTypeID = -1;
		private int selectedAccountIndex = -1;

		public AccountModel Account = null;

		public SDXAddAccountVC (IntPtr handle) : base (handle)
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

			// Set Main ScrollView Content Size
			ScrollView.ContentSize = new SizeF (320, 730);

			// Set DropTableView Row Select Event
			DropTableView.SelectedEvent += OnDropTableCell_Selected;

			NextBtn.Layer.CornerRadius = 3;
			NextBtn.Layer.MasksToBounds = true;

			if (Account == null) {
				// Add Toolbar on Keyboard
				UIBarButtonItem space = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
				UIBarButtonItem doneBtnItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, (s, e) => {
					AccountNumberTF.ResignFirstResponder ();
				});
				UIToolbar toolBar = new UIToolbar (new RectangleF (0, 0, 320, 40));
				toolBar.BarStyle = UIBarStyle.Black;
				UIBarButtonItem[] barBtnItems = { space, doneBtnItem };
				toolBar.Items = barBtnItems;
				AccountNumberTF.InputAccessoryView = toolBar;
			} else {
				TitleLB.Text = "UPDATE ACCOUNT";
				NavigationItem.Title = "ACCOUNT";
				NextBtn.SetTitle ("DONE", UIControlState.Normal);

				AccountNumberTF.Enabled = false;
				AccountNumberTF.Text = Account.LocationId;
				FillLocationInfoToViews (Account.Location);
				ConsumerTypeTF.Text = Account.ConsumerType.Description;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (1);

			if (Account == null) {
				// Set Keyboard Notifications
				NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_Appear:"), UIKeyboard.DidShowNotification, null);
				NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_WillDisappear:"), UIKeyboard.WillHideNotification, null);
			}
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await LoadReferenceData ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if (Account == null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.DidShowNotification, null);
				NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.WillHideNotification, null);
			}
		}
		#endregion

		#region Private Functions
		async private void LookupLocation ()
		{
			ShowLoading ("Looking up...");
			SDXLocationManager manager = TinyIoCContainer.Current.Resolve <SDXLocationManager> ();
			LocationModel location = await manager.LoadLocation (AccountNumberTF.Text);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", "Can't find location. Please try another account", null, "OK", null).Show ();
				AccountNumberTF.Text = "";
				AccountNumberTF.BecomeFirstResponder ();
				return;
			}

			FillLocationInfoToViews (location);
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

		async private void UpdateAccount ()
		{
			string locationId = Account.LocationId;
			string consumerTypeId = selectedConsumerTypeID.ToString ();
			string rowVersion = System.Convert.ToBase64String (Account.RowVersion);

			ShowLoading ("Updating...");
			SDXAccountManager manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			AccountModel account = await manager.UpdateAccount(locationId, consumerTypeId, rowVersion);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", "Can't update account. Please try again.", null, "OK", null).Show ();
				return;
			} else {
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Account has been successfully updated.", null, "OK", null);
				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					SDXAccountDetailVC vc = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC as SDXAccountDetailVC;
					vc.Account = account;
					NavigationController.PopToViewController (vc, true);
//					DismissViewController (true, null);
				};
				alert.Show ();
			}
		}

		private void FillLocationInfoToViews(LocationModel location)
		{
			AccountNameTF.Text = location.LocationName;
			Address1TF.Text = location.LocationAddress1;
			Address2TF.Text = " ";
			CityTF.Text = location.LocationCity;
			StateTF.Text = location.LocationStateCd;
			ZipTF.Text = location.LocationZip;
		}
		#endregion

		#region Keyboard
		partial void OnReturnKey_Pressed (SDXTextField sender)
		{
			var tf = sender as UITextField;
			var nextTF = ScrollView.ViewWithTag (tf.Tag + 1);
			nextTF.BecomeFirstResponder ();
		}
		#endregion

		#region Button Actions
		async partial void OnDropBtn_Pressed (UIButton sender)
		{
			if (DropView.Hidden == false)
				OnHideDropBtn_Pressed (null);

			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				await LoadReferenceData ();
				return;
			}

			List<string> dropDescList = new List<string> ();
			foreach (ConsumerTypeModel item in manager.DataModel.ConsumerTypes) {
				dropDescList.Add (item.Description);
			}
			DropTableView.SetDropDescriptionList (dropDescList);
			DropTableView.ReloadData ();
			DropTableView.ScrollToRow(NSIndexPath.FromRowSection(0,0), UITableViewScrollPosition.Top, false);

			RectangleF frame = ConsumerTypeTF.Frame;
			DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			DropView.Hidden = false;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 100.0f);
			});
		}

		partial void OnHideDropBtn_Pressed (UIButton sender)
		{
			RectangleF frame = ConsumerTypeTF.Frame;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			}, () => {
				DropView.Hidden = true;
			});
		}

		async partial void OnSearchBtn_Pressed (UIButton sender)
		{
			if (Account != null)
				return;

			// Check If Account Number has 8 length
			if (AccountNumberTF.Text.Length != 8) {
				AccountNumberTF.BecomeFirstResponder ();
				new UIAlertView ("Retail Ranger", "The length of account number should be 8.", null, "OK", null).Show ();
				return;
			}

			// Check If Account is Already Associated.
			SDXUserManager userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			AccountModel associatedAccount = null;
			int index = 0;
			foreach (AccountModel myaccount in userManager.Me.Accounts) {
				if (myaccount.LocationId == AccountNumberTF.Text) {
					associatedAccount = myaccount;
					break;
				}
				index ++;
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
						AccountNumberTF.Text = "";
						AccountNumberTF.BecomeFirstResponder ();
					}
				});
				alert.Show ();
				return;
			}

			ShowLoading ("Searching...");
			SDXAccountManager accountMgr = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			AccountModel account = await accountMgr.LoadAccount (AccountNumberTF.Text, false);

			if (accountMgr.IsSuccessed) {
				HideLoading ();
				UIAlertView alert = new UIAlertView ("This Account is already set", "Do you want to add this account?", null, "Add", new string[] {"Cancel"});
				alert.Clicked += ((object s, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0) {
						AddMeToAccount (account);
					} else {
						AccountNumberTF.Text = "";
						AccountNumberTF.BecomeFirstResponder ();
					}
				});
				alert.Show ();
				return;
			}

			LookupLocation ();
		}

		partial void OnNextBtn_Pressed (SDXButton sender)
		{
			if (Account == null) {
				if (AccountNumberTF.Text.Length != 8) {
					AccountNumberTF.BecomeFirstResponder ();
					UIAlertView alert = new UIAlertView ("Retail Ranger", "Please input correct Account Number.", null, "OK", null);
					alert.Clicked += (s, e) => {};
					alert.Show ();
					return;
				}

				for (int i = 2; i <= 7; i++) {
					var tf = ScrollView.ViewWithTag (i) as UITextField;
					if (tf.Text == "") {
						UIAlertView alert = new UIAlertView ("Retail Ranger", "Please lookup account's location to import all data.", null, "OK", null);
						alert.Clicked += (s, e) => {};
						alert.Show ();
						return;
					}
				}

				if (selectedConsumerTypeID == -1) {
					new UIAlertView ("Retail Ranger", "Please select consumer type.", null, "OK", null).Show();
					return;
				}

				PerformSegue ("SegueToAddOutlet", this);
			} else {
				if (Account.ConsumerType.Description == ConsumerTypeTF.Text) {
					new UIAlertView ("Messages", "No changes, you can change Consumer Type to upate.", null, "OK", null).Show ();
					return;
				} else {
					UpdateAccount ();
				}
			}
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAddOutlet") {
				SDXAddOutletVC vc = segue.DestinationViewController as SDXAddOutletVC;

				AccountModel account = new AccountModel ();
				account.LocationId = AccountNumberTF.Text;
				account.Location = new LocationModel ();
				account.Location.LocationName = AccountNameTF.Text;
				account.Location.LocationAddress1 = Address1TF.Text + " " + Address2TF.Text;
				account.Location.LocationCity = CityTF.Text;
				account.Location.LocationStateCd = StateTF.Text;
				account.Location.LocationZip = ZipTF.Text;
				account.ConsumerTypeId = selectedConsumerTypeID;

				vc.Account = account;
			} else if (segue.Identifier == "SegueToAccountDetail") {
				SDXAccountDetailVC vc = segue.DestinationViewController as SDXAccountDetailVC;
				vc.Account = forDetailAccount;
				SDXAccountsVC accountsVC = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountsVC as SDXAccountsVC;
				if (accountsVC != null) {
					accountsVC.IsNeedReload = selectedAccountIndex >= 0 ? false : true;
					vc.PopVC = accountsVC;
				} else
					vc.PopVC = NavigationController.ViewControllers [0];
			}
		}
		#endregion

		#region Notifications
		[Export ("OnKeyboard_Appear:")]
		void OnKeyboard_Appear (NSNotification notification)
		{
			if (ScrollView.Frame.Size.Height < 504)
				return;

			RectangleF frame = ScrollView.Frame;
			frame.Height = 504 - 216;
			ScrollView.Frame = frame;
		}

		[Export ("OnKeyboard_WillDisappear:")]
		void OnKeyboard_WillDisappear (NSNotification notification)
		{
			if (ScrollView.Frame.Size.Height == View.Frame.Size.Height)
				return;

			ScrollView.Frame = new RectangleF (0, 0, 320, 504);
		}
		#endregion

		#region Event Handler
		void OnDropTableCell_Selected(object sender, int row)
		{
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			ConsumerTypeModel type = manager.DataModel.ConsumerTypes.ElementAt (row);
			selectedConsumerTypeID = type.ConsumerTypeId;
			ConsumerTypeTF.Text = type.Description;

			OnHideDropBtn_Pressed (null);
		}
		#endregion
	}
}

