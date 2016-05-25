
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
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Net;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;
using System.Threading.Tasks;

namespace Sodexo.Android
{
	public class SDXOfferDetailFragment : SDXBaseFragment
	{
		public AccountModel Account;
		public int OutletIndex;
		public int OfferIndex;

		private const int PHOTOIV_TAG_BASE = 10;

		private OutletModel outlet;
		private PlanogramModel planogram;
		private OfferResponseModel offerResponse;

		View view;
		ImageButton addPhotoBtn;
		TextView addPhotoTv, statusTv, statusTv1;
		FrameLayout photosView;
		ImageView planogramPictureIv, statusIv;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.OfferDetail, container, false);
			LayoutView (view);

			GetInstance ();

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			outlet = Account.Outlets [OutletIndex];
			SetHeaderTitle (outlet.Name, 1);

			FillContents ();
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			addPhotoBtn = view.FindViewById (Resource.Id.offerdetail_photosview_add_imgbtn) as ImageButton;
			addPhotoTv = view.FindViewById (Resource.Id.offerdetail_photosview_addphoto_tv) as TextView;
			photosView = view.FindViewById (Resource.Id.offerdetail_photos_fl) as FrameLayout;
			planogramPictureIv = view.FindViewById (Resource.Id.offerdetail_planogram_picture_iv) as ImageView;

			planogramPictureIv.Click += (object sender, EventArgs e) => {
				var fragment = new FullImageFragment ();
				fragment.bitmap = ((BitmapDrawable)(planogramPictureIv.Drawable)).Bitmap;
				PushFragment (fragment, this.Class.SimpleName);
			};

			addPhotoBtn.Click += AddPhotoBtn_OnClicked;
			var viewPogBtn = view.FindViewById (Resource.Id.offerdetail_viewpog_btn) as Button;
			viewPogBtn.Click += ViewPogBtn_OnClicked;
			var sendFeedbackBtn = view.FindViewById (Resource.Id.offerdetail_sendfeedback_btn) as Button;
			sendFeedbackBtn.Click += SendFeedbackBtn_OnClicked;
			var sendToMailBtn = view.FindViewById (Resource.Id.offerdetail_sendtomail_btn) as Button;
			sendToMailBtn.Click += SendToMailBtn_OnClicked;

