
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Ipad2
{
	public partial class SDXOfferDetailVC : SDXBaseVC
	{
		enum DetailType
		{
			FULL_IMAGE,
			DOC,
		};

		public AccountModel Account;
		public int OutletIndex;
		public int OfferIndex;

		private OutletModel outlet;
		private PlanogramModel planogram;
		private OfferResponseModel offerResponse;
		private SDXAccountDetailVC parentVC = null;

		public SDXOfferDetailVC (IntPtr handle) : base (handle)
		{
			SDXAccountDetailVC._offerDetailVC = this;
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
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ScrollView.Frame = new RectangleF (0, 0, View.Frame.Width, 564);
		}

		public void InitializeView(SDXAccountDetailVC parentVC)
		{
			this.parentVC = parentVC;

			foreach (UIView subview in PhotosView.Subviews) {
				if (subview is UIImageView)
					subview.RemoveFromSuperview ();
			}

			outlet = Account.Outlets [OutletIndex];
			NavigationItem.Title = outlet.Name;
			FillContents ();

			//AddBackButton (1);
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			OfferModel offer = outlet.Offers [OfferIndex];
			/*OfferNameLB.Text = offer.Name;
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString () + ".png";
			OfferPictureIV.Image = UIImage.FromBundle (imgName);*/

			if (offer.Responses.Count == 0) {
				//OfferStatusLB.Text = "NOT SELECTED";
			} else {
				offerResponse = offer.Responses [0];
				planogram = offerResponse.AnswerNode.Planogram;
				if (!offerResponse.PlanogramActivated) {
					//OfferStatusLB.Text = planogram.Name +  "/NOT ACTIVE";
					//OfferStatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
				} else {
					//OfferStatusLB.Text = planogram.Name + "/ACTIVE";
					//OfferStatusLB.TextColor = UIColor.FromRGB (125 / 255.0f, 244 / 255.0f, 146 / 255.0f);
				}
			}

			ContentView.Hidden = true;
			PhotosView.Hidden = true;

			if (planogram == null)
				return;

			ShowLoading ("Loading...");
			var photoUrl = planogram.Versions [0].Photo.ProcessedPhotoBaseUrl;
			var imgManager = new JHImageManager ();
			imgManager.LoadCompleted += (object s, byte[] bytes) => {
				InvokeOnMainThread (delegate {
					HideLoading ();

					UIImage img = Util.GetImageFromByteArray (bytes);
					float h = PlanogramPictureIV.Frame.Width * img.Size.Height / img.Size.Width;
					PlanogramPictureIV.Image = img;
					Util.ChangeViewHeight (PlanogramPictureIV, h);

					UITapGestureRecognizer tapGesture = new UITapGestureRecognizer ();
					tapGesture.AddTarget (() => {
						//PerformSegue ("SegueToFullImage", this);
						PostChangeView(DetailType.FULL_IMAGE);
					});
					PlanogramPictureIV.AddGestureRecognizer (tapGesture);

					Util.MoveViewToY (PlanogramDetailsLB, PlanogramPictureIV.Frame.Y + h + 10);
					PlanogramDetailsLB.Text = planogram.Details;
					PlanogramDetailsLB.SizeToFit ();
					Util.ChangeViewHeight (PlanogramDetailsLB, PlanogramDetailsLB.Frame.Height + 10);

					Util.MoveViewToY (ActionsView, PlanogramDetailsLB.Frame.Y + PlanogramDetailsLB.Frame.Height + 10);

					Util.MoveViewToY (StatusView, ActionsView.Frame.Y + ActionsView.Frame.Height + 5);
					if (offerResponse.PlanogramActivated) {
						PlanogramStatusLB.TextColor = UIColor.FromRGB (0, 191 / 255.0f, 77 / 255.0f);
						PlanogramStatusLB.Text = "STATUS: ACTIVE";
						PlanogramStatusIV.Image = UIImage.FromBundle ("img_active.png");
					} else {
						PlanogramStatusLB.TextColor = UIColor.FromRGB (255 / 255.0f, 0, 114 / 255.0f);
						PlanogramStatusLB.Text = "STATUS: NOT ACTIVE";
						PlanogramStatusIV.Image = UIImage.FromBundle ("img_inactive.png");
					}

					Util.ChangeViewHeight (ContentView, StatusView.Frame.Y + StatusView.Frame.Height);

					Util.MoveViewToY (PhotosView, ContentView.Frame.Y + ContentView.Frame.Height + 10);
					FillPhotosView (offerResponse);

					ContentView.Hidden = false;
					PhotosView.Hidden = false;

					ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, PhotosView.Frame.Y + PhotosView.Frame.Height + 10);
					Console.WriteLine("------------scrollview height------------"+ScrollView.Frame.Height);
				});
			};
			imgManager.LoadImageAsync (photoUrl, 600, 0);
		}

		private void FillPhotosView (OfferResponseModel response)
		{
			int iCounter = 0, idx = 0;

			if (response.Activations != null && response.Activations.Count != 0)
				AddPhotoLB.Hidden = true;

			foreach (PhotoModel photo in response.Activations)
			{
				RectangleF frm = AddPhotoBtn.Frame;
				frm.X = (idx % 2) * (AddPhotoBtn.Frame.Width + 4);
				frm.Y = (idx / 2) * (AddPhotoBtn.Frame.Height + 4);

				UIImageView iv = new UIImageView (frm);
				iv.ContentMode = UIViewContentMode.ScaleAspectFill;
				iv.ClipsToBounds = true;
				iv.Tag = idx + 1;
				PhotosView.Add (iv);

				JHImageManager imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (PhotosView == null) return;
						var imgV = (UIImageView) PhotosView.ViewWithTag (iCounter + 1);
						if (imgV != null)
							imgV.Image = Util.GetImageFromByteArray (bytes);
						iCounter ++;
					});
				};
				imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int) AddPhotoBtn.Frame.Width * 2, 0);
				idx++;
			}

			RectangleF frmPhoto = AddPhotoBtn.Frame;
			frmPhoto.X = (response.Activations.Count % 2) * (AddPhotoBtn.Frame.Width + 4);
			frmPhoto.Y = (response.Activations.Count / 2) * (AddPhotoBtn.Frame.Height + 4);
			AddPhotoBtn.Frame = frmPhoto;

			Util.ChangeViewHeight (PhotosView, ((int)((response.Activations.Count + 1) / 2.0f + 0.5f)) * (AddPhotoBtn.Frame.Height + 4) - 4);
		}

		private void SetPhoto (UIImage image)
		{
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());

			UIImageView iv = new UIImageView (AddPhotoBtn.Frame);
			iv.ContentMode = UIViewContentMode.ScaleAspectFill;
			iv.ClipsToBounds = true;
			iv.Image = image;
			iv.Tag = offerResponse.Activations.Count + 1;
			PhotosView.Add (iv);

			RectangleF frame = AddPhotoBtn.Frame;
			frame.X = ((offerResponse.Activations.Count + 1) % 2) * (AddPhotoBtn.Frame.Width + 4);
			frame.Y = ((offerResponse.Activations.Count + 1) / 2) * (AddPhotoBtn.Frame.Height + 4);
			AddPhotoBtn.Frame = frame;

			if ((offerResponse.Activations.Count + 1) % 2 == 0) {
				float contentH = ScrollView.ContentSize.Height + AddPhotoBtn.Frame.Height + 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height + AddPhotoBtn.Frame.Height + 4);
				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, contentH);
				ScrollView.ScrollRectToVisible (new RectangleF (0, contentH - 50, ScrollView.Frame.Width, 50), true);
			}

			PerformSelector (new MonoTouch.ObjCRuntime.Selector ("UploadPhoto:"), image, 0.5f);
		}

		[Export ("UploadPhoto:")]
		private async void UploadPhoto (UIImage image)
		{
			ShowLoading ("Uploading...");

			SDXImageManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			string fileName = planogram.Name.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, "OfferResponse", offerResponse.OfferResponseId.ToString ());

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (image);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (offerResponse.Activations.Count == 0) {
				PlanogramStatusLB.TextColor = UIColor.FromRGB (0, 191 / 255.0f, 77 / 255.0f);
				PlanogramStatusLB.Text = "STATUS: ACTIVE";
				PlanogramStatusIV.Image = UIImage.FromBundle ("img_active.png");
			}

			offerResponse.Activations.Add (photo);

			if (AddPhotoLB.Hidden == false)
				AddPhotoLB.Hidden = true;

			SizeF scaledSize = new SizeF (AddPhotoBtn.Frame.Width * 2, AddPhotoBtn.Frame.Height * 2);
			UIImage scaledImg = Util.GetScaledImage (image, scaledSize);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.WriteImageToFile (sasRequest.FileName, Util.GetByteArrayFromImage(scaledImg), (int)scaledSize.Width, 0);
			HideLoading ();

			parentVC.RefreshTable ();

			new UIAlertView ("Retail Ranger", "Successfully uploaded.", null, "OK", null).Show ();
		}

		private void RemoveAddedPhotoIV ()
		{
			var iv = PhotosView.ViewWithTag (offerResponse.Activations.Count + 1) as UIImageView;
			AddPhotoBtn.Frame = iv.Frame;
			iv.RemoveFromSuperview ();

			if ((offerResponse.Activations.Count) % 2 == 1) {
				float contentH = ScrollView.ContentSize.Height - AddPhotoBtn.Frame.Height - 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height - AddPhotoBtn.Frame.Height - 4);
				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, contentH);
				ScrollView.ScrollRectToVisible (new RectangleF (0, contentH - 50, ScrollView.Frame.Width, 50), true);
			}
		}
		#endregion

		#region Button Actions
		partial void OnAddPhotoBtn_Pressed (UIButton sender)
		{
			JHImagePickHelper helper = new JHImagePickHelper (parentVC);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}

		partial void OnReadDocBtn_Pressed (SDXButton sender)
		{
			var storageContent = planogram.Versions[0].StorageContent;

			if (storageContent == null) {
				Util.ShowAlert ("Doc isn't available.");
				return;
			}

			//PerformSegue ("SegueToDoc", this);
			PostChangeView(DetailType.DOC);
		}

		async partial void OnSendFeedbackBtn_Pressed (SDXButton sender)
		{
			var isSuccess = await LoadReferenceData ();
			if (!isSuccess)
				return;

			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			FeedbackTypeModel feedbackType = null;
			foreach (var type in manager.DataModel.FeedbackTypes) {
				if (type.FeedbackTypeId == 1) {
					feedbackType = type;
					break;
				}
			}
			if (feedbackType == null)
				return;

			var offer = outlet.Offers [OfferIndex];

			var vc = Storyboard.InstantiateViewController ("SDXPlanogramsFeedbackVC") as SDXPlanogramsFeedbackVC;
			vc.FeedbackType = feedbackType;
			vc.PlanogramOffer = offer;
			this.parentVC.NavigationController.PushViewController (vc, true);
			//NavigationController.PushViewController (vc, true);

		}

		async partial void OnSendToMyMailBtn_Pressed (SDXButton sender)
		{
			string email = "";
			// Get Email Address
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (manager.Me == null) {
				ShowLoading ("Loading...");
				UserModel me = await manager.LoadMe (false);
				if (!manager.IsSuccessed) {
					HideLoading ();
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
				email = me.Email;
			} else
				email = manager.Me.Email;

			if (email == null || email == "") {
				HideLoading ();
				Util.ShowAlert ("Not Valid Email");
				return;
			}

			int versionId = planogram.Versions [0].PlanogramVersionId;

			ShowLoading ("Sending...");
			var accountMgr = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await accountMgr.SendPlanogramViaEmail (versionId, email);
			HideLoading ();

			if (!accountMgr.IsSuccessed) {
				ShowErrorMessage (accountMgr.ErrorMessage);
				return;
			}
			Util.ShowAlert ("Email has been successfully sent.");
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToFullImage") {
				var vc = segue.DestinationViewController as SDXFullImageVC;
				vc.Picture = PlanogramPictureIV.Image;
			} else if (segue.Identifier == "SegueToDoc") {
				var vc = segue.DestinationViewController as SDXDocVC;
				vc.DocUrl = planogram.Versions[0].StorageContent.StorageContentUrl;
			}
		}
		#endregion

		#region Private Functions
		private void PostChangeView(DetailType type)
		{
			switch (type) {
			case DetailType.DOC:
				if (parentVC != null)
					parentVC.ChangeToDocView (planogram.Versions[0].StorageContent.StorageContentUrl);
				break;
			case DetailType.FULL_IMAGE:
				if (parentVC != null)
					parentVC.ChangeToFullImageView (PlanogramPictureIV.Image);
				break;
			}
		}
		#endregion
	}
}

