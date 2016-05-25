
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;
using System.Collections.Generic;
using Sodexo.Core;
using TinyIoC;
using System.Linq;

namespace Sodexo.Ipad2
{
	public partial class SDXAccountDetailVC : SDXBaseVC
	{
		enum DetailType
		{
			SELECT_PLANOGRAM = 0,
			OFFER_DETAIL
		};

		private bool forOutletsFlag = false;
		public bool IsNeedReload = false;
		private IList<AccountModel> _accounts;

		public AccountModel Account;
		public string LocationId;

		private IList <OutletModel> _outlets;
		private List<bool> isAddDetailAppearAry = new List<bool> ();

		private OutletModel selectedOutlet;
		private int offerIdx, outletIdx;
		private RectangleF rectTobeScrolled;

		private UIView[] _accountViews;

		private UIViewController _curVC;
		private UIImage imageforFull;
		private String docUrl;
		private AccountModel _editAccount;

		public static SDXSelectPlanogramVC _selectPlanogramVC = null;
		public static SDXOfferDetailVC _offerDetailVC = null;


		public SDXAccountDetailVC (IntPtr handle) : base (handle)
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

			TableView.Delegate = new SDXOutletTableDelegate ();
			TableView.DataSource = new SDXOutletTableDataSource ();
			((SDXOutletTableDataSource)TableView.DataSource).OfferRowSelectedEvent += (s, o) => OnOfferRow_Selected (s, o);

			AccountListView.Hidden = true;
			Util.MoveViewToY (AccountListView, -120);
			SelectPlanogramView.Hidden = true;
			OfferDetailView.Hidden = true;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (1);
			SetSectionName ("Planograms");

			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;

			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector ("OnDeleteOffer_Notificaton:"), Constants.NotificationDeleteOffer, null);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			AccountScrollView.PanGestureRecognizer.DelaysTouchesBegan = AccountScrollView.DelaysContentTouches;

			if (_accounts == null || _accounts.Count == 0 || IsNeedReload)
				LoadAccounts ();
			else {
				//TableView.ReloadData ();
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			NSNotificationCenter.DefaultCenter.RemoveObserver (this, (NSString)Constants.NotificationDeleteOffer, null);
		}
		#endregion

		#region Notifications
		[Export ("OnDeleteOffer_Notificaton:")]
		public async void OnDeleteOffer_Notificaton (NSNotification notification)
		{
			NSDictionary dict = notification.UserInfo;
			string outletId = dict.ObjectForKey ((NSString)"outlet_id").ToString();
			string offerId = dict.ObjectForKey ((NSString)"offer_id").ToString();
			string locationId = Account.LocationId;

			ShowLoading ("Deleting...");
			SDXAccountManager manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteOffer (locationId, outletId, offerId);
			HideLoading ();

			if (manager.IsSuccessed) {
				OutletModel outlet = null;
				int row = 0;
				foreach (OutletModel model in _outlets) {
					if (model.OutletId.ToString () == outletId) {
						outlet = model;
						break;
					}
					row++;
				}
				foreach (OfferModel offer in outlet.Offers) {
					if (offer.OfferId.ToString () == offerId) {
						((List<OfferModel>)outlet.Offers).Remove (offer);
						break;
					}
				}
				ReloadCell (row);
			} else {
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
			}
		}
		#endregion

