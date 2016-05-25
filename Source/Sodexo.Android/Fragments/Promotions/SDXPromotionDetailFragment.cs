
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
using System.Threading.Tasks;

namespace Sodexo.Android
{
	public class SDXPromotionDetailFragment : SDXBaseFragment
	{
		View view;

		public List<PromotionModel> Promotions;
		public List<PromotionModel> FilteredPromotions;
		public int index;
		public int PromotionId = -1;
		public string PromotionTitle = "";

		private PromotionModel promotion;
		private const int PHOTOIV_TAG_BASE = 10;

		ScrollView scrollV;
		ImageView pictureIv, categoryIv, statusIv;
		TextView nameTv, dateTv, descTv, statusTv, addPhotoTv;
		ImageButton addPhotoBtn;
		FrameLayout acceptView, photosView;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			AddBackBtn (2);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.PromotionDetail, container, false);
			LayoutView (view);

			GetInstance ();

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			if (PromotionId < 0) {
				promotion = FilteredPromotions [index];
				SetHeaderTitle (promotion.Title, 2);
				FillContents ();
			} else {
				SetHeaderTitle (PromotionTitle, 2);
				scrollV.Visibility = ViewStates.Invisible;
			}
			if (PromotionId > 0) {
				if (promotion == null)
					LoadPromotion ();
				else
					FillContents ();
			}
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			scrollV = view.FindViewById (Resource.Id.promotiondetail_sv) as ScrollView;
			pictureIv = view.FindViewById (Resource.Id.promotiondetail_picture_iv) as ImageView;
			categoryIv = view.FindViewById (Resource.Id.promotiondetail_category_iv) as ImageView;
			statusIv = view.FindViewById (Resource.Id.promotiondetail_status_iv) as ImageView;
			nameTv = view.FindViewById (Resource.Id.promotiondetail_name_tv) as TextView;
			dateTv = view.FindViewById (Resource.Id.promotiondetail_date_tv) as TextView;
			descTv = view.FindViewById (Resource.Id.promotiondetail_desc_tv) as TextView;
			statusTv = view.FindViewById (Resource.Id.promotiondetail_status_tv) as TextView;
			addPhotoTv = view.FindViewById (Resource.Id.promotiondetail_photosview_addphoto_tv) as TextView;
			acceptView = view.FindViewById (Resource.Id.promotiondetail_accept_fl) as FrameLayout;
			photosView = view.FindViewById (Resource.Id.promotiondetail_photos_fl) as FrameLayout;

