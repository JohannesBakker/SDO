
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Content.PM;
using Android.OS;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Net;
using Java.Interop;
using Java.IO;
using Android.Graphics;
using Android.Views.Animations;
using Android.Animation;
using ExifInterface = Android.Media.ExifInterface;
using Android.Media;

using Sodexo.Core;
using TinyIoC;
using System.Threading.Tasks;

namespace Sodexo.Android
{
	[Activity (Label = "MainActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]

	public class MainActivity : BaseActivity
	{
		public TextView HeaderTitleTv;
		public ImageButton BackBtn, FilterBtn, InfoBtn, AddBtn, ChangeBtn;

		LinearLayout menuView;
		FrameLayout contentView;
		ImageView headerBgIv;

		string[] menuItemNames = {"dashboard", "accounts", "promotions", "feedback", "prices", "me"};
		int[] menuItemOffImageResourceIds, menuItemOnImageResourceIds, menuItemViewResourceIds, headerBgImgResourceIds;

		int selectedMenuItemIndex = 0;
		int highlightedMenuItemIndex = 0;
		bool isMenuHidden = true;

		File imgFile, imgDir;
		const int CameraRequestCode = 1;
		const int GalleryRequestCode = 2;

		public SDXAccountDetailFragment AccountDetailFragment;

		int backPressCnt = 0;

		#region Activity Lifecycle
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			GetViews ();
			GetResourceIds ();
			CreateDirectoryForPictures ();
		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);

			if (!hasFocus || isViewAdjusted)
				return;

			var view = FindViewById (Resource.Id.main_view) as FrameLayout;
			LayoutAdjuster.FitToScreen (this, view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);

			isViewAdjusted = true;

			menuView.Animate ().SetDuration (0).X (-menuView.LayoutParameters.Width);
			ChangeScreen ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode != Result.Ok) {
				if (requestCode == CameraRequestCode || requestCode == GalleryRequestCode) {
					SDXBaseFragment curFrag = GetCurrentFragment () as SDXBaseFragment;
					curFrag.SetPhoto (null);
				}
				return;
			}

			string path = "";
			if (requestCode == CameraRequestCode) {
				Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
				Uri contentUri = Uri.FromFile (imgFile);
				mediaScanIntent.SetData (contentUri);
				SendBroadcast (mediaScanIntent);

				path = imgFile.Path;
			} else if (requestCode == GalleryRequestCode) {
				Uri uri = data.Data;
				path = Util.GetPathFromImageUri (this, uri);
			}

			if (path != "") {
				var manager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
				int maxW = manager.MaxPhotoUploadDimension;
				Bitmap scaledbmp = Util.LoadAndResizeBitmap (path, maxW, maxW);
				Bitmap bmp = Util.GetOrientedBitmap (scaledbmp, path);
				SDXBaseFragment curFrag = GetCurrentFragment () as SDXBaseFragment;
				curFrag.SetPhoto (bmp);
			}
		}

		public override void OnBackPressed ()
		{
//			base.OnBackPressed ();
			backPressCnt++;
			if (backPressCnt == 2)
				Finish ();
		}
		#endregion

		#region Private Functions
		private void GetViews ()
		{
			contentView = FindViewById (Resource.Id.main_content_fl) as FrameLayout;
			menuView = FindViewById (Resource.Id.main_menu_ll) as LinearLayout;
			BackBtn = FindViewById (Resource.Id.main_header_back_btn) as ImageButton;
			AddBtn = FindViewById (Resource.Id.main_header_add_btn) as ImageButton;
			FilterBtn = FindViewById (Resource.Id.main_header_filter_btn) as ImageButton;
			ChangeBtn = FindViewById (Resource.Id.main_header_change_btn) as ImageButton;
			InfoBtn = FindViewById (Resource.Id.main_header_info_btn) as ImageButton;
			HeaderTitleTv = FindViewById (Resource.Id.main_header_title_tv) as TextView;
			headerBgIv = FindViewById (Resource.Id.main_header_iv) as ImageView;
		}

