
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
	public partial class SDXEditOutletVC : SDXBaseVC
	{
		public AccountModel Account { get; set; }		// For Adding Multi Outlets
		public OutletModel Outlet = null;				// For Updating Outlet
		private OutletModel UpOutlet;
		public string LocationId = null;				// For Adding One Outlet

		private UIButton currentDropBtn = null;
		private UIViewController _curVC;

		public SDXEditOutletVC (IntPtr handle) : base (handle)
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

			OutletImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			if (Outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (bytes == null)
							return;
						var img = Util.GetImageFromByteArray (bytes);
						OutletImage.Image = img;
					});
				};
				imgManager.LoadImageAsync (Outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}

			UpOutlet = new OutletModel ();
			UpOutlet.AccountId = Outlet.AccountId;
			UpOutlet.AnnualSalesRange = Outlet.AnnualSalesRange;
			UpOutlet.AnnualSalesRangeId = Outlet.AnnualSalesRangeId;
			UpOutlet.ConsumersOnSiteRange = Outlet.ConsumersOnSiteRange;
			UpOutlet.ConsumersOnSiteRangeId = Outlet.ConsumersOnSiteRangeId;
			UpOutlet.LocalCompetitionType = Outlet.LocalCompetitionType;
			UpOutlet.LocalCompetitionTypeId = Outlet.LocalCompetitionTypeId;
			UpOutlet.Name = Outlet.Name;
			UpOutlet.Offers = Outlet.Offers;
			UpOutlet.OutletId = Outlet.OutletId;
			UpOutlet.Photo = Outlet.Photo;
			UpOutlet.PhotoId = Outlet.PhotoId;
			UpOutlet.RowVersion = Outlet.RowVersion;
			UpOutlet.Url = Outlet.Url;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			FillContents (Outlet);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
		}
		#endregion

		partial void OnNextBtn_Pressed (SDXButton sender)
		{
			if (Outlet != null) {
				if (OutletNameTF.Text.Length > 0)
				{
					UpOutlet.Name = OutletNameTF.Text;

					if (Outlet.Name == UpOutlet.Name && Outlet.ConsumersOnSiteRangeId == UpOutlet.ConsumersOnSiteRangeId && 
						Outlet.AnnualSalesRangeId == UpOutlet.AnnualSalesRangeId && Outlet.LocalCompetitionTypeId == UpOutlet.LocalCompetitionTypeId) {
						new UIAlertView ("Retail Ranger", "No changes, please change items and press done button.", null, "OK", null).Show ();
					} else {
						Outlet.Name = UpOutlet.Name;
						Outlet.ConsumersOnSiteRangeId = UpOutlet.ConsumersOnSiteRangeId;
						Outlet.AnnualSalesRangeId = UpOutlet.AnnualSalesRangeId;
						Outlet.LocalCompetitionTypeId = UpOutlet.LocalCompetitionTypeId;
						UpdateOutlet ();
					}
				} else {
					new UIAlertView ("Retail Ranger", "Outlet should have name.", null, "OK", null).Show ();
				}
				return;
			}
		}

		partial void OnCancelBtn_Pressed (Sodexo.Ipad2.SDXButton sender)
		{
			SDXAccountDetailVC accountDetailVC = (SDXAccountDetailVC)((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC;
			accountDetailVC.RefreshTable();

			//PerformSegue("SegueToAccountDetail", this);
			this.View.RemoveFromSuperview();
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
			frame.Y = btn.Frame.Y + btn.Frame.Size.Height + 90;
			frame.X = btn.Frame.X + 280;
			frame.Height = 1.0f;

			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			DropTableView.Frame = frame;
			DropTableView.BackgroundColor = UIColor.White;
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

		#region Private Functions
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
					PerformSegue("SegueToAccountDetail", this);
				};
				alert.Show ();
			}
		}

		private void SetPhoto (UIImage image)
		{
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());

			OutletImage.Image = image;

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
				OutletImage.Image = UIImage.FromBundle ("outlet_default.png");
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (image);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				OutletImage.Image = UIImage.FromBundle ("outlet_default.png");
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				OutletImage.Image = UIImage.FromBundle ("outlet_default.png");
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

				UpOutlet.LocalCompetitionTypeId = id;
				LocalCompetitionTF.Text = desc;
			} else if (currentDropBtn.Tag >= 200) {
				AnnualSalesRangeModel model = manager.DataModel.AnnualSalesRanges.ElementAt (row);
				desc = model.Description;
				id = model.AnnualSalesRangeId;
				cellRow = currentDropBtn.Tag - 200;
				tag = 3;

				UpOutlet.AnnualSalesRangeId = id;
				AnnualTF.Text = desc;
			} else {
				ConsumersOnSiteRangeModel model = manager.DataModel.ConsumersOnSiteRanges.ElementAt (row);
				desc = model.Description;
				id = model.ConsumersOnSiteRangeId;
				cellRow = currentDropBtn.Tag - 100;
				tag = 2;

				UpOutlet.ConsumersOnSiteRangeId = id;
				ConsumerTF.Text = desc;
			}

			OnHideDropBtn_Pressed (null);
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void FillContents(OutletModel outlet)
		{
			OutletNameTF.Text = outlet.Name;
			AnnualTF.Text = outlet.AnnualSalesRange.Description;
			ConsumerTF.Text = outlet.ConsumersOnSiteRange.Description;
			LocalCompetitionTF.Text = outlet.LocalCompetitionType.Description;
		}
		#endregion
	}
}