		#region Event Handler
		private void OnOfferRow_Selected (object sender, OfferModel offer)
		{
			OutletModel outlet = null;
			outletIdx = 0;
			foreach (OutletModel item in Account.Outlets) {
				if (item.OutletId == offer.OutletId) {
					outlet = item;
					break;
				}
				outletIdx++;
			}
			if (outlet == null)
				return;
			offerIdx = outlet.Offers.IndexOf (offer);

			if (offer.Responses.Count != 0) {
				selectedOutlet = outlet;
				((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
				ChangeDetailView (DetailType.OFFER_DETAIL);
				//PerformSegue ("SegueToOfferDetail", this);
			} else {
				((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
				//PerformSegue ("SegueToSelectPlanogram", this);
				ChangeDetailView (DetailType.SELECT_PLANOGRAM);
			}
		}
		#endregion

		#region Button Actions
		partial void AddNewAccount_Pressed (MonoTouch.UIKit.UIButton sender)
		{
			forOutletsFlag = false;
			PerformSegue("SegueToAccount", this);
		}

		partial void AddNewOutlet_Pressed(MonoTouch.UIKit.UIButton sender)
		{
			if (this.Account == null)
				return;

			forOutletsFlag = true;
			PerformSegue("SegueToAccount", this);
		}

		private void OnAddMenuBtn_Pressed (object sender, EventArgs ea)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			selectedOutlet = null;
			//PerformSegue ("SegueToAddOutlet", this);
		}

		partial void OnCellOutletEditBtn_Pressed (UIButton sender)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;

			UIButton btn = sender as UIButton;
			selectedOutlet = _outlets.ElementAt (btn.Tag - 400);
			//PerformSegue ("SegueToAddOutlet", this);
			PerformSegue("SegueToEditOutlet", this);
		}

		async partial void OnCellAddDetailAddBtn_Pressed (Sodexo.Ipad2.SDXButton sender)
		{
			UIButton btn = sender as UIButton;
			OutletModel outlet = _outlets[btn.Tag - 200];
			string outletId = outlet.OutletId.ToString ();
			SDXOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(btn.Tag - 200, 0)) as SDXOutletCell;
			if (cell.SelectedOfferCategoryIdx == -1)
				return;
			var dataManager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			OfferCategoryModel offerCategory = dataManager.DataModel.OfferCategories [cell.SelectedOfferCategoryIdx];

			ShowLoading ("Adding...");
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OfferModel newOffer = await manager.AddOffer (Account.LocationId, outletId, offerCategory.Description, offerCategory.OfferCategoryId.ToString());
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Error", manager.ErrorMessage, null, "OK", null).Show ();
			} else {
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Offer has been successfully added.", null, "OK", null);
				alert.Clicked += (object s, UIButtonEventArgs e) => {
					outlet.Offers.Add (newOffer);
					isAddDetailAppearAry[btn.Tag - 200] = false;
					ReloadCell (btn.Tag - 200);
				};
				alert.Show ();
			}

		}

		partial void OnCellAddDetailCancelBtn_Pressed (Sodexo.Ipad2.SDXButton sender)
		{
			UIButton btn = sender as UIButton;
			isAddDetailAppearAry[btn.Tag - 300] = false;

			ReloadCell (btn.Tag - 300);
		}

		partial void OnCellAddNewBtn_Pressed (UIButton sender)
		{
			UIButton btn = sender as UIButton;
			isAddDetailAppearAry[btn.Tag - 100] = true;

			ReloadCell (btn.Tag - 100);

			RectangleF rect = TableView.RectForRowAtIndexPath (NSIndexPath.FromRowSection (btn.Tag - 100, 0));
			rect.Y = rect.Y + rect.Height - 165;
			rect.Height = 165;
			rectTobeScrolled = rect;

			PerformSelector (new MonoTouch.ObjCRuntime.Selector ("ScrollRectToVisible"), null, 0.5f);
		}

		partial void ManageAccount_Pressed (MonoTouch.UIKit.UIButton sender)
		{
			if (AccountListView.Hidden == false)
			{
				RectangleF frameList = AccountListView.Frame;
				RectangleF frameManage = ManagePanelView.Frame;

				UIView.Animate (0.3f, () => {
					AccountListView.Frame = new RectangleF (0, -frameList.Height, frameList.Width, frameList.Height);
					ManagePanelView.Frame = new RectangleF (frameManage.X, 0, frameManage.Width, frameManage.Height);	
				}, () => {
					AccountListView.Hidden = true;
				});
				//Util.MoveViewToY(ManagePanelView, 0);
			}
			else
			{
				RectangleF frameList = AccountListView.Frame;
				RectangleF frameManage = ManagePanelView.Frame;

				AccountListView.Hidden = false;

				UIView.Animate (0.3f, () => {
					AccountListView.Frame = new RectangleF (0, 0, frameList.Width, frameList.Height);
					ManagePanelView.Frame = new RectangleF (frameManage.X, 115, frameManage.Width, frameManage.Height);	
				}, () => {
					//AccountListView.Hidden = false;
				});

				//Util.MoveViewToY(ManagePanelView, 120);
			}
		}
		#endregion