		private void GetResourceIds ()
		{
			menuItemOffImageResourceIds = new int[menuItemNames.Count ()];
			menuItemOnImageResourceIds = new int[menuItemNames.Count ()];
			menuItemViewResourceIds = new int[menuItemNames.Count ()];
			headerBgImgResourceIds = new int[menuItemNames.Count ()];

			for (int i = 0; i < menuItemNames.Count (); i++) {
				var str = "menu_" + menuItemNames [i] + "_off";
				var id = Resources.GetIdentifier (str, "drawable", PackageName);
				menuItemOffImageResourceIds [i] = id;

				str = "menu_" + menuItemNames [i] + "_on";
				id = Resources.GetIdentifier (str, "drawable", PackageName);
				menuItemOnImageResourceIds [i] = id;

				str = "main_menu_" + menuItemNames [i] + "_btn";
				id = Resources.GetIdentifier (str, "id", PackageName);
				menuItemViewResourceIds [i] = id;

				str = menuItemNames[i] + "_header_bg";
				id = Resources.GetIdentifier (str, "drawable", PackageName);
				headerBgImgResourceIds [i] = id;
			}
		}

		private void PopToRootScreen ()
		{
            try
            {
                SDXBaseFragment.bPopFlag = true;
                for (int i = 0; i < FragmentManager.BackStackEntryCount; i++)
                {
                    FragmentManager.PopBackStackImmediate();
                }
                SDXBaseFragment.bPopFlag = false;
            }
            catch (Exception e)
            {

            }
		}

