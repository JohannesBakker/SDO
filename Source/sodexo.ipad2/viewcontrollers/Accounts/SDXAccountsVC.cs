
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;
using System.Collections.Generic;
using TinyIoC;
using Sodexo.Core;
using System.Linq;

namespace Sodexo.Ipad2
{
	public partial class SDXAccountsVC : SDXBaseVC
	{
		public AccountModel Account = null;
		public bool IsFromDashboard = false;

		private IList <AccountModel> accountList;
		private AccountModel forDetailAccount = null;
		private LocationModel locationModel = null;
		private int selectedConsumerTypeId = -1;
		private int selectedAccountIndex = -1;

		private UIViewController _curVC;

		public bool IsNeedReload = false;
		private IList<AccountModel> _accounts;
		public int AccountIndex;
		public string LocationId;
		public LocationModel Location;

		public List <OutletModel> Outlets = new List<OutletModel> ();
		private AccountModel newAccount = null;

		public OutletModel Outlet = null;				// For Updating Outlet
		public bool bUpdateOutlet = false;

		private UIButton currentDropBtn = null;

		public SDXAccountsVC (IntPtr handle) : base (handle)
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

			DropTableView.SelectedEvent += OnDropTableCell_Selected;

			// For outlet
			OutletDropTableView.SelectedEvent += ((object sender, int index) => OnOutletDropTableCell_Selected(sender, index));

			TableView.Delegate = new SDXAddOutletTableDelegate (Outlets);

			if (bUpdateOutlet == false)
				TableView.DataSource = new SDXAddOutletTableDataSource (Outlets, "");
			else
				TableView.DataSource = new SDXAddOutletTableDataSource (Outlets, "Add another outlet to " + Account.LocationId.ToString());

			RectangleF mainViewFrame = View.Frame;
			RectangleF lookupViewFrame = LookupView.Frame;
			RectangleF outletViewFrame = OutletView.Frame;

			if (bUpdateOutlet == false) {
				OutletView.Hidden = true;
				LookupView.Frame = new RectangleF (262, lookupViewFrame.Top, lookupViewFrame.Width, lookupViewFrame.Height);
				UnitView.Hidden = true;
				TitleOneLB.Hidden = false;
				TitleTwoLB.Hidden = false;
			} else {
				OutletView.Hidden = false;
				LookupView.Hidden = true;
				OutletView.Frame = new RectangleF (262, outletViewFrame.Top, outletViewFrame.Width, outletViewFrame.Height);
				TitleOneLB.Hidden = true;
				TitleTwoLB.Hidden = true;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await LoadReferenceData ();

			if (Location != null) {
				UnitNumberTF.Text = Location.LocationId;

				FillLocationInfoToViews (Location);
			}
			/*
			if (bUpdateOutlet == true) {
				int index = 0;
				foreach (OutletModel outletData in Outlets) {
					SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection (index, 0)) as SDXAddOutletCell;
					if (outletData != null && cell != null) {
						cell.FillContents (outletData);
					}
					index++;
				}
			}*/
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

		partial void OnHideDropView (UIButton sender)
		{
			RectangleF frame = ConsumerTypeLB.Frame;
			frame.Y += UnitView.Frame.Y;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			}, () => {
				DropView.Hidden = true;
			});
		}

		partial void OnHideOutletDropTableView (UIButton sender)
		{
			RectangleF frame = OutletDropTableView.Frame;
			UIView.Animate (0.3f, () => {
				OutletDropTableView.Frame = new RectangleF (frame.X, frame.Y, frame.Width, 1.0f);
			}, () => {
				OutletDropView.Hidden = true;
			});
		}