		#region Selector
		[Export ("ScrollRectToVisible")]
		private void ScrollRectToVisible ()
		{
			TableView.ScrollRectToVisible (rectTobeScrolled, true);
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAccount") {
				if (forOutletsFlag == true) {
					var vc = segue.DestinationViewController as SDXAccountsVC;
					vc.LocationId = this.Account.LocationId;
					vc.Account = this.Account;
					//vc.Outlets = this.Account.Outlets.ToList();
					vc.bUpdateOutlet = true;
				}
			}
			if (segue.Identifier == "SegueToLookupAccount") {
				var vc = segue.DestinationViewController as SDXLookupAccountVC;
				vc.Account = Account;
			} else if (segue.Identifier == "SegueToAddOutlet") {
				//var vc = segue.DestinationViewController as SDXAddOutletVC;
				//vc.Outlet = selectedOutlet;
				//vc.LocationId = Account.LocationId;
				//vc.Account = Account;
			} else if (segue.Identifier == "SegueToSelectPlanogram") {
				var vc = segue.DestinationViewController as SDXSelectPlanogramVC;
				vc.Account = Account;
				vc.OutletIndex = outletIdx;
				vc.OfferIndex = offerIdx;
			} else if (segue.Identifier == "SegueToOfferDetail") {
				var vc = segue.DestinationViewController as SDXOfferDetailVC;
				vc.Account = Account;
				vc.OutletIndex = outletIdx;
				vc.OfferIndex = offerIdx;
			} else if (segue.Identifier == "SegueToFullImage") {
				var vc = segue.DestinationViewController as SDXFullImageVC;
				vc.Picture = imageforFull;
			} else if (segue.Identifier == "SegueToDoc") {
				var vc = segue.DestinationViewController as SDXDocVC;
				vc.DocUrl = docUrl;
			} else if (segue.Identifier == "SegueToEditAccount") {
				var vc = segue.DestinationViewController as SDXEditAccountVC;
				vc.Account = _editAccount;
			} else if (segue.Identifier == "SegueToEditOutlet") {
				var vc = segue.DestinationViewController as SDXEditOutletVC;
				vc.LocationId = Account.LocationId;
				vc.Outlet = selectedOutlet;
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

		public void ChangeToOfferDetail()
		{
			FillContents ();
			ChangeDetailView (DetailType.OFFER_DETAIL);
		}

		public void ChangeToDocView(String docUrl)
		{
			this.docUrl = docUrl;
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			PerformSegue ("SegueToDoc", this);
		}

		public void ChangeToFullImageView(UIImage image)
		{
			imageforFull = image;
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			PerformSegue ("SegueToFullImage", this);
		}

		public void RefreshTable()
		{
			FillContents ();
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			_outlets = Account.Outlets;

			isAddDetailAppearAry.Clear ();
			for (int i = 0; i < _outlets.Count(); i++)
				isAddDetailAppearAry.Add (false);

			var tvDelegate = (SDXOutletTableDelegate)TableView.Delegate;
			tvDelegate.Outlets = _outlets;
			tvDelegate.IsAddDetailAppearAry = isAddDetailAppearAry;
			var tvDataSource = (SDXOutletTableDataSource)TableView.DataSource;
			tvDataSource.Outlets = _outlets;
			tvDataSource.IsAddDetailAppearAry = isAddDetailAppearAry;

			TableView.ReloadData ();
		}

		async private void LoadAccounts()
		{
			var authManager = TinyIoCContainer.Current.Resolve <SDXAuthManager> ();
			if (!authManager.IsAuthenticated)
				return;

			var userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			IsNeedReload = true; //force refresh always
			ShowLoading ("Loading...");
			if (userManager.Me == null || IsNeedReload || userManager.Me.Accounts == null) {
				IsNeedReload = false;

				await userManager.LoadMe ();
				//HideLoading ();

				if (!userManager.IsSuccessed) {
					HideLoading ();
					ShowErrorMessage (userManager.ErrorMessage);
					return;
				}
			}

			_accounts  = userManager.Me.Accounts;

			if (_accounts.Count == 0) {
				PerformSegue ("SegueToAccount", this);
			} else {
				//ShowLoading ("Loading...");
				UIView accountListView = CreateAccountListView ();
				AccountScrollView.AddSubview (accountListView);				
				AccountScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				AccountScrollView.ContentSize = new SizeF (accountListView.Frame.Width, AccountScrollView.Frame.Height);

				if (accountListView.Frame.Width < 920)
					AccountScrollView.Frame = new RectangleF(AccountScrollView.Frame.Left, AccountScrollView.Frame.Top, accountListView.Frame.Width, AccountScrollView.Frame.Height);
				//AccountScrollView.CanCancelContentTouches = true;
				//AccountScrollView.DelaysContentTouches = true;


				Console.WriteLine (accountListView.Frame.Width + "------------------");

				((SDXAccountItemView)accountListView.Subviews [0]).SetFocus (true);
				if (dashboardItem != null && dashboardItem.ModelId>0) {
					var accountNumber = dashboardItem.ModelId;
					Account = _accounts.First (account => account.LocationId==accountNumber.ToString());
					if (Account == null) {
						Account = _accounts [0];
					}
						
					//Console.WriteLine (accountNumber.ToString () + " " + _accounts [0].LocationId + "------------------");
				} else {
					Account = _accounts [0];
				}

				if (Account != null) {
					FillContents ();
				}

				//HideLoading ();

				if (Account == null) {
					LoadAccount ();
				} else {
					//ShowLoading ("Loading...");
					bool isSuccess = await LoadReferenceData ();
					if (isSuccess)
						TableView.ReloadData ();
				}
			}

			HideLoading ();
		}

		async private void LoadAccount ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			ShowLoading ("Loading...");
			Account = await manager.LoadAccount (LocationId, false);

			if (!manager.IsSuccessed) {
				HideLoading ();
				Util.ShowAlert (manager.ErrorMessage);
				return;
			}

			FillContents ();

			bool isSuccess = await LoadReferenceData ();
			HideLoading ();
			if (isSuccess)
				TableView.ReloadData ();
		}