			addPhotoBtn = view.FindViewById (Resource.Id.promotiondetail_photosview_add_imgbtn) as ImageButton;
			addPhotoBtn.Click += AddPhotoBtn_OnClicked;
			var viewDocBtn = view.FindViewById (Resource.Id.promotiondetail_viewdoc_btn) as Button;
			viewDocBtn.Click += ViewDocBtn_OnClicked;
			var sendFeedbackBtn = view.FindViewById (Resource.Id.promotiondetail_sendfeedback_btn) as Button;
			sendFeedbackBtn.Click += SendFeedbackBtn_OnClicked;
			var sendToMailBtn = view.FindViewById (Resource.Id.promotiondetail_sendtomail_btn) as Button;
			sendToMailBtn.Click += SendToMailBtn_OnClicked;
			var addToCalendarBtn = view.FindViewById (Resource.Id.promotiondetail_addtocalendar_btn) as Button;
			addToCalendarBtn.Click += AddToCalendarBtn_OnClicked;
			var acceptYesBtn = view.FindViewById (Resource.Id.promotiondetail_accept_yes_btn) as Button;
			acceptYesBtn.Click += AcceptYesBtn_OnClicked;
			var acceptNoBtn = view.FindViewById (Resource.Id.promotiondetail_accept_no_btn) as Button;
			acceptNoBtn.Click += AcceptNoBtn_OnClicked;
		}

		async private void LoadPromotion ()
		{
			Util.ShowLoading (Activity);
			var manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			promotion = await manager.LoadPromotion (PromotionId);

			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			if (promotion.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object sender, byte[] bytes) => {
					Activity.RunOnUiThread (delegate {
						Util.HideLoading ();
						if (bytes == null) {
							promotion.Photo = null;
							promotion.PhotoId = null;
						} else {
							using (Bitmap bitmap = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
								if (bitmap == null)
									promotion.PhotoId = 0;
								else
									promotion.PhotoId = (int)(1000 * bitmap.Height / bitmap.Width);
							}
						}
						FillContents ();
					});
				};
				imgMgr.LoadImageAsync (promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);
			} else {
				Util.HideLoading ();
				FillContents ();
			}
		}

		private void FillContents ()
		{
			scrollV.Visibility = ViewStates.Visible;

			/* Picture ImageView */

			if (promotion.PhotoId != null) {
				int h = (int)(promotion.PhotoId * pictureIv.LayoutParameters.Width / 1000.0f);
				Util.ChangeViewHeight (pictureIv, h);
				JHImageManager manager = new JHImageManager ();
				manager.LoadCompleted += (object sender, byte[] bytes) => {
					Activity.RunOnUiThread (delegate {
						if (bytes == null) {
							pictureIv.Visibility = ViewStates.Gone;
							return;
						}
						using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bmp != null)
								pictureIv.SetImageBitmap (bmp);
						}
					});
				};
				manager.LoadImageAsync (promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);
			} else {
				pictureIv.Visibility = ViewStates.Gone;
			}

			/* ContentView */

			// Number, Title, Date
			nameTv.Text = promotion.Title;
			dateTv.Text = "START:" + promotion.StartDate.ToShortDateString () + "-FINISH:" + promotion.EndDate.ToShortDateString ();

			// Description
			descTv.Text = promotion.Description;

			// Status View
			if (promotion.PromotionActivated) {
				statusTv.SetTextColor (Color.Rgb (0, 191, 77));
				statusTv.Text = "STATUS: ACTIVE";
				statusIv.SetImageResource (Resource.Drawable.img_active);
			} else {
				statusTv.SetTextColor (Color.Rgb (255, 0, 114));
				statusTv.Text = "STATUS: NOT ACTIVE";
				statusIv.SetImageResource (Resource.Drawable.img_inactive);
			}

			if (promotion.PromotionCategories.Count > 0) {
				var categoryName = promotion.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower ();
				categoryIv.SetImageResource (Activity.Resources.GetIdentifier (fileName, "drawable", Activity.PackageName));
			}

			/* Photos View Or Accept View */
			if (!promotion.PromotionAccepted) {
				photosView.Visibility = ViewStates.Invisible;
			} else {
				acceptView.Visibility = ViewStates.Gone;
				int iCounter = 0, idx = 0;
				if (promotion.Activations != null && promotion.Activations.Count != 0) {
					addPhotoTv.Visibility = ViewStates.Gone;
				}
				foreach (PhotoModel photo in promotion.Activations) 
				{
					ImageView iv = new ImageView (Activity);
					iv.SetScaleType (ImageView.ScaleType.CenterCrop);
					iv.Tag = (idx + PHOTOIV_TAG_BASE).ToString("00") + "sw-sh-sl-st";
					iv.SetImageResource (Resource.Drawable.outlet_default);
					photosView.AddView (iv);
					FrameLayout.LayoutParams param = new FrameLayout.LayoutParams (addPhotoBtn.LayoutParameters);
					param.LeftMargin = (idx % 2) * (param.Width + 4);
					param.TopMargin = (idx / 2) * (param.Height + 4);
					iv.LayoutParameters = param;

					JHImageManager imgManager = new JHImageManager ();
					imgManager.LoadCompleted += (object s, byte[] bytes) => {
						if (Activity == null)
							return;
						Activity.RunOnUiThread (delegate {
							if (photosView == null || bytes == null) return;
							var imgV = (ImageView) photosView.FindViewWithTag ((iCounter + PHOTOIV_TAG_BASE).ToString("00")+"sw-sh-sl-st");
							if (imgV != null) {
								using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
									if (bmp != null)
										imgV.SetImageBitmap (bmp);
								}
							}
							iCounter ++;
						});
					};
					imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int) addPhotoBtn.LayoutParameters.Width, 0);
					idx++;
				}

				FrameLayout.LayoutParams flParam = (FrameLayout.LayoutParams)addPhotoBtn.LayoutParameters;
				flParam.LeftMargin = (promotion.Activations.Count % 2) * (flParam.Width + 4);
				flParam.TopMargin = (promotion.Activations.Count / 2) * (flParam.Height + 4);
				addPhotoBtn.LayoutParameters = flParam;
			}
		}

		private void UploadPhoto (Bitmap bmp)
		{
			string fileName = promotion.Title.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			DoUploadPhoto (bmp, fileName, "Promotion", promotion.PromotionId.ToString (), addPhotoBtn.LayoutParameters.Width, 
				(object sender, PhotoModel photo) => {
					if (photo == null) {
						RemoveAddedPhotoIV ();
					} else {
						if (promotion.Activations.Count == 0) {
							promotion.PromotionActivated = true;
							statusTv.SetTextColor (Color.Rgb (0, 191, 77));
							statusTv.Text = "STATUS: PROMOTION IS ACTIVE";
							statusIv.SetImageResource (Resource.Drawable.img_active);
						}
						promotion.Activations.Add (photo);
					}
				});
		}

		private void RemoveAddedPhotoIV ()
		{
			var iv = photosView.FindViewWithTag ((promotion.Activations.Count + PHOTOIV_TAG_BASE).ToString("00")+"sw-sh-sl-st") as ImageView;
			addPhotoBtn.LayoutParameters = new FrameLayout.LayoutParams((FrameLayout.LayoutParams)iv.LayoutParameters);
			photosView.RemoveView (iv);
		}
		#endregion

		#region Event Handler
		private void OnPhotoSelected (object sender, Bitmap bitmap)
		{
			if (bitmap == null)
				return;

			addPhotoTv.Visibility = ViewStates.Gone;

			Console.WriteLine ("Picked Image Size : (" + bitmap.Width + "," + bitmap.Height + ")");

			ImageView iv = new ImageView (Activity);
			iv.SetScaleType (ImageView.ScaleType.CenterCrop);
			iv.Tag = (promotion.Activations.Count + PHOTOIV_TAG_BASE).ToString ("00") + "sw-sh-sl-st";
			iv.SetImageBitmap (bitmap);
			photosView.AddView (iv);
			iv.LayoutParameters = new FrameLayout.LayoutParams ((FrameLayout.LayoutParams)addPhotoBtn.LayoutParameters);

			FrameLayout.LayoutParams flParam = (FrameLayout.LayoutParams)addPhotoBtn.LayoutParameters;
			flParam.LeftMargin = ((promotion.Activations.Count + 1) % 2) * (flParam.Width + 4);
			flParam.TopMargin = ((promotion.Activations.Count + 1) / 2) * (flParam.Height + 4);
			addPhotoBtn.LayoutParameters = flParam;

			(new Handler ()).PostDelayed (new Java.Lang.Runnable (() => {
				Activity.RunOnUiThread (() => {
					UploadPhoto (bitmap);
				});
			}), 500);
		}
		#endregion

		#region Button Actions
		private void AddPhotoBtn_OnClicked (object sender, EventArgs args)
		{
			if (PhotoSelectedEvent == null)
				PhotoSelectedEvent = OnPhotoSelected;
			PickPhoto ();
		}

		private void ViewDocBtn_OnClicked (object sender, EventArgs args)
		{
			var storageContent = promotion.StorageContent;

			if (storageContent == null) {
				ShowAlert ("Doc isn't available.");
				return;
			}

			var fragment = new ViewDocFragment ();
			fragment.StorageContent = storageContent;
			PushFragment (fragment, this.Class.SimpleName);
		}

		async private void SendFeedbackBtn_OnClicked (object sender, EventArgs args)
		{
			var isSuccess = await LoadReferenceData ();
			if (!isSuccess)
				return;

			var manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			FeedbackTypeModel feedbackType = null;
			foreach (var type in manager.DataModel.FeedbackTypes) {
				if (type.FeedbackTypeId == 2) {
					feedbackType = type;
					break;
				}
			}
			if (feedbackType == null)
				return;

			var fragment = new LeaveFeedbackFragment ();
			fragment.FeedbackType = feedbackType;
			fragment.Promotion = promotion;
			PushFragment (fragment, this.Class.SimpleName);
		}

		async private void SendToMailBtn_OnClicked (object sender, EventArgs args)
		{
			string email = "";
			// Get Email Address
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (manager.Me == null) {
				ShowLoading ();
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
				ShowAlert ("Email is Empty");
				return;
			}

			int promotionId = promotion.PromotionId;

			ShowLoading ();
			var mgr = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			await mgr.SendPromotionViaEmail (promotionId, email);
			HideLoading ();

			if (!mgr.IsSuccessed) {
				ShowErrorMessage (mgr.ErrorMessage);
				return;
			}
			ShowAlert ("Email has been successfully sent.");
		}

		private void AddToCalendarBtn_OnClicked (object sender, EventArgs args)
		{
			Console.WriteLine ("Start Date = " + promotion.StartDate);
			Console.WriteLine ("End Date = " + promotion.EndDate);

			var eventNotes= string.Empty;
			var settingManager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
			if (settingManager.IsAuthorizedLoaded) {
				eventNotes = settingManager.CalendarEventText;
			}

			string title = promotion.Title;
			DateTime startDate = promotion.StartDate.Date.AddHours (9);
			DateTime endDate = promotion.EndDate.Date.AddHours(9).AddMinutes(30);
			Util.AddEvent (Activity, title, eventNotes, startDate, endDate);
		}

		async private void AcceptYesBtn_OnClicked (object sender, EventArgs args)
		{
			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			Util.ShowLoading (Activity);
			await manager.AcceptPromotion (promotion.PromotionId);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			Util.ShowAlert ("Promotion has been accepted.", Activity);

			photosView.Visibility = ViewStates.Visible;

			scrollV.RefreshDrawableState ();
			acceptView.Visibility = ViewStates.Gone;

			promotion.PromotionAccepted = true;

			Util.StartFadeAnimation (photosView, 300);
		}

		async private void AcceptNoBtn_OnClicked (object sender, EventArgs args)
		{
			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			Util.ShowLoading (Activity);
			await manager.DeclinePromotion (promotion.PromotionId);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Promotion has been declined.");
			builder.SetPositiveButton ("OK", (object s, DialogClickEventArgs e) => {
				if (FilteredPromotions != null) {
					FilteredPromotions.RemoveAt (index);
					Promotions.Remove (promotion);
				}
				PopFragment ();
			});
			builder.Create ().Show ();
		}
		#endregion
	}
}

