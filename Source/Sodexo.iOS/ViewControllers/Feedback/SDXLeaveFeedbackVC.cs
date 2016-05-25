
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
	public partial class SDXLeaveFeedbackVC : SDXBaseVC
	{
		public FeedbackTypeModel FeedbackType;
		public PromotionModel Promotion;
		public OfferModel Offer;

		public int FeedbackTypeId = -1;
		public int ModelId = -1;

		private int starRatingCount = 0;

		public SDXLeaveFeedbackVC (IntPtr handle) : base (handle)
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

			UIBarButtonItem space = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
			UIBarButtonItem doneBtnItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, (s, e) => {
				CommentTV.ResignFirstResponder ();
			});
			UIToolbar toolBar = new UIToolbar (new RectangleF (0, 0, 320, 40));
			toolBar.BarStyle = UIBarStyle.Black;
			UIBarButtonItem[] barBtnItems = { space, doneBtnItem };
			toolBar.Items = barBtnItems;
			CommentTV.InputAccessoryView = toolBar;

			if (FeedbackTypeId > 0) {
				ScrollView.Hidden = true;
			} else {
				FillContents ();
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (3);

			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_Appear:"), UIKeyboard.DidShowNotification, null);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new MonoTouch.ObjCRuntime.Selector("OnKeyboard_WillDisappear:"), UIKeyboard.WillHideNotification, null);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (FeedbackTypeId > 0 && FeedbackType == null) {
				LoadFeedbackType ();
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.DidShowNotification, null);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, UIKeyboard.WillHideNotification, null);
		}
		#endregion

		#region Button Actions
		partial void OnAttachPhotoBtn_Pressed (SDXButton sender) 
		{
			CommentTV.ResignFirstResponder ();

			JHImagePickHelper helper = new JHImagePickHelper (this);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}

		async partial void OnSendFeedbackBtn_Pressed (SDXButton sender)
		{
//			if (starRatingCount == 0) {
//				Util.ShowAlert ("Please choose star.");
//				return;
//			}
//			if (CommentTV.Text == "") {
//				Util.ShowAlert ("Please type comments.");
//				CommentTV.BecomeFirstResponder ();
//				return;
//			}

			CommentTV.ResignFirstResponder ();

			var manager = TinyIoCContainer.Current.Resolve <SDXFeedbackManager> ();
			ShowLoading ("Sending...");
			FeedbackModel feedback = await manager.AddFeedback (FeedbackType.FeedbackTypeId, starRatingCount, CommentTV.Text, ModelId);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (AttachedPictureIV.Image != null)
				UploadPhoto (feedback.FeedbackId);
			else {
				HideLoading ();
				UIAlertView alert = new UIAlertView ("Retail Ranger", "Feedback has been successfully sent.", null, "OK", null);
				alert.Clicked += (object s, UIButtonEventArgs e) => {
					NavigationController.PopViewControllerAnimated (true);
				};
				alert.Show ();
			}
		}

		partial void OnStarBtn_Pressed (UIButton sender)
		{
			var selectedBtn = sender as UIButton;
			starRatingCount = selectedBtn.Tag;
			for (int i = 0; i < 5; i++) {
				var btn = selectedBtn.Superview.ViewWithTag (i + 1) as UIButton;
				if (i < selectedBtn.Tag)
					btn.Selected = true;
				else
					btn.Selected = false;
			}
		}
		#endregion

		#region Notifications
		[Export ("OnKeyboard_Appear:")]
		void OnKeyboard_Appear (NSNotification notification)
		{
			if (ScrollView.Frame.Size.Height < View.Frame.Size.Height)
				return;

			Util.ChangeViewHeight (ScrollView, View.Frame.Size.Height - 216 - 40);
			float yOffset = ContentView.Frame.Y + CommentTV.Frame.Y - (UIScreen.MainScreen.Bounds.Height > 480 ? 49 : 5);
			ScrollView.SetContentOffset (new PointF(0, yOffset), true);
		}

		[Export ("OnKeyboard_WillDisappear:")]
		void OnKeyboard_WillDisappear (NSNotification notification)
		{
			if (ScrollView.Frame.Size.Height == View.Frame.Size.Height)
				return;

			Util.ChangeViewHeight (ScrollView, View.Frame.Size.Height);
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			ScrollView.Hidden = false;

			TitleView.Hidden = true;
			PromotionView.Hidden = true;
			OfferView.Hidden = true;

			if (FeedbackType.FeedbackTypeId == 1) {
				OfferView.Hidden = false;
				FillOfferView ();
				Util.MoveViewToY (ContentView, OfferView.Frame.Y + OfferView.Frame.Height + 1);
				if (Offer.Responses.Count > 0) {
					var response = Offer.Responses [0];
					var planogram = response.AnswerNode.Planogram;
					ModelId = planogram.PlanogramId;
				}
			} else if (FeedbackType.FeedbackTypeId == 2) {
				PromotionView.Hidden = false;
				FillPromotionView ();
				Util.MoveViewToY (ContentView, PromotionView.Frame.Y + PromotionView.Frame.Height + 1);
				ModelId = Promotion.PromotionId;
			} else if (FeedbackType.FeedbackTypeId >= 3) {
				TitleView.Hidden = false;
				TitleLB.Text = FeedbackType.Description;
				ModelId = 0;
			}

			QuestionLB.Text = FeedbackType.RatingDescription;
			CommentLB.Text = FeedbackType.CommentDescription;
			CommentTV.Text = "";

			ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, ContentView.Frame.Y + ContentView.Frame.Height);
		}

		private void FillOfferView ()
		{
			OfferNameLB.Text = Offer.Name;
			var imgName = "img_offer_categories_" + Offer.OfferCategory.OfferCategoryId.ToString () + ".png";
			OfferPictureIV.Image = UIImage.FromBundle (imgName);
			OfferStatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
			if (Offer.Responses.Count == 0) {
				OfferStatusLB.Text = "NOT SELECTED";
			} else {
				var response = Offer.Responses [0];
				var planogram = response.AnswerNode.Planogram;
				if (!response.PlanogramActivated) {
					OfferStatusLB.Text = planogram.Name +  "/NOT ACTIVE";
				} else {
					OfferStatusLB.Text = planogram.Name + "/ACTIVE";
					OfferStatusLB.TextColor = UIColor.FromRGB (125 / 255.0f, 244 / 255.0f, 146 / 255.0f);
				}
			}
		}

		private void FillPromotionView ()
		{
			float y = 0;
			PromotionPictureIV.Hidden = true;
			if (Promotion.Photo != null) {
				PromotionPictureIV.Hidden = false;
				Util.ChangeViewHeight (PromotionPictureIV, ((float)Promotion.PhotoId * 300 / 1000.0f));
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (bytes == null) {
							return;
						}
						PromotionPictureIV.Alpha = 0.5f;
						PromotionPictureIV.Image = Util.GetImageFromByteArray (bytes);
						UIView.Animate (0.5f, () => {
							PromotionPictureIV.Alpha = 1;
						});
					});
				};
				imageManager.LoadImageAsync (Promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);

				y += PromotionPictureIV.Frame.Height;
			}

			PromotionNameLB.Text = Promotion.Title;
			PromotionDateLB.Text = "START:" + Promotion.StartDate.ToShortDateString () + "-FINISH:" + Promotion.EndDate.ToShortDateString ();
			if (Promotion.PromotionActivated)
				PromotionStatusIV.Image = UIImage.FromBundle ("img_active.png");
			else
				PromotionStatusIV.Image = UIImage.FromBundle ("img_inactive.png");

			PromotionDescLB.Text = Promotion.Description;
			PromotionDescLB.SizeToFit ();

			float h = PromotionDescLB.Frame.Y + PromotionDescLB.Frame.Height + 5;

			RectangleF frame = PromotionContentView.Frame;
			frame.Y = y;
			frame.Height = h;
			PromotionContentView.Frame = frame;

			if (Promotion.PromotionCategories.Count > 0) {
				var categoryName = Promotion.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower () + ".png";
				PromotionCategoryIV.Image = UIImage.FromBundle (fileName);
			}

			Util.ChangeViewHeight (PromotionView, PromotionContentView.Frame.Y + PromotionContentView.Frame.Height);
		}

		private void SetPhoto (UIImage image)
		{
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());
			AttachedPictureIV.Image = image;
			AttachedPictureIV.Alpha = 0.5f;
			UIView.Animate (0.3f, ()=>{
				AttachedPictureIV.Alpha = 1;
			});

			AttachPhotoBtn.SetTitle ("", UIControlState.Normal);
		}

		private async void UploadPhoto (int feedbackId)
		{
			SDXImageManager manager = TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			string fileName = "feedback.png";
			Console.WriteLine ("FileName = " + fileName);

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, "Feedback", feedbackId.ToString ());

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (AttachedPictureIV.Image);

			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			HideLoading ();
			new UIAlertView ("Retail Ranger", "Feedback has been successfully sent.", null, "OK", null).Show ();
		}

		async private void LoadFeedbackType ()
		{
			var isSuccess = await LoadReferenceData ();
			if (!isSuccess)
				return;

			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			foreach (var type in manager.DataModel.FeedbackTypes) {
				if (type.FeedbackTypeId == FeedbackTypeId) {
					FeedbackType = type;
					break;
				}
			}

			if (FeedbackType == null) {
				Util.ShowAlert ("FeedbackType is incorrect.");
				return;
			}

			if (FeedbackType.FeedbackTypeId == 1) {
				LoadOffer ();
			} else if (FeedbackType.FeedbackTypeId == 2) {
				LoadPromotion ();
			} else {
				HideLoading ();
				FillContents ();
			}
		}

		private void LoadOffer ()
		{
			if (ModelId <= 0) {
				HideLoading ();
				Util.ShowAlert ("OfferID is incorrect.");
				return;
			}
			HideLoading ();
			Util.ShowAlert ("");
		}

		async private void LoadPromotion ()
		{
			if (ModelId <= 0) {
				HideLoading ();
				Util.ShowAlert ("PromotionID is incorrect.");
				return;
			}

			ShowLoading ("Loading...");
			var manager = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			Promotion = await manager.LoadPromotion (ModelId);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (Promotion.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object sender, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						HideLoading ();
						if (bytes == null) {
							Promotion.Photo = null;
							Promotion.PhotoId = null;
						} else {
							UIImage img = Util.GetImageFromByteArray (bytes);
							Promotion.PhotoId = (int)(1000 * img.Size.Height / img.Size.Width);
						}
						FillContents ();
					});
				};
				imgMgr.LoadImageAsync (Promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);
			} else {
				HideLoading ();
				FillContents ();
			}
		}
		#endregion
	}
}

