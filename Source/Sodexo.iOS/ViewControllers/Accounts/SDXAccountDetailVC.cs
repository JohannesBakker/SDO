
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.iOS
{
	public partial class SDXAccountDetailVC : SDXBaseVC
	{
		public AccountModel Account;
		public string LocationId;

		private IList <OutletModel> _outlets;
		private List<bool> isAddDetailAppearAry = new List<bool> ();

		private OutletModel selectedOutlet;
		private int offerIdx, outletIdx;
		private RectangleF rectTobeScrolled;

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

			UIBarButtonItem addMenuItem = new UIBarButtonItem (UIImage.FromBundle("icon_add.png"), 
				UIBarButtonItemStyle.Plain, (s, e)=>{ OnAddMenuBtn_Pressed(s, e); });
			NavigationItem.RightBarButtonItem = addMenuItem;

			TableView.Delegate = new SDXOutletTableDelegate ();
			TableView.DataSource = new SDXOutletTableDataSource ();
			((SDXOutletTableDataSource)TableView.DataSource).OfferRowSelectedEvent += (s, o) => OnOfferRow_Selected (s, o);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (1);

			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = null;

			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector ("OnDeleteOffer_Notificaton:"), Constants.NotificationDeleteOffer, null);

			AccountInfoView.Hidden = true;

			if (Account != null) {
				FillContents ();
			}
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (Account == null) {
				LoadAccount ();
			} else {
				bool isSuccess = await LoadReferenceData ();
				if (isSuccess)
					TableView.ReloadData ();
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
				ShowErrorMessage (manager.ErrorMessage);
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
				PerformSegue ("SegueToOfferDetail", this);
			} else {
				((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
				PerformSegue ("SegueToSelectPlanogram", this);
			}
		}
		#endregion

		#region Button Actions
		private void OnAddMenuBtn_Pressed (object sender, EventArgs ea)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			selectedOutlet = null;
			PerformSegue ("SegueToAddOutlet", this);
		}

		partial void OnAccountEditBtn_Pressed (UIButton sender)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;
			PerformSegue ("SegueToLookupAccount", this);
		}

		partial void OnCellOutletEditBtn_Pressed (UIButton sender)
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC = this;

			UIButton btn = sender as UIButton;
			selectedOutlet = _outlets.ElementAt (btn.Tag - 400);
			PerformSegue ("SegueToAddOutlet", this);
		}

		async partial void OnCellAddDetailAddBtn_Pressed (SDXButton sender)
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
				ShowErrorMessage (manager.ErrorMessage);
			} else {
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Offer has been successfully added.", null, "OK", null);
				alert.Clicked += (object s, UIButtonEventArgs e) => {
					outlet.Offers.Add (newOffer);
					ReloadCell (btn.Tag - 200);
				};
				alert.Show ();
			}
		}

		partial void OnCellAddDetailCancelBtn_Pressed (SDXButton sender)
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

			if (segue.Identifier == "SegueToLookupAccount") {
				var vc = segue.DestinationViewController as SDXLookupAccountVC;
				vc.Account = Account;
			} else if (segue.Identifier == "SegueToAddOutlet") {
				var vc = segue.DestinationViewController as SDXAddOutletVC;
				vc.Outlet = selectedOutlet;
				vc.LocationId = Account.LocationId;
				vc.Account = Account;
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
			}
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			AccountInfoView.Hidden = false;

			LocationNameLB.Text = Account.Location.LocationName;
			AddressLB.Text = Account.Location.LocationCity + " " + Account.Location.LocationAddress1;
			LocationIDLB.Text = Account.Location.LocationId;

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

		async private void LoadAccount ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			ShowLoading ("Loading...");
			Account = await manager.LoadAccount (LocationId, false);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
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
		#endregion
	}
}