			var editBtn = view.FindViewById (Resource.Id.offerdetail_offer_edit_imgbtn) as ImageButton;
			editBtn.Click += (object sender, EventArgs e) => {
				var fragment = new SDXSelectPlanogramFragment ();
				fragment.Account = Account;
				fragment.OutletIndex = OutletIndex;
				fragment.OfferIndex = OfferIndex;
				fragment.IsFromOfferDetail = true;
				PushFragment (fragment, this.Class.SimpleName);
			};
		}

		private void FillContents ()
		{
			OfferModel offer = outlet.Offers [OfferIndex];
			var offerNameTv = view.FindViewById (Resource.Id.offerdetail_offer_name_tv) as TextView;
			offerNameTv.Text = offer.Name;
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString ();
			var offerPicIv = view.FindViewById (Resource.Id.offerdetail_offer_picture_iv) as ImageView;
			offerPicIv.SetImageResource (Activity.Resources.GetIdentifier (imgName, "drawable", Activity.PackageName));

			var detailsTv = view.FindViewById (Resource.Id.offerdetail_planogram_detail_tv) as TextView;
			statusTv = view.FindViewById (Resource.Id.offerdetail_planogram_status_tv) as TextView;
			statusIv = view.FindViewById (Resource.Id.offerdetail_planogram_status_iv) as ImageView;
			statusTv1 = view.FindViewById (Resource.Id.offerdetail_offer_status_tv) as TextView;

			if (offer.Responses.Count == 0) {
				statusTv1.Text = "NOT SELECTED";
			} else {
				offerResponse = offer.Responses [0];
				planogram = offerResponse.AnswerNode.Planogram;
				if (!offerResponse.PlanogramActivated) {
					statusTv1.Text = planogram.Name +  "/NOT ACTIVE";
					statusTv1.SetTextColor (Color.Rgb (233, 70, 122));
				} else {
					statusTv1.Text = planogram.Name + "/ACTIVE";
					statusTv1.SetTextColor (Color.Rgb (125, 244, 146));
				}
			}

			var contentV = view.FindViewById (Resource.Id.offerdetail_content_ll) as LinearLayout;

			contentV.Visibility = ViewStates.Invisible;
			photosView.Visibility = ViewStates.Invisible;

			if (planogram == null)
				return;

			ShowLoading ();
			var photoUrl = planogram.Versions [0].Photo.ProcessedPhotoBaseUrl;
			var imgManager = new JHImageManager ();
			imgManager.LoadCompleted += (object s, byte[] bytes) => {
				Activity.RunOnUiThread (delegate {
					HideLoading ();

					contentV.Visibility = ViewStates.Visible;
					photosView.Visibility = ViewStates.Visible;

					if (bytes != null) {
						using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bmp != null) {
								float h = planogramPictureIv.LayoutParameters.Width * bmp.Height / bmp.Width;
								planogramPictureIv.SetImageBitmap (bmp);
								Util.ChangeViewHeight (planogramPictureIv, (int)h);
							}
						}
					} else {
						planogramPictureIv.Visibility = ViewStates.Gone;
					}

					detailsTv.Text = planogram.Details;

					if (offerResponse.PlanogramActivated) {
						statusTv.SetTextColor (Color.Rgb (0, 191, 77));
						statusTv.Text = "STATUS: ACTIVE";
						statusIv.SetImageResource (Resource.Drawable.img_active);
					} else {
						statusTv.SetTextColor (Color.Rgb (255, 0, 114));
						statusTv.Text = "STATUS: NOT ACTIVE";
						statusIv.SetImageResource (Resource.Drawable.img_inactive);
					}

					FillPhotosView (offerResponse);
				});
			};
			int w = (int)(600 * Data.XRate);
			imgManager.LoadImageAsync (photoUrl, w, 0);
		}

		private void FillPhotosView (OfferResponseModel response)
		{
			int iCounter = 0, idx = 0;

			if (response.Activations != null && response.Activations.Count != 0)
				addPhotoTv.Visibility = ViewStates.Gone;

			foreach (PhotoModel photo in response.Activations)
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
			flParam.LeftMargin = (response.Activations.Count % 2) * (flParam.Width + 4);
			flParam.TopMargin = (response.Activations.Count / 2) * (flParam.Height + 4);
			addPhotoBtn.LayoutParameters = flParam;
		}

		private void UploadPhoto (Bitmap bmp)
		{
			string fileName = planogram.Name.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			DoUploadPhoto (bmp, fileName, "OfferResponse", offerResponse.OfferResponseId.ToString (), addPhotoBtn.LayoutParameters.Width, 
				(object sender, PhotoModel photo) => {
					if (photo == null) {
						RemoveAddedPhotoIV ();
					} else {
						if (offerResponse.Activations.Count == 0) {
							statusTv.SetTextColor (Color.Rgb (0, 191, 77));
							statusTv.Text = "STATUS: PROMOTION IS ACTIVE";
							statusTv1.SetTextColor (Color.Rgb (0, 191, 77));
							statusTv1.Text = planogram.Name + "/ACTIVE";
							statusIv.SetImageResource (Resource.Drawable.img_active);
						}
						offerResponse.Activations.Add (photo);
					}
				});
		}

		private void RemoveAddedPhotoIV ()
		{
			var iv = photosView.FindViewWithTag ((offerResponse.Activations.Count + PHOTOIV_TAG_BASE).ToString("00")+"sw-sh-sl-st") as ImageView;
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
			iv.Tag = (offerResponse.Activations.Count + PHOTOIV_TAG_BASE).ToString ("00") + "sw-sh-sl-st";
			iv.SetImageBitmap (bitmap);
			photosView.AddView (iv);
			iv.LayoutParameters = new FrameLayout.LayoutParams ((FrameLayout.LayoutParams)addPhotoBtn.LayoutParameters);

			FrameLayout.LayoutParams flParam = (FrameLayout.LayoutParams)addPhotoBtn.LayoutParameters;
			flParam.LeftMargin = ((offerResponse.Activations.Count + 1) % 2) * (flParam.Width + 4);
			flParam.TopMargin = ((offerResponse.Activations.Count + 1) / 2) * (flParam.Height + 4);
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

		private void ViewPogBtn_OnClicked (object sender, EventArgs arg)
		{
			var storageContent = planogram.Versions[0].StorageContent;

			if (storageContent == null) {
				ShowAlert ("Doc isn't available.");
				return;
			}

			var fragment = new ViewDocFragment ();
			fragment.StorageContent = planogram.Versions[0].StorageContent;
			PushFragment (fragment, this.Class.SimpleName);
		}

		async private void SendFeedbackBtn_OnClicked (object sender, EventArgs arg)
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

			var fragment = new LeaveFeedbackFragment ();
			fragment.FeedbackType = feedbackType;
			fragment.Offer = offer;
			PushFragment (fragment, this.Class.SimpleName);
		}

		async private void SendToMailBtn_OnClicked (object sender, EventArgs arg)
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
				ShowAlert ("Not Valid Email");
				return;
			}

			int versionId = planogram.Versions [0].PlanogramVersionId;

			ShowLoading ();
			var accountMgr = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await accountMgr.SendPlanogramViaEmail (versionId, email);
			HideLoading ();

			if (!accountMgr.IsSuccessed) {
				ShowErrorMessage (accountMgr.ErrorMessage);
				return;
			}
			ShowAlert ("Email has been successfully sent.");
		}
		#endregion
	}
}