		partial void OnNextBtn_Pressed (SDXButton sender)
		{
			if (Account != null)
				return;

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

				//PerformSegue ("SegueToAccountDetail", this);
				AccountModel account = new AccountModel ();
				account.LocationId = UnitNumberTF.Text;
				account.Location = new LocationModel ();
				account.Location.LocationName = UnitNameLB.Text;
				account.Location.LocationAddress1 = AddressOneLB.Text + " " + AddressTwoLB.Text;
				account.Location.LocationCity = CityLB.Text;
				account.Location.LocationStateCd = StateLB.Text;
				account.Location.LocationZip = ZipLB.Text;
				account.Location.DivisionDescription = BusinessSegLB.Text;
				account.ConsumerTypeId = selectedConsumerTypeId;

				Account = account;

				if (OutletView.Hidden == true)
				{
					RectangleF lookupDestFrame = new RectangleF(8, LookupView.Frame.Top, LookupView.Frame.Width, LookupView.Frame.Height);
					OutletView.Hidden = false;
					OutletView.Alpha = 0;

					UIView.Animate (0.3f, () => {
						LookupView.Frame = lookupDestFrame;
						OutletView.Alpha = 1;	
					}, () => {

					});
				}
			} else {
				if (Account.ConsumerType.Description == ConsumerTypeLB.Text) {
					new UIAlertView ("Retail Ranger", "No changes, you can change Consumer Type to update.", null, "OK", null).Show ();
					return;
				} else {
					UpdateAccount ();
				}
			}
		}

		async partial void OnConsumerBtn_Pressed (UIButton sender)
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
			frame.Y += UnitView.Frame.Y;
			DropTableView.Frame = new RectangleF (frame.X, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			DropTableView.BackgroundColor = UIColor.White;
			DropView.Hidden = false;
			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X, frame.Y + frame.Height, frame.Width - 10, fHeight);
			});
		}

		partial void AddBtn_Pressed (UIButton sender)
		{
			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(Outlets.Count, 0)) as SDXAddOutletCell;

			if (cell != null) {
				if (cell.IsFilled()) {
					Outlets.Add (cell.GetOutlet());
				} else {
					new UIAlertView("Message", "Please fill all fields.", null, "OK", null).Show();
					return;
				}
			}

			//NumberLB.Text = (Outlets.Count + 2).ToString ("00");

			NSIndexPath[] paths = {NSIndexPath.FromRowSection(Outlets.Count, 0)};
			TableView.InsertRows (paths, UITableViewRowAnimation.Fade);
		}

		partial void OnFinishSetupBtn_Pressed (SDXButton sender)
		{
			if (Outlet != null) {
				SDXAddOutletCell firstCell = TableView.CellAt (NSIndexPath.FromRowSection(0, 0)) as SDXAddOutletCell;
				if (firstCell.IsFilled()) {
					OutletModel upOutlet = firstCell.GetOutlet ();
					if (Outlet.Name == upOutlet.Name && Outlet.ConsumersOnSiteRangeId == upOutlet.ConsumersOnSiteRangeId && 
						Outlet.AnnualSalesRangeId == upOutlet.AnnualSalesRangeId && Outlet.LocalCompetitionTypeId == upOutlet.LocalCompetitionTypeId) {
						new UIAlertView ("Retail Ranger", "No changes, please change items and press done button.", null, "OK", null).Show ();
					} else {
						Outlet.Name = upOutlet.Name;
						Outlet.ConsumersOnSiteRangeId = upOutlet.ConsumersOnSiteRangeId;
						Outlet.AnnualSalesRangeId = upOutlet.AnnualSalesRangeId;
						Outlet.LocalCompetitionTypeId = upOutlet.LocalCompetitionTypeId;
						UpdateOutlet ();
					}
				} else {
					new UIAlertView ("Retail Ranger", "Outlet should have name.", null, "OK", null).Show ();
				}
				return;
			}

			if (LocationId != null) {
				SDXAddOutletCell firstCell = TableView.CellAt (NSIndexPath.FromRowSection(0, 0)) as SDXAddOutletCell;
				if (firstCell.IsFilled()) {
					OutletModel outlet = firstCell.GetOutlet ();
					AddOutlet (outlet);
				} else {
					new UIAlertView ("Retail Ranger", "Please fill all fields.", null, "OK", null).Show ();
				}
				return;
			}

			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(Outlets.Count, 0)) as SDXAddOutletCell;
			if (cell != null && cell.IsFilled())
				Outlets.Add (cell.GetOutlet());

			if (Outlets.Count == 0) {
				new UIAlertView ("Retail Ranger", "Please add at least one outlet.", null, "OK", null).Show();
				return;
			}

			//Account.ConsumerTypeId = selectedConsumerTypeId;
			Account.Outlets = Outlets;

			AddAccount ();
		}

		partial void OnCellDropBtn_Pressed (UIButton sender)
		{
			if (OutletDropView.Hidden == false)
				OnHideOutletDropTableView (null);

			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;

			UIButton btn = currentDropBtn = sender as UIButton;
			List<string> dropDescList = new List<string> ();
			int row = 0;

			if (btn.Tag >= 300) {
				foreach (LocalCompetitionTypeModel item in manager.DataModel.LocalCompetitionTypes) {
					dropDescList.Add (item.Description);
				}
				row = btn.Tag - 300;
			} else if (btn.Tag >= 200) {
				foreach (AnnualSalesRangeModel item in manager.DataModel.AnnualSalesRanges) {
					dropDescList.Add (item.Description);
				}
				row = btn.Tag - 200;
			} else {
				foreach (ConsumersOnSiteRangeModel item in manager.DataModel.ConsumersOnSiteRanges) {
					dropDescList.Add (item.Description);
				}
				row = btn.Tag - 100;
			}

			OutletDropTableView.SetDropDescriptionList (dropDescList);
			OutletDropTableView.ReloadData ();
			OutletDropTableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, false);

			RectangleF frame = OutletDropTableView.Frame;
			frame.Y = ((230 * row) + btn.Frame.Y + btn.Frame.Size.Height) - TableView.ContentOffset.Y + TableView.Frame.Y;
			frame.Height = 1.0f;

			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			OutletDropTableView.Frame = frame;
			OutletDropTableView.BackgroundColor = UIColor.White;
			OutletDropView.Hidden = false;
			UIView.Animate (0.3f, () => {
				frame.Height = fHeight;
				OutletDropTableView.Frame = frame;
			});
		}

		partial void OnCellAnnualSalesInfoBtn_Pressed (UIButton sender)
		{
			Util.ShowAlert ("This is AnnualSales Field.");
		}

		partial void OnCellConsumersInfoBtn_Pressed (UIButton sender)
		{
			Util.ShowAlert ("This is Consumers on Site Field.");
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

		void OnOutletDropTableCell_Selected(object sender, int row)
		{
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();

			int cellRow = 0, tag = 0;
			string desc = "";
			int id = 0;
			if (currentDropBtn.Tag >= 300) {
				LocalCompetitionTypeModel model = manager.DataModel.LocalCompetitionTypes.ElementAt (row);
				desc = model.Description;
				id = model.LocalCompetitionTypeId;
				cellRow = currentDropBtn.Tag - 300;
				tag = 4;
			} else if (currentDropBtn.Tag >= 200) {
				AnnualSalesRangeModel model = manager.DataModel.AnnualSalesRanges.ElementAt (row);
				desc = model.Description;
				id = model.AnnualSalesRangeId;
				cellRow = currentDropBtn.Tag - 200;
				tag = 3;
			} else {
				ConsumersOnSiteRangeModel model = manager.DataModel.ConsumersOnSiteRanges.ElementAt (row);
				desc = model.Description;
				id = model.ConsumersOnSiteRangeId;
				cellRow = currentDropBtn.Tag - 100;
				tag = 2;
			}

			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(cellRow, 0)) as SDXAddOutletCell;
			UITextField tf = cell.ContentView.ViewWithTag (tag) as UITextField;
			tf.Text = desc;

			if (cellRow < Outlets.Count) {
				OutletModel outlet = Outlets.ElementAt (cellRow);
				if (tag == 2) {
					outlet.ConsumersOnSiteRangeId = id;
				} else if (tag == 3) {
					outlet.AnnualSalesRangeId = id;
				} else {
					outlet.LocalCompetitionTypeId = id;
				}
			} else {
				if (tag == 2) {
					cell.ConsumerID = id;
				} else if (tag == 3) {
					cell.AnnualSalesID = id;
				} else {
					cell.LocalCompetitonID = id;
				}
			}

			OnHideOutletDropTableView (null);
		}
		#endregion

		#region Private Functions
		private void FillLocationInfoToViews(LocationModel location)
		{
			UnitView.Hidden = false;

			UnitNameLB.Text = location.LocationName;
			AddressOneLB.Text = location.LocationAddress1;
			AddressTwoLB.Text = " ";
			CityLB.Text = location.LocationCity;
			StateLB.Text = location.LocationStateCd;
			ZipLB.Text = location.LocationZip;
			BusinessSegLB.Text = location.DivisionDescription;

			if (OutletView.Hidden == false) {
				UIView.Animate (0.3f, () => {
					LookupView.Frame = new RectangleF(262, LookupView.Frame.Top, LookupView.Frame.Width, LookupView.Frame.Height);
					OutletView.Alpha = 0;
				}, () => {
					OutletView.Hidden = true;
				});
			}
			TitleTwoLB.Text = "Let's gather some basic information about your location";
		}

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
				/*	SDXAccountDetailVC vc = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC as SDXAccountDetailVC;
					vc.Account = account;
					NavigationController.PopToViewController (vc, true);*/
				};
				alert.Show ();
			}
		}

		async private void AddAccount ()
		{
			ShowLoading ("Creating...");
			var accountManger = TinyIoCContainer.Current.Resolve<SDXAccountManager> ();
			newAccount = await accountManger.AddNewAccount (Account);
			HideLoading ();
			if (!accountManger.IsSuccessed) {
				new UIAlertView ("Error", accountManger.ErrorMessage, null, "OK", null).Show ();
			} else {
				// Go To Account Details
				PerformSegue ("SegueToAccountDetail", this);
			}
		}

		async private void UpdateOutlet ()
		{
			ShowLoading ("Updating...");
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OutletModel upOutlet = await manager.UpdateOutlet (LocationId, Outlet);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Error", manager.ErrorMessage, null, "OK", null).Show ();
			} else {
				Outlet.ConsumersOnSiteRange.Description = upOutlet.ConsumersOnSiteRange.Description;
				Outlet.AnnualSalesRange.Description = upOutlet.AnnualSalesRange.Description;
				Outlet.LocalCompetitionType.Description = upOutlet.LocalCompetitionType.Description;
				Outlet.RowVersion = upOutlet.RowVersion;

				UIAlertView alert = new UIAlertView ("Retail Ranger", "Outlet has been successfully updated.", null, "OK", null);
				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					SDXAccountDetailVC vc = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC as SDXAccountDetailVC;
					NavigationController.PopToViewController (vc, true);
				};
				alert.Show ();
			}
		}

		async private void AddOutlet (OutletModel outlet)
		{
			ShowLoading ("Adding...");
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OutletModel newOutlet = await manager.AddOutlet (LocationId, outlet);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Error", manager.ErrorMessage, null, "OK", null).Show ();
			} else {
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Outlet has been successfully added.", null, "OK", null);
				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					//SDXAccountDetailVC vc = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC as SDXAccountDetailVC;
					//vc.Account.Outlets.Add (newOutlet);
					//NavigationController.PopToViewController (vc, true);
					PerformSegue ("SegueToAccountDetail", this);
				};
				alert.Show ();
			}
		}

		async private void DeleteOutlet ()
		{
			ShowLoading ("Deleting");
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteOutlet (LocationId, Outlet.OutletId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage);
				return;
			}

			if (Account != null)
				Account.Outlets.Remove (Outlet);

			UIAlertView alertV = new UIAlertView ("Retail Ranger", "Outlet has been successfully deleted.", null, "OK", null);
			alertV.Clicked += (object s, UIButtonEventArgs e) => {
				NavigationController.PopViewControllerAnimated (true);
			};
			alertV.Show ();
		}

		private void SetPhoto (UIImage image)
		{

		}

		[Export ("UploadPhoto:")]
		private async void UploadPhoto (UIImage image)
		{

		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAddOutlet") {
				/*SDXAddOutletVC vc = segue.DestinationViewController as SDXAddOutletVC;

				AccountModel account = new AccountModel ();
				account.LocationId = UnitNumberTF.Text;
				account.Location = new LocationModel ();
				account.Location.LocationName = UnitNameLB.Text;
				account.Location.LocationAddress1 = AddressOneLB.Text + " " + AddressTwoLB.Text;
				account.Location.LocationCity = CityLB.Text;
				account.Location.LocationStateCd = StateLB.Text;
				account.Location.LocationZip = ZipLB.Text;
				account.ConsumerTypeId = selectedConsumerTypeId;

				vc.Account = account;*/
			} else if (segue.Identifier == "SegueToAccountDetail") {
				SDXAccountDetailVC vc = segue.DestinationViewController as SDXAccountDetailVC;
				vc.Account = newAccount;
				SDXAccountsVC accountsVC = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountsVC as SDXAccountsVC;
				if (accountsVC != null) {
					accountsVC.IsNeedReload = true;
					vc.PopVC = accountsVC;
				} else
					vc.PopVC = NavigationController.ViewControllers [0];

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
		}
		#endregion
	}
}

