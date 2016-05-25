
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Android
{
	public class LeaveFeedbackFragment : SDXBaseFragment
	{
		public FeedbackTypeModel FeedbackType;
		public PromotionModel Promotion;
		public OfferModel Offer;

		public int FeedbackTypeId = -1;
		public int ModelId = -1;

		private int starRatingCount = 0;

		View view;
		ScrollView scrollView;
		FrameLayout rateView;
		EditText commentEt;
		ImageView attachedPictureIv;
		Button attachBtn;

		Bitmap attachedBmp = null;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			AddBackBtn (3);
			SetHeaderTitle ("Leave Feedback", 3);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.LeaveFeedback, container, false);
			LayoutView (view);

			GetInstance (view);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			if (FeedbackTypeId > 0 && FeedbackType == null) {
				scrollView.Visibility = ViewStates.Invisible;
				LoadFeedbackType ();
			} else {
				FillContents ();
			}
		}
		#endregion

		#region Button
		private void AttachBtn_OnClicked (object sender, EventArgs e)
		{
			if (PhotoSelectedEvent == null)
				PhotoSelectedEvent = OnPhotoSelected;
			PickPhoto ();
		}

		async private void SendFeedbackBtn_OnClicked (object sender, EventArgs arg)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXFeedbackManager> ();
			ShowLoading ();
            String comment = commentEt.Text;
            FeedbackModel feedback = await manager.AddFeedback(FeedbackType.FeedbackTypeId, starRatingCount, comment, ModelId);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (attachedBmp != null)
				UploadPhoto (feedback.FeedbackId);
			else {
				HideLoading ();
				ShowSuccessMsgAndPop ();
			}
		}

		private void StarBtn_OnClicked (object sender, EventArgs e)
		{
			var selectedBtn = sender as ImageButton;
			starRatingCount = int.Parse ((((string)selectedBtn.Tag).Substring (0, 2))) - 1;
			for (int i = 2; i <= 6; i++) {
				var tag = i.ToString ("00") + "-sw-sh-sl-st";
				var btn = rateView.FindViewWithTag (tag) as ImageButton;
				if ((i - 1) <= starRatingCount)
					btn.SetImageResource (Resource.Drawable.star_full);
				else
					btn.SetImageResource (Resource.Drawable.star_empty);
			}
		}
		#endregion

		#region Private Functions
		private void GetInstance (View view)
		{
			scrollView = view.FindViewById (Resource.Id.leavefeedback_sv) as ScrollView;

			rateView = view.FindViewById (Resource.Id.leavefeedback_rate_fl) as FrameLayout;
			for (int i = 1; i <= 6; i++) {
				var tag = i.ToString ("00") + "-sw-sh-sl-st";
				var btn = rateView.FindViewWithTag (tag) as ImageButton;
				btn.Click += StarBtn_OnClicked;
			}

			var sendBtn = view.FindViewById (Resource.Id.leavefeedback_send_btn) as Button;
			sendBtn.Click += SendFeedbackBtn_OnClicked;

			commentEt = view.FindViewById (Resource.Id.leavefeedback_comment_et) as EditText;

			attachedPictureIv = view.FindViewById (Resource.Id.leavefeedback_picture_iv) as ImageView;
			attachBtn = view.FindViewById (Resource.Id.leavefeedback_attach_btn) as Button;
			attachBtn.Click += AttachBtn_OnClicked;
		}

		private void FillContents ()
		{
			scrollView.Visibility = ViewStates.Visible;

			var titleView = view.FindViewById (Resource.Id.leavefeedback_title_fl) as FrameLayout;
			var promotionView = view.FindViewById (Resource.Id.leavefeedback_promotion_ll) as LinearLayout;
			var offerView = view.FindViewById (Resource.Id.leavefeedback_offer_fl) as FrameLayout;

			if (FeedbackType.FeedbackTypeId == 1) {
				titleView.Visibility = ViewStates.Gone;
				promotionView.Visibility = ViewStates.Gone;
				FillOfferView ();
				if (Offer.Responses.Count > 0) {
					var response = Offer.Responses [0];
					var planogram = response.AnswerNode.Planogram;
					ModelId = planogram.PlanogramId;
				}
			} else if (FeedbackType.FeedbackTypeId == 2) {
				titleView.Visibility = ViewStates.Gone;
				offerView.Visibility = ViewStates.Gone;
				FillPromotionView ();
				ModelId = Promotion.PromotionId;
			} else if (FeedbackType.FeedbackTypeId >= 3) {
				offerView.Visibility = ViewStates.Gone;
				promotionView.Visibility = ViewStates.Gone;
				var titleTv = view.FindViewById (Resource.Id.leavefeedback_title_tv) as TextView;
				titleTv.Text = FeedbackType.Description;
				ModelId = 0;
			}

			var questionTv = view.FindViewById (Resource.Id.leavefeedback_question_tv) as TextView;
			var commentTv = view.FindViewById (Resource.Id.leavefeedback_comment_tv) as TextView;
			questionTv.Text = FeedbackType.RatingDescription;
			commentTv.Text = FeedbackType.CommentDescription;
		}

		private void FillOfferView ()
		{
			var nameTv = view.FindViewById (Resource.Id.leavefeedback_offer_name_tv) as TextView;
			nameTv.Text = Offer.Name;
			var imgName = "img_offer_categories_" + Offer.OfferCategory.OfferCategoryId.ToString ();
			var pictureIv = view.FindViewById (Resource.Id.leavefeedback_offer_picture_iv) as ImageView;
			pictureIv.SetImageResource (Activity.Resources.GetIdentifier (imgName, "drawable", Activity.PackageName));
			var statusTv = view.FindViewById (Resource.Id.leavefeedback_offer_status_tv) as TextView;
			statusTv.SetTextColor (Color.Rgb (233, 70, 122));
			if (Offer.Responses.Count == 0) {
				statusTv.Text = "NOT SELECTED";
			} else {
				var response = Offer.Responses [0];
				var planogram = response.AnswerNode.Planogram;
				if (!response.PlanogramActivated) {
					statusTv.Text = planogram.Name +  "/NOT ACTIVE";
				} else {
					statusTv.Text = planogram.Name + "/ACTIVE";
					statusTv.SetTextColor (Color.Rgb (125, 244, 146));
				}
			}
		}

		private void FillPromotionView ()
		{
			var pictureIv = view.FindViewById (Resource.Id.leavefeedback_promotion_picture_iv) as ImageView;
			if (Promotion.Photo != null) {
				Util.ChangeViewHeight (pictureIv, (int)((float)Promotion.PhotoId * pictureIv.LayoutParameters.Width / 1000.0f));
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					if (Activity == null)
						return;
					Activity.RunOnUiThread (delegate {
						if (bytes == null) {
							return;
						}
						using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bmp != null)
								pictureIv.SetImageBitmap (bmp);
						}
					});
				};
				imageManager.LoadImageAsync (Promotion.Photo.ProcessedPhotoBaseUrl, pictureIv.LayoutParameters.Width, 0);
			} else {
				pictureIv.Visibility = ViewStates.Gone;
			}

			var nameTv = view.FindViewById (Resource.Id.leavefeedback_promotion_name_tv) as TextView;
			nameTv.Text = Promotion.Title;
			var dateTv = view.FindViewById (Resource.Id.leavefeedback_promotion_date_tv) as TextView;
			dateTv.Text = "START:" + Promotion.StartDate.ToShortDateString () + "-FINISH:" + Promotion.EndDate.ToShortDateString ();
			var statusIv = view.FindViewById (Resource.Id.leavefeedback_promotion_status_iv) as ImageView;
			if (Promotion.PromotionActivated)
				statusIv.SetImageResource (Resource.Drawable.img_active);
			else
				statusIv.SetImageResource (Resource.Drawable.img_inactive);

			var descTv = view.FindViewById (Resource.Id.leavefeedback_promotion_desc_tv) as TextView;
			descTv.Text = Promotion.Description;

			if (Promotion.PromotionCategories.Count > 0) {
				var categoryName = Promotion.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower ();
				var categoryIv = view.FindViewById (Resource.Id.leavefeedback_promotion_category_iv) as ImageView;
				categoryIv.SetImageResource (Activity.Resources.GetIdentifier (fileName, "drawable", Activity.PackageName));
			}
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
				Util.ShowAlert ("FeedbackType is incorrect.", Activity);
				return;
			}

			if (FeedbackType.FeedbackTypeId == 1) {
				LoadOffer ();
			} else if (FeedbackType.FeedbackTypeId == 2) {
				LoadPromotion ();
			} else {
				Util.HideLoading ();
				FillContents ();
			}
		}

		private void LoadOffer ()
		{
			if (ModelId <= 0) {
				Util.HideLoading ();
				ShowAlert ("OfferID is incorrect.");
				return;
			}
			HideLoading ();
			ShowAlert ("");
		}

		async private void LoadPromotion ()
		{
			if (ModelId <= 0) {
				HideLoading ();
				ShowAlert ("PromotionID is incorrect.");
				return;
			}

			ShowLoading ();
			var manager = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			Promotion = await manager.LoadPromotion (ModelId);

			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (Promotion.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object sender, byte[] bytes) => {
					Activity.RunOnUiThread (delegate {
						HideLoading ();
						if (bytes == null) {
							Promotion.Photo = null;
							Promotion.PhotoId = null;
						} else {
							using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
								if (bmp == null)
									Promotion.PhotoId = 0;
								else
									Promotion.PhotoId = (int)(1000 * bmp.Height / bmp.Width);
							}
						}
						FillContents ();
					});
				};
				var pictureIv = view.FindViewById (Resource.Id.leavefeedback_promotion_picture_iv) as ImageView;
				imgMgr.LoadImageAsync (Promotion.Photo.ProcessedPhotoBaseUrl, pictureIv.LayoutParameters.Width, 0);
			} else {
				HideLoading ();
				FillContents ();
			}
		}

		private void OnPhotoSelected (object sender, Bitmap bitmap)
		{
			if (bitmap == null)
				return;

			Console.WriteLine ("Picked Image Size : (" + bitmap.Width + "," + bitmap.Height + ")");
			attachedPictureIv.SetImageBitmap (bitmap);
			attachBtn.Text = "";
		}

		private void UploadPhoto (int feedbackId)
		{
			string fileName = "feedback.png";
			Console.WriteLine ("FileName = " + fileName);

			DoUploadPhoto (attachedBmp, fileName, "Feedback", feedbackId.ToString (), 0, (object sender, PhotoModel photo) => {
				ShowSuccessMsgAndPop ();
			});
		}

		private void ShowSuccessMsgAndPop ()
		{
			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Feedback has been successfully sent.");
			builder.SetPositiveButton ("OK", (object s, DialogClickEventArgs e) => {
				if(FeedbackType.FeedbackTypeId==2) {
					PopFragment ();
				}
				PopFragment ();

			});
			builder.Create ().Show ();
		}
		#endregion
	}
}