		private void ReloadCell (int row)
		{
			NSIndexPath[] paths = {NSIndexPath.FromRowSection (row, 0)};
			TableView.ReloadRows (paths, UITableViewRowAnimation.Automatic);
		}

		private void ChangeDetailView(DetailType type)
		{
			switch (type) {
			case DetailType.SELECT_PLANOGRAM:
				if (_selectPlanogramVC == null) {
					_selectPlanogramVC = (SDXSelectPlanogramVC)Storyboard.InstantiateViewController ("SDXSelectPlanogramVC");
				}

				if (_selectPlanogramVC != null) {
					_selectPlanogramVC.Account = Account;
					_selectPlanogramVC.OutletIndex = outletIdx;
					_selectPlanogramVC.OfferIndex = offerIdx;
					_selectPlanogramVC.InitializeView (this);

					SelectPlanogramView.Hidden = false;
					OfferDetailView.Hidden = true;

					SelectPlanogramView.Add (_selectPlanogramVC.View);
					//this.View.BringSubviewToFront (SelectPlanogramView);
					//this.View.BringSubviewToFront (ManagePanelView);
				}
				break;
			case DetailType.OFFER_DETAIL:
				if (_offerDetailVC == null) {
					_offerDetailVC = (SDXOfferDetailVC)Storyboard.InstantiateViewController ("SDXOfferDetailVC");
				}

				if (_offerDetailVC != null) {
					_offerDetailVC.Account = Account;
					_offerDetailVC.OutletIndex = outletIdx;
					_offerDetailVC.OfferIndex = offerIdx;
					_offerDetailVC.InitializeView (this);

					SelectPlanogramView.Hidden = true;
					OfferDetailView.Hidden = false;

					OfferDetailView.Add (_offerDetailVC.View);

					//this.View.BringSubviewToFront (OfferDetailView);
					//this.View.BringSubviewToFront (ManagePanelView);
				}
				break;
			}
		}

		private UIView CreateAccountListView()
		{
			UIView view = new UIView ();
			view.BackgroundColor = UIColor.Red;
			if (_accounts == null || _accounts.Count <= 0)
				return view;

			int number = 0;
			float left = 0;
			float padding = 5;

			foreach (AccountModel account in _accounts) {
				var node = SDXAccountItemView.Create ();
				node.Update (account, number);

				node.AccountBtnPressedEvent = OnAccountListItem_Pressed;
				node.AccountEditBtnPressedEvent = OnAccountEditItem_Pressed;
				node.Tag = 100 + number;

				view.AddSubview (node);
				Util.MoveViewToX (node, left);
				number++;

				left += node.Frame.Width + padding;
			}

			view.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			view.Frame = new RectangleF (0, 0, left, 100);

			return view;
		}

		async void OnAccountListItem_Pressed (object sender, int tag)
		{
			var selectedBtn = sender as UIButton;
			UIView accountView = selectedBtn.Superview.Superview.Superview;
			//((UIImageView)answersView.Superview).Highlighted = true;

			for (int i = 0;; i++) {
				var view = accountView.ViewWithTag (i + 100) as UIView;
				if (view == null)
					break;

				((SDXAccountItemView)view).SetFocus (false);
			}

			UIView focusView = selectedBtn.Superview.Superview;
			((SDXAccountItemView)focusView).SetFocus (true);

			ManageAccount_Pressed (null);

			Account = _accounts [tag];

			if (Account != null) {
				FillContents ();
			}

			if (Account == null) {
				LoadAccount ();
			} else {
				bool isSuccess = await LoadReferenceData ();
				if (isSuccess)
					TableView.ReloadData ();
			}

			SelectPlanogramView.Hidden = true;
			OfferDetailView.Hidden = true;
		}

		async void OnAccountEditItem_Pressed(object sender, int tag)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			_editAccount = _accounts [tag];
			PerformSegue ("SegueToEditAccount", this);
		}
		#endregion
	}
}

