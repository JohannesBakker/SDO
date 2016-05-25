
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
	public class SDXAddOutletFragment : SDXBaseFragment
	{
		public AccountModel Account { get; set; }		// For Adding Multi Outlets
		public OutletModel Outlet = null;				// For Updating Outlet
		public string LocationId = null;				// For Adding One Outlet

		private List <OutletModel> outlets = new List<OutletModel> ();
		private AccountModel newAccount = null;

		View view;

		private List <SDXAddOutletView> viewList = new List<SDXAddOutletView> ();
		private List <string> consumersOnSiteList = new List <string> ();
		private List <string> annualSalesList = new List <string> ();
		private List <string> localCompetitionList = new List<string> ();

		private ImageButton addPhotoBtn;
		private LinearLayout addoutletViews;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHeaderTitle ("In the Zone Setup", 1);
			HideAllHeaderButtons ();
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.AddOutlet, container, false);
			LayoutView (view);

			return view;
		}

		async public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			SDXReferenceDataManager refMgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (refMgr.DataModel == null) {
				await LoadReferenceData ();
			}
			AddReferenceDatas (refMgr.DataModel);

			GetInstance ();
			AppendAddOuletView (0);

			if (LocationId != null || Outlet != null) {
				if (Outlet != null) {
					SetHeaderTitle ("Update Outlet", 1);
				} else
					SetHeaderTitle ("Add Outlet", 1);

				var nextBtn = view.FindViewById <Button> (Resource.Id.addoutlet_next_btn);
				nextBtn.Text = "DONE";
				var headerView = view.FindViewById <FrameLayout> (Resource.Id.addoutlet_header_fl);
				headerView.Visibility = ViewStates.Gone;
				var addView = view.FindViewById (Resource.Id.addoutlet_add_fl) as FrameLayout;
				addView.Visibility = ViewStates.Gone;
			}

			if (Outlet == null) {
				var photosView = view.FindViewById (Resource.Id.addoutlet_photos_fl) as FrameLayout;
				photosView.Visibility = ViewStates.Gone;
			} else {
				if (Outlet.Photo != null) {
					var imgManager = new JHImageManager ();
					imgManager.LoadCompleted += (object s, byte[] bytes) => {
						Activity.RunOnUiThread (delegate {
							if (bytes == null)
								return;
							using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
								if (bmp != null) {
									addPhotoBtn.SetScaleType (ImageView.ScaleType.CenterCrop);
									addPhotoBtn.SetImageBitmap (bmp);
								}
							}
						});
					};
					imgManager.LoadImageAsync (Outlet.Photo.ProcessedPhotoBaseUrl, addPhotoBtn.LayoutParameters.Width, 0);
				}
			}

			if (Outlet != null) {
				var addoutletV = viewList.ElementAt (0);
				addoutletV.FillContents (Outlet);
			}
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			if (addPhotoBtn != null)
				return;

			addPhotoBtn = view.FindViewById (Resource.Id.addoutlet_addphoto_imgbtn) as ImageButton;
			addoutletViews = view.FindViewById (Resource.Id.addoutlet_views_ll) as LinearLayout;

			var addBtn = view.FindViewById (Resource.Id.addoutlet_addnew_btn) as Button;
			addBtn.Click += AddBtn_OnClicked;
			var nextBtn = view.FindViewById (Resource.Id.addoutlet_next_btn) as Button;
			nextBtn.Click += NextBtn_OnClicked;
			addPhotoBtn.Click += AddPhotoBtn_OnClicked;
		}

		private void AppendAddOuletView (int index)
		{
			var addoutletV = SDXAddOutletView.Create (Activity, (ViewGroup)view, index == 0 ? true : false);
			addoutletV.PutList (consumersOnSiteList, annualSalesList, localCompetitionList);
			addoutletV.Update (index);
			addoutletV.DeleteBtnEvent = DeleteBtn_OnClicked;
			addoutletViews.AddView (addoutletV.view);
			viewList.Add (addoutletV);
			if (index != 0)
				Util.StartFadeAnimation (addoutletV.view, 300);
		}

		private void AddReferenceDatas (ReferenceDataModel data)
		{
			consumersOnSiteList.Clear ();
			annualSalesList.Clear ();
			localCompetitionList.Clear ();

			foreach (ConsumersOnSiteRangeModel model in data.ConsumersOnSiteRanges)
				consumersOnSiteList.Add (model.Description);
			foreach (AnnualSalesRangeModel model in data.AnnualSalesRanges)
				annualSalesList.Add (model.Description);
			foreach (LocalCompetitionTypeModel model in data.LocalCompetitionTypes)
				localCompetitionList.Add (model.Description);
		}

		async private void AddAccount ()
		{
			Util.ShowLoading (Activity);
			var accountManger = TinyIoCContainer.Current.Resolve<SDXAccountManager> ();
			newAccount = await accountManger.AddNewAccount (Account);
			Util.HideLoading ();
			if (!accountManger.IsSuccessed) {
				Util.ShowAlert (accountManger.ErrorMessage, Activity);
			} else {
				var userMgr = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
				userMgr.Me.Accounts.Add (newAccount);

				// Go To Account Details
				var fragment = new SDXAccountDetailFragment ();
				fragment.Account = newAccount;
				fragment.PopCount = 3;
				PushFragment (fragment, "SDXAddOutletFragment");
			}
		}

		async private void UpdateOutlet ()
		{
			Util.ShowLoading (Activity);
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OutletModel upOutlet = await manager.UpdateOutlet (LocationId, Outlet);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
			} else {
				Outlet.ConsumersOnSiteRange.Description = upOutlet.ConsumersOnSiteRange.Description;
				Outlet.AnnualSalesRange.Description = upOutlet.AnnualSalesRange.Description;
				Outlet.LocalCompetitionType.Description = upOutlet.LocalCompetitionType.Description;
				Outlet.RowVersion = upOutlet.RowVersion;

				var builder = new AlertDialog.Builder (Activity);
				builder.SetMessage ("Outlet has been successfully updated.");
				builder.SetPositiveButton ("OK", (o, e) => {
					PopFragment ();
				});
				builder.Create ().Show ();
			}
		}

		async private void AddOutlet (OutletModel outlet)
		{
			Util.ShowLoading (Activity);
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OutletModel newOutlet = await manager.AddOutlet (LocationId, outlet);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
			} else {
				if (Account != null)
					Account.Outlets.Add (newOutlet);

				var builder = new AlertDialog.Builder (Activity);
				builder.SetMessage ("Outlet has been successfully added.");
				builder.SetPositiveButton ("OK", (o, e) => {
					PopFragment ();
				});
				builder.Create ().Show ();
			}
		}

		async private void DeleteOutlet ()
		{
			Util.ShowLoading (Activity);
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteOutlet (LocationId, Outlet.OutletId);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			if (Account != null)
				Account.Outlets.Remove (Outlet);

			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Outlet has been successfully deleted.");
			builder.SetPositiveButton ("OK", (o, e) => {
				PopFragment ();
			});
			builder.Create ().Show ();
		}

		private void OnPhotoSelected (object sender, Bitmap bitmap)
		{
			if (bitmap == null)
				return;

			Console.WriteLine ("Picked Image Size : (" + bitmap.Width.ToString () + "," + bitmap.Height.ToString () + ")");

			addPhotoBtn.SetScaleType (ImageView.ScaleType.CenterCrop);
			addPhotoBtn.SetImageBitmap (bitmap);

			(new Handler ()).PostDelayed (() => {
				Activity.RunOnUiThread (()=>{
					UploadPhoto (bitmap);
				});
			}, 500);
		}

		private void UploadPhoto (Bitmap bitmap)
		{
			string fileName = Outlet.Name.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			int maxImgSz = Math.Max (addPhotoBtn.LayoutParameters.Width, 0);
			DoUploadPhoto (bitmap, fileName, "Outlet", Outlet.OutletId.ToString (), maxImgSz, (object sender, PhotoModel photo) => {
				if (photo == null) {
					ResetAddPhotoBtn ();
					return;
				}
				Outlet.Photo = photo;
				Outlet.PhotoId = photo.PhotoId;
			});
		}

		private void ResetAddPhotoBtn ()
		{
			addPhotoBtn.SetScaleType (ImageView.ScaleType.CenterInside);
			addPhotoBtn.SetImageResource (Resource.Drawable.icon_plus);
		}
		#endregion

		#region Button Actions
		void AddBtn_OnClicked (object sender, EventArgs arg)
		{
			SDXAddOutletView outletV = viewList.ElementAt (outlets.Count);
			if (outletV != null) {
				if (outletV.IsFilled ()) {
					outlets.Add (outletV.GetOutlet ());
				} else {
					Util.ShowAlert ("Please fill all fields.", Activity);
					return;
				}
			}

			var numberTv = view.FindViewById (Resource.Id.addoutlet_number_tv) as TextView;
			numberTv.Text = (outlets.Count + 2).ToString ("00");

			AppendAddOuletView (outlets.Count);
		}

		void NextBtn_OnClicked (object sender, EventArgs arg)
		{
			if (Outlet != null) {
				SDXAddOutletView outletV = viewList.ElementAt (0);
				if (outletV.IsFilled()) {
					OutletModel upOutlet = outletV.GetOutlet ();
					if (Outlet.Name == upOutlet.Name && Outlet.ConsumersOnSiteRangeId == upOutlet.ConsumersOnSiteRangeId && 
						Outlet.AnnualSalesRangeId == upOutlet.AnnualSalesRangeId && Outlet.LocalCompetitionTypeId == upOutlet.LocalCompetitionTypeId) {
						Util.ShowAlert ("No changes, please change items and press done button.", Activity);
					} else {
						Outlet.Name = upOutlet.Name;
						Outlet.ConsumersOnSiteRangeId = upOutlet.ConsumersOnSiteRangeId;
						Outlet.AnnualSalesRangeId = upOutlet.AnnualSalesRangeId;
						Outlet.LocalCompetitionTypeId = upOutlet.LocalCompetitionTypeId;
						UpdateOutlet ();
					}
				} else {
					Util.ShowAlert ("Outlet should have name.", Activity);
				}
				return;
			}

			if (LocationId != null) {
				SDXAddOutletView outletV = viewList.ElementAt (0);
				if (outletV.IsFilled()) {
					OutletModel outlet = outletV.GetOutlet ();
					AddOutlet (outlet);
				} else {
					Util.ShowAlert ("Please fill all fields.", Activity);
				}
				return;
			}

			SDXAddOutletView lastOutletV = viewList.ElementAt (outlets.Count);
			if (lastOutletV != null && lastOutletV.IsFilled())
				outlets.Add (lastOutletV.GetOutlet());

			if (outlets.Count == 0) {
				Util.ShowAlert ("Please add at least one outlet.", Activity);
				return;
			}

			Account.Outlets = outlets;

			AddAccount ();
		}

		void DeleteBtn_OnClicked (object sender, EventArgs args)
		{
			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Are you sure, you want to remove this outlet?");
			builder.SetPositiveButton ("Yes", (o, e) => {
				DeleteOutlet ();
			});
			builder.SetNegativeButton ("No", (o, e) => {});
			builder.Create ().Show ();
		}

		void AddPhotoBtn_OnClicked (object sender, EventArgs args)
		{
			if (PhotoSelectedEvent == null)
				PhotoSelectedEvent = OnPhotoSelected;
			PickPhoto ();
		}
		#endregion
	}
}

