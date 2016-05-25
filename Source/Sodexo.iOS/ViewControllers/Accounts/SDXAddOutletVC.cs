
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
	public partial class SDXAddOutletVC : SDXBaseVC
	{

		public AccountModel Account { get; set; }		// For Adding Multi Outlets
		public OutletModel Outlet = null;				// For Updating Outlet
		public string LocationId = null;				// For Adding One Outlet

		private List <OutletModel> outlets = new List<OutletModel> ();
		private AccountModel newAccount = null;

		private UIButton currentDropBtn = null;

		public SDXAddOutletVC (IntPtr handle) : base (handle)
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

			DropTableView.SelectedEvent += ((object sender, int index) => OnDropTableCell_Selected(sender, index));

			TableView.Delegate = new SDXAddOutletTableDelegate (outlets);
			TableView.DataSource = new SDXAddOutletTableDataSource (outlets);

			NextBtn.Layer.CornerRadius = 3;
			NextBtn.Layer.MasksToBounds = true;

			if (LocationId != null || Outlet != null) {
				if (Outlet != null) {
					NavigationItem.Title = "Update Outlet";
				} else
					NavigationItem.Title = "Add Outlet";
				NextBtn.SetTitle ("DONE", UIControlState.Normal);
				HeaderView.Hidden = true;
				AddView.Hidden = true;

				Util.MoveViewToY (TableView, TableView.Frame.Y - 80);
			}

			if (Outlet == null) {
				PhotosView.Hidden = true;
				Util.MoveViewToY (NextBtn, 70);
				Util.ChangeViewHeight (BottomView, 125);
			} else {
				AddPhotoBtn.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
				if (Outlet.Photo != null) {
					var imgManager = new JHImageManager ();
					imgManager.LoadCompleted += (object s, byte[] bytes) => {
						InvokeOnMainThread (delegate {
							if (bytes == null)
								return;
							var img = Util.GetImageFromByteArray (bytes);
							AddPhotoBtn.SetImage (img, UIControlState.Normal);
						});
					};
					imgManager.LoadImageAsync (Outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (1);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection (0, 0)) as SDXAddOutletCell;
			if (Outlet != null && cell != null) {
				cell.FillContents (Outlet);
			}
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (LocationId != null || Outlet != null) {
				RectangleF frame = TableView.Frame;
				frame.Height += 80;
				TableView.Frame = frame;
			}
		}
		#endregion

		#region Button Actions
		partial void OnAddBtn_Pressed (UIButton sender)
		{
			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(outlets.Count, 0)) as SDXAddOutletCell;

			if (cell != null) {
				if (cell.IsFilled()) {
					outlets.Add (cell.GetOutlet());
				} else {
					new UIAlertView("Message", "Please fill all fields.", null, "OK", null).Show();
					return;
				}
			}

			NumberLB.Text = (outlets.Count + 2).ToString ("00");

			NSIndexPath[] paths = {NSIndexPath.FromRowSection(outlets.Count, 0)};
			TableView.InsertRows (paths, UITableViewRowAnimation.Fade);
		}

		partial void OnNextBtn_Pressed (SDXButton sender)
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

			SDXAddOutletCell cell = TableView.CellAt (NSIndexPath.FromRowSection(outlets.Count, 0)) as SDXAddOutletCell;
			if (cell != null && cell.IsFilled())
				outlets.Add (cell.GetOutlet());

			if (outlets.Count == 0) {
				new UIAlertView ("Retail Ranger", "Please add at least one outlet.", null, "OK", null).Show();
				return;
			}

			Account.Outlets = outlets;

			AddAccount ();
		}

		partial void OnDeleteOutletBtn_Pressed (SDXButton sender)
		{
			UIAlertView alert = new UIAlertView ("Retail Ranger", "Are you sure, you want to remove this outlet?", null, null, "Yes", "No");
			alert.Clicked += (object s, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0)
					DeleteOutlet ();
			};
			alert.Show ();
		}

		partial void OnCellDropBtn_Pressed (UIButton sender)
		{
			if (DropView.Hidden == false)
				OnHideDropBtn_Pressed (null);

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

			DropTableView.SetDropDescriptionList (dropDescList);
			DropTableView.ReloadData ();
			DropTableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, false);

			RectangleF frame = DropTableView.Frame;
			frame.Y = ((100 + 230 * row) + btn.Frame.Y + btn.Frame.Size.Height) - TableView.ContentOffset.Y + TableView.Frame.Y;
			frame.Height = 1.0f;

			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			DropTableView.Frame = frame;
			DropView.Hidden = false;
			UIView.Animate (0.3f, () => {
				frame.Height = fHeight;
				DropTableView.Frame = frame;
			});
		}

		partial void OnHideDropBtn_Pressed (UIButton sender)
		{
			RectangleF frame = DropTableView.Frame;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X, frame.Y, frame.Width, 1.0f);
			}, () => {
				DropView.Hidden = true;
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

		partial void OnAddPhotoBtn_Pressed (UIButton sender)
		{
			JHImagePickHelper helper = new JHImagePickHelper (this);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAccountDetail") {
				SDXAccountDetailVC vc = segue.DestinationViewController as SDXAccountDetailVC;
				vc.Account = newAccount;
				SDXAccountsVC accountsVC = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountsVC as SDXAccountsVC;
				if (accountsVC != null) {
					accountsVC.IsNeedReload = true;
					vc.PopVC = accountsVC;
				} else
					vc.PopVC = NavigationController.ViewControllers [0];
			}
		}
		#endregion

		#region Private Functions
		async private void AddAccount ()
		{
			ShowLoading ("Creating...");
			var accountManger = TinyIoCContainer.Current.Resolve<SDXAccountManager> ();
			newAccount = await accountManger.AddNewAccount (Account);
			HideLoading ();
			if (!accountManger.IsSuccessed) {
				ShowErrorMessage (accountManger.ErrorMessage);
			} else {
				// Go To Account Details
				var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
				if (manager.Me == null || manager.Me.Accounts == null) {
					ShowLoading ("Loading...");
					await manager.LoadMe ();
					HideLoading ();

					if (!manager.IsSuccessed) {
						ShowErrorMessage (manager.ErrorMessage);
						return;
					}
				}

				manager.Me.Accounts.Add (newAccount);
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
				ShowErrorMessage (manager.ErrorMessage);
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
				ShowErrorMessage (manager.ErrorMessage);
			} else {
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Outlet has been successfully added.", null, "OK", null);
				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					SDXAccountDetailVC vc = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC as SDXAccountDetailVC;
					vc.Account.Outlets.Add (newOutlet);
					NavigationController.PopToViewController (vc, true);
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
				ShowErrorMessage (manager.ErrorMessage);
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
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());

			AddPhotoBtn.SetImage (image, UIControlState.Normal);

			PerformSelector (new MonoTouch.ObjCRuntime.Selector ("UploadPhoto:"), image, 0.5f);
		}

		[Export ("UploadPhoto:")]
		private async void UploadPhoto (UIImage image)
		{
			ShowLoading ("Uploading...");

			SDXImageManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			string fileName = Outlet.Name.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, "Outlet", Outlet.OutletId.ToString ());

			if (!manager.IsSuccessed) {
				AddPhotoBtn.SetImage (UIImage.FromBundle ("icon_plus.png"), UIControlState.Normal);
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (image);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				AddPhotoBtn.SetImage (UIImage.FromBundle ("icon_plus.png"), UIControlState.Normal);
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				AddPhotoBtn.SetImage (UIImage.FromBundle ("icon_plus.png"), UIControlState.Normal);
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			Outlet.Photo = photo;
			Outlet.PhotoId = photo.PhotoId;

			SizeF scaledSize = new SizeF (148 * 2, 148 * 2);
			UIImage scaledImg = Util.GetScaledImage (image, scaledSize);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.WriteImageToFile (sasRequest.FileName, Util.GetByteArrayFromImage(scaledImg), (int)scaledSize.Width, (int)scaledSize.Height);
			HideLoading ();
			new UIAlertView ("Retail Ranger", "Successfully uploaded.", null, "OK", null).Show ();
		}
		#endregion

		#region Event Handler
		void OnDropTableCell_Selected(object sender, int row)
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

			if (cellRow < outlets.Count) {
				OutletModel outlet = outlets.ElementAt (cellRow);
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

			OnHideDropBtn_Pressed (null);
		}
		#endregion
	}
}

