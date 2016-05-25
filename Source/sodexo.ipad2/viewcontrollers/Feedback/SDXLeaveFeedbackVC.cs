using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	public partial class SDXLeaveFeedbackVC : SDXFeedbackBaseVC
	{
		public FeedbackTypeModel FeedbackType;

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

			FillContents ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TitleLB.Font = UIFont.FromName (Constants.OSWALD_BOLD, 20);
			QuestionLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 20);
			CommentLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 12);

			CancelBtn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);
			CancelBtn.SetTitleShadowColor (UIColor.FromRGB (203, 203, 203), UIControlState.Normal);
			CancelBtn.TitleShadowOffset = new SizeF (1, 1);

			SendFeedbackBtn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);
			SendFeedbackBtn.SetTitleShadowColor (UIColor.FromRGB (203, 203, 203), UIControlState.Normal);
			SendFeedbackBtn.TitleShadowOffset = new SizeF (1, 1);

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

		partial void CancelButton_Pressed(SDXButton sender)
		{
			this.NavigationController.PopViewControllerAnimated(true);
		}

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
			if (CommentTV.Text == "") {
				Util.ShowAlert ("Please type comments.");
				CommentTV.BecomeFirstResponder ();
				return;
			}

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
		
			TitleLB.Text = FeedbackType.Description;
			ModelId = 0;


			QuestionLB.Text = FeedbackType.RatingDescription;
			CommentLB.Text = FeedbackType.CommentDescription;
			CommentTV.Text = "";

			ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, ContentView.Frame.Y + ContentView.Frame.Height);
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
			var alert = new UIAlertView ("Retail Ranger", "Feedback has been successfully sent.", null, "OK", null);
			alert.Clicked += (object s, UIButtonEventArgs e) => {
				NavigationController.PopViewControllerAnimated (true);
			};
			alert.Show ();
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

			HideLoading ();
			FillContents ();

		}
		#endregion
	}
}
