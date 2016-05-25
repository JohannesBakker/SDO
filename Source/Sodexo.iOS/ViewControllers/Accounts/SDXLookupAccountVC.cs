
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXLookupAccountVC : SDXBaseVC
	{
		public AccountModel Account = null;
		public bool IsFromDashboard = false;

		private IList <AccountModel> accountList;
		private AccountModel forDetailAccount = null;
		private int selectedConsumerTypeId = -1;
		private int selectedAccountIndex = -1;

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

			// Set DropTableView Row Select Event
			DropTableView.SelectedEvent += OnDropTableCell_Selected;

			if (Account == null) {
				// Add Toolbar on Keyboard
				UIBarButtonItem space = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
				UIBarButtonItem doneBtnItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, (s, e) => {
					UnitNumberTF.ResignFirstResponder ();
				});
				UIToolbar toolBar = new UIToolbar (new RectangleF (0, 0, 320, 40));
				toolBar.BarStyle = UIBarStyle.Black;
				UIBarButtonItem[] barBtnItems = { space, doneBtnItem };
				toolBar.Items = barBtnItems;
				UnitNumberTF.InputAccessoryView = toolBar;

				UnitContentView.Hidden = true;
			} else {
				TitleLB.Text = "UPDATE ACCOUNT";
				NavigationItem.Title = "ACCOUNT";
				NextBtn.SetTitle ("DONE", UIControlState.Normal);

				UnitNumberTF.Enabled = false;
				UnitNumberTF.Text = Account.LocationId;
				ConsumerTypeLB.Text = Account.ConsumerType.Description;
				FillLocationInfoToViews (Account.Location);

				DeleteAccountBtn.Hidden = false;

				DescriptionLB.Text = "";
				Util.MoveViewToY (UnitNumberView, 50);
				LookupBtn.Hidden = true;
				Util.MoveViewToY (UnitContentView, UnitNumberView.Frame.Y + UnitNumberView.Frame.Height);
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (Account != null || accountList.Count != 0 || IsFromDashboard)
				AddBackButton (1);
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await LoadReferenceData ();
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

		async partial void OnConsumerTypeBtn_Pressed (UIButton sender)
		{
			if (DropView.Hidden == false)
				OnHideDropView (null);

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

			RectangleF frame = ConsumerTypeLB.Frame;
			frame.Y += UnitContentView.Frame.Y;
			DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			DropView.Hidden = false;
			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, fHeight);
			});
		}

		partial void OnHideDropView (UIButton sender)
		{
			RectangleF frame = ConsumerTypeLB.Frame;
			frame.Y += UnitContentView.Frame.Y;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			}, () => {
				DropView.Hidden = true;
			});
		}

		partial void OnNextBtn_Pressed (SDXButton sender)
		{
			if (Account == null) {
				if (UnitNumberTF.Text.Length != 8) {
					UnitNumberTF.BecomeFirstResponder ();
					UIAlertView alert = new UIAlertView ("Retail Ranger", "Please input correct Account Number.", null, "OK", null);
					alert.Clicked += (s, e) => {};
					alert.Show ();
					return;
				}

				if (selectedConsumerTypeId == -1) {
					new UIAlertView ("Retail Ranger", "Please select consumer type.", null, "OK", null).Show();
					return;
				}

				PerformSegue ("SegueToAddOutlet", this);
			} else {
				if (Account.ConsumerType.Description == ConsumerTypeLB.Text) {
					new UIAlertView ("Retail Ranger", "No changes, you can change Consumer Type to update.", null, "OK", null).Show ();
					return;
				} else {
					UpdateAccount ();
				}
			}
		}

		async partial void OnDeleteAccountBtn_Pressed (SDXButton sender)
		{
			ShowLoading ("Deleting");
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteAccount (Account.LocationId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			UIAlertView alert = new UIAlertView ("Retail Ranger", "Account has been successfully deleted.", null, "OK", null);
			alert.Clicked += (object s, UIButtonEventArgs e) => {
				if (accountList.Contains (Account))
					accountList.Remove (Account);
				NavigationController.PopToRootViewController (true);
			};
			alert.Show ();
		}
		#endregion

		#region Event Handler
		void OnDropTableCell_Selected(object sender, int row)
		{
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			ConsumerTypeModel type = manager.DataModel.ConsumerTypes.ElementAt (row);
			selectedConsumerTypeId = type.ConsumerTypeId;
			ConsumerTypeLB.Text = type.Description;

			OnHideDropView (null);
		}
		#endregion

		#region Private Functions
		async private void LookupLocation ()
		{
			ShowLoading ("Looking up...");
			SDXLocationManager manager = TinyIoCContainer.Current.Resolve <SDXLocationManager> ();
			LocationModel location = await manager.LoadLocation (UnitNumberTF.Text);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", "Can't find location. Please try another account", null, "OK", null).Show ();
				UnitNumberTF.Text = "";
				UnitNumberTF.BecomeFirstResponder ();
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

			if (manager.Me == null || manager.Me.Accounts == null) {
				ShowLoading ("Loading...");
				await manager.LoadMe ();
				HideLoading ();

				if (!manager.IsSuccessed) {
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
			}

			manager.Me.Accounts.Add (account);

			// Go To Account Detail
			selectedAccountIndex = -1;
			forDetailAccount = account;
			PerformSegue ("SegueToAccountDetail", this);
		}

		async private void UpdateAccount ()
		{
			string locationId = Account.LocationId;
			string consumerTypeId = selectedConsumerTypeId.ToString ();
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
				};
				alert.Show ();
			}
		}

		private void FillLocationInfoToViews(LocationModel location)
		{
			UnitContentView.Hidden = false;
			ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, 500.0f);

			UnitNameLB.Text = location.LocationName;
			Address1LB.Text = location.LocationAddress1;
			Address2LB.Text = " ";
			CityLB.Text = location.LocationCity;
			StateLB.Text = location.LocationStateCd;
			ZipLB.Text = location.LocationZip;
			BusinessSegmentLB.Text = location.DivisionDescription;
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAddOutlet") {
				SDXAddOutletVC vc = segue.DestinationViewController as SDXAddOutletVC;

				AccountModel account = new AccountModel ();
				account.LocationId = UnitNumberTF.Text;
				account.Location = new LocationModel ();
				account.Location.LocationName = UnitNameLB.Text;
				account.Location.LocationAddress1 = Address1LB.Text + " " + Address2LB.Text;
				account.Location.LocationCity = CityLB.Text;
				account.Location.LocationStateCd = StateLB.Text;
				account.Location.LocationZip = ZipLB.Text;
				account.Location.DivisionDescription = BusinessSegmentLB.Text;
				account.ConsumerTypeId = selectedConsumerTypeId;

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
	}
}