		private void ChangeFragment (Fragment fragment, AnimationType animType)
		{
			if (fragment == null)
				return;

            try
            {
                PopToRootScreen();

                contentView.RemoveAllViews();

                var ft = FragmentManager.BeginTransaction();

                if (animType == AnimationType.FadeIn)
                {
                    ft.SetCustomAnimations(Resource.Animation.Anim_Fade_In, Resource.Animation.Anim_Fade_Out);
                }
                else if (animType == AnimationType.TransitionToLeft)
                {
                    ft.SetCustomAnimations(Resource.Animation.Slide_In_Right, Resource.Animation.Slide_Out_Left,
                        Resource.Animation.Slide_In_Left, Resource.Animation.Slide_Out_Right);
                }
                else if (animType == AnimationType.TransitionToRight)
                {
                    ft.SetCustomAnimations(Resource.Animation.Slide_In_Left, Resource.Animation.Slide_Out_Right,
                        Resource.Animation.Slide_In_Right, Resource.Animation.Slide_Out_Left);
                }

                ft.Replace(Resource.Id.main_content_fl, fragment);

                ft.Commit();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
		}

		private void ChangeScreen ()
		{
			Fragment fragment = null;

			HideAllHeaderButtons ();

			switch (selectedMenuItemIndex) {
			case 0:
				fragment = new SDXDashboardFragment ();
				break;
			case 1:
				fragment = new SDXAccountsFragment ();
				break;
			case 2:
				fragment = new SDXPromotionsFragment ();
				break;
			case 3:
				fragment = new SDXFeedbackFragment ();
				break;
			case 4:
				fragment = new SDXPricesFragment ();
				break;
			case 5:
				fragment = new SDXMeFragment ();
				break;
			}

			ChangeFragment (fragment, AnimationType.FadeIn);
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void CreateDirectoryForPictures()
		{
			imgDir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "Sodexo");
			if (!imgDir.Exists ()) {
				imgDir.Mkdirs ();
			}
		}

		private void TakePhoto ()
		{
			if (!IsThereAnAppToTakePictures ()) {
				Util.ShowAlert ("Camera is not availabile.", this);
				return;
			}

			Intent intent = new Intent (MediaStore.ActionImageCapture);
			imgFile = null;
			imgFile = new File (imgDir, String.Format ("sodexo_{0}.jpg", Guid.NewGuid ()));
			intent.PutExtra (MediaStore.ExtraOutput, Uri.FromFile (imgFile));
			StartActivityForResult (intent, CameraRequestCode);
		}

		private void SelectPhoto ()
		{
			Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
			StartActivityForResult (intent, GalleryRequestCode);
		}

		private Fragment GetCurrentFragment ()
		{
			Fragment fragment = FragmentManager.FindFragmentById (Resource.Id.main_content_fl);
			return fragment;
		}
		#endregion

		#region Public Functions
		public void PushFragment (Fragment fragment, string curFragmentName)
		{
			contentView.RemoveAllViews ();
			var ft = FragmentManager.BeginTransaction ();
//			ft.SetCustomAnimations (Resource.Animation.Slide_In_Right, Resource.Animation.Slide_Out_Left,
//				Resource.Animation.Slide_In_Left, Resource.Animation.Slide_Out_Right);
			ft.SetCustomAnimations (Resource.Animation.Anim_Fade_In, Resource.Animation.Anim_Fade_Out,
				Resource.Animation.Anim_Fade_In, Resource.Animation.Anim_Fade_Out);
			ft.Replace (Resource.Id.main_content_fl, fragment);
			ft.AddToBackStack (curFragmentName);
			ft.Commit ();
		}

		public void PopFragment (int popCnt)
		{
			for (int i = 0; i < popCnt; i++) {
				contentView.RemoveAllViews ();
				FragmentManager.PopBackStackImmediate ();
			}
		}

		public void HideAllHeaderButtons ()
		{
			BackBtn.Visibility = ViewStates.Gone;
			FilterBtn.Visibility = ViewStates.Gone;
			InfoBtn.Visibility = ViewStates.Gone;
			AddBtn.Visibility = ViewStates.Gone;
			ChangeBtn.Visibility = ViewStates.Gone;
		}

		public void SetHeaderBg (int idx)
		{
			headerBgIv.SetImageResource (headerBgImgResourceIds [idx]);
		}

		public void LogOut ()
		{
			Finish ();

			var intent = new Intent (this, typeof(SplashActivity));
			StartActivity (intent);
		}

		public void PickPhoto ()
		{
			string[] options = {@"Take a Photo", @"Select Existing One", @"Cancel"};
			AlertDialog.Builder builder = new AlertDialog.Builder (this);
			builder.SetTitle ("");
			builder.SetItems (options, ((object sender, DialogClickEventArgs e) => {
				RunOnUiThread (delegate() {
					((AlertDialog)sender).Dismiss ();
					if (options [e.Which] == "Take a Photo") {
						TakePhoto ();
					} else if (options [e.Which] == "Select Existing One") {
						SelectPhoto();
					}
				});
			}));
			builder.Show ();
		}

		public void HighlightMenuItem (int idx)
		{
			//if (highlightedMenuItemIndex != selectedMenuItemIndex) {
            if (highlightedMenuItemIndex != idx)
            {
				var highlightedBtn = FindViewById (menuItemViewResourceIds [highlightedMenuItemIndex]) as Button;
				highlightedBtn.SetBackgroundResource (menuItemOffImageResourceIds [highlightedMenuItemIndex]);
			}
            selectedMenuItemIndex = highlightedMenuItemIndex = idx;
            //highlightedMenuItemIndex = idx;
			var btn = FindViewById (menuItemViewResourceIds [idx]) as Button;
			btn.SetBackgroundResource (menuItemOnImageResourceIds [idx]);
		}
		#endregion

		#region Button Actions
		[Export]
		public void MenuBtn_OnClick (View view)
		{
			int xDelta = menuView.Width;

			if (isMenuHidden) {
				menuView.Animate ().SetDuration (300).X (0);
				contentView.Animate ().SetDuration (300).X (xDelta);
			} else {
				menuView.Animate ().SetDuration (300).X (-xDelta);
				contentView.Animate ().SetDuration (300).X (0);
			}

			isMenuHidden = !isMenuHidden;
		}

		[Export]
		public void BackBtn_OnClick (View view)
		{
			System.Console.WriteLine ("Back Button Clicked");
			SDXBaseFragment baseFrg = GetCurrentFragment () as SDXBaseFragment;
			baseFrg.PopFragment ();
		}

		[Export]
		public void MenuItem_OnClick (View view)
		{
			System.Console.WriteLine (view.Tag);

			int idx = Convert.ToInt32 (((string)view.Tag).Substring (0, 1));

			if (idx == selectedMenuItemIndex) {
				MenuBtn_OnClick (null);
				if (highlightedMenuItemIndex != selectedMenuItemIndex) {
					var selectedBtn = FindViewById (menuItemViewResourceIds [selectedMenuItemIndex]) as Button;
					selectedBtn.SetBackgroundResource (menuItemOnImageResourceIds [selectedMenuItemIndex]);
					var highlightedBtn = FindViewById (menuItemViewResourceIds [highlightedMenuItemIndex]) as Button;
					highlightedBtn.SetBackgroundResource (menuItemOffImageResourceIds [highlightedMenuItemIndex]);
					PopToRootScreen ();
					highlightedMenuItemIndex = selectedMenuItemIndex;
				}
				return;
			}

			var prevBtn = FindViewById (menuItemViewResourceIds [selectedMenuItemIndex]) as Button;
			prevBtn.SetBackgroundResource (menuItemOffImageResourceIds [selectedMenuItemIndex]);

			var btn = view as Button;
			btn.SetBackgroundResource (menuItemOnImageResourceIds [idx]);

			selectedMenuItemIndex = highlightedMenuItemIndex = idx;

			ChangeScreen ();

			MenuBtn_OnClick (null);
		}
		#endregion
	}
}

