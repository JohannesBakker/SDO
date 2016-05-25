
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using System.Collections.Generic;
using TinyIoC;
using System.Linq;

namespace Sodexo.Ipad2
{
	public partial class SDXEditAccountVC : SDXBaseVC
	{
		private AccountModel forDetailAccount = null;

		private int selectedConsumerTypeID = -1;
		private int selectedAccountIndex = -1;

		public AccountModel Account = null;
		private UIViewController _curVC;

		public SDXEditAccountVC (IntPtr handle) : base (handle)
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

			// Set DropTableView Row Select Event
			DropTableView.SelectedEvent += OnDropTableCell_Selected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		async public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await LoadReferenceData ();

			UnitNumberTF.Text = Account.Location.LocationId;

			FillLocationInfoToViews (Account.Location);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}
		#endregion

		#region Private Functions
		private void FillLocationInfoToViews(LocationModel location)
		{
			UnitNameLB.Text = location.LocationName;
			Address1LB.Text = location.LocationAddress1;
			Address2LB.Text = " ";
			CityLB.Text = location.LocationCity;
			StateLB.Text = location.LocationStateCd;
			ZipLB.Text = location.LocationZip;
			ConsumerTypeLB.Text = Account.ConsumerType.Description;

			selectedConsumerTypeID = Account.ConsumerTypeId;
		}
		#endregion

		#region BUTTON EVENTS
		partial void OnHideDropView (MonoTouch.UIKit.UIButton sender)
		{
			RectangleF frame = ConsumerTypeLB.Frame;
			frame.Y += UnitContentView.Frame.Y;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			}, () => {
				DropView.Hidden = true;
			});
		}

		async partial void OnConsumerTypeBtn_Pressed (MonoTouch.UIKit.UIButton sender)
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
			DropTableView.Frame = new RectangleF (frame.X + 2, frame.Y + frame.Height, frame.Width - 10, 1.0f);
			DropTableView.BackgroundColor = UIColor.White;
			DropView.Hidden = false;
			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X + 5, frame.Y + frame.Height, frame.Width - 10, fHeight);
			});
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
				PerformSegue("SegueToAccountDetail", this);
			};
			alert.Show ();
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

				if (selectedConsumerTypeID == -1) {
					new UIAlertView ("Retail Ranger", "Please select consumer type.", null, "OK", null).Show();
					return;
				}

				PerformSegue("SegueToAccountDetail", this);
			} else {
				if (Account.ConsumerType.Description == ConsumerTypeLB.Text) {
					new UIAlertView ("Retail Ranger", "No changes, you can change Consumer Type to update.", null, "OK", null).Show ();
					return;
				} else {
					UpdateAccount ();
				}
			}

		}
		#endregion

		#region Event Handler
		void OnDropTableCell_Selected(object sender, int row)
		{
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			ConsumerTypeModel type = manager.DataModel.ConsumerTypes.ElementAt (row);
			selectedConsumerTypeID = type.ConsumerTypeId;
			ConsumerTypeLB.Text = type.Description;

			OnHideDropView (null);
		}
		#endregion

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
					PerformSegue("SegueToAccountDetail", this);
				};
				alert.Show ();
			}
		}

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAccountDetail") {
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
	}
}

